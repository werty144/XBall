import asyncio
import json
import sys
from time import sleep

import websockets
from kivy.app import App

from MenuScreen import MenuScreen


async def listen(websocket):
    sm = App.get_running_app().root
    menu_screen = sm.get_screen('menu')
    game_screen = sm.get_screen('game')

    while True:
        message = await websocket.recv()
        message = json.loads(message)
        if message["path"] == "invite":
            invite_id = message["body"]["inviteId"]
            inviter_id = message["body"]["inviterId"]
            menu_screen.add_invite(invite_id, inviter_id)
        elif message["path"] == "game":
            if sm.current != "game":
                sm.current = "game"
            game_screen.set_game(message["body"])


async def say(websocket):
    menu_screen = App.get_running_app().root.get_screen('menu')
    game_screen = App.get_running_app().root.get_screen('game')
    if len(sys.argv) == 1:
        user_id = '1'
    else:
        user_id = sys.argv[1]
    menu_screen.set_user_id_label(user_id)
    await websocket.send(f'{user_id}_salt')
    while True:
        await asyncio.sleep(0.1)
        while len(menu_screen.requests) > 0:
            await websocket.send(menu_screen.requests.pop())
        while len(game_screen.requests) > 0:
            await websocket.send(game_screen.requests.pop())


async def hello(app):
    # return
    try:
        await asyncio.sleep(0.1)
        uri = "ws://localhost:8080"
        async with websockets.connect(uri + "/") as websocket:
            await asyncio.gather(listen(websocket), say(websocket))

    except asyncio.CancelledError:
        pass
