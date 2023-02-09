using UnityEngine;
using static Enums;

public class DamageCollider : MonoBehaviour
{
    public int teamID = 0;
    public float currentWeaponDamage = 25;
    public bool enableColliderOnStartUp = false;

    public CharacterManager characterManager;

    Collider damageCollider;
    protected bool parried = false;
    protected bool blocked = false;

    void Awake(){
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = enableColliderOnStartUp;
    }

    public void EnableCollider(){
        damageCollider.enabled = true;
    }

    public void DisableCollider(){
        damageCollider.enabled = false;
    }

    protected virtual void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            parried = false;
            blocked = false;

            PlayerManager playerManager = other.GetComponent<PlayerManager>();

            if(playerManager != null){
                if(playerManager.isInvulnerable) return;

                // Parrying
                CheckParry(playerManager);
                
                // Blocking
                CheckBlock(playerManager);
            }

            if(playerManager.playerStats != null){
                if(playerManager.playerStats.teamID == teamID || parried || blocked) return;

                // Máu tại điểm tiếp xúc
                Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                playerManager.playerEffects.PlayBloodSplatterFX(contactPoint);

                // Sát thương dựa theo loại tấn công
                playerManager.playerStats.TakeDame(currentWeaponDamage);
                DisableCollider();

                if(playerManager.playerStats.isDead){
                    EnemyManager enemyManager = gameObject.GetComponentInParent<EnemyManager>();
                    if(enemyManager != null) enemyManager.HandleCharacterDead();
                }
            }
        }

        if(other.tag == "Enemy"){
            EnemyManager enemyManager = other.GetComponent<EnemyManager>();
            
            if(enemyManager != null){
                if(enemyManager.isInvulnerable) return;
                if(enemyManager.enemyStats.teamID == teamID) return;
                
                // Hiệu ứng tóe máu tại điểm va chạm
                Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                enemyManager.enemyEffects.PlayBloodSplatterFX(contactPoint);

                DealDamage(characterManager as PlayerManager, enemyManager.enemyStats);
                DisableCollider();
                enemyManager.enemyWeaponManager.DisableWeaponDamage();

                if(enemyManager.target == null){
                    enemyManager.target = gameObject.GetComponentInParent<CharacterStats>();
                }
            }
        }
    }

    protected virtual void CheckParry(PlayerManager playerManager){
        if(playerManager.isParrying){
            (characterManager as EnemyManager).enemyWeaponManager.DisableWeaponDamage();
            // Quái bị choáng + Không nhận sát thương
            characterManager.GetComponent<AnimatorManager>().PlayAnimation("Stun", true);
            parried = true;
        }
    }

    protected virtual void CheckBlock(PlayerManager playerManager){
        Vector3 directionFromPlayerToEnemy = characterManager.transform.position - playerManager.transform.position;
        float dotValueFromPlayerToEnemy = Vector3.Dot(directionFromPlayerToEnemy, playerManager.transform.forward);
        
        if(playerManager.isBlocking && dotValueFromPlayerToEnemy > 0.3f){
            blocked = true;
            float damageAfterBlock = currentWeaponDamage - (currentWeaponDamage * playerManager.playerStats.blockingDamageAbsorption / 100);
        
            playerManager.playerCombat.AttemptBlock(this, damageAfterBlock);
            DisableCollider();

            if(playerManager.playerStats.isDead){
                EnemyManager enemyManager = gameObject.GetComponentInParent<EnemyManager>();
                if(enemyManager != null) enemyManager.HandleCharacterDead();
            }
        }
    }

    protected virtual void DealDamage(PlayerManager playerManager, EnemyStats enemyStats){
        float finalDamage = currentWeaponDamage;

        if(playerManager.isUsingRightHand){
            if(playerManager.playerCombat.currentAttackType == AttackType.Light){
                finalDamage *= playerManager.playerEquipment.rightWeapon.lightAttackDamageModifier;
            }else if(playerManager.playerCombat.currentAttackType == AttackType.Heavy){
                finalDamage *= playerManager.playerEquipment.rightWeapon.heavyAttackDamageModifier;
            }
        }else if(playerManager.isUsingLeftHand){
            if(playerManager.playerCombat.currentAttackType == AttackType.Light){
                finalDamage *= playerManager.playerEquipment.leftWeapon.lightAttackDamageModifier;
            }else if(playerManager.playerCombat.currentAttackType == AttackType.Heavy){
                finalDamage *= playerManager.playerEquipment.leftWeapon.heavyAttackDamageModifier;
            }
        }

        enemyStats.TakeDame(finalDamage);
    }
}