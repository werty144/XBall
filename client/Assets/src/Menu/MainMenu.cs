using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


using static SocketConnection;
using static InviteReceiver;


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

    public static void test()
    {
        var invite = new Invite();
        invite.invitedId = 0;
        invite.inviteId = 0;
        invite.inviterId = 228;
        var gameProperties = new GameProperties();
        gameProperties.playersNumber = 0;
        gameProperties.speed = "ass";
        invite.gameProperties = gameProperties;
        receiveInvite(invite);
    }
}
