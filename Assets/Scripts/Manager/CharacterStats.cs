using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Information")]
    public string ID = "ENEMY";
    public int teamID = 0;
    public bool isDead;

    [Header("LEVELS")]
    public int level = 1;
    public int healthByLevel = 10;
    public int expByLevel = 10;
    public int staminaByLevel = 10;
    
    [Header("HP")]
    public float maxHealth = 100;
    public float currentHealth = 100;

    [Header("EXP")]
    public float levelEXP = 100;
    public float currentEXP = 0;

    [Header("Stamina")]
    public float maxStamina = 10;
    public float currentStamina = 0;

    [Header("Currency")]
    public int coin = 0;

    [Header("Armor Absorptions")]
    public float damageAbsorptionHelmet;
    public float damageAbsorptionArmor;
    public float damageAbsorptionLegs;
    public float damageAbsorptionBoots;
    public float damageAbsorptionGloves;
    public float damageAbsorptionCape;

    [Header("Blocking Absorption")]
    public float blockingDamageAbsorption; // % damage giảm
    public float blockingStabilityRating; // % stamina mất khi chặn lượng damage

    public virtual void TakeDame(float damage){
        
    }

    public virtual void TakeDameAfterBlock(float damage, string animation = "Block Guard"){
        
    }
}
