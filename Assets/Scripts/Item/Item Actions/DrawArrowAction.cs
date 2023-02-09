using UnityEngine;

[CreateAssetMenu(menuName = "Item Actions/Draw Arrow Action")]
public class DrawArrowAction : ItemAction
{
    public override void PerformAction(PlayerManager playerManager){
        if(playerManager.isInteracting || playerManager.isHoldingArrow || playerManager.playerEquipment.arrowStack <= 0) return;

        Aim(playerManager);

        // Animation
        playerManager.playerAnimator.anim.SetBool("isHoldingArrow", true);
        playerManager.playerAnimator.PlayAnimation("Get Arrow", false);

        // Tạo arrow
        GameObject loadedArrow = Instantiate(playerManager.playerEquipment.arrow.loadedItemModel, playerManager.weaponSlotManager.rightHand.transform);
        playerManager.playerEffects.currentRangeFX = loadedArrow;

        // Bow Animation: draw -> aim
        Animator bowAnim = playerManager.weaponSlotManager.leftHand.GetComponentInChildren<Animator>();
        bowAnim.SetBool("isDrawn", true);
        bowAnim.Play("Draw");
    }

    void Aim(PlayerManager playerManager){
        if(playerManager.isAiming) return;
        
        playerManager.isAiming = true;
    }
}
