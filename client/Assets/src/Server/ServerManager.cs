using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text; 
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;


using static GameManager;
using static MainMenu;


public class ServerManager : MonoBehaviour
{
    private static Process serverProcess = null;
    private static Task dispatchMessagesTask = null;
    private static int port;
    private static HttpClient client = new HttpClient();
    public static bool serverReady = false;
    public static Queue<string> messages = new Queue<string>();


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

    public static void setServerReady(int port_)
    {
        port = port_;
        serverReady = true;
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
        ServerMessageProcessor.processServerMessage(output);
    }

    private static async void dispatchMessages()
    {
        while (true)
        {
            await Task.Delay(5);
            while (messages.Count > 0)
            {
                var message = messages.Dequeue();
                var url = string.Format("http://localhost:{0}/", port);
                await client.PostAsync(
                    url,
                    new StringContent(message, Encoding.UTF8, "application/json")
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


