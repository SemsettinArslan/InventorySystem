using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    
    public static EquipmentManager instance;

    [SerializeField] private Slot helmetSlot;
    [SerializeField] private Slot chestSlot;
    [SerializeField] private Slot bootsSlot;
    [SerializeField] private Slot ringSlot;
    [SerializeField] private Slot necklaceSlot;
    [SerializeField] private Slot weaponSlot;


    private Dictionary<Item.EquipmentType, Item> equippedItems = new Dictionary<Item.EquipmentType, Item>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitalizeSlots();
    }


    private void InitalizeSlots()
    {
        if(helmetSlot != null)
        {
            helmetSlot.isEquipmentSlot = true;
            helmetSlot.equipmentType = Item.EquipmentType.Helmet;
        }
        
        if(chestSlot != null)
        {
            chestSlot.isEquipmentSlot = true;
            chestSlot.equipmentType= Item.EquipmentType.Chest;
        }
        if(bootsSlot != null)
        {
            bootsSlot.isEquipmentSlot = true;
            bootsSlot.equipmentType=Item.EquipmentType.Boots;
        }
        if(ringSlot != null)
        {
            ringSlot.isEquipmentSlot = true;
            ringSlot.equipmentType = Item.EquipmentType.Ring;
        }
        if(necklaceSlot != null)
        {
            necklaceSlot.isEquipmentSlot= true;
            necklaceSlot.equipmentType = Item.EquipmentType.Necklace;
        }
        if(weaponSlot != null)
        {
            weaponSlot.isEquipmentSlot= true;
            weaponSlot.equipmentType = Item.EquipmentType.Weapon;
        }
    }

    public void EquipItem(Item item,int sourceSlotIndex)
    {
        if(item == null ||!item.isEquippable) 
        {
            return;
        }

        Slot targetSlot = GetEquipmentSlot(item.equipmentType);
        if(targetSlot != null)
        {
            Debug.Log($"{item.itemName} donatılıyor...");

            if(equippedItems.ContainsKey(item.equipmentType))
            {
                UnequipItem(item.equipmentType);
            }

            InventoryManager.instance.RemoveItem(sourceSlotIndex);

            targetSlot.SetItem(item, 1);

            equippedItems[item.equipmentType] = item;
            InventoryManager.instance.RefreshUI();
            UpdateCharacterStats();
        }
        else
        {
            Debug.LogWarning($"Bu ekipman türü için slot bulunamadı: {item.equipmentType}");
        }
    }

    public void UnequipItem(Item.EquipmentType type)
    {
        if(equippedItems.ContainsKey(type))
        {
            Item item = equippedItems[type];
            Debug.Log($"{item.itemName} çıkarılıyor");

            bool success = InventoryManager.instance.AddItem(item);

            if (success)
            {
                Slot targetSlot=GetEquipmentSlot(type);
                if(targetSlot != null)
                {
                    targetSlot.ClearSlot();
                }

                equippedItems.Remove(type);
                UpdateCharacterStats();
                InventoryManager.instance.RefreshUI();
            }
            else
            {
                Debug.LogWarning("Envanter dolu, ekipman çıkarılamadı!");
            }
        }
    }

    private Slot GetEquipmentSlot(Item.EquipmentType type)
    {
        switch (type)
        {
            case Item.EquipmentType.Helmet:
                return helmetSlot;
            case Item.EquipmentType.Chest:
                return chestSlot;
            case Item.EquipmentType.Boots:
                return bootsSlot;
            case Item.EquipmentType.Weapon:
                return weaponSlot;
            case Item.EquipmentType.Ring:
                return ringSlot;
            case Item.EquipmentType.Necklace:
                return necklaceSlot;
            default:
                return null;
        }
    }

    private void UpdateCharacterStats()
    {
        int totalAttack = 0;
        int totalDefense = 0;
        int totalAgility = 0;
        int totalLuck = 0;

        foreach(Item item in equippedItems.Values)
        {
            totalAttack += item.attackValue;
            totalDefense+= item.defenseValue;
            totalAgility+= item.agilityValue;
            totalLuck+= item.luckValue;
        }
        if (Player.instance != null)
        {
            Player.instance.UpdateEquipmentBonuses(totalAttack, totalDefense, totalAgility, totalLuck);
        }
        else
        {
            Debug.LogError("Player instance bulunamadı!");
        }
    }



}
