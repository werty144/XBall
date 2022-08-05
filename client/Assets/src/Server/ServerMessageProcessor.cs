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

    static IFormatterResolver resolver = MessagePack.Resolvers.CompositeResolver.Create(
            MessagePack.Resolvers.BuiltinResolver.Instance,

            // replace enum resolver
            MessagePack.Resolvers.DynamicEnumAsStringResolver.Instance,

            MessagePack.Resolvers.DynamicGenericResolver.Instance,

            // final fallback(last priority)
            MessagePack.Resolvers.DynamicContractlessObjectResolver.Instance
        );
    static MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(resolver);


    public static void processServerMessages(IEnumerable<string> messages)
    {
        string lastGameMessage = null;
        foreach (string message in messages)
        {
            var addressee = Convert.ToUInt64(getStringProperty(message, "addressee"));
            if (SteamAuth.isMe(addressee))
            {
                var path = getStringProperty(message, "path");
                if (path == "game")
                {
                    lastGameMessage = message;
                } else
                {
                    processServerMessage(message);
                }
            } else
            {
                SteamP2P.sendMessage(message, addressee);
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
            string path = getStringProperty(message, "path");
            byte[] bin = MessagePackSerializer.ConvertFromJson(message);
            ulong addressee;
            switch (path)
            {
                case "serverReady":
                    var readyMessage = MessagePackSerializer.Deserialize<ApiServerReady>(bin, options);
                    Log.Info(string.Format("Server started at port: {0}", readyMessage.port));
                    ServerManager.OnServerReady(readyMessage.port);
                    break;
                case "game":
                    var gameMessage = MessagePackSerializer.Deserialize<ApiGameInfoAddressee>(bin, options);
                    addressee = Convert.ToUInt64(gameMessage.addressee);
                    if (SteamAuth.isMe(addressee))
                    {
                        var gameInfo = gameMessage.body;
                        GameManager.setGameInfo(gameInfo);
                    } else
                    {
                        SteamP2P.sendMessage(message, addressee);
                    }
                    break;
                case "prepareGame":
                    var prepareGameMessage = MessagePackSerializer.Deserialize<ApiPrepareGameAddressee>(bin, options);
                    addressee = Convert.ToUInt64(prepareGameMessage.addressee);
                    if (SteamAuth.isMe(addressee))
                    {
                        var body = prepareGameMessage.body;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => MainMenu.prepareGame(body.game.state, body.side));
                    } else 
                    {
                        SteamP2P.sendMessage(message, addressee);
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

    static string getStringProperty(string message, string key)
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
