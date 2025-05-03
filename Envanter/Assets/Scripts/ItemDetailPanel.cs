using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour
{
    public static ItemDetailPanel instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private GameObject statsContainer;

    [Header("Stat Elements")]
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI agilityText;
    [SerializeField] private TextMeshProUGUI luckText;

    [Header("Quality Colors")]
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color uncommonColor = Color.green;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color epicColor = new Color(0.5f, 0, 0.5f);
    [SerializeField] private Color legendaryColor = Color.red;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (detailsPanel != null)
            detailsPanel.SetActive(false);
    }

    public void ShowDetailPanel(Item item)
    {
        Outline outline = itemIconImage.GetComponent<Outline>();

        if (item== null || detailsPanel == null)
        {
            return;
        }

        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription;

        if(itemIconImage != null && item.itemIcon != null)
        {
            itemIconImage.sprite = item.itemIcon;
            itemIconImage.enabled = true;
            outline.effectColor = GetQualityColor(item.itemQuality);
        }
        if(statsContainer!= null)
        {
            statsContainer.SetActive(item.isEquippable);
        }
        if(item.isEquippable)
        {
            attackText.text = "Saldırı: " + item.attackValue;
            defenseText.text = "Savunma: " + item.defenseValue;
            agilityText.text = "Çeviklik: " + item.agilityValue;
            luckText.text = "Şans: " + item.luckValue;
        }

        detailsPanel.SetActive(true);
    }
    public void HideItemDetails()
    {
        if (detailsPanel != null)
            detailsPanel.SetActive(false);
    }

    private Color GetQualityColor(int quality)
    {
        switch (quality)
        {
            case 0: return normalColor;
            case 1: return uncommonColor;
            case 2: return rareColor;
            case 3: return epicColor;
            case 4: return legendaryColor;
            default: return normalColor;
        }
    }
}
