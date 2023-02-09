using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Settings")]
    public bool isActive;
    public bool isAwakened;
    public bool isDefeated;

    [Header("FX")]
    public GameObject normalEffect;
    public GameObject secondPhaseEffect;

    [Header("Refs")]
    public BossUIManager bossUIManager;
    public EnemyAnimator enemyAnimator;
    public EnemyManager enemyManager;
    public EnemyStats enemyStats;
    public CharacterStats playerStats;
    
    public BossCombatStanceState bossCombatStanceState;
    BossFightCollider bossFightCollider;

    private void Awake() {    
        bossCombatStanceState = GetComponentInChildren<BossCombatStanceState>();

        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyManager = GetComponent<EnemyManager>();
        enemyStats = GetComponent<EnemyStats>();

        enemyStats.isBoss = true;

        normalEffect.SetActive(true);
        secondPhaseEffect.SetActive(false);

        // Gán sự kiện
        EventManager.OnBossFight += BossFight;
    }

    private void LateUpdate() {
        if(playerStats != null && playerStats.isDead){
            enemyStats.currentHealth = enemyStats.maxHealth;
            UpdateHealth(enemyStats.currentHealth, enemyStats.maxHealth);
            bossUIManager.DeactivateUI();
            
            isActive = false;
            normalEffect.SetActive(true);
            secondPhaseEffect.SetActive(false);

            // Mở cửa lại
            foreach (var wall in bossFightCollider.walls){
                wall.SetActive(false);
            }
        }
    }

    void BossFight(BossFightCollider _collider, CharacterStats _playerStats){
        if(!isActive){
            playerStats = _playerStats;
            bossFightCollider = _collider;
            bossUIManager = playerStats.GetComponentInChildren<BossUIManager>();
            bossUIManager.InitializeUI(enemyStats.enemyName, enemyStats.GetHealthByLevel());

            ActivateBossFight();
        }
    }

    public void ActivateBossFight(){
        isActive = true;
        bossUIManager.ActivateUI();
        
        enemyStats.enemyNameText.gameObject.SetActive(false);
        enemyManager.target = playerStats;

        // Đóng cửa lại
        foreach (var wall in bossFightCollider.walls){
            wall.SetActive(true);
        }
    }

    public void BossHasBeenDefeated(){
        bossUIManager.DeactivateUI();

        // Hủy sự kiện
        EventManager.OnBossFight -= BossFight;

        // Mở cửa lại
        foreach (var wall in bossFightCollider.walls){
            wall.SetActive(false);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth){
        bossUIManager.UpdateHealth(currentHealth);

        if(currentHealth <= maxHealth / 2 && !bossCombatStanceState.hasPhaseShifted){
            ActivateSecondPhase();
        }
    }

    public void ActivateSecondPhase(){
        // Animation 
        enemyAnimator.PlayAnimation("Boost", true);

        normalEffect.SetActive(false);
        secondPhaseEffect.SetActive(true);

        if(enemyManager.enemyWeaponManager.leftWeapon != null){
            enemyManager.enemyWeaponManager.leftHandDamage.currentWeaponDamage *= 2;
        }

        if(enemyManager.enemyWeaponManager.rightWeapon != null){
            enemyManager.enemyWeaponManager.rightHandDamage.currentWeaponDamage *= 2;
        }

        bossCombatStanceState.hasPhaseShifted = true;
    }
}
