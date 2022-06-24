using System.IO;


using log4net;
using log4net.Config;
using UnityEngine;

public class LoggingConfiguration
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Configure()
    {
        log4net.GlobalContext.Properties["LogFileName"] = $"{Application.dataPath}/src/Log/log.txt";
        XmlConfigurator.Configure(new FileInfo($"{Application.dataPath}/src/Log/log4net.xml"));
        // XmlConfigurator.Configure(new FileInfo("/home/anton/coding/XBall/client/Assets/src/Log/log4net.xml"));
    }
    
}
