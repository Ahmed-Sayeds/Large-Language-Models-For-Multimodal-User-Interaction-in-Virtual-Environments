using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteraction : MonoBehaviour
{

    public bool moveToFlag = false;
    public Vector3 moveToPosition;
    private float speed = 5;
    public GameObject playerObject;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (moveToFlag)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveToPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, moveToPosition) < 1)
            {
                moveToFlag = false;
            }
        }
    }

}
