import json

from kivy.uix.screenmanager import Screen


class MenuScreen(Screen):
    requests = set()
    cur_invite_id = None

    def send_invite(self, text: str):
        print("Added invite request")
        self.requests.add(json.dumps({"path": "invite", "body": {"invitedId": int(text)}}))

    def add_invite(self, invite_id, inviter_id):
        self.ids["invite_from_label"].text = f"Invite from {inviter_id}"
        self.cur_invite_id = invite_id

    def accept_invite(self):
        print("Added invite accept request")
        self.requests.add(json.dumps({"path": "acceptInvite", "body": {"inviteId": self.cur_invite_id}}))

    def set_user_id_label(self, id):
        self.ids['user_id_label'].text = f"Your Id is {id}"
