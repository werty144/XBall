using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static GameConstants;
using static GameManager;


public class CameraController : MonoBehaviour
{
    float movingSpeed = 15f;
    Vector3 movingDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.side == Side.LEFT)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
            transform.position = new Vector3(3, transform.position.y, transform.position.z);
        } else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, -90, transform.eulerAngles.z);
            transform.position = new Vector3(GameConstants.fieldWidth - 3, transform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        performMovement();
        movingDirection = Vector3.zero;
    }

    public void setMovingDirection(Vector3 direction)
    {
        if (direction == Vector3.forward)
        {
            if (GameManager.side == Side.LEFT)
            {
                movingDirection = Vector3.right;
            } else
            {
                movingDirection = Vector3.left;
            }
        } else 
        {
            if (GameManager.side == Side.LEFT)
            {
                movingDirection = Vector3.left;
            } else
            {
                movingDirection = Vector3.right;
            }
        }
    }

    void performMovement()
    {
        if (movingDirection != Vector3.zero)
        {
            transform.Translate(movingDirection * Time.deltaTime * movingSpeed, Space.World);
        }
    }
}
