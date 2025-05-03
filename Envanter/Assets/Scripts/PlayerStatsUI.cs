using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI agilityText;
    [SerializeField] private TextMeshProUGUI luckText;

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (attackText != null)
        {
            attackText.text = $"Saldýrý: {Player.instance.TotalAttack}";
        }
        if (defenseText != null)
        {
            defenseText.text = $"Savunma: {Player.instance.TotalDefense}";
        }
        if (agilityText != null)
        {
            agilityText.text = $"Çeviklik: {Player.instance.TotalAgility}";
        }
        if (luckText != null)
        {
            luckText.text = $"Þans: {Player.instance.TotalLuck}";
        }
    }
}
