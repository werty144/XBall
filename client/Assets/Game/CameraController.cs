using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static GameConstants;


public class CameraController : MonoBehaviour
{
    float movingSpeed = 10f;
    Vector3 movingDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        performMovement();
        movingDirection = Vector3.zero;
    }

    public void setMovingDirection(Vector3 direction)
    {
        movingDirection = direction;
    }

    void performMovement()
    {
        if (movingDirection != Vector3.zero)
        {
            transform.Translate(movingDirection * Time.deltaTime * movingSpeed);
        }
    }
}
