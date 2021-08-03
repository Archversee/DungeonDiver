using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerMovement : MonoBehaviour
{

    [SerializeField]
    private LayerMask dashLayerMask;

    [SerializeField]
    private Transform DamagePopup;

    public GameObject ArrowPrefab;
    public GameObject LifeStealArrowPrefab;

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
    public bool isdashcooldown;
    public bool isattackcooldown = false;

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

    private float LastBurnTime = 0f;
    private int burnDmg = 2;
    private float burnDelay = 1f;

    //UI
    public Text CurrencyValue;
    public Transform PlayerStats;
    public UIShop ShopUI;

    //AUDIO
    public AudioClip footstepSFX;
    public AudioClip MudfootstepSFX;
    public AudioClip LavafootstepSFX;
    public AudioClip dashSFX;
    public AudioClip SwordSFX;
    public AudioClip Sword2SFX;
    public AudioClip BowSFX;
    public AudioClip arrowSFX;
    public AudioClip CrithitSFX;
    public AudioClip EffectlostSFX;
    public AudioClip PortalSFX;
    public AudioClip DodgeSFX;
    public AudioClip DamagedSFX;

    //FOOTSTEP:
    private float m_footstepDistanceCounter;
    public float footstepSFXFrequency = 1f;

    //GUI
    [SerializeField]
    private Image dashImageCooldown;
    [SerializeField]
    private TMP_Text dashTextCooldown;
    [SerializeField]
    private Image swordImageCooldown;
    [SerializeField]
    private TMP_Text swordTextCooldown;
    [SerializeField]
    private Image bowImageCooldown;
    [SerializeField]
    private TMP_Text bowTextCooldown;
    [SerializeField]
    private Transform LifebowImage;
    [SerializeField]
    private Image mudicon;
    [SerializeField]
    private Image burnImage;
    [SerializeField]
    private Transform dmgUPImage;
    [SerializeField]
    private Transform dodgeImage;
    [SerializeField]
    private Transform arrowImage;

    private bool isDashButtonDown = false;

    EnemyManager m_EnemyManager = EnemyManager.Singleton;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        dashTextCooldown.gameObject.SetActive(false);
        dashImageCooldown.fillAmount = 0.0f;
        swordTextCooldown.gameObject.SetActive(false);
        swordImageCooldown.fillAmount = 0.0f;
        bowTextCooldown.gameObject.SetActive(false);
        bowImageCooldown.fillAmount = 0.0f;
        isattackcooldown = false;
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

        if (!isattackcooldown)
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

        //if (Input.GetKey(KeyCode.E))
        // {
        CheckInteractable();
        // }

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
            if (!isdashcooldown && !isattackcooldown)
            {
                isDashButtonDown = true;
                isdashcooldown = true;
                dashTextCooldown.gameObject.SetActive(true);
                dashcooldown = 7f;
            }
        }

        if (Input.GetMouseButtonDown(0) && !isattackcooldown) //Melee Attack
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
                    AudioUtility.CreateSFX(SwordSFX, targetenemy.transform.position, AudioUtility.AudioGroups.Player, 10.0f);
                    AudioUtility.CreateSFX(CrithitSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
                }
                else
                {
                    AudioUtility.CreateSFX(Sword2SFX, targetenemy.transform.position, AudioUtility.AudioGroups.Player, 10.0f);
                }
                if (timestwodamage)
                {
                    normaldmg *= 2;
                }
                targetenemy.GetComponent<Health>().DealDmg(normaldmg);
                Transform damagePopupTransform = Instantiate(DamagePopup, targetenemy.transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.Setup(normaldmg, isCrit, false);

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
            attackcooldown = 0.3f;
            isattackcooldown = true;
            swordTextCooldown.gameObject.SetActive(true);
            bowTextCooldown.gameObject.SetActive(true);
        }
        if (Input.GetMouseButtonDown(1) && !isattackcooldown) //Ranged Attack
        {
            movedir = Vector3.zero;
            Vector3 Mousepos = GetMouseWorldPos();
            Vector3 Mousedir = (Mousepos - transform.position).normalized;

            if (Mousedir.x < 0)
            {
                animator.SetTrigger("RangedAttackTriggerL");
            }
            else if (Mousedir.x >= 0)
            {
                animator.SetTrigger("RangedAttackTriggerR");
            }
            attackcooldown = 0.7f;
            isattackcooldown = true;
            swordTextCooldown.gameObject.SetActive(true);
            bowTextCooldown.gameObject.SetActive(true);

            AudioUtility.CreateSFX(BowSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
            StartCoroutine(firearrow(7f, Mousedir));

            if (arroweffect)
            {
                StartCoroutine(firearrow(14f, Mousedir));
                StartCoroutine(firearrow(3.5f, Mousedir));
            }
        }
        if (Input.GetMouseButtonDown(2) && !isattackcooldown && gameController.LifestealArrowCounter >= 5) //LifeSteal Ranged Attack
        {
            gameController.LifestealArrowCounter = 0;
            movedir = Vector3.zero;
            Vector3 Mousepos = GetMouseWorldPos();
            Vector3 Mousedir = (Mousepos - transform.position).normalized;

            if (Mousedir.x < 0)
            {
                animator.SetTrigger("RangedAttackTriggerL");
            }
            else if (Mousedir.x >= 0)
            {
                animator.SetTrigger("RangedAttackTriggerR");
            }
            attackcooldown = 0.7f;
            isattackcooldown = true;
            swordTextCooldown.gameObject.SetActive(true);
            bowTextCooldown.gameObject.SetActive(true);

            AudioUtility.CreateSFX(BowSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
            StartCoroutine(fireLifearrow(7f, Mousedir));
        }
    }


    IEnumerator firearrow(float speed, Vector3 mousedir)
    {
        yield return new WaitForSeconds(0.2f);
        AudioUtility.CreateSFX(arrowSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
        GameObject arrow = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(mousedir.x, mousedir.y) * speed;
        arrow.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(mousedir.y, mousedir.x) * Mathf.Rad2Deg);
    }

    IEnumerator fireLifearrow(float speed, Vector3 mousedir)
    {
        yield return new WaitForSeconds(0.2f);
        AudioUtility.CreateSFX(arrowSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
        GameObject arrow = Instantiate(LifeStealArrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(mousedir.x, mousedir.y) * speed;
        arrow.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(mousedir.y, mousedir.x) * Mathf.Rad2Deg);
    }

    private void CheckInteractable()
    {
        if (CollidingwithShop)
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
            AudioUtility.CreateSFX(DamagedSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
        }
        else
        {
            Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.TextSetup("Dodged!");
            AudioUtility.CreateSFX(DodgeSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
        }
    }

    private void FixedUpdate()
    {
        //GUI
        if (isdashcooldown)
        {
            dashcooldown -= Time.deltaTime;
            if (dashcooldown < 0.0f)
            {
                isdashcooldown = false;
                dashTextCooldown.gameObject.SetActive(false);
                dashImageCooldown.fillAmount = 0.0f;
            }
            else
            {
                dashTextCooldown.text = Mathf.RoundToInt(dashcooldown).ToString();
                dashImageCooldown.fillAmount = dashcooldown / 7.0f;
            }
        }
        if (isattackcooldown)
        {
            attackcooldown -= Time.deltaTime;
            if (attackcooldown < 0.0f)
            {
                attackcooldown = -1.0f;
                isattackcooldown = false;
                swordTextCooldown.gameObject.SetActive(false);
                swordImageCooldown.fillAmount = 0.0f;
                bowTextCooldown.gameObject.SetActive(false);
                bowImageCooldown.fillAmount = 0.0f;
            }
            else
            {
                swordTextCooldown.text = attackcooldown.ToString("F1");
                swordImageCooldown.fillAmount = attackcooldown / 0.7f;
                bowTextCooldown.text = attackcooldown.ToString("F1");
                bowImageCooldown.fillAmount = attackcooldown / 0.7f;
            }
        }
        if (CollidingwithMud)
        {
            mudicon.gameObject.SetActive(true);
        }
        if(gameController.LifestealArrowCounter >= 1)
        {
            LifebowImage.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
            LifebowImage.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(5.0f - gameController.LifestealArrowCounter).ToString();
            LifebowImage.GetComponent<Image>().fillAmount = gameController.LifestealArrowCounter / 5f;
        }
        else
        {
            LifebowImage.GetComponent<Image>().fillAmount = 0f;
            LifebowImage.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
        }

        //GUI^ 
        float mudmovespeed = speed * 0.5f;
        if (!CollidingwithMud)
        {
            GetComponent<Rigidbody2D>().velocity = movedir * speed;
            mudicon.gameObject.SetActive(false);
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
                AudioUtility.CreateSFX(EffectlostSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
                dmgUPImage.Find("Cooldown").GetComponent<Image>().fillAmount = 0.0f;
                dmgUPImage.gameObject.SetActive(false);
            }
            else
            {
                dmgUPImage.gameObject.SetActive(true);
                dmgUPImage.Find("Cooldowntimer").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(timestwotimer).ToString();
                dmgUPImage.Find("Cooldown").GetComponent<Image>().fillAmount =  (10.0f - timestwotimer) / 10.0f;
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
                AudioUtility.CreateSFX(EffectlostSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
                dodgeImage.Find("Cooldown").GetComponent<Image>().fillAmount = 0.0f;
                dodgeImage.gameObject.SetActive(false);
            }
            else
            {
                dodgeImage.gameObject.SetActive(true);
                dodgeImage.Find("Cooldowntimer").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(dodgeeffecttimer).ToString();
                dodgeImage.Find("Cooldown").GetComponent<Image>().fillAmount = (10.0f - dodgeeffecttimer) / 10.0f;
            }
        }

        if (arroweffect == true)
        {
            arroweffecttimer -= Time.deltaTime;
            if (arroweffecttimer < 0.0f)
            {
                arroweffect = false;
                Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.TextSetup("More Arrows ended");
                AudioUtility.CreateSFX(EffectlostSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
                arrowImage.Find("Cooldown").GetComponent<Image>().fillAmount = 0.0f;
                arrowImage.gameObject.SetActive(false);
            }
            else
            {
                arrowImage.gameObject.SetActive(true);
                arrowImage.Find("Cooldowntimer").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(arroweffecttimer).ToString();
                arrowImage.Find("Cooldown").GetComponent<Image>().fillAmount = (10.0f - arroweffecttimer) / 10.0f;
            }
        }


        if (isDashButtonDown)
        {
            float dashAmt = 1.5f;
            Vector3 dashpos = transform.position + movedir * dashAmt;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, movedir, dashAmt, dashLayerMask);
            if (raycastHit2D.collider != null)
            {
                dashpos = raycastHit2D.point;
            }

            GetComponent<Rigidbody2D>().MovePosition(dashpos);
            AudioUtility.CreateSFX(dashSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
            isDashButtonDown = false;
        }

        if (CollidingwithLava)
        {
            burnImage.gameObject.SetActive(true);
            if (Time.time > LastBurnTime + burnDelay)
            {
                LastBurnTime = Time.time;
                TakeDamage(burnDmg);
            }
        }
        else
        {
            burnImage.gameObject.SetActive(false);
        }

        Vector3 worldspaceMoveInput = transform.TransformVector(GetMoveInput());

        //Footsetep audio
        if (m_footstepDistanceCounter >= 3.0f / footstepSFXFrequency)
        {
            m_footstepDistanceCounter = 0f;

            if (CollidingwithMud)
            {
                AudioUtility.CreateSFX(MudfootstepSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
            }
            else if (CollidingwithLava)
            {
                AudioUtility.CreateSFX(LavafootstepSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
            }
            else
            {
                AudioUtility.CreateSFX(footstepSFX, transform.position, AudioUtility.AudioGroups.Player, 10.0f);
            }
        }

        m_footstepDistanceCounter += worldspaceMoveInput.magnitude * speed * Time.deltaTime;
        UpdateStats();
    }

    public Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
        move = Vector3.ClampMagnitude(move, 1);
        return move;
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
            AudioUtility.CreateSFX(PortalSFX, transform.position, AudioUtility.AudioGroups.Pickups, 10.0f);
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
