using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using UnityEngine;
using Steamworks;

using static MainMenu;

public class SteamP2P : MonoBehaviour
{
    const int channelToUse = 0;
    void OnEnable()
    {
        Callback<SteamNetworkingMessagesSessionRequest_t>.Create(OnFirstMessage);
        Callback<SteamNetworkingMessagesSessionFailed_t>.Create(OnConnectionFailed);
    }

    void OnConnectionFailed(SteamNetworkingMessagesSessionFailed_t pCallbcak)
    {
        MainMenu.log(string.Format("End reason: {0}", pCallbcak.m_info.m_eEndReason));
    }

    void OnFirstMessage(SteamNetworkingMessagesSessionRequest_t pCallback)
    {
        bool res = SteamNetworkingMessages.AcceptSessionWithUser(ref pCallback.m_identityRemote);
        MainMenu.log(string.Format("Accepted: {0}", res));
        receiveMessages(1);
    }

    void receiveMessages(int nMaxMessages)
    {
        IntPtr[] messages = new IntPtr[nMaxMessages];
        int nMessages = SteamNetworkingMessages.ReceiveMessagesOnChannel(channelToUse, messages, nMaxMessages);

        for (int i = 0; i < nMessages; i++)
        {
            SteamNetworkingMessage_t msgStruct = (SteamNetworkingMessage_t) Marshal.PtrToStructure(messages[i], typeof(SteamNetworkingMessage_t));
            string msg = Marshal.PtrToStringAuto(msgStruct.m_pData);
            MainMenu.log(string.Format("Message: {0}", msg));
            SteamNetworkingMessage_t.Release(messages[i]);
        }
    }

    public static void sendMessage(string msg, CSteamID steamID)
    {
        SteamNetworkingIdentity identity = new SteamNetworkingIdentity();
        identity.SetSteamID(steamID);
        // need to end null symbol in order to parse on the other end
        byte[] sendData = Encoding.UTF8.GetBytes(msg + "\0");
        GCHandle pinnedArray = GCHandle.Alloc(sendData, GCHandleType.Pinned);
        IntPtr pointer = pinnedArray.AddrOfPinnedObject();
        EResult res = SteamNetworkingMessages.SendMessageToUser(ref identity, pointer, (uint)sendData.Length, 0, channelToUse);
        MainMenu.log(string.Format("Message sent: {0}", (res)));
    }
}
