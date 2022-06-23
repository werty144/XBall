using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using Newtonsoft.Json;


using static ServerManager;


public class RequestCreator
{
    public static bool isHost;

    const string makeMove = "makeMove";
    const string movement = "movement";
    const string grab = "grab";
    const string throwString = "throw";
    const string attack = "attack";
    const string orientation = "orientation";
    const string stop = "stop";
    const string bend = "bend";

    public static void createMoveRequest(GameObject player, string action, dynamic actionData)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = makeMove,
                body = new
                {
                    addressant = SteamAuth.GetSteamID().ToString(),
                    playerId = player.GetComponent<PlayerController>().id,
                    action = action,
                    actionData = actionData
                }
            }
            );
        sendRequest(request);
    }

    public static void moveRequest(GameObject player, Point point)
    {
        createMoveRequest(player, movement, point);
    }

    public static void grabRequest(GameObject player)
    {
        createMoveRequest(player, grab, new{});
    }

    public static void throwRequest(GameObject player, Point point)
    {
        createMoveRequest(player, throwString, point);
    }

    public static void orientationRequest(GameObject player, Point point)
    {
        createMoveRequest(player, orientation, point);
    }

    public static void attackRequest(GameObject player)
    {
        createMoveRequest(player, attack, new {});
    }

    public static void bendRequest(GameObject player, Point point)
    {
        createMoveRequest(player, bend, point);
    }

    public static void stopRequest(GameObject player)
    {
        createMoveRequest(player, stop, new {});    
    }

    // public static void readyRequest()
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
                path = "lobbyReady",
                body = new
                {
                    // Can't deserialize ulong on the server
                    id = lobbyData.ID.ToString(),
                    nMembers = lobbyData.membersData.Count,
                    gameProperties = new
                    {
                        speed = lobbyData.metaData.speed,
                        playersNumber = lobbyData.metaData.playersNumber
                    },
                    // Can't deserialize ulong on the server
                    members = lobbyData.membersData.ConvertAll(data => data.ID.ToString())
                }
            }
            );
        sendRequest(request);
    }

    private static void sendRequest(string request)
    {
        if (isHost)
        {
            ServerManager.messages.Enqueue(request);
        } else 
        {
            SteamP2P.sendMessage(request);
        }
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

