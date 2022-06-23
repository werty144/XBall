using System.Collections;
using System.Collections.Generic;
using System;


using UnityEngine;
using Newtonsoft.Json;

public class ServerMessageProcessor 
{
    public static bool isHost;
    public static void processServerMessage(string message)
    {   
        try 
        {
            // If compiler not working on this line you just need to set the Api Compatibility Level to .Net 4.x in your Player Settings
            dynamic json = JsonConvert.DeserializeObject(message);
            switch ((string) json.path)
            {
                case "serverReady":
                    int port = json.port;
                    ServerManager.setServerReady(port);
                    break;
                case "game":
                    var gameMessage = JsonConvert.DeserializeObject<ApiGameInfoAddressee>(message);
                    if (SteamAuth.isMe(gameMessage.addressee))
                    {
                        var gameInfo = gameMessage.body;
                        GameManager.setGameInfo(gameInfo);
                    } else
                    {
                        SteamP2P.sendMessage(message, gameMessage.addressee);
                    }
                    break;
                case "prepareGame":
                    var prepareGameMessage = JsonConvert.DeserializeObject<ApiPrepareGameAddressee>(message);
                    if (SteamAuth.isMe(prepareGameMessage.addressee))
                    {
                        var body = prepareGameMessage.body;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => MainMenu.prepareGame(body.game.state, body.side));
                    } else 
                    {
                        SteamP2P.sendMessage(message, prepareGameMessage.addressee);
                    }
                    break;
                default:
                    break;
            }
        } catch (Exception e)
        {
            Debug.Log(string.Format("Error when parsing {0}! {1}", message, e));
        }
    }
}
