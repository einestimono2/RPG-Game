using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Items/Equipment")]
public class EquipmentData : Item
{
    [Header("Display")]
    public string modelName;

    [Header("Def")]
    public float def = 10;

    public void Awake(){
        itemType = ItemType.Equipment;
    }
}