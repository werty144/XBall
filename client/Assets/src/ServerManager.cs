using System;
using System.Diagnostics;
using UnityEngine;


public class ServerManager : MonoBehaviour
{
    private Process serverProcess;


    public void startServer()
    {
        serverProcess = new Process();
        serverProcess.StartInfo.FileName = "java";
        serverProcess.StartInfo.Arguments = "-jar /home/anton/coding/XBall/server/build/libs/example-0.0.1-all.jar";
        serverProcess.EnableRaisingEvents = true;
        serverProcess.Exited += serverExited;
        serverProcess.Start();
    }

    private void serverExited(object sender, System.EventArgs e)
    {
        print(string.Format("Server process exited with code: {0}", serverProcess.ExitCode));
    }

    public void killServer()
    {
        if ((serverProcess != null) && (!serverProcess.HasExited))
        {
            serverProcess.Kill();
        }
    }

    void OnApplicationQuit()
    {
        killServer();
    }
}
