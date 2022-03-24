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
        Debug.Log("Sending invite!");
        SocketConnection.messages.Enqueue(request);
    }
}
