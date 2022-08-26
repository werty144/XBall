using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;


using UnityEngine;
using log4net;
using MessagePack;



public class ServerMessageProcessor 
{
    public static bool isHost;
    public static readonly ILog Log = LogManager.GetLogger(typeof(ServerMessageProcessor));


    public static void processServerMessages(IEnumerable<string> messages)
    {
        string lastGameMessage = null;
        foreach (string messageWithAddresse in messages)
        {
            PerformanceTracker.MessagesFromServer += 1;
            string[] splitted = messageWithAddresse.Split('\n');
            var addressee = Convert.ToUInt64(splitted[0]);
            string message = splitted[1]; 
            if (SteamAuth.isMe(addressee))
            {
                var path = getJSONStringProperty(message, "path");
                if (path == "game")
                {
                    lastGameMessage = message;
                } else
                {
                    processServerMessageToMe(message);
                }
            } else
            {
                SteamP2P.sendMessage(messageWithAddresse, addressee);
            }
        }
        if (lastGameMessage != null)
        {
            processServerMessageToMe(lastGameMessage);
        }
    }


    public static void processServerMessageToMe(string message)
    {   
        try 
        {
            string path = getJSONStringProperty(message, "path");
            byte[] bin = MessagePackSerializer.ConvertFromJson(message);
            switch (path)
            {
                case "serverReady":
                    var readyMessage = MessagePackSerializer.Deserialize<ApiServerReady>(bin);
                    Log.Info(string.Format("Server started at port: {0}", readyMessage.port));
                    ServerManager.OnServerReady(readyMessage.port);
                    break;
                case "game":
                    var gameMessage = MessagePackSerializer.Deserialize<ApiGameInfo>(bin);
                    var gameInfo = gameMessage.body;
                    GameManager.setGameInfo(gameInfo);
                    break;
                case "prepareGame":
                    var prepareGameMessage = MessagePackSerializer.Deserialize<ApiPrepareGame>(bin);          
                    var body = prepareGameMessage.body;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => GameStarter.prepareGame(body.game.state, body.side));
                    break;
                case "startGame":
                    GameStarter.startGame();
                    break;
                case "cancelGame":
                    GameStarter.cancelGame();
                    break;
                case "endGame":
                    GameObject.Find("UIController").GetComponent<GameUIController>().onGameEnd();
                    break;
                default:
                    break;
            }
        } catch (Exception e)
        {
            Log.Error(string.Format("Error when parsing {0}! {1}", message, e));
        }
    }

    static string getJSONStringProperty(string message, string key)
    {
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
