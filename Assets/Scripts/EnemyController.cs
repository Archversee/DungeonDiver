using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    EnemyManager m_EnemyManager = EnemyManager.Singleton;
    Health m_health;

    // Start is called before the first frame update
    void Start()
    {
        //register in enemymanager
        m_EnemyManager.RegisterEnemy(this);
        //get components
        m_health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        /*Debug.Log(m_EnemyManager.Enemies.Count);*/
        if(m_health.currhealth <= 0)
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
       Destroy(gameObject);
    }
}



