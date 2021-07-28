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

    public PlayerData(playerMovement player, GameController gameController)
    {
        score = gameController.Score;
        levelcount = gameController.levelCount;

        speed = player.speed;
        attackrange = player.attackrange;
        attackdamage = player.attackdamage;
        attackcooldown = player.attackcooldown;
        critrate = player.critrate;
        critMultiplier = player.critMultiplier;
        Coins = player.Coins;
    }
}
