using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private Transform slotsLayout;
    [SerializeField] private GameObject slotPrefab; 
    [SerializeField] private int slotCount = 24; 

    private List<GameObject> slots = new List<GameObject>();

    private void Start()
    {
        CreateSlots();
    }
    private void CreateSlots()
    {
         if (slotsLayout == null)
        slotsLayout = transform;
        ClearSlots();

        for(int i = 0; i < slotCount; i++)
        {
            GameObject SlotGo = Instantiate(slotPrefab, slotsLayout);
            SlotGo.name = "SlotGo_" + i;
            
            Slot slot=SlotGo.GetComponent<Slot>();
            if(slot != null )
            {
                slot.slotID= i;
                slot.isEquipmentSlot = false;
            }
            slots.Add(SlotGo);
        }
    }

    private void ClearSlots()
    {
        foreach(GameObject slot in slots)
        {
            if(slot != null)
            {
                Destroy(slot);
            }
        }
        slots.Clear();
    }

    public List<GameObject> GetAllSlots()
    {
        return slots;
    }
    public GameObject GetSlot(int index)
    {
        if(index>=0&& index < slots.Count)
        {
            return slots[index];
        }
        return null;
    }
}
