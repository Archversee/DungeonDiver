using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyManager
{
    private static EnemyManager instance;
    private List<EnemyController> enemies = new List<EnemyController>();
    public List<EnemyController> Enemies { get { return enemies; } }
    public int numberOfEnemiesTotal { get; private set; }
    public int numberOfEnemiesRemaining => enemies.Count;


    public void RegisterEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);

        numberOfEnemiesTotal++;
    }

    public void UnregisterEnemy(EnemyController enemyKilled)
    {
        // removes the enemy from the list, so that we can keep track of how many are left on the map
        enemies.Remove(enemyKilled);
    }

    public void ClearList()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            enemies[i].istodestroy = true;
        }
    }

    public static EnemyManager Singleton
    {
        get
        {
            if (instance == null)
            {
                instance = new EnemyManager();
            }
            return instance;
        }
    }

    public EnemyController GetClosestEnemy(Vector3 pos, float range)
    {
        if (enemies == null) return null; //error check if no enemies
        EnemyController closestEnemy = null;

        for (int i = 0; i < Enemies.Count; i++)
        {
            EnemyController TempEnemy = Enemies[i];

            if(Vector3.Distance(pos, TempEnemy.transform.position) > range)
            {
                continue;
            }
            if(closestEnemy == null)
            {
                closestEnemy = TempEnemy;
            }
            else
            {
                if(Vector3.Distance(pos, TempEnemy.transform.position) < Vector3.Distance(pos, closestEnemy.transform.position))
                {
                    closestEnemy = TempEnemy;
                }
            }

        }
        return closestEnemy;
    }
}

