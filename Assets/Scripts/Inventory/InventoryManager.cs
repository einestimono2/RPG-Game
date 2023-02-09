using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enums;

public class InventoryManager : MonoBehaviour
{
    [Header("Settings")]
    public int inventorySize = 48;

    [Header("UI")]
    public GameObject inventoryUI;
    public GameObject hudUI;

    [Header("Drop Item")]
    public GameObject dropModel;
    public Transform dropPos;

    [Header("Item Info")]
    public Transform canvas;
    public GameObject itemInfoPrefab;
    private GameObject currentItemInfo = null;

    [Header("Inventory Slot")]
    public InventorySlot[] inventorySlots;
    public Transform inventorySlotParent;
    public GameObject inventorySlotPrefab;
    
    [Header("Equipment Slot")]
    public TMP_Text playerInfo;
    public InventorySlot[] equipmentSlots;

    [Header("Ref")]
    PlayerEquipment playerEquipment;
    PlayerStats playerStats;

    void Start(){
        playerEquipment = GetComponent<PlayerEquipment>();
        playerStats = GetComponent<PlayerStats>();

        GenerateInventory();
    }

    void GenerateEquipment(){
        // Helmet: index 0
        if(playerEquipment.helmet != null){
            equipmentSlots[0].AddItem(playerEquipment.helmet);
            equipmentSlots[0].UpdateSlot();
        }else{
            equipmentSlots[0].DeleteItem();
        }

        // Cape: index 1
        if(playerEquipment.cape != null){
            equipmentSlots[1].AddItem(playerEquipment.cape);
            equipmentSlots[1].UpdateSlot();
        }else{
            equipmentSlots[1].DeleteItem();
        }

        // Armor: index 2
        if(playerEquipment.armor != null){
            equipmentSlots[2].AddItem(playerEquipment.armor);
            equipmentSlots[2].UpdateSlot();
        }else{
            equipmentSlots[2].DeleteItem();
        }

        // Armor: index 3
        if(playerEquipment.legs != null){
            equipmentSlots[3].AddItem(playerEquipment.legs);
            equipmentSlots[3].UpdateSlot();
        }else{
            equipmentSlots[3].DeleteItem();
        }

        // Boots: index 4
        if(playerEquipment.boots != null){
            equipmentSlots[4].AddItem(playerEquipment.boots);
            equipmentSlots[4].UpdateSlot();
        }else{
            equipmentSlots[4].DeleteItem();
        }

        // Boots: index 5
        if(playerEquipment.gloves != null){
            equipmentSlots[5].AddItem(playerEquipment.gloves);
            equipmentSlots[5].UpdateSlot();
        }else{
            equipmentSlots[5].DeleteItem();
        }

        // Left Weapon: index 8
        if(playerEquipment.leftWeapon.weaponType != WeaponType.Unarmed){
            equipmentSlots[8].AddItem(playerEquipment.leftWeapon);
            equipmentSlots[8].UpdateSlot();
        }else{
            equipmentSlots[8].DeleteItem();
        }

        // Right Weapon: index 7
        if(playerEquipment.rightWeapon.weaponType != WeaponType.Unarmed){
            equipmentSlots[7].AddItem(playerEquipment.rightWeapon);
            equipmentSlots[7].UpdateSlot();
        }else{
            equipmentSlots[7].DeleteItem();
        }

        // Hotbar: index 6 
        if(playerEquipment.consumable != null && playerEquipment.consumableStack > 0){
            equipmentSlots[6].AddItem(playerEquipment.consumable, playerEquipment.consumableStack);
            equipmentSlots[6].UpdateSlot();
        }else{
            equipmentSlots[6].DeleteItem();
        }

        // Arrow: index 9
        if(playerEquipment.arrow != null && playerEquipment.arrowStack > 0){
            equipmentSlots[9].AddItem(playerEquipment.arrow, playerEquipment.arrowStack);
            equipmentSlots[9].UpdateSlot();
        }else{
            equipmentSlots[9].DeleteItem();
        }

        // Player Information
        playerInfo.text = 
        $@"Level: {playerStats.level} ({playerStats.currentEXP}/{playerStats.levelEXP})
HP: {playerStats.currentHealth} ({playerStats.currentHealth}/{playerStats.maxHealth})
Stamina: {playerStats.currentStamina} ({playerStats.currentStamina}/{playerStats.maxStamina})

Coins: {playerStats.coin}
        ";
    }

    void GenerateInventory(){
        List<InventorySlot> _inventorySlots = new List<InventorySlot>();

        for (int i = 0; i < inventorySize; i++){
            InventorySlot slot = Instantiate(inventorySlotPrefab.gameObject, inventorySlotParent).GetComponent<InventorySlot>();
        
            _inventorySlots.Add(slot);
        }

        inventorySlots = _inventorySlots.ToArray();
    }

    public void AddItem(PickUp pickUp){
        // Stackable
        if(pickUp.item.maxStack > 1){
            InventorySlot stackableSlot = null;

            for (int i = 0; i < inventorySlots.Length; i++){
                if(inventorySlots[i].item != null){
                    if(inventorySlots[i].item == pickUp.item && inventorySlots[i].stackSize < pickUp.item.maxStack){
                        stackableSlot = inventorySlots[i];
                        break;
                    }
                }
            }

            if(stackableSlot != null){
                // Vượt quá maxStack
                if(stackableSlot.stackSize + pickUp.stackSize > pickUp.item.maxStack){
                    int amountLeft = (stackableSlot.stackSize + pickUp.stackSize) - pickUp.item.maxStack;

                    stackableSlot.AddItem(pickUp.item, pickUp.item.maxStack);

                    for (int i = 0; i < inventorySlots.Length; i++){
                        if (inventorySlots[i].item == null){
                            inventorySlots[i].AddItem(pickUp.item, amountLeft);
                            inventorySlots[i].UpdateSlot();
                            // Event
                            EventManager.ItemFetched(pickUp.item, pickUp.stackSize);

                            break;
                        }
                    }

                    Destroy(pickUp.gameObject);
                }else{
                    stackableSlot.AddStackAmount(pickUp.stackSize);
                    // Event
                    EventManager.ItemFetched(pickUp.item, pickUp.stackSize);
                    Destroy(pickUp.gameObject);
                }

                stackableSlot.UpdateSlot();
            }else{
                InventorySlot emptySlot = null;

                // Tìm vị trí trống
                for (int i = 0; i < inventorySlots.Length; i++){
                    if (inventorySlots[i].item == null){
                        emptySlot = inventorySlots[i];
                        break;
                    }
                }

                // Nếu tìm thấy ==> Thêm vào
                if (emptySlot != null){
                    emptySlot.AddItem(pickUp.item, pickUp.stackSize);
                    emptySlot.UpdateSlot();
                    // Event
                    EventManager.ItemFetched(pickUp.item, pickUp.stackSize);

                    Destroy(pickUp.gameObject);
                }else{
                    pickUp.transform.position = dropPos.position;
                }
            }
        }else{ // Vật phẩm chỉ có 1 stack
            InventorySlot emptySlot = null;

            // Tìm vị trí trống
            for (int i = 0; i < inventorySlots.Length; i++){
                if (inventorySlots[i].item == null){
                    emptySlot = inventorySlots[i];
                    break;
                }
            }

            // Nếu còn ==> thêm vào
            if (emptySlot != null){
                emptySlot.AddItem(pickUp.item, pickUp.stackSize);
                emptySlot.UpdateSlot();
                // Event
                EventManager.ItemFetched(pickUp.item, pickUp.stackSize);

                Destroy(pickUp.gameObject);
            }else{
                pickUp.transform.position = dropPos.position;
            }

        }
    }

    public void AddItem(Item item, int stackSize){
        // Stackable
        if(item.maxStack > 1){
            InventorySlot stackableSlot = null;

            for (int i = 0; i < inventorySlots.Length; i++){
                if(inventorySlots[i].item != null){
                    if(inventorySlots[i].item == item && inventorySlots[i].stackSize < item.maxStack){
                        stackableSlot = inventorySlots[i];
                        break;
                    }
                }
            }

            if(stackableSlot != null){
                // Vượt quá maxStack
                if(stackableSlot.stackSize + stackSize > item.maxStack){
                    int amountLeft = (stackableSlot.stackSize + stackSize) - item.maxStack;

                    stackableSlot.AddItem(item, item.maxStack);

                    for (int i = 0; i < inventorySlots.Length; i++){
                        if (inventorySlots[i].item == null){
                            inventorySlots[i].AddItem(item, amountLeft);
                            inventorySlots[i].UpdateSlot();
                            // Event
                            EventManager.ItemFetched(item, stackSize);

                            break;
                        }
                    }
                }else{
                    stackableSlot.AddStackAmount(stackSize);
                    // Event
                    EventManager.ItemFetched(item, stackSize);
                }

                stackableSlot.UpdateSlot();
            }else{
                InventorySlot emptySlot = null;

                // Tìm vị trí trống
                for (int i = 0; i < inventorySlots.Length; i++){
                    if (inventorySlots[i].item == null){
                        emptySlot = inventorySlots[i];
                        break;
                    }
                }

                // Nếu tìm thấy ==> Thêm vào
                if (emptySlot != null){
                    emptySlot.AddItem(item, stackSize);
                    emptySlot.UpdateSlot();

                    EventManager.ItemFetched(item, stackSize);
                }else{
                    DropItem(item, stackSize);
                }
            }
        }else{ // Vật phẩm chỉ có 1 stack
            InventorySlot emptySlot = null;

            // Tìm vị trí trống
            for (int i = 0; i < inventorySlots.Length; i++){
                if (inventorySlots[i].item == null){
                    emptySlot = inventorySlots[i];
                    break;
                }
            }

            // Nếu còn ==> thêm vào
            if (emptySlot != null){
                emptySlot.AddItem(item, stackSize);
                emptySlot.UpdateSlot();
                // Event
                EventManager.ItemFetched(item, stackSize);
            }else{
                DropItem(item, stackSize);
            }
        }
    }

    public void DropItem(InventorySlot item){
        PickUp pickUp = Instantiate(dropModel, dropPos).GetComponent<PickUp>();

        pickUp.transform.position = dropPos.position;
        pickUp.transform.SetParent(null);

        pickUp.item = item.item;
        pickUp.stackSize = item.stackSize;
        pickUp.SetSerialize();

        item.DeleteItem();
    }

    public void DropItem(Item item, int stackSize){
        PickUp pickUp = Instantiate(dropModel, dropPos).GetComponent<PickUp>();

        pickUp.transform.position = dropPos.position;
        pickUp.transform.SetParent(null);

        pickUp.item = item;
        pickUp.stackSize = stackSize;
        pickUp.SetSerialize();
    }
    
    public void SwapItems(InventorySlot from, InventorySlot to){
        // Sự kiện khi nhặt vật phẩm trong rương
        if(from.itemType == ItemType.Chest && to.itemType == ItemType.None){
            // Event
            EventManager.ItemFetched(from.item, from.stackSize);
        }

        // Cập nhật trang bị hoặc vũ khí
        if(from.itemType != ItemType.Chest && to.itemType != ItemType.Chest){
            // Kéo vật phẩm từ kho đồ vào trang bị 
            if(IsEquipmentItem(to) && !IsEquipmentItem(from)){
                playerEquipment.UpdateEquipment(from.item, to.equipType, from.stackSize, to.itemType);
            }
            // Kéo từ trang bị về kho đồ
            else if(!IsEquipmentItem(to) && IsEquipmentItem(from)){
                playerEquipment.UpdateEquipment(to.item, from.equipType, to.stackSize, from.itemType);
            }
            // Đổi chỗ 2 vũ khí trái và phải
            else{
                playerEquipment.UpdateEquipment(to.item, from.equipType);
                playerEquipment.UpdateEquipment(from.item, to.equipType);
            }
        }

        // Swap
        if(from.item != to.item){
            Item _item = from.item;
            int _stackSize = from.stackSize;

            from.item = to.item;
            from.stackSize = to.stackSize;

            to.item = _item;
            to.stackSize = _stackSize;
        }
        else{
            if(from.item.maxStack > 1){
                if(from.stackSize + to.stackSize > from.item.maxStack){
                    int amountLeft = (from.stackSize + to.stackSize) - from.item.maxStack;

                    from.stackSize = amountLeft;
                    to.stackSize = to.item.maxStack;
                }
            }else{
                Item _item = from.item;
                int _stackSize = from.stackSize;

                from.item = to.item;
                from.stackSize = to.stackSize;

                to.item = _item;
                to.stackSize = _stackSize;
            }
        }

        // Cập nhật slot
        from.UpdateSlot();
        to.UpdateSlot();

    }

    bool IsEquipmentItem(InventorySlot slot){
        return slot.itemType != ItemType.None && slot.itemType != ItemType.Chest;
    } 

    public void OpenInventory(){
        GenerateEquipment();

        inventoryUI.SetActive(true);
        hudUI.SetActive(false);
    }

    public void CloseInventory(){
        inventoryUI.SetActive(false);
        hudUI.SetActive(true);

        DestroyItemInfo();
    }

    public void DropAllItems(){
        // Drop items
        for (int i = 0; i < inventorySlots.Length; i++){
            if(inventorySlots[i].item != null){
                DropItem(inventorySlots[i]);
            }
        }

        // Drop equipment
        for (int i = 0; i < equipmentSlots.Length; i++){
            if(equipmentSlots[i].item != null){
                DropItem(equipmentSlots[i]);
            }
        }

        // Equipment
        playerEquipment.DeleteAllEquipments();
    }

    public void DisplayItemInfo(Item item, Vector2 buttonPosition, float offsetX, float offsetY){
        if(currentItemInfo != null){
            Destroy(currentItemInfo.gameObject);
        }

        buttonPosition.x += offsetX;
        buttonPosition.y += offsetY;

        currentItemInfo = Instantiate(itemInfoPrefab, buttonPosition, Quaternion.identity, canvas);
        currentItemInfo.GetComponent<ItemInfo>().Initialize(item);
    }

    public void DestroyItemInfo(){
        if(currentItemInfo != null){
            Destroy(currentItemInfo.gameObject);
        }
    }

    public bool CheckPrisonKey(string ID){
        for (int i = 0; i < inventorySlots.Length; i++){
            if(inventorySlots[i].item != null && inventorySlots[i].item.ID == ID){
                return true;
            }
        }

        return false;
    }
}
