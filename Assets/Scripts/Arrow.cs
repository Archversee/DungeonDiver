using System.Collections;
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
            float normaldmg = player.GetComponent<playerMovement>().attackdamage;
            bool isCrit = UnityEngine.Random.Range(0, 100) < player.GetComponent<playerMovement>().critrate;
            if (isCrit)
            {
                player.GetComponent<playerMovement>().attackdamage *= player.GetComponent<playerMovement>().critMultiplier;
            }
            other.GetComponent<Health>().DealDmg(player.GetComponent<playerMovement>().attackdamage);
            Transform damagePopupTransform = Instantiate(DamagePopup, other.transform.position, Quaternion.identity);
            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(player.GetComponent<playerMovement>().attackdamage, isCrit, false);
            if (isCrit)
            {
                player.GetComponent<playerMovement>().attackdamage = normaldmg;
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
