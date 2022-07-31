using System.Collections;
using System.Collections.Generic;
using System;


using UnityEngine;
using Newtonsoft.Json;
using log4net;


public class ServerMessageProcessor 
{
    public static bool isHost;
    public static readonly ILog Log = LogManager.GetLogger(typeof(ServerMessageProcessor));


    public static void processServerMessage(string message)
    {   
        try 
        {
            string path = message.Split(new string[] { "path\":\"" }, StringSplitOptions.None)[1].Split('\"')[0];
            switch (path)
            {
                case "serverReady":
                    var readyMessage = JsonConvert.DeserializeObject<ApiServerReady>(message);
                    Log.Info(string.Format("Server started at port: {0}", readyMessage.port));
                    ServerManager.OnServerReady(readyMessage.port);
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
            Log.Error(string.Format("Error when parsing {0}! {1}", message, e));
        }
    }
}
