using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    public float moveSpeed;
    public float rotationLerpValue;

    Vector3 velocity;

    void Start()
    {
        gameObject.transform.SetPositionAndRotation(target.position, target.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.SetPositionAndRotation(
            Vector3.SmoothDamp(gameObject.transform.localPosition, target.position, ref velocity, moveSpeed), 
            Quaternion.Slerp(gameObject.transform.localRotation, target.rotation, rotationLerpValue));
    }
}
