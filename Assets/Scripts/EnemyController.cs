using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    EnemyManager m_EnemyManager = EnemyManager.Singleton;
    Health m_health;
    public bool istodestroy;
    public bool killedbyplayer;
    public GameObject gameController;

    public GameObject coinLootDrop;
    public GameObject diamondLootDrop;

    public bool CollidingwithLava = false;
    public bool CollidingwithMud = false;

    float LastBurnTime = 0f;
    int burnDmg = 2;
    float burnDelay = 1f;

    [SerializeField]
    private Transform DamagePopup;

    // Start is called before the first frame update
    void Start()
    {
        //register in enemymanager
        m_EnemyManager.RegisterEnemy(this);
        //get components
        m_health = GetComponent<Health>();

        gameController = GameObject.FindGameObjectWithTag("GameController");

        istodestroy = false;
        killedbyplayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_health.currhealth <= 0)
        {
            istodestroy = true;
            killedbyplayer = true;
            gameController.GetComponent<GameController>().Score++;
        }

        if(istodestroy)
        {
            UnregisterEnemy();
            DestroyEnemy();
        }
    }

    private void FixedUpdate()
    {
        if (CollidingwithLava)
        {
            if (Time.time > LastBurnTime + burnDelay)
            {
                LastBurnTime = Time.time;
                TakeDamage(burnDmg);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        GetComponent<Health>().DealDmg(dmg);
        Transform damagePopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(dmg, false, true);
    }

    void UnregisterEnemy()
    {
        // removes the enemy from the list, so that we can keep track of how many are left on the map
        m_EnemyManager.UnregisterEnemy(this);
    }

    public void DestroyEnemy()
    {
        if (killedbyplayer)
        {
            float randnum = Random.Range(0f, 10f);
            if (randnum > 1)
            {
                Instantiate(coinLootDrop, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(diamondLootDrop, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Environment"))
        {
            if (other.name == "Lava")
            {
                CollidingwithLava = true;
            }
            if (other.name == "Mud")
            {
                CollidingwithMud = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment"))
        {
            if (other.name == "Lava")
            {
               CollidingwithLava = false;
            }
            if (other.name == "Mud")
            {
                CollidingwithMud = false;
            }
        }
    }
}



