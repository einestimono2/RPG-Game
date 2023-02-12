using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

// NPC giao nhiệm vụ
public class QuestGiver : NPC
{
    [Header("Refs")]
    public PlayerStats playerStats;
    public InventoryManager inventoryManager;
    public QuestManager questManager;

    [Header("Quest Info")]
    public QuestStatus questStatus; // Trạng thái của quest hiện tại
    public int currentQuest = -1; // index quest nếu có nhiều nv
    public Quest preReceivedQuest; // Cần phải nhận quest này trước để mở khóa ds nhiệm vụ của NPC này
    public bool questLocked; // Dựa vào preReceivedQuest, true thì không nhận đc nv nào và ngược lại
    public Quest[] quests;

    [Header("Quest Marker")] // icon ở trên đuầ NPC
    public Image questMarker;
    public Sprite unavailable;
    public Sprite available;
    public Sprite accepted;
    public Sprite completed;

    protected override void Awake() {
        base.Awake();

        currentQuest = -1;
        questStatus = QuestStatus.Unavailable;
        questLocked = preReceivedQuest != null;
        
        foreach (Quest quest in quests){
            quest.ResetToDefault();
        }

        // Gán Event
        EventManager.OnQuestReceived -= QuestReceived;
        EventManager.OnQuestReceived += QuestReceived; // Gán event

        CheckQuest();
    }

    // Sự kiện khi một quest nào đó được nhận
    // Kiểm tra nếu thỏa mãn thì mở khóa danh sách nhiệm vụ của NPC này
    void QuestReceived(Quest pre){
        if(preReceivedQuest == pre){
            // Mở khóa ds nhiệm vụ của NPC này
            questLocked = false;

            // Hủy Event
            EventManager.OnQuestReceived -= QuestReceived;

            Debug.Log("Mở khóa danh sách nhiệm vụ của " + npcName);

            CheckQuest(); // Lấy thông tin quest trong ds và cập nhật icon trên đầu
        }
    }

    public void CheckQuest(){
        // Nếu khóa thì k hiển thị icon gì cả
        if(questLocked){
            UpdateMarker();
            return;
        } 

        currentQuest = -1;
        questStatus = QuestStatus.Unavailable;

        // Tìm trong ds quest những quest chưa kích hoạt (chưa nhận)
        for(int i = 0; i < quests.Length; i++){
            if(quests[i].questProgress == QuestProgress.Inactive){
                currentQuest = i;
                questStatus = QuestStatus.Available;

                break;
            }
        }

        UpdateMarker(); // Cập nhật icon
    } 

    // Cập nhật icon tương ứng vs trạng thái quest hiện tại
    public void UpdateMarker(){
        if(questMarker == null) return;

        if(questStatus == QuestStatus.Available){
            questMarker.sprite = available;
        }
        else if(questStatus == QuestStatus.Accepted){
            questMarker.sprite = accepted;
        }
        else if(questStatus == QuestStatus.Completed){
            questMarker.sprite = completed;
        }else{
            questMarker.sprite = unavailable;
        }
    }

    // Ghi đè từ lớp NPC
    public override void Interact(PlayerInteract playerInteract){
        base.Interact(playerInteract);

        // Event khi nói chuyện vs NPC ==> Có goal cần phải liên lạc với NPC khác
        EventManager.NPCInteraction(this);

        // Init Refs
        questManager = playerInteract.GetComponent<QuestManager>();
        playerStats = playerInteract.GetComponent<PlayerStats>();
        inventoryManager = playerInteract.GetComponent<InventoryManager>();

        // Hết / Không có quest
        if(questStatus == QuestStatus.Unavailable){
            InitializeDialogue(baseDialogues);
        }
        // Nhận quest
        else if(questStatus == QuestStatus.Available){
            // Khởi tạo dialogue và khi xong sẽ gọi hàm AssignQuest()
            InitializeDialogue(quests[currentQuest].availableDialogues, () => AssignQuest());
        }
        // Đang thực hiện quest
        else if(questStatus == QuestStatus.Accepted){
            InitializeDialogue(quests[currentQuest].acceptedDialogues);
        }
        // Đã hoàn thành quest ==> Trao phần thưởng
        else if(questStatus == QuestStatus.Completed){
            // Sau khi kết thúc hội hoại thì trao thưởng
            InitializeDialogue(quests[currentQuest].completedDialogues, () => GiveRewards());
        }
    }

    // Nhận NV
    void AssignQuest(){
        // Cập nhật thành đã nhận quest
        questStatus = QuestStatus.Accepted;
        UpdateMarker();

        // Cập nhật quest đó thành đang thực hiện
        quests[currentQuest].questProgress = QuestProgress.Active;

        // Khởi tạo quest đó
        quests[currentQuest].InitializeQuest(this);

        // Thêm vào list quest của player
        questManager.TakeQuest(quests[currentQuest]);

        Debug.Log("Assigned Quest");

        // Trường hợp nhận quest hộ tống ==> BĐ hộ tống luôn
        if(npcName == "Tyeis" && quests[currentQuest].questName == "Escort Tyeis"){
            GetComponent<EscortManager>().StartEscort(playerManager);

            questMarker.sprite = unavailable;
        }
    }

    // Trao thưởng
    void GiveRewards(){
        // Cập nhật thành chưa nhận quest
        questStatus = QuestStatus.Unavailable;

        // Cập nhật quest đó thành đã xog
        quests[currentQuest].questProgress = QuestProgress.Complete;

        // Cập nhật các nhiệm vụ trước đó nếu nó liên quan tới nv hiện tại
        // VD như hoàn thành hộ tống thì sẽ hoàn thành nhiệm vụ 2 của dennis luôn
        CheckQuestCompleted();

        // Xóa khỏi list quest hiện tại của player
        questManager.CompleteQuest(quests[currentQuest]);

        // Phần thưởng
        if(quests[currentQuest].expReward != 0){
            playerStats.AddEXP(quests[currentQuest].expReward);
        }

        if(quests[currentQuest].goldReward != 0){
            playerStats.AddCoins(quests[currentQuest].goldReward);
        }

        if(quests[currentQuest].itemsReward.Length != 0){
            for (int i = 0; i < quests[currentQuest].itemsReward.Length; i++){
                inventoryManager.AddItem(quests[currentQuest].itemsReward[i].item, quests[currentQuest].itemsReward[i].stackSize);
            }
        }

        // Hoàn thành tất cả nhiệm vụ <=> Hoàn thành nhiệm vụ "Conquer Dungeon" của Dawkins
        // END Game
        if(npcName == "Dawkins" && quests[currentQuest].questName == "Conquer Dungeon"){
            StartCoroutine(Win()); // Kiểu có chạy hàm đó ngay hoặc chạy sau x giây
        }

        // Xóa nv này khởi ds nhiệm vụ của NPC hiện tại
        List<Quest> _list = quests.ToList();
        _list.RemoveAt(currentQuest);
        quests = _list.ToArray();

        CheckQuest(); // Kiểm tra quest tiếp theo là gì và icon
    }

    // Sau 1s thì hiển thị màn hình chiến thắng
    IEnumerator Win(){
        yield return new WaitForSeconds(1f);

        playerStats.GetComponentInChildren<WinScreenManager>().ShowWinScreen(playerStats.playerManager.playerInput);
    }

    // Nếu quest hiện tại hoàn thành thì có thể quest trước đó nhận cũng hoàn thành nếu liên quan tới nhau
    void CheckQuestCompleted(){
        
        // Hoàn thành nhiệm vụ hộ tống ==> Nhiệm vụ do thám trước đó của Dennis cũng hoàn thành
        if(npcName == "Tyeis" && quests[currentQuest].questName == "Escort Tyeis"){
            GetComponent<EscortManager>().MoveToStandPoint();

            for (int i = 0; i < questManager.currentQuests.Count; i++){
                // Quest này chỉ có 1 goal
                if(questManager.currentQuests[i].name == "Scout Monsters"){
                    questManager.currentQuests[i].goals[0].currentAmount += 1;
                    questManager.currentQuests[i].goals[0].Evaluate();
                }
            }
        }
        // Hoàn thành tất cả nhiệm vụ của Dennis ==> Hoàn thành quest 'Rescue Moongarden' của Dawkins
        else if(npcName == "Dennis" && quests[currentQuest].questName == "Clean Monsters"){
            for (int i = 0; i < questManager.currentQuests.Count; i++){
                // Thuộc Goal thứ 2 trong ds goals
                if(questManager.currentQuests[i].name == "Rescue Moongarden"){
                    questManager.currentQuests[i].goals[1].currentAmount += 1;
                    questManager.currentQuests[i].goals[1].Evaluate();
                }
            }
        }
        // Hoàn thành tất cả nhiệm vụ của Keegan ==> Hoàn thành quest 'Conquer Dungeon' của Dawkins
        else if(npcName == "Keegan" && quests[currentQuest].questName == "Explore Dungeon"){
            for (int i = 0; i < questManager.currentQuests.Count; i++){
                // Thuộc Goal thứ 2 trong ds goals
                if(questManager.currentQuests[i].name == "Conquer Dungeon"){
                    questManager.currentQuests[i].goals[1].currentAmount += 1;
                    questManager.currentQuests[i].goals[1].Evaluate();
                }
            }
        }
        
    }
}
