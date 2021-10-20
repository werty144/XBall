import asyncio
from time import sleep

import websockets

from MenuScreen import MenuScreen


async def hello(app):
    try:
        await asyncio.sleep(0.1)
        menu_screen: MenuScreen = app.root

        uri = "ws://localhost:8080"
        async with websockets.connect(uri + "/") as websocket:
            while True:
                # await websocket.send(name)

                greeting = await websocket.recv()
                menu_screen.set_user_id_label(greeting)

    except asyncio.CancelledError:
        pass
