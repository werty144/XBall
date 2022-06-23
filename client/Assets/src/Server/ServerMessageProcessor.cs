using System.Collections;
using System.Collections.Generic;
using System;


using UnityEngine;
using Newtonsoft.Json;

public class ServerMessageProcessor 
{
    public static void processServerMessage(string message)
    {   
        try 
        {
            // If compiler not working on this line you just need to set the Api Compatibility Level to .Net 4.x in your Player Settings
            dynamic json = JsonConvert.DeserializeObject(message);
            ulong addressee;
            switch ((string) json.path)
            {
                case "serverReady":
                    int port = json.port;
                    ServerManager.setServerReady(port);
                    break;
                case "game":
                    var gameMessage = JsonConvert.DeserializeObject<ApiGameInfoAddressee>(message);
                    var gameInfo = gameMessage.body;
                    addressee = gameMessage.addressee;
                    GameManager.setGameInfo(gameInfo);
                    break;
                case "prepareGame":
                    var prepareGameMessage = JsonConvert.DeserializeObject<ApiPrepareGameAddressee>(message);
                    var body = prepareGameMessage.body;
                    addressee = prepareGameMessage.addressee;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => MainMenu.prepareGame(body.game.state, body.side));
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
