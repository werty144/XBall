using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    static bool instantiated = false;
    void Awake()
    {
        if (!instantiated)
        {
            DontDestroyOnLoad(this);
            instantiated = true;
        } else 
        {
            Destroy(gameObject);
        }
    }
}
