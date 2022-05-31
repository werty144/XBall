using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Net;


using static SocketConnection;
using static InviteReceiver;
using static SteamAuth;



public class MainMenu : MonoBehaviour
{
    public void autoInvite()
    {
        sendInvite(0, "FAST", 3);
    }

    public static void sendInvite(int invitedId, string speed, int playersNumber)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = "invite", 
                body = new
                {
                    invitedId = invitedId,
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

    public static void authenticate(string ticket)
    {
        WebClient webClient = new WebClient();
        webClient.QueryString.Add("key", "E5E11C2593D64822EB9F4F29FE04B470");
        webClient.QueryString.Add("appid", "480");
        webClient.QueryString.Add("ticket", ticket);
        string result = webClient.DownloadString("https://partner.steam-api.com/ISteamUserAuth/AuthenticateUserTicket/v1/");
        print(result);
    }

    public static void test()
    {
        SteamAuth.authenticate();
    }
}