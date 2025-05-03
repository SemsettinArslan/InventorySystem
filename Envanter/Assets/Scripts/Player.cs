using UnityEngine;

public class Player : MonoBehaviour
{
      
    public static Player instance;

    [Header("Base Stats")]
    public int baseAttack = 150;
    public int baseDefense = 25;
    public int baseAgility = 80;
    public int baseLuck = 55;

    [Header("Ekipman Bonusları")]
    public int equipmentAttackBonus = 0;
    public int equipmentDefenseBonus = 0;
    public int equipmentAgilityBonus = 0;
    public int equipmentLuckBonus = 0;

    [Header("Total Stats")]
    [SerializeField] private int totalAttack;
    [SerializeField] private int totalDefense;
    [SerializeField] private int totalAgility;
    [SerializeField] private int totalLuck;

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
        UpdateStats();
    }

    public void UpdateEquipmentBonuses(int attack, int defense, int agility, int luck)
    {
        equipmentAttackBonus = attack;
        equipmentDefenseBonus = defense;
        equipmentAgilityBonus = agility;
        equipmentLuckBonus = luck;

        UpdateStats();
    }

    private void UpdateStats()
    {
        totalAttack = baseAttack + equipmentAttackBonus;
        totalDefense = baseDefense + equipmentDefenseBonus;
        totalAgility = baseAgility + equipmentAgilityBonus;
        totalLuck = baseLuck + equipmentLuckBonus;

        Debug.Log($"Karakter statları güncellendi: Saldırı={totalAttack}, Savunma={totalDefense}, çeviklik={totalAgility}, şans={totalLuck}");

    }


    public int TotalAttack => totalAttack;
    public int TotalDefense => totalDefense;
    public int TotalAgility => totalAgility;
    public int TotalLuck => totalLuck;

}
