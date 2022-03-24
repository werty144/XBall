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
    string orientation = "orientation";
    string stop = "stop";
    string bend = "bend";

    public void createMoveRequest(GameObject player, string action, dynamic actionData)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = makeMove, 
                body = new
                {
                    move = new
                    {
                        playerId = player.GetComponent<PlayerController>().id,
                        action = action,
                        actionData = actionData
                    }
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public void moveRequest(GameObject player, Point point)
    {
        createMoveRequest(player, movement, point);
    }

    public void grabRequest(GameObject player)
    {
        createMoveRequest(player, grab, new{});
    }

    public void throwRequest(GameObject player, Point point)
    {
        createMoveRequest(player, throwString, point);
    }

    public void orientationRequest(GameObject player, Point point)
    {
        createMoveRequest(player, orientation, point);
    }

    public void attackRequest(GameObject player)
    {
        createMoveRequest(player, attack, new {});
    }

    public void bendRequest(GameObject player, Point point)
    {
        createMoveRequest(player, bend, point);
    }

    public void stopRequest(GameObject player)
    {
        createMoveRequest(player, stop, new {});    
    }
}

