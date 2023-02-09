using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSellSlot : MonoBehaviour
{
    public ShopManager shopManager; 
    [Header("Item")]
    public Item item;
    public int stackSize;
    public int index;

    [Header("Shop Slot Refs")]
    public Image icon;
    public TMP_Text itemName;
    public TMP_Text itemStack;
    public TMP_Text itemDescription;
    public TMP_Text itemPrice;

    public void InitialSlot(Item _item, int _stackSize, int _index, ShopManager _shopManager){
        item = _item;
        index = _index;
        stackSize = _stackSize;
        shopManager = _shopManager;

        icon.sprite = item.itemIcon;
        itemName.text = item.itemName;
        if(stackSize > 1){
            itemStack.text = "x" + stackSize.ToString();
        }else{
            itemStack.transform.parent.gameObject.SetActive(false);
        }
        itemDescription.text = item.itemDescription;
        itemPrice.text = item.price.ToString();
    }

    public void InitialItemInfomation(){
        shopManager.itemInfo.SetActive(true);
        shopManager.itemInforButtonText.text = "SELL";
        shopManager.currentSellItem = this;

        shopManager.itemInforIcon.sprite = item.itemIcon;
        shopManager.itemInforName.text = item.itemName;
        shopManager.itemInforDescription.text = item.itemDescription;
        shopManager.itemInforStats.text = GetAllItemStats();
        
        if(item.maxStack > 1){
            shopManager.itemInforStack.gameObject.SetActive(true);
            shopManager.itemInforStackText.transform.parent.gameObject.SetActive(true);

            shopManager.itemInforStack.minValue = 1;
            shopManager.itemInforStack.maxValue = stackSize;
            shopManager.itemInforStack.value = 1;
            shopManager.itemInforStackText.text = Mathf.RoundToInt(shopManager.itemInforStack.value).ToString();

            // Add Event
            shopManager.itemInforStack.onValueChanged.AddListener(delegate {ItemStackChange ();});
        }else{
            shopManager.itemInforStack.value = 1;
            shopManager.itemInforStack.gameObject.SetActive(false);
            shopManager.itemInforStackText.transform.parent.gameObject.SetActive(false);
        }
        shopManager.itemInforTotalPrice.text =  "TOTAL: " + item.price.ToString();
    }

    public void UpdateStackSize(int newStack){
        int preValue = Mathf.RoundToInt(shopManager.itemInforStack.value);
        stackSize = newStack;

        itemStack.text = "x" + stackSize.ToString();
        
        shopManager.itemInforStack.minValue = 1;
        shopManager.itemInforStack.maxValue = stackSize;
        if(preValue > stackSize){
            shopManager.itemInforStack.value = stackSize;
        }
        shopManager.itemInforStackText.text = Mathf.RoundToInt(shopManager.itemInforStack.value).ToString();
    }

    string GetAllItemStats(){
        return "Damage: 20\nDef: 5\nStamina: 5";
    }

    void ItemStackChange(){
        int stack = Mathf.RoundToInt(shopManager.itemInforStack.value);
		shopManager.itemInforStackText.text = stack.ToString();

        int totalPrice = item.price * stack;
        shopManager.itemInforTotalPrice.text =  "TOTAL: " + totalPrice.ToString();
	}
}
