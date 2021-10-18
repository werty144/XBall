import asyncio
import websockets


async def hello():
    uri = "ws://localhost:8080"
    async with websockets.connect(uri + "/test1") as websocket:
        name = input("What's your name? ")

        await websocket.send(name)
        print(f"> {name}")

        async with websockets.connect(uri + "/test2") as websocket2:
            name = input("What's your name? ")

            await websocket2.send(name)
            print(f"> {name}")

            greeting = await websocket2.recv()
            print(f"< {greeting}")

        greeting = await websocket.recv()
        print(f"< {greeting}")


asyncio.get_event_loop().run_until_complete(hello())
