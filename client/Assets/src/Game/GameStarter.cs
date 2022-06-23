using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


using UnityEngine;


using static P2PReceiver;
using static LobbyManager;
using static RequestCreator;


public class GameStarter
{
    public static void lobbyReady(LobbyData lobbyData)
    {
        P2PReceiver.startReceivingMessages();

        if (LobbyManager.IAmLobbyOwner())
        {
            prepareToBeHost(lobbyData);
        } else
        {
            prepareToBeClient();
        }
    }

    static async void prepareToBeHost(LobbyData lobbyData)
    {
        P2PReceiver.isHost = true;
        RequestCreator.isHost = true;
        ServerMessageProcessor.isHost = true;

        if (await startServer())
        {
            RequestCreator.lobbyReady(lobbyData);
        } else 
        {
            Debug.Log("Can't start server");
        }
    }

    static void prepareToBeClient()
    {
        P2PReceiver.isHost = false;
        RequestCreator.isHost = false;
        ServerMessageProcessor.isHost = false;

        SteamP2P.sendInitialMessage(LobbyManager.getLobbyOwner());
    }

    static async Task<bool> startServer()
    {
        ServerManager.startServer();

        var timePassed = 0;
        while (!ServerManager.serverReady)
        {
            await System.Threading.Tasks.Task.Delay(25);
            timePassed += 25;   

            if (timePassed > 5000)
            {
                return false;
            }
        }
        return true;
    }
}
