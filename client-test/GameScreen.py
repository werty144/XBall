import json

from kivy.graphics import Rectangle, Color, Line
from kivy.uix.screenmanager import Screen


class GameScreen(Screen):
    requests = set()
    game_id = None
    was = False
    drawing_instructions = []

    def set_game_state(self, state):
        for instruction in self.drawing_instructions:
            self.canvas.remove(instruction)
        self.drawing_instructions.clear()

        for player in state['players']:
            player_state = player['state']
            self.canvas.add(Color(rgba=(0, 0, 1, 1)))
            instruction = Rectangle(pos=(player_state['x'] + 150, player_state['y'] + 150), size=(10, 10))
            self.drawing_instructions.append(instruction)
            self.canvas.add(instruction)

        ball_state = state['ballState']
        self.canvas.add(Color(rgba=(1, 0, 0, 1)))
        instruction = Line(circle=(ball_state['x'] + 150, ball_state['y'] + 150, 3))
        self.drawing_instructions.append(instruction)
        self.canvas.add(instruction)

    def make_move(self, move):
        self.requests.add(json.dumps({"path": "makeMove", "body": {"gameId": self.game_id, "move": move}}))