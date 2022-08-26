using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


using UnityEngine;
using Steamworks;
using log4net;


using static MainMenu;


public class SteamP2P : MonoBehaviour
{
    const int channelToUse = 0;
    private static CSteamID interlocutor;
    public static readonly ILog Log = LogManager.GetLogger(typeof(SteamP2P));
    void OnEnable()
    {
        Callback<SteamNetworkingMessagesSessionRequest_t>.Create(OnFirstMessage);
        Callback<SteamNetworkingMessagesSessionFailed_t>.Create(OnConnectionFailed);
    }

    void OnConnectionFailed(SteamNetworkingMessagesSessionFailed_t pCallbcak)
    {
        Log.Error(string.Format("P2P connection failed. End reason: {0}", pCallbcak.m_info.m_eEndReason));
    }

    void OnFirstMessage(SteamNetworkingMessagesSessionRequest_t pCallback)
    {
        if (P2PReceiver.acceptSessionRequest(pCallback.m_identityRemote.GetSteamID().m_SteamID))
        {
            interlocutor = pCallback.m_identityRemote.GetSteamID();
            SteamNetworkingMessages.AcceptSessionWithUser(ref pCallback.m_identityRemote);
        }
    }

    public static Queue<string> receiveMessages(int nMaxMessages)
    {
        IntPtr[] messages = new IntPtr[nMaxMessages];
        int nMessages = SteamNetworkingMessages.ReceiveMessagesOnChannel(channelToUse, messages, nMaxMessages);
        PerformanceTracker.P2PReceived.Add(nMessages);
        
        Queue<string> messageQueue = new Queue<string>();
        for (int i = 0; i < nMessages; i++)
        {
            SteamNetworkingMessage_t msgStruct = (SteamNetworkingMessage_t) Marshal.PtrToStructure(messages[i], typeof(SteamNetworkingMessage_t));
            string msg = Marshal.PtrToStringAuto(msgStruct.m_pData);
            messageQueue.Enqueue(msg);
            SteamNetworkingMessage_t.Release(messages[i]);
        }
        return messageQueue;
    }

    public static void sendMessage(string msg, CSteamID steamID)
    {
        SteamNetworkingIdentity identity = new SteamNetworkingIdentity();
        identity.SetSteamID(steamID);
        // need to end null symbol in order to parse
        byte[] sendData = Encoding.UTF8.GetBytes(msg + "\0");
        GCHandle pinnedArray = GCHandle.Alloc(sendData, GCHandleType.Pinned);
        IntPtr pointer = pinnedArray.AddrOfPinnedObject();
        EResult res = SteamNetworkingMessages.SendMessageToUser(ref identity, pointer, (uint)sendData.Length, 0, channelToUse);
    }

    public static void sendMessage(string msg, ulong ID)
    {
        sendMessage(msg, new CSteamID(ID));
    }

    public static void sendMessage(string msg)
    {
        sendMessage(msg, interlocutor);
    }

    public static void sendInitialMessage(ulong lobbyOwnerID)
    {
        interlocutor = new CSteamID(lobbyOwnerID);
        sendMessage("ping");
    }
}
