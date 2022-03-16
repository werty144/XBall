import asyncio
import sys

from kivy.config import Config

from bot import BotGame

Config.set('input', 'mouse', 'mouse,multitouch_on_demand')

from kivy.app import App
from kivy.uix.screenmanager import ScreenManager

from GameScreen import GameScreen
from connection import hello
from MenuScreen import MenuScreen
from kivy.core.window import Window
Window.size = (1300, 800)


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
    bot = None if len(sys.argv) <= 2 else BotGame()
    connection_task = asyncio.ensure_future(hello(app, bot))
    asyncio.get_event_loop().run_until_complete(asyncio.gather(run_app_with_close(app, connection_task), connection_task))
