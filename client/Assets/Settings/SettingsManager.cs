using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static Dictionary<string, KeyCode> actionsToKey = new Dictionary<string, KeyCode>();

    void Start()
    {
        loadActionsToKey();
    }

    void loadActionsToKey()
    {
        actionsToKey["attack"] = KeyCode.R;
        actionsToKey["stop"] = KeyCode.S;
        actionsToKey["throw"] = KeyCode.Q;
        actionsToKey["grab"] = KeyCode.W;
        actionsToKey["turn"] = KeyCode.E;
        actionsToKey["bend"] = KeyCode.D;
        actionsToKey["switchPlayer"] = KeyCode.Tab;
    }
}
