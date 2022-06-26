using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


using UnityEngine;
using log4net;


public class P2PReceiver : MonoBehaviour
{
    public static bool isHost;
    private static bool receivingMessages = false;
    public static readonly ILog Log = LogManager.GetLogger(typeof(P2PReceiver));

    void Update()
    {
        if (receivingMessages)
        {
            SteamP2P.receiveMessages(8);
        }
    }

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
        ServerManager.sendMessageToServer(message);
    }

    private static void processMessageClient(string message)
    {
        ServerMessageProcessor.processServerMessage(message);
    }

    public static void startReceivingMessages()
    {
        receivingMessages = true;
    }

    public static void stopReceivingMessages()
    {
        receivingMessages = false;
    }
}
