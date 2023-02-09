using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Refs")]
    PlayerInput playerInput;
    DialogueManager dialogManager;

    [Header("UI")]
    public GameObject pauseScreen;

    void Awake() {
        playerInput = GetComponentInParent<PlayerInput>();
        dialogManager = GetComponent<DialogueManager>();

        pauseScreen.SetActive(false);
    }

    void Update() {
        if(playerInput.pause_Input){
            if(playerInput.shopFlag || playerInput.chestFlag){
                return;
            }
            
            // Skip dialogue
            if(playerInput.dialogueFlag){
                dialogManager.SkipDialogues();
                return;
            }

            // Đóng kho đồ khi ấn ESC
            if(playerInput.inventoryFlag){
                playerInput.cameraHandler.ChangeCursor(true, true);
                playerInput.inventoryFlag = false;
                playerInput.inventoryManager.CloseInventory();
                return;
            }

            if(playerInput.pauseFlag) ResumeGame();
            else PauseGame();
        
        }
    }

    public void PauseGame(){
        playerInput.cameraHandler.ChangeCursor(playerInput.pauseFlag, true);
        pauseScreen.SetActive(true);
            
        playerInput.pauseFlag = true;
    }

    public void ResumeGame(){
        playerInput.cameraHandler.ChangeCursor(playerInput.pauseFlag, true);
        pauseScreen.SetActive(false);
                
        playerInput.pauseFlag = false;
    }

    public void QuitGame(){
        SceneController.instance.LoadScene(0);
    }
}
