using UnityEngine;
using static Enums;

//[System.Serializable] -> Để hiện thị (điền được thông tin) luôn trong Inspector
// Goal các mục tiêu của nhiệm vụ
[System.Serializable] 
public class Goal{
    public GoalType goalType; // Kiểu mục tiêu: Thu thập, nhặt vp, đánh quái ...
    public bool completed;
    public string ID; // ID của mục tiêu: Đánh quái (ID quái cần đánh), thu nhập (ID vật phẩm cần thu thập) ...
    public string description; // Kiểu như tên của goal
    public int currentAmount; // Tiến độ hiện tại
    public int requiredAmount; // Số lượng cần để hoàn thành

    private Quest parent; // Để xác định goal này thuộc quest nào

    // Cập nhật lại thông tin khi điền sai (quá slg tối đa, không hợp lệ ...)
    void OnValidate() {
        if(currentAmount < 0) currentAmount = 0;

        if(requiredAmount < currentAmount) requiredAmount = currentAmount + 1;
    }

    // Khởi tạo goal và gán sự kiện cho nó
    public void InitializeGoal(Quest quest){
        parent = quest;

        InitializeEvent();
    }

    // Gán sự kiện cho goal này ==> để update goal luôn khi đạt đc
    void InitializeEvent(){
        if(goalType == GoalType.Fetch){
            EventManager.OnItemFetched += ItemFetched;
        }else if(goalType == GoalType.Kill){
            EventManager.OnEnemyDeath += EnemyDied;
        }else if(goalType == GoalType.Contact){
            EventManager.OnNPCInteraction += NPCInteracted;
        }else if(goalType == GoalType.Escort){
            EventManager.OnEscortCompleted += EscortCompleted;
        }else{
            return;
        }
    }

    // Xóa sự kiện khi đã hoàn thành goal
    void DeleteEvent(){
        if(goalType == GoalType.Fetch){
            EventManager.OnItemFetched -= ItemFetched;
        }else if(goalType == GoalType.Kill){
            EventManager.OnEnemyDeath -= EnemyDied;
        }else if(goalType == GoalType.Contact){
            EventManager.OnNPCInteraction -= NPCInteracted;
        }else if(goalType == GoalType.Escort){
            EventManager.OnEscortCompleted -= EscortCompleted;
        }else{
            return;
        }
    }

    // thực hiện Đánh giá ==> Cập nhật tiến độ
    // Nếu hoàn thành thì check các goal khác của quest đã hoàn thành chưa 
    public void Evaluate(){
        // Khi đã hoàn thành goal
        if(currentAmount >= requiredAmount){
            completed = true;
            parent.CheckProgress(this); // Kiểm tra các goal khác của quest
            DeleteEvent();
        }

        // Cập nhật lại giao diện quest (update lại số lượng hiện tại / số luộng tối đa ở phần giao diện)
        parent.UpdateGoalStatus(this);
    }

    // Các sự kiện tương ứng
    // #region để bọc lại thôi, search gg ấy
    #region Events
    void EnemyDied(EnemyStats enemy){
        if (enemy.ID == this.ID)
        {
            Debug.Log("Detected enemy death: " + ID);
            this.currentAmount++;
            Debug.Log("Progress " + currentAmount + "/" + requiredAmount);
            Evaluate();
        }
    }

    void ItemFetched(Item item, int stack){
        if (item.ID == this.ID)
        {
            Debug.Log("Detected quest item: " + item.itemName);
            this.currentAmount += stack;
            Debug.Log("Progress " + currentAmount + "/" + requiredAmount);
            Evaluate();
        }
    }

    void NPCInteracted(NPC npc){
        if(npc.npcName == this.ID){
            Debug.Log("Detected target: " + this.ID);
            this.currentAmount += 1;
            Debug.Log("Progress " + currentAmount + "/" + requiredAmount);
            Evaluate();
        }
    }
    
    void EscortCompleted(NPC npc){
        if(npc.npcName == this.ID){
            this.currentAmount += 1;
            Evaluate();
        }
    }

    #endregion
}