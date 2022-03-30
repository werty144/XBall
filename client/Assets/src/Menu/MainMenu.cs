using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


using static SocketConnection;

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
}
