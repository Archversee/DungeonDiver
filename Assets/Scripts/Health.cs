using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{

    public float maxhealth;
    public float currhealth;

    public event Action<float> OnHealthPctChanged = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        currhealth = maxhealth;
    }

    public void DealDmg(float dmg)
    {
        if (currhealth > 0)
        {
            currhealth -= dmg;
            //clamp
            if (currhealth < 0)
            {
               currhealth = 0;
            }
            float currhealthpercent = currhealth / maxhealth;
            GetComponent<HealthBar>().HandleHealthChanged(currhealthpercent);
        }
    }

    public void Heal(float healamt)
    {

        currhealth += healamt;
        if (currhealth > maxhealth)
        {
            currhealth = maxhealth;
        }
        float currhealthpercent = currhealth / maxhealth;
        GetComponent<HealthBar>().HandleHealthChanged(currhealthpercent);
    }
}
