using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    private float xOffset;
    [SerializeField] private Transform target;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal < 0f) {
            xOffset = -1f;
        } 
        if (horizontal > 0f) {
            xOffset = 1f;
        }


        offset = new Vector3(xOffset * 5f, 2f, -10f);

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
