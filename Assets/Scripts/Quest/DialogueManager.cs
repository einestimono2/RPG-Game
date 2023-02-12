using UnityEngine;
using TMPro;
using System;

// Quản lý hộp thoại nói chuyện của NPC
public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogueUI;
    public TMP_Text npcNameText;
    public TMP_Text dialogueText;
    public TMP_Text buttonText;

    [Header("Quest Info")]
    public string npcName;
    public string[] dialogues;
    int currentDialogueIndex = 0;

    PlayerInput playerInput;
    Action action;

    private void Awake() {
        dialogueUI.SetActive(false);
    }

    // Khởi tạo nội dung cuộc hội thoại (các dòng)
    public void InitializeDialogue(PlayerInput _playerInput, string[] _dialogues, string _name, Action _action = null){
        // Cập nhật refs
        playerInput = _playerInput;
        action = _action;
        dialogues = _dialogues;
        npcName = _name;
        currentDialogueIndex = 0;

        // Hiển thị chuột lên màn hình
        playerInput.cameraHandler.ChangeCursor(false);
        playerInput.dialogueFlag = true;

        // Chạy từng dòng một của cuộc hội thoại
        PlayDialogue();
        dialogueUI.SetActive(true);
    }

    // Hiển thị từng nội dung một của cuộc hội thoại
    public void PlayDialogue(){
        // Khi chạy tới cuối cùng thì cập nhật tên button
        if(currentDialogueIndex == dialogues.Length - 2){
            buttonText.text = "OK";
        }else{
            buttonText.text = "Next";
        }

        npcNameText.text = npcName; // không cần cùng được
        // cập nhật dòng hiện tại
        dialogueText.text = dialogues[currentDialogueIndex++];
    }

    // Khi ấn nút Next thì chạy hiển thị tiếp theo
    // Khi là cái cuối thì ẩn giao diện nói chuyện đi và thực hiện action (gọi hàm) nếu có truyền vào
    public void NextDialogue(){
        if(currentDialogueIndex < dialogues.Length - 1){
            PlayDialogue();
        }else{
            // Ẩn chuột đi và tắt giao diện
            playerInput.cameraHandler.ChangeCursor(true, false);
            dialogueUI.SetActive(false);
            playerInput.dialogueFlag = false;

            // Thực hiện hàm truyền vào nếu có
            if(action != null){
                action();
            }
        }
    }

    // Nhảy đén cuối cùng luôn
    public void SkipDialogues(){
        currentDialogueIndex = dialogues.Length - 1;
        NextDialogue();
    }
}
