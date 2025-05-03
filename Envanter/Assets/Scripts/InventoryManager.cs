using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField] private int inventorySize = 24;
    public List<ItemSlot> inventorySlots = new List<ItemSlot>();

    [SerializeField] private InventoryUIManager uiManager;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeInventory();
        
        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<InventoryUIManager>();
            if (uiManager == null)
            {
                Debug.LogError("InventoryUIManager bulunamadı!");
                return;
            }
        }
        
        Invoke("LoadExampleItems", 0.1f);
    }
    
    private void InitializeInventory()
    {
        inventorySlots.Clear();

        Debug.Log($"Envanter slotları oluşturuluyor. Boyut: {inventorySize}");

        for (int i = 0; i < inventorySize; i++)
        {
            ItemSlot newSlot = new ItemSlot();
            newSlot.item = null;
            newSlot.amount = 0;
            inventorySlots.Add(newSlot);
        }

        Debug.Log($"Envanter slotları oluşturuldu. Toplam: {inventorySlots.Count}");
    }
    
    public void ClearInventory()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].item = null;
            inventorySlots[i].amount = 0;
        }
        
        RefreshUI();
        Debug.Log("Envanter tamamen temizlendi.");
    }

    private void LoadExampleItems()
    {
        if (ItemDatabase.instance == null)
        {
            Debug.LogError("ItemDatabase Bulunamadı!");
            return;
        }

        List<string> allItemID = ItemDatabase.instance.GetAllItemIDs();
        Debug.Log($"Bulunan item ID sayısı: {allItemID.Count}");

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].item = null;
            inventorySlots[i].amount = 0;
        }

        foreach (string itemID in allItemID)
        {
            Debug.Log($"Item ekleniyor: {itemID}");
            AddItemByID(itemID);
        }
        RefreshUI();
    }

    public void AddItemByID(string ItemID,int amount = 1)
    {
        Item item=ItemDatabase.instance.GetItemByID(ItemID);
        if(item != null)
        {
            AddItem(item, amount);
        }
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogError("AddItem null item ile çağrıldı!");
            return false;
        }
        
        if (item.itemIcon == null)
        {
            Debug.LogError($"Eklenmek istenen '{item.itemName}' sprite null!");
            return false;
        }
        
        if (item.isStackable)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].item != null && 
                    inventorySlots[i].item.itemID == item.itemID && 
                    inventorySlots[i].amount < item.maxStackAmount)
                {
                    inventorySlots[i].amount += amount;
                    RefreshUI();
                    return true;
                }
            }
        }

        int emptySlotIndex = FindEmptySlot();
        
        if (emptySlotIndex != -1)
        {
            Debug.Log($"Boş slot bulundu: {emptySlotIndex}");
            inventorySlots[emptySlotIndex].item = item;
            inventorySlots[emptySlotIndex].amount = amount;
            RefreshUI();
            return true;
        }

        Debug.Log("Envanter dolu! (Toplam slot sayısı: " + inventorySlots.Count + ")");
        return false;
    }
    
    private int FindEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = new ItemSlot();
            }
            
            if (inventorySlots[i].item == null || inventorySlots[i].amount <= 0)
            {
                return i;
            }
        }
        return -1;
    }

    public void RefreshUI()
    {
        if (uiManager == null)
        {
            Debug.LogError("uiManager null");
            return;
        }

        List<GameObject> allSlots = uiManager.GetAllSlots();

        foreach (GameObject slot in allSlots)
        {
            Slot slotScript = slot.GetComponent<Slot>();
            if (slotScript != null)
                slotScript.ClearSlot();
        }

        for (int i = 0; i < inventorySlots.Count && i < allSlots.Count; i++)
        {
            Slot slotScript = allSlots[i].GetComponent<Slot>();

            if (slotScript != null)
            {
                if (inventorySlots[i].item != null && inventorySlots[i].amount > 0)
                {
                    slotScript.SetItem(inventorySlots[i].item, inventorySlots[i].amount);
                }
                else
                {
                    slotScript.ClearSlot();
                    inventorySlots[i].item = null;
                    inventorySlots[i].amount = 0;
                }
            }
        }
    }

    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Count)
        {
            if (inventorySlots[slotIndex].item != null)
            {
                string itemName = inventorySlots[slotIndex].item.itemName;

                if (inventorySlots[slotIndex].amount <= amount)
                {
                    Debug.Log($"{itemName} envanterden tamamen çıkarıldı.");
                    inventorySlots[slotIndex].item = null;
                    inventorySlots[slotIndex].amount = 0;
                }
                else
                {
                    inventorySlots[slotIndex].amount -= amount;
                    Debug.Log($"{amount} adet {itemName} envanterden çıkarıldı. Kalan: {inventorySlots[slotIndex].amount}");
                }

                RefreshUI();
            }
        }
    }

    public void UseItem(int slotID)
{
    if (slotID >= 0 && slotID < inventorySlots.Count && inventorySlots[slotID].item != null)
    {
        Item item = inventorySlots[slotID].item;
        
        if (item.isEquippable && EquipmentManager.instance != null)
        {
            EquipmentManager.instance.EquipItem(item, slotID);
        }
        else
        {
            Debug.Log($"{item.itemName} kullanıldı!");
            
            inventorySlots[slotID].amount--;
            
            if (inventorySlots[slotID].amount <= 0)
            {
                inventorySlots[slotID].item = null;
                inventorySlots[slotID].amount = 0;
            }
            
            RefreshUI();
        }
    }
}

    public void AddRandomItem()
    {
        System.Random random = new System.Random();
        if (ItemDatabase.instance == null)
        {
            Debug.LogError("ItemDatabase instance bulunamadı");
            return;
        }

        List<string> allItemIDs = ItemDatabase.instance.GetAllItemIDs();
        if(allItemIDs.Count == 0)
        {
            Debug.Log("Item Bulunamadı.");
            return;
        }
        
        string randomID = allItemIDs[random.Next(0, allItemIDs.Count)];
        Item item = ItemDatabase.instance.GetItemByID(randomID);

        if (item != null)
        {
            bool added = AddItem(item);

            if (added)
            {
                Debug.Log($"{item.itemName} envanterinize eklendi!");
            }
            else
            {
                Debug.Log("Envanter dolu, item eklenemedi!");
            }
        }
    }
}
