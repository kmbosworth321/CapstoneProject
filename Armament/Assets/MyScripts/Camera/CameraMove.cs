﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;
    public float distance;

    private const bool DEBUG = false; // flag the indicates whether we are debugging this script
    private const bool DEBUG_Update = true;

    private float moveSpeed = 500.0f;
    private bool timeToMoveBack = false;
    private bool timeToMoveForward = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        distance = transform.position.z - target.position.z;

        if (timeToMoveBack && distance > 5)
        {
            if (DEBUG && DEBUG_Update) Debug.LogFormat("CameraMove: Update() timeToMoveBack = {1}, distance = {0}", distance, timeToMoveBack);
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

        else if (timeToMoveForward && distance < 940)
        {
            if (DEBUG && DEBUG_Update) Debug.LogFormat("CameraMove: Update() timeToMoveForward = {1}, distance = {0}", distance, timeToMoveForward);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    public void MoveBack()
    {
        timeToMoveForward = false;
        timeToMoveBack = true;
    }

    public void MoveForward() {
        timeToMoveBack = false;
        timeToMoveForward = true;
    }

    public void AssignTarget(Transform t)
    {
        target = t;
    }
}
