using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SocketConnection;

public class MainMenu : MonoBehaviour
{
    public void autoInvite() {
        SocketConnection.messages.Enqueue("{\"path\": \"invite\", \"body\": {\"invitedId\": 0, \"speed\": \"FAST\", \"playersNumber\": 3}}");
    }
}
