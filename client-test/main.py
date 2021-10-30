import asyncio

from kivy.app import App
from kivy.uix.screenmanager import ScreenManager

from GameScreen import GameScreen
from connection import hello
from MenuScreen import MenuScreen


class XBallClientApp(App):

    def build(self):
        sm = ScreenManager()
        menu_screen = MenuScreen(name='menu')
        game_screen = GameScreen(name='game')
        sm.add_widget(menu_screen)
        sm.add_widget(game_screen)
        return sm


async def run_app_with_close(app, connection_task):
    await app.async_run()
    connection_task.cancel()


if __name__ == '__main__':
    app = XBallClientApp()
    connection_task = asyncio.ensure_future(hello(app))
    asyncio.get_event_loop().run_until_complete(asyncio.gather(run_app_with_close(app, connection_task), connection_task))