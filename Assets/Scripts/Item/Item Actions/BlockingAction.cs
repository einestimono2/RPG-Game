using UnityEngine;

[CreateAssetMenu(menuName = "Item Actions/Blocking Action")]
public class BlockingAction : ItemAction{
    public override void PerformAction(PlayerManager playerManager){
        if(playerManager.isInteracting || playerManager.isBlocking || playerManager.playerStats.currentStamina <= 0) return;

        playerManager.playerCombat.SetBlockingAbsorptionFromWeapon();

        playerManager.isBlocking = true;
    }
}
