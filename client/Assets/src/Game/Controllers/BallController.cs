using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static GameManager;


public class BallController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.state != null) {
            gameObject.transform.position = new Vector3
            (
                GameManager.state.ballState.x,
                GameManager.state.ballState.z,
                GameManager.state.ballState.y
            );
        }
    }
}
