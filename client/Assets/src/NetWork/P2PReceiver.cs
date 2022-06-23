using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2PReceiver 
{
    public static bool isHost;

    public static void receiveMessage(string message)
    {
        if (isHost)
        {
            processMessageHost(message);
        } else
        {
            processMessageClient(message);
        }
    }

    public static bool acceptSessionRequest(ulong inviterID)
    {
        return LobbyManager.isMember(inviterID);
    }

    private static void processMessageHost(string message)
    {
        ServerManager.messages.Enqueue(message);
    }

    private static void processMessageClient(string message)
    {
        ServerMessageProcessor.processServerMessage(message);
    }
}
