using UnityEngine;
using System.Collections.Generic;
using static Item;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    [SerializeField] private string jsonFilePath = "Data/items";// Resources yol
    [SerializeField] private string spriteFolderPath = "icons"; // Resources yol

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LoadItemsFromJson();
    }
    private void LoadItemsFromJson()
    {
        TextAsset itemsJson = Resources.Load<TextAsset>(jsonFilePath);

        if (itemsJson == null)
        {
            Debug.Log("itemsJson dosyası bulunumadı.");
            return;
        }

        ItemData itemData = JsonUtility.FromJson<ItemData>(itemsJson.text);

        foreach(ItemJson itemJson in itemData.items)
        {
            Item newItem=new Item();
            newItem.itemID = itemJson.id;
            newItem.itemName=itemJson.name;
            newItem.itemDescription=itemJson.description;
            newItem.isStackable = itemJson.stackable;
            newItem.maxStackAmount = itemJson.maxStackSize;
            newItem.isEquippable = itemJson.equipable;
            newItem.equipmentType = GetEquipmentTypeFromString(itemJson.equipmentType);
            newItem.attackValue= itemJson.attackValue;
            newItem.defenseValue= itemJson.defenseValue;
            newItem.luckValue= itemJson.luckValue;
            newItem.agilityValue= itemJson.agilityValue;
            newItem.itemQuality= itemJson.itemQuality;

            newItem.itemIcon= LoadSpriteFromResources(itemJson.iconName);

            itemDictionary[newItem.itemID] = newItem;
        }
        Debug.Log($"{itemDictionary.Count} item JSON'dan y�klendi.");

    }
    private Sprite LoadSpriteFromResources(string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
        {
            Debug.LogError("iconName boş veya null!");
            return null;
        }
        string fullPath = string.IsNullOrEmpty(spriteFolderPath)
            ? iconName
            : spriteFolderPath + "/" + iconName;

        Sprite sprite = Resources.Load<Sprite>(fullPath);

        if (sprite == null)
        {
            Debug.LogWarning($"Sprite not found: {fullPath}");
            return Resources.Load<Sprite>("ItemIcons/default_icon");
        }

        return sprite;
    }
    private EquipmentType GetEquipmentTypeFromString(string typeString)
    {
        if (string.IsNullOrEmpty(typeString))
            return EquipmentType.None;

        try
        {
            return (EquipmentType)System.Enum.Parse(typeof(EquipmentType), typeString, true);
        }
        catch
        {
            return EquipmentType.None;
        }
    }

    public Item GetItemByID(string id)
    {
        if (itemDictionary.ContainsKey(id))
            return itemDictionary[id];

        Debug.LogWarning($"Item not found with ID: {id}");
        return null;
    }

    public List<string> GetAllItemIDs()
    {
        return new List<string>(itemDictionary.Keys);
    }



















    // JSON yap�lar�
    [System.Serializable]
    public class ItemData
    {
        public List<ItemJson> items = new List<ItemJson>();
    }

    [System.Serializable]
    public class ItemJson
    {
        public string id;
        public string name;
        public string description;
        public string iconName;
        public bool stackable;
        public int maxStackSize;
        public bool equipable;
        public string equipmentType;
        public int attackValue;
        public int defenseValue;
        public int agilityValue;
        public int luckValue;
        public int itemQuality;
    }
}
