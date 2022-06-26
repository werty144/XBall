using System.IO;
using System.Diagnostics;


using log4net;
using log4net.Config;
using UnityEngine;

public class LoggingConfiguration
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Configure()
    {
        log4net.GlobalContext.Properties["LogFileName"] = $"{Application.streamingAssetsPath}/log.txt";
        XmlConfigurator.Configure(new FileInfo($"{Application.streamingAssetsPath}/log4net.xml"));

        if(!log4net.LogManager.GetRepository().Configured)
        {
            UnityEngine.Debug.Log("Not configured");
        }
    }
    
}