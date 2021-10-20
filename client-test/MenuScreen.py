from kivy.uix.screenmanager import Screen


class MenuScreen(Screen):
    def send_invite(self, text):
        print(text)

    def accept_invite(self):
        print('Invite accepted')
        self.set_user_id_label()

    def set_user_id_label(self, id):
        self.ids['user_id_label'].text = f"Your Id is {id}"
