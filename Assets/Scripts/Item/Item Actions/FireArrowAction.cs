using UnityEngine;

[CreateAssetMenu(menuName = "Item Actions/Fire Arrow Action")]
public class FireArrowAction : ItemAction
{
    public override void PerformAction(PlayerManager playerManager){
        // Vị trí arrow
        ArrowInstantiationLocation arrowInstantiationLocation = playerManager.weaponSlotManager.leftHand.GetComponentInChildren<ArrowInstantiationLocation>();

        // Animation bắn
        Animator bowAnim = playerManager.weaponSlotManager.leftHand.GetComponentInChildren<Animator>();
        bowAnim.SetBool("isDrawn", false);
        bowAnim.Play("Fire");

        // Cập nhật flag
        playerManager.playerAnimator.PlayAnimation("Shoot", true);
        playerManager.playerAnimator.anim.SetBool("isHoldingArrow", false);

        // Destroy arrow đã tạo ở cung
        Destroy(playerManager.playerEffects.currentRangeFX);

        // Tạo và bắn tên
        GameObject liveArrow = Instantiate(playerManager.playerEquipment.arrow.liveItemModel, arrowInstantiationLocation.transform.position, playerManager.transform.rotation);


        Ray ray = playerManager.cameraObject.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitPoint;

        if(Physics.Raycast(ray, out hitPoint, 100f)){
            liveArrow.transform.LookAt(hitPoint.point);
        }else{
            liveArrow.transform.rotation = Quaternion.Euler(playerManager.cameraObject.transform.localEulerAngles.x, playerManager.cameraObject.transform.eulerAngles.y, 0);
        }

        // Gán vận tốc
        Rigidbody rig = liveArrow.GetComponentInChildren<Rigidbody>();

        rig.AddForce(liveArrow.transform.forward * playerManager.playerEquipment.arrow.forwardVelocity);
        rig.AddForce(liveArrow.transform.up * playerManager.playerEquipment.arrow.upwardVelocity);
        rig.useGravity = playerManager.playerEquipment.arrow.useGravity;
        rig.mass = playerManager.playerEquipment.arrow.arrowMass;

        liveArrow.transform.parent = null;

        // Set damage
        RangedProjectileDamageCollider damageCollider = liveArrow.GetComponentInChildren<RangedProjectileDamageCollider>();
        damageCollider.arrow = playerManager.playerEquipment.arrow;
        damageCollider.currentWeaponDamage = playerManager.playerEquipment.arrow.damage + playerManager.playerEquipment.leftWeapon.baseDamage;

        playerManager.isAiming = false;
        playerManager.playerEquipment.arrowStack--;
        playerManager.playerEquipment.equipmentManager.UpdateArrow(playerManager.playerEquipment.arrow, playerManager.playerEquipment.arrowStack);
    }
}
