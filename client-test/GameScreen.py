import json

from kivy.uix.screenmanager import Screen


class GameScreen(Screen):
    requests = set()
    game_id = None

    def set_game_state(self, state):
        self.ids['game_state'].text = str(state)

    def make_move(self, move):
        self.requests.add(json.dumps({"path": "makeMove", "body": {"gameId": self.game_id, "move": move}}))