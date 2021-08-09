using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimap : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        Vector3 newpos = player.position;
        newpos.z = transform.position.z;
        transform.position = newpos;
    }
}
