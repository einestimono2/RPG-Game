using UnityEngine;

public class RangedProjectileDamageCollider : DamageCollider
{
    public ArrowData arrow;
    protected bool hasAlreadyPenetratedASurface;
    protected GameObject penetratedProjectile;

    protected override void OnTriggerEnter(Collider other){
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

                Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                playerManager.playerEffects.PlayBloodSplatterFX(contactPoint);

                playerManager.playerStats.TakeDame(currentWeaponDamage);

                if(playerManager.playerStats.isDead){
                    (characterManager as EnemyManager).HandleCharacterDead();
                }
            }
        }

        if(other.tag == "Enemy"){
            EnemyManager enemyManager = other.GetComponent<EnemyManager>();

            if(enemyManager != null){
                if(enemyManager.isInvulnerable) return;

                Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                enemyManager.enemyEffects.PlayBloodSplatterFX(contactPoint);

                enemyManager.enemyStats.TakeDame(currentWeaponDamage);

                if(enemyManager.target == null){
                    enemyManager.target = characterManager.GetComponent<CharacterStats>();
                }
            }
        }
    
        if(!hasAlreadyPenetratedASurface && penetratedProjectile == null){
            hasAlreadyPenetratedASurface = true;
        
            Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            GameObject penetratedArrow = Instantiate(arrow.penetratedItemModel, contactPoint, Quaternion.Euler(0, 0, 0));
        
            penetratedProjectile = penetratedArrow;
            penetratedArrow.transform.parent = other.transform;
            penetratedArrow.transform.rotation = Quaternion.LookRotation(gameObject.transform.forward);
        }

        Destroy(transform.root.gameObject);
    }
}
