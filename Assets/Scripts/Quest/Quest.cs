using System.Linq;
using UnityEngine;
using static Enums;

// Nhiệm vụ
[CreateAssetMenu(fileName = "QuestData", menuName = "Quests")] // Dùng để tạo scriptable object ở unity (gg cho dễ hiểu)
public class Quest : ScriptableObject
{
    [Header("Available Quest Dialogue")]
    public string[] availableDialogues; // Cuộc hội thoại khi nhận nv

    [Header("Accepted Quest Dialogue")]
    public string[] acceptedDialogues; // Cuộc hội thoại khi đã nhận nv rồi

    [Header("Completed Quest Dialogue")]
    public string[] completedDialogues; // Cuộc hội thoại khi trả nv (đã hoàn thành)

    [Header("Quest Info")]
    public string questName; // Tên quest (ID quest luôn)
    public QuestProgress questProgress; // Tiến độ: available (có thể nhận), ....
    public Goal[] goals; // Các mục tiêu của nhiệm vụ này (kiểu như nhặt 1 sword, nhặt 1 shield ...)

    [Header("Quest Reward")]
    public int expReward;
    public int goldReward;
    public ChestItem[] itemsReward;

    QuestGiver parent; // NPC giao nhiệm vụ này

    // Kiểm tra hợp lệ
    void OnValidate() {
        foreach(ChestItem item in itemsReward) {
            if(item.item != null) {
                if(item.stackSize > item.item.maxStack){
                    item.stackSize = item.item.maxStack;
                }

                if(item.stackSize < 1){
                    item.stackSize = 1;
                }

                if(expReward < 0){
                    expReward = 0;
                }

                if(goldReward < 0){
                    goldReward = 0;
                }
            }
        }
    }

    // Khởi tạo quest
    // Gán cho NPC và khởi tạo từng goal
    public void InitializeQuest(QuestGiver questGiver) {
        parent = questGiver;

        InitializeGoals();
    }

    // Kiểm tra tiến độ của cả nhiệm vụ
    // Mỗi khi một goal hoàn thành thì check luôn (trường hợp tất cả goal hoàn thành)
    public void CheckProgress(Goal goal) {
        if(goals.All(goal => goal.completed)){
            questProgress = QuestProgress.Complete; // Cập nhật tiến độ quest
            parent.questStatus = QuestStatus.Completed; // Cập nhật trạng thái của nv của NPC ==> Để update lại icon tương ứng
            parent.UpdateMarker(); // Cập nhật icon (chấm than vàng, chấm hỏi vòng ... tương ứng)
            parent.questManager.UpdateQuest(this, goal, true); // Cập nhật giao diện (đổi màu, đổi thành in nghiêng)
        }
    }

    // Cập nhật từng goal khi goal đó xog
    // Cập nhật thành màu xanh, in nghiêng
    public void UpdateGoalStatus(Goal goal){
        parent.questManager.UpdateQuest(this, goal);
    }

    // Khởi tạo tất cả goals
    void InitializeGoals(){
        for (int i = 0; i < goals.Length; i++) {
            goals[i].InitializeGoal(this);
        }
    }

    // Set về mặc định mỗi khi chạy mới (do scriptable object thay đổi thì thay đổi luôn chứ không như các thứ khác (thay đổi khi chạy nhưng dừng chạy sẽ về mặc định))
    public void ResetToDefault(){
        questProgress = QuestProgress.Inactive;

        for (int i = 0; i < goals.Length; i++){
            goals[i].completed = false;
            goals[i].currentAmount = 0;
        }
    }

}

// NAME
// Male: Kahn, Quint, O’brouwer, D’romain, Lemanneville, Nathraichean, Vann
// Female: Keegan, Mcbostrom, Lavail, Dennis, Leroy, Tyeis