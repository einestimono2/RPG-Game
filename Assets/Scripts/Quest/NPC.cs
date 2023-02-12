using System;
using UnityEngine;

// Chứa thông tin về NPC
// Kế thừa Interactable (Có thể tương tác được)
public class NPC : Interactable
{
    [Header("NPC Info")]
    public Quaternion baseRotation; // Rotation gốc của NPC ==> Để quay lại vị trí cũ khi player đi xa ra
    public string npcName;
    // mặc định
    public string[] baseDialogues = new string[]{
        "Hello adventurer, nice to meet you!",
        "Now, I still have no commission for you. If so, I will contact you immediately!"
    };

    [Header("Refs")]
    public PlayerInteract playerInteract;
    public PlayerInput playerInput;
    public DialogueManager dialogueManager;

    [Header("Target")]
    public bool isEscorting = false; // NPC này có đang đi theo player không?
    public PlayerManager playerManager; // Để nhìn vào player

    protected virtual void Awake() {
        baseRotation = transform.rotation;
    }
    
    void LateUpdate() {
        if(isEscorting) return;
        
        // Trường hợp player tới gần
        if(playerManager != null) {
            // Tính khoảng cách giữa 2 người
            float distance = Vector3.Distance(playerManager.transform.position, transform.position);

            // Nếu gần thì xoay NPC để luôn nhìn vào player 
            if(distance <= 3f){
                // Vector giữa npc và player
                Vector3 rotation = playerManager.transform.position - transform.position;
                rotation.y = 0;
                rotation.Normalize(); // Chuẩn hóa vector (tổng bình phương xyz = 1)

                Quaternion tr = Quaternion.LookRotation(rotation); // Tạo phép quay từ vector trên
                // Quaternion.Slerp: hàm nội suy phép quay giữa hai Quaternion và hiển thị việc quay ra màn hình theo thời gian
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, 300 * Time.deltaTime);

                // Cập nhật rotation của NPC
                transform.rotation = targetRotation;
            }
            // Player nằm ngoài khoảng cách
            else{
                // Quay về hướng mặc định
                transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, 300 * Time.deltaTime);
                playerManager = null;
            }
            
        }
    }

    // Tương tác khi ấn F
    // Bắt đầu cuộc nói chuyện
    public override void Interact(PlayerInteract playerInteract){
        // Refs
        this.playerInteract = playerInteract;
        playerInput = playerInteract.GetComponent<PlayerInput>();
        dialogueManager = playerInteract.GetComponentInChildren<DialogueManager>();
        
        // Base
        base.Interact(playerInteract);

        // Hiển thị cuộc hội thoại với baseDialogues
        InitializeDialogue(baseDialogues);
    }

    public void InitializeDialogue(string[] _dialogues, Action _action = null){
        // Khởi tạo Dialogue
        dialogueManager.InitializeDialogue(playerInput, _dialogues, npcName, _action);
    }

}
