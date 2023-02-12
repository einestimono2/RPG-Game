using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Lưu danh sách nhiệm vụ hiện tại của player
public class QuestManager : MonoBehaviour
{
    [Header("UI")]
    public Transform questContainer;
    public GameObject questPrefab;

    [Header("Quest")]
    public List<Quest> currentQuests;
    public List<GameObject> questObjects; // Vị trí tương ứng của quest đó trong giao diện

    // Mở khóa ds nhiệm vụ của NPC khác nếu nhiệm vụ hiện tại thỏa mãn
    void UnlockedNextQuests(Quest pre){
        if(pre.questName == "Rescue Moongarden" || pre.questName == "Scout Monsters" || pre.questName == "Conquer Dungeon"){
            EventManager.QuestReceived(pre);
        }
    }

    // Khi nhận quest
    public void TakeQuest(Quest quest){
        currentQuests.Add(quest); // thêm vào ds nv của player

        UnlockedNextQuests(quest); // Xem có thể mở khóa ds nhiệm vụ của npc khác k ?

        // Hiển thị giao diện
        GameObject questObject = Instantiate(questPrefab.gameObject, questContainer);
        questObjects.Add(questObject);

        TMP_Text[] texts = questObject.GetComponentsInChildren<TMP_Text>();

        // index 0 <=> Quest name
        texts[0].text = $"- {quest.questName}";
        // index 1,2,3... ==> từng goal
        for (int i = 1; i < texts.Length; i++){
            if(i <= quest.goals.Length){
                texts[i].text = $"+ {quest.goals[i-1].description} {quest.goals[i-1].currentAmount}/{quest.goals[i-1].requiredAmount}";
            }else{
                texts[i].text = "";
            }
        }

        
    }

    // Cập nhật giao diện khi có goal / quest hoàn thành
    public void UpdateQuest(Quest quest, Goal goal, bool questCompleted = false){
        for (int i = 0; i < currentQuests.Count; i++){
            if(currentQuests[i].questName == quest.questName){
                // Trường hợp hoàn thành 1 goal trong danh sách goal
                // Đổi kiểu và màu chữ của goal đó
                foreach (TMP_Text _goal in questObjects[i].GetComponentsInChildren<TMP_Text>()){
                    if(_goal.text.Contains(goal.description)){
                        _goal.text = $"+ {goal.description} {goal.currentAmount}/{goal.requiredAmount}";
                        
                        if(goal.currentAmount >= goal.requiredAmount){
                            _goal.fontStyle = (FontStyles)FontStyle.Italic;
                            _goal.color = Color.green;
                        }

                        break;
                    }
                }

                // Trường hợp hoàn thành hết tất cả goal ==> Đổi màu tên quest
                if(questCompleted){
                    TMP_Text title = questObjects[i].GetComponentInChildren<TMP_Text>();
                    title.fontStyle = (FontStyles)FontStyle.Italic;
                    title.color = Color.green;
                }

                return;
            }
        }
    }

    // Khi hoàn thành 1 nhiệm vụ nào đó
    public void CompleteQuest(Quest quest){
        int index = -1;

        // Lấy quest đó trong ds
        for (int i = 0; i < currentQuests.Count; i++){
            if(currentQuests[i].questName == quest.questName){
                index = i;

                break;
            }
        }
        
        // Xóa khởi giao diện
        if(index != -1){
            Destroy(questObjects[index]);
            questObjects.RemoveAt(index);
            currentQuests.RemoveAt(index);
        }
    }
}
