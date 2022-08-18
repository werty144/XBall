using System.Diagnostics;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using log4net;


public class ServerManager : MonoBehaviour
{
    private static Process serverProcess = null;
    private static int port;
    private static HttpClient client = new HttpClient();
    private static bool serverUp = false;
    public static Queue<string> messages = new Queue<string>();
    public static readonly ILog Log = LogManager.GetLogger(typeof(ServerManager));

    public static void configureAndStart()
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

    public static async Task<bool> launchServer()
    {
        configureAndStart();

        var timePassed = 0;
        while (!isServerUp())
        {
            await Task.Delay(25);
            timePassed += 25;   

            if (timePassed > 5000)
            {
                return false;
            }
        }
        return true;
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
        serverUp = true;
    }

    public static bool isServerUp()
    {
        return serverUp;
    }

    public static async Task killServer()
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

            while (!serverProcess.HasExited)
            {
                await Task.Delay(25);
            }
            serverProcess = null;
        }
        serverUp = false;
    }

    async void OnApplicationQuit()
    {
        await killServer();
    }

    static void processServerOutput(string output)
    {
        if (output == null)
        {
            // Process terminated
            return;
        }
        ServerMessageProcessor.processServerMessageToMe(output);
    }

    public static void sendMessageToServer(string message)
    {
        SocketConnection.messagesToServer.Enqueue(message);
    }
}



