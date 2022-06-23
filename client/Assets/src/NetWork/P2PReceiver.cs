using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


using UnityEngine;

public class P2PReceiver : MonoBehaviour
{
    public static bool isHost;
    private static Task receiveMessagesTask = null;

    public static void receiveMessage(string message)
    {
        if (message == "ping")
        {
            return;
        }
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

    public static void startReceivingMessages()
    {
        receiveMessagesTask = new Task(receivingMessages);
        receiveMessagesTask.Start();
    }

    private static async void receivingMessages()
    {
        while (true)
        {
            await Task.Delay(5);
            SteamP2P.receiveMessages(8);
        }
    }

    public static void stopReceivingMessages()
    {
        receiveMessagesTask.Dispose();
    }

    void OnApplicationQuit()
    {
        stopReceivingMessages();
    }
}
