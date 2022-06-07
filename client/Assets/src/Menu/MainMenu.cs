using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Net;
using System;
using UnityEngine.SceneManagement;


using static SocketConnection;
using static InviteReceiver;
using static SteamAuth;
using static LobbyViewContoller;


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

    public static void lobbyReady(ulong lobbyID, int nMembers, string speed, int playersNumber)
    {
        print("Lobby ready call");
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = "lobbyReady",
                body = new 
                {
                    lobbyID = lobbyID,
                    nMembers = nMembers,
                    gameProperties = new
                    {
                        speed = speed,
                        playersNumber = playersNumber
                    }
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public static void test()
    {
        SceneManager.LoadScene("GameScene");
    }

    public static void log(string log)
    {
        var logger = GameObject.Find("LoggerContent");
        if (logger != null)
        {   
            logger.GetComponent<LoggerViewController>().addLog(log);
        }
    }
}