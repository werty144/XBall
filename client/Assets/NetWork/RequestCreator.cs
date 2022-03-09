using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static SocketConnection;
using Newtonsoft.Json;


public class RequestCreator
{
    string makeMove = "makeMove";
    string movement = "movement";
    string grab = "grab";
    string throwString = "throw";
    string attack = "attack";

    public void moveRequest(GameObject player, Point point)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = makeMove, 
                body = new
                {
                    move = new
                    {
                        playerId = player.GetComponent<PlayerScript>().id,
                        action = movement,
                        actionData = point
                    }
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public void grabRequest(GameObject player)
    {
        string request = JsonConvert.SerializeObject(
            new
            {
                path = makeMove,
                body = new
                {
                    move = new 
                    {
                        playerId = player.GetComponent<PlayerScript>().id,
                        action = grab,
                        actionData = new {}
                    }
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public void throwRequest(GameObject player, Point point)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = makeMove, 
                body = new
                {
                    move = new
                    {
                        playerId = player.GetComponent<PlayerScript>().id,
                        action = throwString,
                        actionData = point
                    }
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public void attackRequest(GameObject player)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = makeMove, 
                body = new
                {
                    move = new
                    {
                        playerId = player.GetComponent<PlayerScript>().id,
                        action = attack,
                        actionData = new {}
                    }
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }
}

