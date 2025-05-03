using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro.EditorUtilities;
public class Slot : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private Button useButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button dropButton;


    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI amountText;


    private Item currentItem;
    private int currentAmount;
    public int slotID;
    public bool isEquipmentSlot;
    public Item.EquipmentType equipmentType = Item.EquipmentType.None;

    private void Start()
    {
        ClearSlot();

        if(actionPanel != null)
        {
            actionPanel.SetActive(false);
            
            Canvas panelCanvas = actionPanel.GetComponent<Canvas>();
            if (panelCanvas == null)
            {
                panelCanvas = actionPanel.AddComponent<Canvas>();
                panelCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                panelCanvas.sortingOrder = 100;
                
                if (actionPanel.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
                {
                    actionPanel.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }
            }
            else
            {
                panelCanvas.sortingOrder = 100;
            }
        }
        
        if(useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(UseItem);
        }
        if(equipButton != null)
        {
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(EquipItem);
        }
        if(dropButton != null)
        {
            dropButton.onClick.RemoveAllListeners();
            dropButton.onClick.AddListener(DropItem);
        }
    }

    public void SetItem(Item item,int amount)
    {
        Outline outline = itemImage.GetComponent<Outline>();

        if (item == null)
        {
            Debug.LogError("SetItem null item ile çağrıldı!");
            ClearSlot();
            return;
        }

        currentItem = item;
        currentAmount = amount;

        if (itemImage != null)
        {
            // Sprite kontrolü
            if (item.itemIcon == null)
            {
                Debug.LogWarning($"Item '{item.itemName}' için sprite null!");
                itemImage.sprite = null;
                itemImage.enabled = false;
                itemImage.color = new Color(1, 1, 1, 0);
                
            }
            else
            {
                itemImage.sprite = item.itemIcon;
                itemImage.enabled = true;
                itemImage.color = Color.white;

                switch (item.itemQuality)
                {
                    case 0: // Normal
                        outline.effectColor = Color.gray;
                        break;
                    case 1: // Uncommon
                        outline.effectColor = Color.green;
                        break;
                    case 2: // Rare
                        outline.effectColor = Color.blue;
                        break;
                    case 3: // Epic
                        outline.effectColor = new Color(0.5f, 0, 0.5f);
                        break;
                    case 4: // Legendary
                        outline.effectColor = Color.red;
                        break;
                    default:
                        outline.effectColor = Color.white;
                        break;
                }
            }
        }

        if (amountText != null)
        {
            if (amount <= 1)
                amountText.text = "";
            else
                amountText.text = amount.ToString();

            amountText.enabled = amount > 1;
        }
    }
    private void UseItem()
    {
        if (currentItem != null)
        {      
            Debug.Log($"UseItem çağrıldı: Slot ID = {slotID}, Item = {currentItem.itemName}");
              
            // Instance üzerinden erişim yapalım
            if (InventoryManager.instance != null)
            {
                // Item'ı kullan
                InventoryManager.instance.UseItem(slotID);
            }
            else
            {
                Debug.LogError("InventoryManager instance bulunamadı!");
                // Instance yoksa FindObjectOfType ile deneyelim
                InventoryManager inventoryManager = FindAnyObjectByType<InventoryManager>();
                if (inventoryManager != null)
                {
                    inventoryManager.UseItem(slotID);
                }
            }

            if (actionPanel != null)
                actionPanel.SetActive(false);
        }
    }
    private void DropItem()
    {
        if (currentItem != null)
        {
            Debug.Log($"DropItem çağrıldı: Slot ID = {slotID}, Item = {currentItem.itemName}");
            
            // Instance üzerinden erişim yapalım
            if (InventoryManager.instance != null)
            {
                InventoryManager.instance.RemoveItem(slotID);
            }
            else
            {
                Debug.LogError("InventoryManager instance bulunamadı!");
                // Instance yoksa FindObjectOfType ile deneyelim
                InventoryManager inventoryManager = FindAnyObjectByType<InventoryManager>();
                if (inventoryManager != null)
                {
                    inventoryManager.RemoveItem(slotID);
                }
            }

            if (actionPanel != null)
                actionPanel.SetActive(false);
        }
    }
    private void EquipItem()
    {
        if (currentItem != null && currentItem.isEquippable)
        {
            Debug.Log($"EquipItem çağrıldı: Slot ID = {slotID}, Item = {currentItem.itemName}");
            
            if (EquipmentManager.instance != null)
            {
                EquipmentManager.instance.EquipItem(currentItem, slotID);
            }
            else
            {
                Debug.LogError("EquipmentManager instance bulunamadı!");
            }

            if (actionPanel != null)
                actionPanel.SetActive(false);
        }
    }
    private void CloseAllActionPanels()
    {
        Slot[] allSlots = FindObjectsOfType<Slot>();

        foreach (Slot slot in allSlots)
        {
            if (slot.actionPanel != null)
                slot.actionPanel.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null)
            return;

        if (currentItem != null)
        {
            if (ItemDetailPanel.instance != null)
            {
                ItemDetailPanel.instance.ShowDetailPanel(currentItem);
            }
            CloseAllActionPanels();

            if (actionPanel != null)
            {
                actionPanel.transform.SetAsLastSibling();
                actionPanel.SetActive(true);

                if (isEquipmentSlot)
                {
                    if (useButton != null)
                        useButton.gameObject.SetActive(false);

                    if (equipButton != null)
                        equipButton.gameObject.SetActive(false);

                    if (dropButton != null)
                    {
                        dropButton.gameObject.SetActive(true);
                        TextMeshProUGUI buttonText = dropButton.GetComponentInChildren<TextMeshProUGUI>();
                        if (buttonText != null)
                            buttonText.text = "Çıkar";

                        dropButton.onClick.RemoveAllListeners();
                        dropButton.onClick.AddListener(UnequipItem);
                    }
                }
                else
                {
                    if (useButton != null)
                    {
                        useButton.gameObject.SetActive(true);
                        useButton.onClick.RemoveAllListeners();
                        useButton.onClick.AddListener(UseItem);
                    }

                    if (equipButton != null)
                    {
                        equipButton.gameObject.SetActive(currentItem.isEquippable);
                        if (currentItem.isEquippable)
                        {
                            equipButton.onClick.RemoveAllListeners();
                            equipButton.onClick.AddListener(EquipItem);
                        }
                    }

                    if (dropButton != null)
                    {
                        dropButton.gameObject.SetActive(true);
                        TextMeshProUGUI buttonText = dropButton.GetComponentInChildren<TextMeshProUGUI>();
                        if (buttonText != null)
                            buttonText.text = "At";

                        dropButton.onClick.RemoveAllListeners();
                        dropButton.onClick.AddListener(DropItem);
                    }
                }
            }
        }
        else
        {
            if (ItemDetailPanel.instance != null)
            {
                ItemDetailPanel.instance.HideItemDetails();
            }
        }


    }
    public void ClearSlot()
    {
        currentItem = null;
        currentAmount = 0;

        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
            itemImage.color = new Color(1, 1, 1, 0);
        }

        if (amountText != null)
        {
            amountText.text = "";
            amountText.enabled = false;
        }

        if (actionPanel != null)
            actionPanel.SetActive(false);
    }
    private void UnequipItem()
    {
        if (currentItem != null && isEquipmentSlot)
        {
            EquipmentManager.instance.UnequipItem(equipmentType);

            if (actionPanel != null)
                actionPanel.SetActive(false);
            if (InventoryManager.instance != null)
            {
                InventoryManager.instance.RefreshUI();
            }
        }
    }
}
