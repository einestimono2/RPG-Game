using UnityEngine;
using static Enums;

public class PlayerCombat : MonoBehaviour
{
    public string lastAttack;

    public AttackType currentAttackType;

    PlayerManager playerManager;

    [Header("Animation One Hand")]
    public string oh_heavyAttack = "1H Attack 0";
    public string oh_lightAttack1 = "1H Attack 1";
    public string oh_lightAttack2 = "1H Attack 2";

    void Awake(){
        playerManager = GetComponent<PlayerManager>();
    }

    public float GetStaminaCost(){
        if(playerManager.isUsingRightHand){
            if(currentAttackType == AttackType.Light){
                return playerManager.playerEquipment.rightWeapon.baseStaminaCost * playerManager.playerEquipment.rightWeapon.lightAttackStaminaMultiplier;
            }else if(currentAttackType == AttackType.Heavy){
                return playerManager.playerEquipment.rightWeapon.baseStaminaCost * playerManager.playerEquipment.rightWeapon.heavyAttackStaminaMultiplier;
            }
        }else if(playerManager.isUsingLeftHand){
            if(currentAttackType == AttackType.Light){
                return playerManager.playerEquipment.leftWeapon.baseStaminaCost * playerManager.playerEquipment.leftWeapon.lightAttackStaminaMultiplier;
            }else if(currentAttackType == AttackType.Heavy){
                return playerManager.playerEquipment.leftWeapon.baseStaminaCost * playerManager.playerEquipment.leftWeapon.heavyAttackStaminaMultiplier;
            }
        }

        return 0;
    }

    public void DrainStaminaBasedOnAttack(float cost){
        playerManager.playerStats.DeductStamina(cost);
    }

    public void SetBlockingAbsorptionFromWeapon(){
        if(playerManager.isUsingRightHand){
            playerManager.playerStats.blockingDamageAbsorption = playerManager.playerEquipment.rightWeapon.damageAbsorption;
            playerManager.playerStats.blockingStabilityRating = playerManager.playerEquipment.rightWeapon.stability;
        }else if(playerManager.isUsingLeftHand){
            playerManager.playerStats.blockingDamageAbsorption = playerManager.playerEquipment.leftWeapon.damageAbsorption;
            playerManager.playerStats.blockingStabilityRating = playerManager.playerEquipment.leftWeapon.stability;
        }
    }

    public void AttemptBlock(DamageCollider attackingWeapon, float damage){
        float staminaDamageAbsorption = damage * playerManager.playerStats.blockingStabilityRating / 100;

        float staminaDamage = damage - staminaDamageAbsorption;

        DrainStaminaBasedOnAttack(staminaDamage);

        if(playerManager.playerStats.currentStamina <= 0){
            playerManager.isBlocking = false;
            // Guard Break 
            playerManager.playerStats.TakeDameAfterBlock(damage, "Guard Break");
            Debug.Log("Break Shielld");
        }else{
            // Block Animation
            playerManager.playerStats.TakeDameAfterBlock(damage);
        }
    }

}
