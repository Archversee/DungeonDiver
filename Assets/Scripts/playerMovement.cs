using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    [SerializeField]
    private LayerMask dashLayerMask;

    public float speed;
    public float idletime;
    private Vector2 dir;
    private Vector3 movedir;
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    public GameController gameController;

    public float dashrange;
    public float dashcooldown;

    bool isDashButtonDown = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        TakeInput();
        idletime += Time.deltaTime;
    }

    private void TakeInput()
    {
        dir = Vector2.zero;  //reset dir to default every frame

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            dir += Vector2.up;
            moveY = +1f;
            idletime = 0.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir += Vector2.down;
            moveY = -1f;
            idletime = 0.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector2.left;
            moveX = -1f;
            idletime = 0.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector2.right;
            moveX = +1f;
            idletime = 0.0f;
        }

        movedir = new Vector3(moveX, moveY).normalized;

        if (dir.x != 0 || dir.y != 0)
        {
            SetAnimatorMovement(dir, idletime); //change animator if moving
        }
        else if (idletime >= 0.01)
        {
            SetAnimatorMovement(dir, idletime); //change to idle state
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (dashcooldown < 0f)
            {
                isDashButtonDown = true;
                dashcooldown = 7f;
            }
        }
    }   

    private void FixedUpdate()
    {
        dashcooldown -= 0.1f;
        rigidbody2D.velocity = movedir * speed;

        if(isDashButtonDown)
        {
            float dashAmt = 1.5f;
            Vector3 dashpos = transform.position + movedir * dashAmt;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, movedir, dashAmt, dashLayerMask);
            if(raycastHit2D.collider != null)
            {
                dashpos = raycastHit2D.point;
            }

            rigidbody2D.MovePosition(dashpos);
            isDashButtonDown = false;
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
