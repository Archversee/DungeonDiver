using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableManager
{
    private static ConsumableManager instance;
    private List<GameObject> consumables = new List<GameObject>();
    public List<GameObject> Consumables { get { return consumables; } }
    public int numberOfConsumablesTotal { get; private set; }
    public int numberOfConsumablesRemaining => consumables.Count;

    // Start is called before the first frame update
    public void RegisterConsumable(GameObject consum)
    {
        consumables.Add(consum);

        numberOfConsumablesTotal++;
    }

    public void UnregisterConsumable(GameObject consumremoved)
    {
        // removes the enemy from the list, so that we can keep track of how many are left on the map
        consumables.Remove(consumremoved);
    }
/*    public void ClearList()
    {
        for (int i = 0; i < consumables.Count; i++)
        {
            GameObject temp = consumables[i].gameObject;
            Destroy(temp);
        }
    }*/


    public static ConsumableManager Singleton
    {
        get
        {
            if (instance == null)
            {
                instance = new ConsumableManager();
            }
            return instance;
        }
    }
}
