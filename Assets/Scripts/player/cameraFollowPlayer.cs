using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollowPlayer : MonoBehaviour
{
    public Transform player;
    public float smoothing;
    public Vector3 offset;

    private void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 newpos = Vector3.Lerp(transform.position, player.transform.position + offset, smoothing);
            transform.position = newpos;
        }
    }
}
