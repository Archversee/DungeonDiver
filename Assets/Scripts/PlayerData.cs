using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float score;
    public int levelcount;

    public float speed;
    public float attackrange;
    public float attackdamage; 
    public float attackcooldown; 
    public float critrate;
    public float critMultiplier;
    public int Coins;

    public float currhealth;
    public float maxhealth;

    public PlayerData(Transform player, GameController gameController)
    {
        score = gameController.Score;
        levelcount = gameController.levelCount;

        speed = player.GetComponent<playerMovement>().speed;
        attackrange = player.GetComponent<playerMovement>().attackrange;
        attackdamage = player.GetComponent<playerMovement>().attackdamage;
        attackcooldown = player.GetComponent<playerMovement>().attackcooldown;
        critrate = player.GetComponent<playerMovement>().critrate;
        critMultiplier = player.GetComponent<playerMovement>().critMultiplier;
        Coins = player.GetComponent<playerMovement>().Coins;

        currhealth = player.GetComponent<Health>().currhealth;
        maxhealth = player.GetComponent<Health>().maxhealth;
    }
}
