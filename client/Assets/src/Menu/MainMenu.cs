using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Net;
using System;


using static SocketConnection;
using static InviteReceiver;
using static SteamAuth;


public class MainMenu : MonoBehaviour
{
    public static ulong myID;


    void Start()
    {
        myID = SteamAuth.GetSteamID();
    }


    public void autoInvite()
    {
        sendInvite(myID, "FAST", 3);
    }

    public static void sendInvite(ulong invitedId, string speed, int playersNumber)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = "invite", 
                body = new
                {
                    invitedId = invitedId.ToString(),
                    speed = speed,
                    playersNumber = playersNumber
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public static void sendInviteBot(string speed, int playersNumber)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = "inviteBot", 
                body = new
                {
                    invitedId = -1,
                    speed = speed,
                    playersNumber = playersNumber
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public static void receiveInvite(Invite invite)
    {
        InviteReceiver.receiveInvite(invite);
    }

    public static void acceptInvite(int inviteId)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = "acceptInvite",
                body = new 
                {
                    inviteId = inviteId
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public static void test()
    {
        LobbyManager.autoInvite();
    }
}