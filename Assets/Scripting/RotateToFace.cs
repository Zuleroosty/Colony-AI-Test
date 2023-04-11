using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFace : MonoBehaviour
{
    public bool followCamera;
    public Transform target;

    void Update()
    {
        if (followCamera) target = GameObject.Find("Main Camera").transform;
        if (target != null)
        {
            Quaternion rotation = Quaternion.LookRotation((target.position - transform.position).normalized);
            transform.rotation = rotation;
        }
    }
}
