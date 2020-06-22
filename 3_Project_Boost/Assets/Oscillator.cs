using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] //tells the script that we are only allowed one of them on the object in question
public class Oscillator : MonoBehaviour
{

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);//moves 10 in x, 10 in y, etc and do it in a peroid of 2 seconds (period = 2f, see below).
    // Start is called before the first frame update
    [SerializeField] float period = 2f;
    float movementFactor; //0 for not moved, 1 for fully moved.
    Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //todo set movement factor automatically
        if (period <= Mathf.Epsilon)
        {
            return;
        }
        float cycles = Time.time / period; // if game time = 10s  then cycle = 5. Ie it cycles 5 times in 5 seconds.
        //print(Time.time);
        const float tau = Mathf.PI * 2; //2Pi means it's a full cycle of a sign wave ie, 1 period.
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
