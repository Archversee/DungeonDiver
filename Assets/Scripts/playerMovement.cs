using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float speed;
    public float idletime;
    private Vector2 dir;
    private Animator animator;
    public GameController gameController;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        TakeInput();
        Move();
        idletime += Time.deltaTime;
    }

    private void Move()
    {
        dir.Normalize();
        dir *= speed;
        transform.Translate(dir * Time.deltaTime);

        if(dir.x != 0 || dir.y != 0)
        {
            SetAnimatorMovement(dir, idletime); //change animator if moving
        }
        else if(idletime >= 0.01)
        {
            SetAnimatorMovement(dir, idletime); //change to idle state
        }
    }

    private void TakeInput()
    {
        dir = Vector2.zero;  //reset dir to default every frame

        if(Input.GetKey(KeyCode.W))
        {
            dir += Vector2.up;
            idletime = 0.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir += Vector2.down;
            idletime = 0.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector2.left;
            idletime = 0.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector2.right;
            idletime = 0.0f;
        }
    }

    private void SetAnimatorMovement(Vector2 dir , float idletimer)
    {
        animator.SetFloat("xDir", dir.x);
        animator.SetFloat("yDir", dir.y);
        animator.SetFloat("IdleTimer", idletimer);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Portal")
        {
            gameController.NewMap();
        }
    }
}
