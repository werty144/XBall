import json
import math

from kivy.graphics import Rectangle, Color, Line, Rotate
from kivy.uix.screenmanager import Screen


class GameScreen(Screen):
    def __init__(self, **kw):
        self.requests = set()
        self.game_id = None

        self.field_x = 50
        self.field_y = 100
        self.scale = 4
        self.field_height = 150 * self.scale
        self.field_width = self.field_height * 2
        self.player_width = 10 * self.scale
        self.state = None
        self.selected_player_id = None
        super().__init__(**kw)

    def set_game_state(self, state):
        self.state = state
        self.canvas.clear()

        with self.canvas:
            Rectangle(pos=(self.field_x, self.field_y), size=(self.field_width, self.field_height))

        for player in state['players']:
            self.draw_player(player)

        self.draw_ball(state['ballState'])

    def make_move(self, move):
        self.requests.add(json.dumps({"path": "makeMove", "body": {"move": move}}))

    def screen_to_field_coordinates(self, x, y):
        return clip((x - self.field_x) // self.scale, 0, 300),\
               clip((y - self.field_y) // self.scale, 0, 150)

    def field_to_screen_coordinates(self, x, y):
        return x * self.scale + self.field_x, \
               y * self.scale + self.field_y

    def player_id_by_field_coordinates(self, x, y):
        for player in self.state['players']:
            player_x, player_y = player['state']['x'], player['state']['y']
            if abs(player_x - x) <= self.player_width // 2 and abs(player_y - y) <= self.player_width // 2:
                return player['id']

    def on_touch_down(self, touch):
        field_pos = self.screen_to_field_coordinates(*touch.pos)
        if self.selected_player_id is None:
            self.selected_player_id = self.player_id_by_field_coordinates(*field_pos)
        else:
            self.make_move({'playerId': self.selected_player_id, 'action': 'movement',
                            'actionData': {'x': field_pos[0], 'y': field_pos[1]}})
            self.selected_player_id = None

    def draw_player(self, player):
        player_state = player['state']
        self.canvas.add(Color(rgba=(0, 0, 1, 1)))
        screen_player_x, screen_player_y = self.field_to_screen_coordinates(player_state['x'], player_state['y'])
        instruction = Rectangle(pos=(screen_player_x - self.player_width // 2, screen_player_y - self.player_width // 2),
                                size=(self.player_width, self.player_width))

        rotation_angle = rads_to_degs(player_state['rotationAngle'])
        self.canvas.add(Rotate(angle=rotation_angle, origin=(screen_player_x, screen_player_y)))
        self.canvas.add(instruction)
        self.canvas.add(Rotate(angle=-rotation_angle, origin=(screen_player_x, screen_player_y)))

    def draw_ball(self, ball_state):
        self.canvas.add(Color(rgba=(1, 0, 0, 1)))
        instruction = Line(circle=(*self.field_to_screen_coordinates(ball_state['x'], ball_state['y']), self.scale * 3))
        self.canvas.add(instruction)


def clip(x, low, up):
    return min(up, max(x, low))


def rads_to_degs(rads):
    return rads / math.pi * 180
