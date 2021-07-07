using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{

    [SerializeField]
    private LayerMask dashLayerMask;

    [SerializeField]
    private Transform DamagePopup;

    public GameObject ArrowPrefab;

    public float speed;
    public float idletime;
    public float attackrange;
    public float attackdamage;
    public float attackcooldown;
    public float critrate;
    public float critMultiplier;
    private Vector2 dir;
    private Vector3 movedir;
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    public GameController gameController;

    public float dashrange;
    public float dashcooldown;

    public int Coins;

    private bool CollidingwithShop = false;
    public bool CollidingwithLava = false;
    public bool CollidingwithMud = false;


    float LastBurnTime = 0f;
    int burnDmg = 2;
    float burnDelay = 1f;

    //UI
    public Text CurrencyValue;
    public UIShop ShopUI;

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

        if (attackcooldown <= 0)
        {
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
        }

        if (Input.GetKey(KeyCode.E))
        {
            CheckInteractable();
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
            //float atkoffset = 1.5f;
            Vector3 atkPos = transform.position + Mousedir * attackrange;
            EnemyController targetenemy = EnemyManager.Singleton.GetClosestEnemy(atkPos, attackrange);
            if (targetenemy != null)
            {
                //Damage Enemy
                //Debug.Log("damaged enemy");
                float normaldmg = attackdamage;
                bool isCrit = UnityEngine.Random.Range(0, 100) < critrate;
                if (isCrit)
                {
                    attackdamage *= critMultiplier;
                }
                targetenemy.GetComponent<Health>().DealDmg(attackdamage);
                Transform damagePopupTransform = Instantiate(DamagePopup, targetenemy.transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.Setup(attackdamage, isCrit , false);
                if (isCrit)
                {
                    attackdamage = normaldmg;
                }

            }
            movedir = Vector3.zero;
            if (Mousedir.x < 0)
            {
                animator.SetTrigger("AttackTriggerL");
            }
            else if (Mousedir.x >= 0)
            {
                animator.SetTrigger("AttackTriggerR");
            }
            attackcooldown = 1.5f;
        }
        if (Input.GetMouseButtonDown(1) && attackcooldown < 0) //Ranged Attack
        {
            movedir = Vector3.zero;
            Vector3 Mousepos = GetMouseWorldPos();
            Vector3 Mousedir = (Mousepos - transform.position).normalized;

            if(Mousedir.x < 0)
            {
                animator.SetTrigger("RangedAttackTriggerL");
            }
            else if(Mousedir.x >= 0)
            {
                animator.SetTrigger("RangedAttackTriggerR");
            }
            attackcooldown = 3f;

            GameObject arrow = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
            arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(Mousedir.x, Mousedir.y) * 7;
            arrow.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(Mousedir.y, Mousedir.x) * Mathf.Rad2Deg);
        }
    }

    private void CheckInteractable()
    {
        if(CollidingwithShop)
        {
            ShopUI.gameObject.SetActive(true);
        }
    }

    public static Vector3 GetMouseWorldPos()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    public void TakeDamage(float dmg)
    {
       GetComponent<Health>().DealDmg(dmg);
       Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
       DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
       damagePopup.Setup(dmg, false, true);
    }

    private void FixedUpdate()
    {
        dashcooldown -= 0.1f;
        attackcooldown -= 0.1f;
        float mudmovespeed = speed * 0.5f;
        if(!CollidingwithMud)
        {
            rigidbody2D.velocity = movedir * speed;
        }
        else
        {
            rigidbody2D.velocity = movedir * mudmovespeed;
        }

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

        if(CollidingwithLava)
        {
            if (Time.time > LastBurnTime + burnDelay)
            {
                LastBurnTime = Time.time;
                TakeDamage(burnDmg);
            }
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
        if (col.gameObject.name == "Shop")
        {
            CollidingwithShop = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.name == "Shop")
        {
            CollidingwithShop = false;
            ShopUI.gameObject.SetActive(false);
        }
    }

    public void AddCurrency(int amt)
    {
        Coins += amt;
        CurrencyValue.text = Coins.ToString();
    }

    public void UpdateCurrency()
    {
        CurrencyValue.text = Coins.ToString();
    }

    public int getCurrency()
    {
        return Coins;
    }

    public void UpgradeHealth(int cost)
    {
        GetComponent<Health>().maxhealth += 10;
        Coins -= cost;
    }

    public void UpgradeAtkDmg(int cost)
    {
        attackdamage += 5;
        Coins -= cost;
    }

    public void UpgradeCritDmg(int cost)
    {
        critMultiplier += 0.1f;
        Coins -= cost;
    }

    public void UpgradeCritRate(int cost)
    {
        critrate += 1;
        Coins -= cost;
    }
    public void UpgradeMovSpeed(int cost)
    {
        speed += 0.2f;
        Coins -= cost;
    }
}
