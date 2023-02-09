using UnityEngine;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour
{
    public bool isDragging = false;
    public InventorySlot slotFrom;
    public InventorySlot slotTo;
    [Space]
    public Image dragDropIcon;

    private void Update() {
        if(isDragging && slotFrom != null){
            dragDropIcon.gameObject.SetActive(true);
            dragDropIcon.sprite = slotFrom.icon.sprite;

            dragDropIcon.transform.position = Input.mousePosition;
        }else{
            dragDropIcon.gameObject.SetActive(false);
        }
    }
}
