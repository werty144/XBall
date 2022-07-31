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

    public static void processServerMessages(IEnumerable<string> messages)
    {
        string lastGameMessage = null;
        foreach (string message in messages)
        {
            var path = getPath(message);
            if (path == "game")
            {
                lastGameMessage = message;
            } else
            {
                processServerMessage(message);
            }
        }
        if (lastGameMessage != null)
        {
            processServerMessage(lastGameMessage);
        }
    }


    public static void processServerMessage(string message)
    {   
        try 
        {
            string path = getPath(message);
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

    static string getPath(string message)
    {
        string key = "path";
        int curMatch = 0;
        int keyEnd = -1;
        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] == key[curMatch])
            {
                curMatch += 1;
                if (curMatch == key.Length)
                {
                    keyEnd = i;
                    break;
                }
            }
        }

        int valueStart = keyEnd + 4;
        int valueEnd = -1;
        for (int i = valueStart; i < message.Length; i++)
        {
            if (message[i] == '\"')
            {
                valueEnd = i;
                break;
            }
        }

        int valueLength = valueEnd - valueStart;
        string path = message.Substring(valueStart, valueLength); 
        return path;
    }
}
