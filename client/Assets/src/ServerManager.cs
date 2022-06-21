using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text; 
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

using static GameManager;
using static RequestCreator;


public class ServerManager : MonoBehaviour
{
    private static Process serverProcess = null;
    private static Task dispatchMessagesTask = null;
    private static HttpClient client = new HttpClient();
    public static bool serverReady = false;
    public static Queue<Message> messages = new Queue<Message>();


    public static void startServer()
    {
        serverProcess = new Process();
        serverProcess.StartInfo.FileName = "java";
        serverProcess.StartInfo.Arguments = "-jar /home/anton/coding/XBall/server/localServer/build/libs/localServer-0.0.1-all.jar";
        serverProcess.StartInfo.UseShellExecute = false;
        serverProcess.StartInfo.RedirectStandardOutput = true;
        serverProcess.EnableRaisingEvents = true;
        serverProcess.Exited += serverExited;
        serverProcess.OutputDataReceived += (sender, args) => processServerOutput(args.Data);
        serverProcess.Start();
        serverProcess.BeginOutputReadLine();
        dispatchMessagesTask = new Task(dispatchMessages);
        dispatchMessagesTask.Start();
    }

    private static void serverExited(object sender, System.EventArgs e)
    {
        print(string.Format("Server process exited with code: {0}", serverProcess.ExitCode));
    }

    public void killServer()
    {
        if ((serverProcess != null) && (!serverProcess.HasExited))
        {
            serverProcess.Kill();
        }
        serverReady = false;
        if (dispatchMessagesTask != null)
        {
            dispatchMessagesTask.Dispose();
            dispatchMessagesTask = null;
        }
    }

    void OnApplicationQuit()
    {
        killServer();
    }

    static void processServerOutput(string output)
    {
        if (output == null)
        {
            // Process terminated
            return;
        }
        try 
        {
            // If compiler not working on this line you just need to set the Api Compatibility Level to .Net 4.x in your Player Settings
            dynamic json = JsonConvert.DeserializeObject(output);
            ulong addressee;
            switch ((string) json.path)
            {
                case "serverReady":
                    serverReady = true;
                    break;
                case "game":
                    var gameMessage = JsonConvert.DeserializeObject<ApiGameInfoAddressee>(output);
                    var gameInfo = gameMessage.body;
                    addressee = gameMessage.addressee;
                    GameManager.setGameInfo(gameInfo);
                    break;
                case "prepareGame":
                    var prepareGameMessage = JsonConvert.DeserializeObject<ApiPrepareGameAddressee>(output);
                    var body = prepareGameMessage.body;
                    addressee = prepareGameMessage.addressee;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => MainMenu.prepareGame(body.game.state, body.side));
                    break;
                default:
                    break;
            }
        } catch (Exception e)
        {
            print(string.Format("Error when parsing {0}! {1}", output, e));
        }
        
    }

    private static async void dispatchMessages()
    {
        while (true)
        {
            await Task.Delay(5);
            while (messages.Count > 0)
            {
                var message = messages.Dequeue();
                await client.PostAsync(
                "http://localhost:8080/" + message.endPoint,
                new StringContent(message.content, Encoding.UTF8, "application/json")
            );
            }
        }
    }
}

public class ApiGameInfoAddressee
{
  public string path;
  public ulong addressee;
  public GameInfo body;
}

public class ApiPrepareGameAddressee
{
	public string path;
    public ulong addressee;
	public PrepareGameBody body;
}


