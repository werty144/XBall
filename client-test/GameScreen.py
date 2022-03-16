import json
import math

from kivy.app import App
from kivy.core.window import Window
from kivy.graphics import Rectangle, Color, Line, Rotate, Ellipse
from kivy.uix.label import Label
from kivy.uix.screenmanager import Screen


class GameScreen(Screen):
    def __init__(self, **kw):
        self.requests = set()
        self.game_id = None
        self.session_id = None

        self.field_x = 50
        self.field_y = 100
        self.scale = 30
        self.field_height = 15
        self.field_width = 40
        self.player_radius = 0.35
        self.ball_radius = 0.3
        self.target_x_margin = 0.15
        self.target_y_margin = 0.5
        self.target_radius = 0.4
        self.flyHeight = 4
        self.score = None
        self.time = None
        self.score_label = Label(text="0:0",
                                 font_size='60sp',
                                 pos_hint={'center_x': 0.5, 'center_y': 0.95})
        self.time_label = Label(text="0.0",
                                font_size='35sp',
                                pos_hint={'center_x': 0.1, 'center_y': 0.95})
        self.state = None
        self.selected_player_id = None
        self.throw_intention = False
        self.orientation_intention = False
        self._keyboard = None
        super().__init__(**kw)

    def on_enter(self, *args):
        self._keyboard = Window.request_keyboard(
            self._keyboard_closed, self, 'text')
        self._keyboard.bind(on_key_down=self._on_keyboard_down)

    def set_game(self, message_body):
        if message_body['status'] == 'ENDED':
            sm = App.get_running_app().root
            sm.current = 'menu'

        self.state = message_body['state']
        self.score = message_body['score']
        self.time = message_body['time']
        self.canvas.clear()
        self.remove_widget(self.score_label)
        self.remove_widget(self.time_label)

        with self.canvas:
            Rectangle(pos=(self.field_x, self.field_y),
                      size=(self.field_width * self.scale, self.field_height * self.scale))

        for player in self.state['players']:
            self.draw_player(player)

        self.draw_ball(self.state['ballState'])
        self.draw_target()
        # self.draw_score()
        self.draw_time()

    def make_move(self, move):
        self.requests.add(json.dumps({"path": "makeMove", "body": {"move": move}}))

    def screen_to_field_coordinates(self, x, y):
        return (x - self.field_x) / self.scale, (y - self.field_y) / self.scale

    def field_to_screen_coordinates(self, x, y):
        return x * self.scale + self.field_x, \
               y * self.scale + self.field_y

    def player_id_by_field_coordinates(self, x, y):
        for player in self.state['players']:
            player_x, player_y = player['state']['x'], player['state']['y']
            if (player_x - x) ** 2 + (player_y - y) ** 2 <= self.player_radius ** 2:
                return player['id']

    def on_touch_down(self, touch):
        field_pos = self.screen_to_field_coordinates(*touch.pos)

        if touch.button == 'left':
            if self.throw_intention:
                self.make_move({'playerId': self.selected_player_id, 'action': 'throw',
                                'actionData': {'x': field_pos[0], 'y': field_pos[1]}})
                self.throw_intention = False
            elif self.orientation_intention:
                self.make_move({'playerId': self.selected_player_id, 'action': 'orientation',
                                'actionData': {'x': field_pos[0], 'y': field_pos[1]}})
                self.orientation_intention = False
            else:
                self.selected_player_id = self.player_id_by_field_coordinates(*field_pos)

        if touch.button == 'right':
            if self.selected_player_id is not None:
                self.make_move({'playerId': self.selected_player_id, 'action': 'movement',
                                'actionData': {'x': field_pos[0], 'y': field_pos[1]}})

    def _keyboard_closed(self):
        self._keyboard.unbind(on_key_down=self._on_keyboard_down)
        self._keyboard = None

    def _on_keyboard_down(self, keyboard, keycode, text, modifiers):
        if keycode[1] == 'w':
            self.orientation_intention = False
            self.throw_intention = False
            self.make_move({'playerId': self.selected_player_id, 'action': 'grab',
                            'actionData': {}})
        if keycode[1] == 'q':
            self.orientation_intention = False
            self.throw_intention = True

        if keycode[1] == 'e':
            self.orientation_intention = True
            self.throw_intention = False

        if keycode[1] == 'r':
            self.orientation_intention = False
            self.throw_intention = False
            self.make_move({'playerId': self.selected_player_id, 'action': 'attack',
                            'actionData': {}})

        if keycode[1] == 's':
            self.make_move({'playerId': self.selected_player_id, 'action': 'stop', 'actionData': {}})

        return True

    def draw_player(self, player):
        player_state = player['state']
        self.canvas.add(Color(rgba=(0, 0, 1, 1)))
        screen_player_x, screen_player_y = self.field_to_screen_coordinates(player_state['x'], player_state['y'])
        rotation_angle = rads_to_degs(player_state['rotationAngle'])

        with self.canvas:
            Rotate(angle=rotation_angle, origin=(screen_player_x, screen_player_y))
            if player['userId'] == self.session_id:
                Color(rgba=(0, 0, 1, 1))
            else:
                Color(rgba=(1, 0, 0, 1))
            Ellipse(pos=(
            screen_player_x - self.player_radius * self.scale, screen_player_y - self.player_radius * self.scale),
                    size=(self.player_radius * self.scale * 2, self.player_radius * self.scale * 2)
                    )
            Color(rgba=(1, 1, 0, 1))
            Line(points=[screen_player_x, screen_player_y, screen_player_x + self.player_radius * self.scale,
                         screen_player_y],
                 width=2)
            if player['id'] == self.selected_player_id:
                Line(circle=(screen_player_x, screen_player_y, self.player_radius * self.scale), width=2)
            Rotate(angle=-rotation_angle, origin=(screen_player_x, screen_player_y))

    def draw_ball(self, ball_state):
        self.canvas.add(Color(rgba=(1, 0, 0, 1)))
        ball_radius = (ball_state['z'] ** (0.5)) * self.ball_radius
        instruction = Line(
            circle=(*self.field_to_screen_coordinates(ball_state['x'], ball_state['y']), self.scale * ball_radius))
        self.canvas.add(instruction)

    def draw_target(self):
        self.canvas.add(Color(rgba=(0, 0, 0, 1)))
        left = Line(circle=(
            *self.field_to_screen_coordinates(self.field_width * self.target_x_margin,
                                              self.field_height * self.target_y_margin),
            self.scale * self.target_radius
        ))
        right = Line(circle=(
            *self.field_to_screen_coordinates(self.field_width * (1 - self.target_x_margin),
                                              self.field_height * self.target_y_margin),
            self.scale * self.target_radius
        ))
        self.canvas.add(left)
        self.canvas.add(right)

    def draw_score(self):
        self.score_label.text = f"{self.score['LEFT']}:{self.score['RIGHT']}"
        self.add_widget(self.score_label)

    def draw_time(self):
        self.time_label.text = millis_to_min_secs_string(self.time)
        self.add_widget(self.time_label)


def clip(x, low, up):
    return min(up, max(x, low))


def rads_to_degs(rads):
    return rads / math.pi * 180


def millis_to_min_secs_string(mls):
    secs = mls // 1000
    return f'{secs // 60}.{secs % 60}'
