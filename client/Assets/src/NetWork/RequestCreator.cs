using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// using static SocketConnection;
using static ServerManager;
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
                addressant = SteamAuth.GetSteamID().ToString(),
                playerId = player.GetComponent<PlayerController>().id,
                action = action,
                actionData = actionData
            }
            );
        ServerManager.messages.Enqueue(new Message(makeMove, request));
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

    // public void readyRequest()
    // {
    //     string request = JsonConvert.SerializeObject(
    //         new
    //         {
    //             path = "ready",
    //             body = new {}
    //         }
    //     );
    //     // SocketConnection.messages.Enqueue(request);
    //     ServerManager.messages.Enqueue(request);
    // }

    public static void lobbyReady(LobbyData lobbyData)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                id = lobbyData.ID.ToString(),
                nMembers = lobbyData.membersData.Count,
                gameProperties = new
                {
                    speed = lobbyData.metaData.speed,
                    playersNumber = lobbyData.metaData.playersNumber
                },
                members = lobbyData.membersData.ConvertAll(data => data.ID.ToString())
            }
            );
        ServerManager.messages.Enqueue(new Message("lobbyReady", request));
    }
}


public class Message
{
    public string endPoint;
    public string content;

    public Message(string endPoint_, string content_)
    {
        this.endPoint = endPoint_;
        this.content = content_;
    }
}

