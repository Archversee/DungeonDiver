﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private GameObject player;

    [SerializeField]
    private Transform DamagePopup;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Destroy(gameObject, 5.0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.name == "DodgeEnemy(Clone)")
            {
                Transform damagePopupTransform = Instantiate(DamagePopup, other.transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.TextSetup("Dodged!");
            }
            else
            {
                float dmg = player.GetComponent<playerMovement>().attackdamage * 1.5f; //1.5 multilplier for arrow attack as slower attackspeed
                bool isCrit = UnityEngine.Random.Range(0, 100) < player.GetComponent<playerMovement>().critrate; //check if crit
                if (isCrit)
                {
                    dmg *= player.GetComponent<playerMovement>().critMultiplier;
                }
                if(player.GetComponent<playerMovement>().timestwodamage)
                {
                    dmg *= 2;
                }
                other.GetComponent<Health>().DealDmg(dmg);
                Transform damagePopupTransform = Instantiate(DamagePopup, other.transform.position, Quaternion.identity);
                DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
                damagePopup.Setup(dmg, isCrit, false);
                //Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
