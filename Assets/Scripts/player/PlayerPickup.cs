﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerPickup : MonoBehaviour
{

    public GameObject OpenedFullTreasureChest;
    public GameObject OpenedEmptyTreasureChest;

    [SerializeField]
    private Transform DamagePopup;

    ConsumableManager m_consumableManager = ConsumableManager.Singleton;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Currency"))
        {
            if (other.name == "Coin(Clone)")
            {
                GetComponent<playerMovement>().AddCurrency(1);
            }
            else if (other.name == "Diamond(Clone)")
            {
                GetComponent<playerMovement>().AddCurrency(10);
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Consumable"))
        {
            if (other.name == "HealthPot(Clone)")
            {
                GetComponent<Health>().Heal(50);
                Transform healPopupTransform = Instantiate(DamagePopup, transform.position, Quaternion.identity);
                DamagePopup healPopup = healPopupTransform.GetComponent<DamagePopup>();
                healPopup.HealSetup(50);
            }
            else if(other.name == "DodgePotion(Clone)")
            {
                GetComponent<playerMovement>().dodgeeffect = true;
                GetComponent<playerMovement>().dodgeeffecttimer = 10.0f;
                Transform damagePopupTransform = Instantiate(DamagePopup, other.transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.TextSetup("Dodge Effect");
            }
            else if (other.name == "AttackPotion(Clone)")
            {
                GetComponent<playerMovement>().timestwodamage = true;
                GetComponent<playerMovement>().timestwotimer = 10.0f;
                Transform damagePopupTransform = Instantiate(DamagePopup, other.transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.TextSetup("2x Damage!");
            }
            else if (other.name == "ArrowPotion(Clone)")
            {
                GetComponent<playerMovement>().arroweffect = true;
                GetComponent<playerMovement>().arroweffecttimer = 10.0f;
                Transform damagePopupTransform = Instantiate(DamagePopup, other.transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.TextSetup("More Arrows!");
            }
            else if (other.name == "TreasureChest(Clone)")
            {
                int randnum = Random.Range(0, 10);
                if (randnum > 6)
                {
                    GetComponent<playerMovement>().AddCurrency(15);
                    Instantiate(OpenedFullTreasureChest, new Vector3(other.transform.position.x, other.transform.position.y, 0), Quaternion.identity);
                }
                else
                {
                    Instantiate(OpenedEmptyTreasureChest, new Vector3(other.transform.position.x, other.transform.position.y, 0), Quaternion.identity);
                }
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Environment"))
        {
            if (other.name == "Lava")
            {
                GetComponent<playerMovement>().CollidingwithLava = true;
            }
            if (other.name == "Mud")
            {
                GetComponent<playerMovement>().CollidingwithMud = true;
            }
        }
        else if (other.CompareTag("Environmentals"))
        {
            if (other.name == "HealingFountain(Clone)")
            {
                GetComponent<playerMovement>().CollidingwithFountain = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment"))
        {
            if (other.name == "Lava")
            {
                GetComponent<playerMovement>().CollidingwithLava = false;
            }
            if (other.name == "Mud")
            {
                GetComponent<playerMovement>().CollidingwithMud = false;
            }
        }
        else if (other.CompareTag("Environmentals"))
        {
            if (other.name == "HealingFountain(Clone)")
            {
                GetComponent<playerMovement>().CollidingwithFountain = false;
            }
        }
    }
}
