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
    public float attackrange;
    public float attackcooldown;
    private Vector2 dir;
    private Vector3 movedir;
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    public GameController gameController;

    public float dashrange;
    public float dashcooldown;

    bool isDashButtonDown = false;

    EnemyManager m_EnemyManager = EnemyManager.Singleton;


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

        if (Input.GetMouseButtonDown(0) && attackcooldown < 0) //Melee Attack
        {
            Vector3 Mousepos = GetMouseWorldPos();
            Vector3 Mousedir = (Mousepos - transform.position).normalized;
            float atkoffset = 3f;
            Vector3 atkPos = transform.position + movedir * atkoffset;
            EnemyController targetenemy = EnemyManager.Singleton.GetClosestEnemy(atkPos, attackrange);
            if(targetenemy != null)
            {
                //Damage Enemy
                //Debug.Log("damaged enemy");
                targetenemy.GetComponent<Health>().DealDmg(20);
            }
            movedir = Vector3.zero;
            animator.SetTrigger("AttackTrigger");
            attackcooldown = 3f;
        }
    }   

    public static Vector3 GetMouseWorldPos()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    private void FixedUpdate()
    {
        dashcooldown -= 0.1f;
        attackcooldown -= 0.1f;
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
