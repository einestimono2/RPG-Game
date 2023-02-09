using UnityEngine;

public class PlayerEffects : CharacterEffects
{
    public GameObject currentRangeFX;
    PlayerStats playerStats;
    WeaponSlotManager weaponSlotManager;

    public GameObject currentParticleFX;
    public GameObject instantiateFX;

    public int amount;
    public string type;

    private void Awake() {
        playerStats = GetComponent<PlayerStats>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    public void BuffPlayerFromEffect(){
        if(type == "EXP"){
            playerStats.AddEXP(amount);
        }else if(type == "MP"){
            playerStats.AddStamina(amount);
        }else if(type == "HP"){
            playerStats.HealHP(amount);
        }

        Destroy(instantiateFX.gameObject);

        weaponSlotManager.LoadAllWeapons();
    }
    
}
