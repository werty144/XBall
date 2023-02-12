using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using log4net;

public class InputManager : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(InputManager));
    [SerializeField]
    public GameObject ball;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ball.GetComponent<Rigidbody>().velocity = ball.GetComponent<Rigidbody>().velocity + 10 * Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            ball.GetComponent<Rigidbody>().velocity = ball.GetComponent<Rigidbody>().velocity + 10 * Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            ball.GetComponent<Rigidbody>().velocity = ball.GetComponent<Rigidbody>().velocity + 10 * Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            ball.GetComponent<Rigidbody>().velocity = ball.GetComponent<Rigidbody>().velocity + 10 * Vector3.back;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            ball.GetComponent<Rigidbody>().velocity = ball.GetComponent<Rigidbody>().velocity + 10 * Vector3.right;
        }
    }
}
