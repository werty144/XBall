import asyncio

from kivy.app import App

from connection import hello
from MenuScreen import MenuScreen


class XBallClientApp(App):

    def build(self):
        menu_screen = MenuScreen()
        return menu_screen


async def run_app_with_close(app, connection_task):
    await app.async_run()
    connection_task.cancel()


if __name__ == '__main__':
    app = XBallClientApp()
    connection_task = asyncio.ensure_future(hello(app))
    asyncio.get_event_loop().run_until_complete(asyncio.gather(run_app_with_close(app, connection_task), connection_task))
