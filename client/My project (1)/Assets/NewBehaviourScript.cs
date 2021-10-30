using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    int a = 0;
    public int id;
    float speed = 0.05f;
    CoordsGetter coordsGetter;
    // Start is called before the first frame update
    void Start()
    {
        print(gameObject.name);
        coordsGetter = GameObject.FindWithTag("coords").GetComponent<CoordsGetter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w")) {
            gameObject.transform.position += Vector3.forward * speed;
        }
        if (Input.GetKey("a")) {
            gameObject.transform.position += Vector3.left * speed;
        }
        if (Input.GetKey("s")) {
            gameObject.transform.position += Vector3.back * speed;
        }
        if (Input.GetKey("d")) {
            gameObject.transform.position += Vector3.right * speed;
        }
        gameObject.transform.position = coordsGetter.getPosition(id);
        
    }
    void OnGUI()
    {
        Event e = Event.current;
        if (e.control)
        {
            Debug.Log("Control was pressed.");
        }
    }
}
