using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


using UnityEngine;
using log4net;


public class P2PReceiver : MonoBehaviour
{
    public static bool isHost;
    public static readonly ILog Log = LogManager.GetLogger(typeof(P2PReceiver));

    void Update()
    {
        Queue<string> messageQueue = SteamP2P.receiveMessages(8);
        receiveMessages(messageQueue);
    }

    public static void receiveMessages(Queue<string> messages)
    {
        if (isHost)
        {
            processMessagesHost(messages);
        } else
        {
            processMessagesClient(messages);
        }
    }

    public static bool acceptSessionRequest(ulong inviterID)
    {
        return LobbyManager.isMember(inviterID);
    }

    private static void processMessagesHost(Queue<string> messages)
    {
        while (messages.Count > 0)
        {
            string message = messages.Dequeue();
            if (message == "ping")
            {
                continue;
            }
            ServerManager.sendMessageToServer(message);
        }
    }

    private static void processMessagesClient(Queue<string> messages)
    {
        ServerMessageProcessor.processServerMessages(messages);
    }
}
