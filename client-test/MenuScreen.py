import asyncio
import json

from kivy.uix.screenmanager import Screen


GRAY = (0.9, 0.9, 0.9, 1)
BLUE = (0.3, 0.3, 1, 1)


class MenuScreen(Screen):
    requests = set()
    selected_speed_button = None
    selected_players_button = None
    cur_invite_id = None
    invited_id = None
    invite_speed = None
    invite_players_number = None
    my_id = None

    def send_invite(self):
        self.requests.add(json.dumps({"path": "invite", "body": {
            "invitedId": self.invited_id,
            "speed": self.invite_speed,
            "playersNumber": self.invite_players_number
        }}))

    def add_invite(self, invite_id, inviter_id):
        self.ids["invite_from_label"].text = f"Invite from {inviter_id}"
        self.cur_invite_id = invite_id

    def accept_invite(self):
        self.requests.add(json.dumps({"path": "acceptInvite", "body": {"inviteId": self.cur_invite_id}}))

    def set_user_id_label(self, id):
        self.my_id = id
        self.auto_invite()
        self.ids['user_id_label'].text = f"Your Id is {id}"

    def auto_invite(self):
        self.invited_id = self.my_id
        self.invite_speed = 'FAST'
        self.invite_players_number = 3
        self.send_invite()
        # await asyncio.sleep(1)
        # self.accept_invite()

    def select_speed(self, button_id):
        if self.selected_speed_button is not None:
            self.ids[self.selected_speed_button].background_color = GRAY
        self.selected_speed_button = button_id
        self.ids[self.selected_speed_button].background_color = BLUE

        if button_id == 'button_slow':
            self.invite_speed = 'SLOW'
        elif button_id == 'button_normal':
            self.invite_speed = 'NORMAL'
        elif button_id == 'button_fast':
            self.invite_speed = 'FAST'

    def select_players_number(self, button_id):
        if self.selected_players_button is not None:
            self.ids[self.selected_players_button].background_color = GRAY
        self.selected_players_button = button_id
        self.ids[self.selected_players_button].background_color = BLUE

        if button_id == 'button_one_player':
            self.invite_players_number = 1
        elif button_id == 'button_two_player':
            self.invite_players_number = 2
        elif button_id == 'button_three_player':
            self.invite_players_number = 3

    def set_invited_id(self, text):
        self.invited_id = int(text)

