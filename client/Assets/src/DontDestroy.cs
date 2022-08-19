using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    static bool instantiated = false;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (!instantiated)
        {
            instantiated = true;
        } else 
        {
            Destroy(gameObject);
        }
    }
}
