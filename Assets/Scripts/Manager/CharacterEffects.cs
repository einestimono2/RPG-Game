using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
    [Header("Weapon FX")]
    public WeaponFX rightWeaponFX;
    public WeaponFX leftWeaponFX;

    [Header("Blood FX")]
    public GameObject bloodSplatterFX;


    public virtual void PlayWeaponFX(bool isLeft){
        if(isLeft){
            if(leftWeaponFX != null){
                leftWeaponFX.PlayWeaponFX();
            }
        }else{
            if(rightWeaponFX != null){
                rightWeaponFX.PlayWeaponFX();
            }
        }
    }

    public virtual void PlayBloodSplatterFX(Vector3 location){
        GameObject blood = Instantiate(bloodSplatterFX, location, Quaternion.identity);
    }
}
