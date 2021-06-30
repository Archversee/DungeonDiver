﻿using System.Collections;
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
    public GameController gameController;

    public GameObject coinLootDrop;
    public GameObject diamondLootDrop;

    public bool CollidingwithLava = false;
    public bool CollidingwithMud = false;

    // Start is called before the first frame update
    void Start()
    {
        //register in enemymanager
        m_EnemyManager.RegisterEnemy(this);
        //get components
        m_health = GetComponent<Health>();

        gameController = FindObjectOfType<GameController>();

        istodestroy = false;
        killedbyplayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*Debug.Log(m_EnemyManager.Enemies.Count);*/
        if (m_health.currhealth <= 0)
        {
            istodestroy = true;
            killedbyplayer = true;
            gameController.Score++;
        }

        if(istodestroy)
        {
            UnregisterEnemy();
            DestroyEnemy();
        }
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
}



