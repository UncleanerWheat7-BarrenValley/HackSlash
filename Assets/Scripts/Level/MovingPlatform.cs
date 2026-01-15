using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float XTargetOffset;
    public float YTargetOffset;
    public float ZTargetOffset;
    public float speed = 1;

    Vector3 startPosition;
    Vector3 targetPosition;

    Vector3 tempTargetPosition;
    private bool leaving = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        targetPosition = transform.position + new Vector3(XTargetOffset, YTargetOffset, ZTargetOffset);
        tempTargetPosition = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, tempTargetPosition) > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, tempTargetPosition, Time.deltaTime * speed);
        }
        else
        {
            leaving = !leaving;
            if (leaving)
            {
                tempTargetPosition = targetPosition;
            }
            else
            {
                tempTargetPosition = startPosition;
            }
        }
    }
}