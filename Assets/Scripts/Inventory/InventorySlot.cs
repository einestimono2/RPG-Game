using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using static Enums;

// Một ô trong kho đồ
public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    // khởi tạo
    [Header("Popup Settings")]
    public float offsetX = 135f;
    public float offsetY = 0f;

    [Header("Item Information")]
    public ItemType itemType = ItemType.None;
    public EquipType equipType = EquipType.None;
    public Image icon;
    public Item item;
    public int stackSize = 1;
    public TMP_Text stackText;

    [Header("Refs")]
    DragDropHandler dragDropHandler;
    InventoryManager inventoryManager;

    private void Start() {
        dragDropHandler = GetComponentInParent<DragDropHandler>();
        inventoryManager = GetComponentInParent<InventoryManager>();

        UpdateSlot();
    }

    // cập nhật lại 1 ô 
    public void UpdateSlot(){
        // vứt đồ hoặc là số lượng vật phẩm = 0 thì xoá đi
        if(item != null && item.itemType == ItemType.Consumable && stackSize <= 0){
            item = null;
        }

        // xoá đi thì cập nhật lại icon
        if(item == null){
            icon.sprite = null;
            icon.gameObject.SetActive(false);
            stackText.gameObject.SetActive(false);
        }else{ // không thì chỉ cập nhật lại icon hoặc số lượng
            icon.sprite = item.itemIcon;
            stackText.text = stackSize > 1 ? $"x{stackSize}" : "";

            icon.gameObject.SetActive(true);
            stackText.gameObject.SetActive(true);
        }
    }

    // thêm vật phẩm vào ô đang trống
    public void AddItem(Item _item, int _stackSze = 1){
        item = _item;
        stackSize = _stackSze;
    }

    // cộng thêm số lượng
    public void AddStackAmount(int _stackSze = 1){
        stackSize += _stackSze;
    }

    // vứt ra ngoài nhưng vẫn nhặt đc lại
    public void DropItem(){
        GetComponentInParent<InventoryManager>().DropItem(this);
    }

    // xoá vật phẩm
    public void DeleteItem(){
        item = null;
        stackSize = 0;

        UpdateSlot();
    }

    // Kiểm tra có thể đặt vật phẩm vào hay không? (Cùng type)
    bool CanPlaceInSlot(InventorySlot _itemFrom, InventorySlot _itemTo){
        // Swap trong kho đồ hoặc trong rương
        if((_itemTo.itemType == ItemType.None || _itemTo.itemType == ItemType.Chest) && _itemTo.equipType == EquipType.None) return true;
        
        // Mặc trang bị/vũ khí/vật phẩm
        if(_itemFrom.item.itemType == _itemTo.itemType){
            // Hand = Right Hand = Left Hand
            if(_itemFrom.item.equipType == EquipType.Hand && (_itemTo.equipType == EquipType.RightHand || _itemTo.equipType == EquipType.LeftHand)) return true;
            else return _itemFrom.item.equipType == _itemTo.equipType;
        }

        return false;
    }

    // Sự kiện với chuột
    // Khi ấn chuột
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData){
        // rường hợp bắt đầu kéo thả vật phẩm -- xóa item info nếu đang hiển thị
        if(!dragDropHandler.isDragging){
            if(eventData.button == PointerEventData.InputButton.Left && item != null){
                // Xóa info item khi có
                inventoryManager.DestroyItemInfo();

                dragDropHandler.slotFrom = this;
                dragDropHandler.isDragging = true;
            }
        }
    }

    // Khi thả chuột
    // Cập nhật lại slotFrom và slotTo
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData){
        // Đang kéo thả item mà click == đặt item vào ô slotto
        if(dragDropHandler.isDragging){
            // ô slotto rỗng
            if(dragDropHandler.slotTo == null){
                // Từ bên ngoài vào trong túi đò
                if(dragDropHandler.slotFrom.itemType != ItemType.None){ 
                    dragDropHandler.slotTo = dragDropHandler.slotFrom;
                    dragDropHandler.isDragging = false;
                }else{ // Vứt ra ngoài bản đồ
                    dragDropHandler.slotFrom.DropItem();
                    dragDropHandler.isDragging = false;
                }
            }
            // Drag & Drop
            else if(dragDropHandler.slotTo != null){
                // Kiểm tra vật phẩm có đúng loại không vd mũ - mũ, khiên - tay trái ...
                if(CanPlaceInSlot(dragDropHandler.slotFrom, dragDropHandler.slotTo)){
                    inventoryManager.SwapItems(dragDropHandler.slotFrom, dragDropHandler.slotTo);
                    dragDropHandler.isDragging = false;
                }else{
                    dragDropHandler.slotTo = dragDropHandler.slotFrom;
                    dragDropHandler.isDragging = false;
                }
            }
        }
    }

    // Khi con trỏ chỉ vào 1 ô ==> Hiển thị thông tin của vật phẩm đấy
    // Cập nhật lại slotFrom và slotTo
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData){
        if(dragDropHandler.isDragging){
            dragDropHandler.slotTo = this;
        }else{
            if(item != null){
                inventoryManager.DisplayItemInfo(item, transform.position, offsetX, offsetY);
            }
        }
    }

    // Khi con trỏ rời khỏi nút đó ==> Ẩn thông tin nếu đã hiển thị
    // Cập nhật lại slotFrom và slotTo
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData){
        if(dragDropHandler.isDragging){
            dragDropHandler.slotTo = null;
        }else{
            dragDropHandler.slotTo = this;
            
            inventoryManager.DestroyItemInfo();
        }
    }
}
