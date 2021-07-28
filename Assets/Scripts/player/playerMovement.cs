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

    public float speed; //3
    public float idletime;
    public float attackrange; //4
    public float attackdamage; //5
    public float attackcooldown; //6
    public float critrate; //7
    public float critMultiplier; //8
    private Vector2 dir;
    private Vector3 movedir;
    private Animator animator;
    //private Rigidbody2D rigidbody2D;
    public GameController gameController;

    public float dashrange;
    public float dashcooldown;

    public int Coins; //9

    private bool CollidingwithShop = false;
    public bool CollidingwithLava = false;
    public bool CollidingwithMud = false;
    public bool CollidingwithFountain = false;

    public bool timestwodamage = false;
    public bool dodgeeffect = false;
    public bool arroweffect = false;

    public float timestwotimer = 0f; 
    public float dodgeeffecttimer = 0f; 
    public float arroweffecttimer = 0f; 

    float LastBurnTime = 0f;
    int burnDmg = 2;
    float burnDelay = 1f;

    //UI
    public Text CurrencyValue;
    public Transform PlayerStats;
    public UIShop ShopUI;

    bool isDashButtonDown = false;

    EnemyManager m_EnemyManager = EnemyManager.Singleton;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        //rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GetComponent<Health>().currhealth > 0)
        {
            TakeInput();
            idletime += Time.deltaTime;
        }
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
                    normaldmg *= critMultiplier;
                }
                if(timestwodamage)
                {
                    normaldmg *= 2;
                }
                targetenemy.GetComponent<Health>().DealDmg(normaldmg);
                Transform damagePopupTransform = Instantiate(DamagePopup, targetenemy.transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.Setup(normaldmg, isCrit , false);

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

            if (arroweffect)
            {
                GameObject arrow2 = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
                arrow2.GetComponent<Rigidbody2D>().velocity = new Vector2(Mousedir.x, Mousedir.y) * 14;
                arrow2.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(Mousedir.y, Mousedir.x) * Mathf.Rad2Deg);

                GameObject arrow3 = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
                arrow3.GetComponent<Rigidbody2D>().velocity = new Vector2(Mousedir.x, Mousedir.y) * 3.5f;
                arrow3.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(Mousedir.y, Mousedir.x) * Mathf.Rad2Deg);
            }
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
        if (dodgeeffect == false)
        {
            GetComponent<Health>().DealDmg(dmg);
            Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(dmg, false, true);
        }
        else
        {
            Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.TextSetup("Dodged!");
        }
    }

    private void FixedUpdate()
    {
        dashcooldown -= 0.1f;
        attackcooldown -= 0.1f;
        float mudmovespeed = speed * 0.5f;
        if(!CollidingwithMud)
        {
            GetComponent<Rigidbody2D>().velocity = movedir * speed;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = movedir * mudmovespeed;
        }

        if (CollidingwithFountain)
        {
            if (GetComponent<Health>().currhealth > 0)
            {
                GetComponent<Health>().Heal(0.1f);
            }
        }

        if (timestwodamage == true)
        {
            timestwotimer -= Time.deltaTime;
            if (timestwotimer < 0.0f)
            {
                timestwodamage = false;
                Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.TextSetup("2x Damage ended");
            }
        }

        if (dodgeeffect == true)
        {
            dodgeeffecttimer -= Time.deltaTime;
            if (dodgeeffecttimer < 0.0f)
            {
                dodgeeffect = false;
                Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.TextSetup("Dodge Effect ended");
            }
        }

        /*if (arroweffect == true)
        {
            arroweffecttimer -= Time.deltaTime;
            if (arroweffecttimer < 0.0f)
            {
                arroweffect = false;
                Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.TextSetup("More Arrows ended");
            }
        }*/


        if (isDashButtonDown)
        {
            float dashAmt = 1.5f;
            Vector3 dashpos = transform.position + movedir * dashAmt;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, movedir, dashAmt, dashLayerMask);
            if(raycastHit2D.collider != null)
            {
                dashpos = raycastHit2D.point;
            }

            GetComponent<Rigidbody2D>().MovePosition(dashpos);
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

        UpdateStats();
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

    public void UpdateStats()
    {
        Transform temp = PlayerStats.Find("Health");
        temp.GetComponent<Text>().text = ("Health : ") + (GetComponent<Health>().maxhealth).ToString();
        temp = PlayerStats.Find("Damage");
        temp.GetComponent<Text>().text = ("Damage : ") + attackdamage.ToString();
        temp = PlayerStats.Find("Critdmg");
        temp.GetComponent<Text>().text = ("Crit Damage : ") + critMultiplier.ToString();
        temp = PlayerStats.Find("Critrate");
        temp.GetComponent<Text>().text = ("Crit Rate : ") + critrate.ToString();
        temp = PlayerStats.Find("MovSpeed");
        temp.GetComponent<Text>().text = ("Speed : ") + speed.ToString();
        temp = PlayerStats.Find("Status");
        if (CollidingwithLava)
        {
            temp.GetComponent<Text>().text = ("Status : Burning!");
        }
        else if(CollidingwithMud)
        {
            temp.GetComponent<Text>().text = ("Status : Slowed!");
        }
        else
        {
            temp.GetComponent<Text>().text = ("Status :");
        }
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
