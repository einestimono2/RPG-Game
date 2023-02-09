using UnityEngine;
using static Enums;

public class Consumable : Item
{
    [Header("Model")]
    public GameObject itemModel;

    [Header("Animation")]
    public string animation;
    public bool isInteracting;

    public void Awake(){
        itemType = ItemType.Consumable;
        equipType = EquipType.Hotbar;
    }

    public virtual void AttemptToConsumableItem(AnimatorManager animatorManager, WeaponSlotManager weaponSlotManager, PlayerEffects playerEffects){
        animatorManager.PlayAnimation(animation, isInteracting);
    }

}
