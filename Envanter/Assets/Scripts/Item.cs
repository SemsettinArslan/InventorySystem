using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemID;
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public bool isStackable;
    public int maxStackAmount = 5;

    public bool isEquippable;
    public EquipmentType equipmentType;
    public int attackValue;
    public int defenseValue;
    public int agilityValue;
    public int luckValue;
    public int itemQuality;

    public enum EquipmentType
    {
        None,
        Helmet,
        Chest,
        Gloves,
        Legs,
        Boots,
        Weapon,
        Shield,
        Ring,
        Necklace,
        Accessory
    }
}
