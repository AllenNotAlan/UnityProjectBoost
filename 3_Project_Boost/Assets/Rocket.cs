using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); //this gives us access to the rigidbody in unity
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up); //.up applies to the y-axis. s
        }

        if (Input.GetKey(KeyCode.A))
        {
            print("Rotating Left");
        }        
        else if (Input.GetKey(KeyCode.D))
        {
            print("Rotating Right");
        }
    }
}
