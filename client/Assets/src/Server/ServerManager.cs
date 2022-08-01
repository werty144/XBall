using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text; 
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;
using log4net;
using MessagePack;


using static GameManager;
using static MainMenu;


public class ServerManager : MonoBehaviour
{
    private static Process serverProcess = null;
    private static int port;
    private static HttpClient client = new HttpClient();
    public static bool serverReady = false;
    public static Queue<string> messages = new Queue<string>();
    public static readonly ILog Log = LogManager.GetLogger(typeof(ServerManager));

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
    }

    private static void serverExited(object sender, System.EventArgs e)
    {
        Log.Info(string.Format("Server process exited with code: {0}", serverProcess.ExitCode));
    }

    public static void OnServerReady(int port)
    {
        SocketConnection.Connect(port);
    }

    public static void OnConnectionOpen()
    {
        serverReady = true;
    }

    public static async void killServer()
    {
        SocketConnection.Close();

        var timeSpent = 0;
        while (!SocketConnection.isClosedOrNull())
        {
            await Task.Delay(25);
            timeSpent += 25;
            if (timeSpent > 5000)
            {
                Log.Error("Server doesn't close the connection, killing");
                break;
            }
        }

        if ((serverProcess != null) && (!serverProcess.HasExited))
        {
            serverProcess.Kill();
        }
        serverReady = false;
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
        ServerMessageProcessor.processServerMessage(output);
    }

    public static void sendMessageToServer(string message)
    {
        SocketConnection.messagesToServer.Enqueue(message);
    }
}



