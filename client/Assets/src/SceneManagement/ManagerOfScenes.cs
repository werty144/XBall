using System.Threading.Tasks;


using UnityEngine.SceneManagement;
using UnityEngine;
using log4net;

public class ManagerOfScenes : MonoBehaviour
{
    public static GameObject loaderCanvas;
    private static AsyncOperation scene;
    public static readonly ILog Log = LogManager.GetLogger(typeof(ManagerOfScenes));

    void Awake()
    {
        loaderCanvas = GameObject.Find("Global/ManagerOfScenes/Canvas");
    }

    public static void loadScene(string sceneName)
    {
        scene = SceneManager.LoadSceneAsync(sceneName);
    }

    public static bool sceneReady()
    {
        return scene.isDone;
    }

    public static void activateOverlay()
    {
        loaderCanvas.SetActive(true);
    }

    public static void deactivateOverlay()
    {
        loaderCanvas.SetActive(false);
    }
}
