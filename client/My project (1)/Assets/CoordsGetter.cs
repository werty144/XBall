using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.WebSockets;

public class CoordsGetter : MonoBehaviour
{

    private float time = 0;
    public float radius = 2.0f;
    public Queue<string> messages = new Queue<string>();
    SocketConnection socketConnection;
    // Start is called before the first frame update
    void Start()
    {
        socketConnection = GameObject.FindWithTag("connection").GetComponent<SocketConnection>();

    }

    // Update is called once per frame
    void Update()
    {
        time += 1;
        if (Input.GetKey("i")) {
            messages.Enqueue("{\"path\": \"invite\", \"body\": {\"invitedId\": 0}}");
        }
    }

    public Vector3 getPosition(int id) {
        if (id == 1) {
            return new Vector3(radius * (float) Math.Sin(time / 1000), 0.0f, radius * (float) Math.Cos(time / 1000));
        }
        if (id == 2) {
            return new Vector3(radius * (float) Math.Sin(time / 1000 + 2), (float) socketConnection.x, radius * (float) Math.Cos(time / 1000 + 2));
        }
        return new Vector3(0f, 0f, 0f);
    }
}
