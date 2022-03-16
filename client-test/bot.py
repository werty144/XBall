import json
import math
from dataclasses import dataclass

import numpy as np

EPS = 0.01


def dist(obj_1, obj_2):
    if isinstance(obj_1, list):
        x1, y1 = obj_1
    else:
        x1, y1 = obj_1['x'], obj_1['y']
    if isinstance(obj_2, list):
        x2, y2 = obj_2
    else:
        x2, y2 = obj_2['x'], obj_2['y']
    return math.dist([x1, y1], [x2, y2])


def point_segment_dist(p0, p1, p, player_radius=0.35):
    """The intersection point of the segment [p0, p1] and the perpendicular from the point p"""
    # TODO: use np.cross and np.linalg.norm
    x0, y0 = p0
    x1, y1 = p1
    x, y = p

    p0 = np.array(p0)
    p1 = np.array(p1)
    p = np.array(p)
    v = p1 - p0
    w0 = p0 - p
    w1 = p1 - p

    if w0 @ v >= 0:
        lam = 2 * player_radius / dist([x0, y0], [x1, y1]) + EPS
        new_x = (x0 + lam * x1) / (1 + lam)
        new_y = (y0 + lam * y1) / (1 + lam)
        return [new_x, new_y]
    if w1 @ v <= 0:
        return [x1, y1]

    # equation of a line [p0, p1] (y = ax + b)
    a = 1e-3 if x0 - x1 == 0 else (y0 - y1) / (x0 - x1)
    # a = (y0 - y1) / (x0 - x1)
    b = y0 - a * x0
    # equation of a perpendicular
    a_per = -1 / a
    b_per = x / a + y

    x_intersection = (b_per - b) / (a - a_per)
    y_intersection = a * x_intersection + b

    return [x_intersection, y_intersection]


@dataclass
class Hero:
    id: int
    is_forward: bool = False


class BotGame:
    requests = set()
    field_height = 15
    field_width = 40
    player_radius = 0.35
    ball_radius = 0.3
    user_id = None

    def set_user_id(self, user_id):
        self.user_id = user_id

    def set_game(self, message_body):
        state = message_body['state']
        players = state['players']
        enemies = [player for player in players if str(player['userId']) != str(self.user_id)]
        friends = [player for player in players if str(player['userId']) == str(self.user_id)]
        ballState = state['ballState']

        forward_enemy_index = int(np.argmin([dist(player['state'], ballState) for player in enemies]))
        forward_enemy = enemies[forward_enemy_index]
        enemies.pop(forward_enemy_index)

        forward_friend_index = int(np.argmin([dist(player['state'], ballState) for player in friends]))
        forward_friend = friends[forward_friend_index]
        friends.pop(forward_friend_index)

        if dist(forward_friend['state'], ballState) <= 3 * self.player_radius + self.ball_radius + EPS and \
                ballState['ownerId'] != forward_friend['id']:
            self.make_move({'playerId': forward_friend['id'], 'action': 'grab',
                            'actionData': {}})
        else:
            self.make_move({'playerId': forward_friend['id'], 'action': 'movement',
                            'actionData': {'x': ballState['x'], 'y': ballState['y']}})

        attack = dist(forward_friend['state'], ballState) < dist(forward_enemy['state'], ballState)
        if not attack:
            dists = []
            inter_points = []
            for friend in friends:
                x, y = friend['state']['x'], friend['state']['y']
                inter_point = [point_segment_dist(
                    [enemy['state']['x'], enemy['state']['y']], [ballState['x'], ballState['y']], [x, y]
                ) for enemy in enemies]
                inter_points.append(inter_point)
                dists.append(list(map(lambda a: dist(a, friend['state']), inter_point)))

            if dists[0][0] + dists[1][1] < dists[0][1] + dists[1][
                0]:  # TODO: This does not work with the number of players != 3
                self.make_move({'playerId': friends[0]['id'], 'action': 'movement',
                                'actionData': {'x': inter_points[0][0][0], 'y': inter_points[0][0][1]}})
                self.make_move({'playerId': friends[1]['id'], 'action': 'movement',
                                'actionData': {'x': inter_points[1][1][0], 'y': inter_points[1][1][1]}})
            else:
                self.make_move({'playerId': friends[0]['id'], 'action': 'movement',
                                'actionData': {'x': inter_points[0][1][0], 'y': inter_points[0][1][1]}})
                self.make_move({'playerId': friends[1]['id'], 'action': 'movement',
                                'actionData': {'x': inter_points[1][0][0], 'y': inter_points[1][0][1]}})
        else:
            if dist(forward_friend['state'], ballState) <= self.player_radius + self.ball_radius + EPS:
                self.make_move({'playerId': forward_friend['id'], 'action': 'attack',
                                'actionData': {}})

        for friend in friends + [forward_friend]:
            for enemy in enemies + [forward_enemy]:
                if dist(friend['state'], enemy['state']) < 2 * self.player_radius + EPS:
                    pass

    def make_move(self, move):
        self.requests.add(json.dumps({"path": "makeMove", "body": {"move": move}}))
