using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIShop : MonoBehaviour
{
    public playerMovement player;

    private Transform container;
    private Transform atkdmgshoptemplate;
    private Transform healthshoptemplate;
    private Transform critdmgshoptemplate;
    private Transform critrateshoptemplate;
    private Transform movspeedshoptemplate;

    private List<Transform> shopTemplates = new List<Transform>();


    private List<PlayerUpgrade> shoplist = new List<PlayerUpgrade>();

    private void Awake()
    {
        container = transform.Find("container");
        atkdmgshoptemplate = container.Find("AtkDmgShop");
        healthshoptemplate = container.Find("HealthShop");
        critdmgshoptemplate = container.Find("CritDmgShop");
        critrateshoptemplate = container.Find("CritRateShop");
        movspeedshoptemplate = container.Find("MovSpeedShop");
        //shopitemTemplate.gameObject.SetActive(false);

        shopTemplates.Add(atkdmgshoptemplate);
        shopTemplates.Add(healthshoptemplate);
        shopTemplates.Add(critdmgshoptemplate);
        shopTemplates.Add(critrateshoptemplate);
        shopTemplates.Add(movspeedshoptemplate);
    }

    private void Start()
    {
        CreateShopButton(PlayerUpgrade.ItemType.AttackDamage,"Attack Damage", 1, 15 , 0);
        CreateShopButton(PlayerUpgrade.ItemType.Health, "Health", 1, 15, 1);
        CreateShopButton(PlayerUpgrade.ItemType.CritDamage, "Crit Damage", 1, 15, 2);
        CreateShopButton(PlayerUpgrade.ItemType.CritRate, "Crit Rate", 1, 15, 3);
        CreateShopButton(PlayerUpgrade.ItemType.MoveSpeed, "Movement Speed", 1, 15, 4);
    }

    public void CreateShopButton(PlayerUpgrade.ItemType itemType,string name, int level , int cost , int positionIndex)
    {
        PlayerUpgrade upgrade = new PlayerUpgrade();
        upgrade.Setup(itemType, name, cost, level);
        shoplist.Add(upgrade);
    }

    public void TryBuyUpgrade(string _name)
    {
        for (int i = 0; i < shoplist.Count; i++)
        {
            if (shoplist[i].name == _name)
            {
                if (player.getCurrency() >= shoplist[i].cost) //if player has more than or equal amt of the cost
                {
                    PurchaseUpgrade(shoplist[i]);
                    UpdateShop();
                    player.UpdateCurrency();
                }
            }
        }
    }

    private void UpdateShop()
    {
        for (int i = 0; i < shopTemplates.Count; i++)
        {
            Transform shoptemplate = shopTemplates[i];
            shoptemplate.Find("Level").GetComponent<TextMeshProUGUI>().SetText("Level " + shoplist[i].level.ToString());
            shoptemplate.Find("Cost").GetComponent<TextMeshProUGUI>().SetText((shoplist[i].cost.ToString()));
        }
    }

    private void PurchaseUpgrade(PlayerUpgrade currupgrade)
    {
        if(currupgrade.itemType == PlayerUpgrade.ItemType.AttackDamage)
        {
            player.UpgradeAtkDmg(currupgrade.cost);
        }
        else if (currupgrade.itemType == PlayerUpgrade.ItemType.Health)
        {
            player.UpgradeHealth(currupgrade.cost);
        }
        else if (currupgrade.itemType == PlayerUpgrade.ItemType.CritDamage)
        {
            player.UpgradeCritDmg(currupgrade.cost);
        }
        else if (currupgrade.itemType == PlayerUpgrade.ItemType.CritRate)
        {
            player.UpgradeCritRate(currupgrade.cost);
        }
        else if (currupgrade.itemType == PlayerUpgrade.ItemType.MoveSpeed)
        {
            player.UpgradeMovSpeed(currupgrade.cost);
        }
        currupgrade.level++;
        currupgrade.cost += 5;
    }
}

public class PlayerUpgrade
{
    public enum ItemType
    {
        AttackDamage,
        Health,
        CritDamage,
        CritRate,
        MoveSpeed,
    }

    public ItemType itemType;
    public int level;
    public int cost;
    public string name;

    public void Setup(ItemType _itemType,String _name, int _cost, int _level)
    {
        itemType = _itemType;
        cost = _cost;
        level = _level;
        name = _name;
    }
}
