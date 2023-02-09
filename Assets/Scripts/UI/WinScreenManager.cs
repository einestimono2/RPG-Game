using UnityEngine;

public class WinScreenManager : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject hudUI;

    PlayerInput playerInput;

    private void Awake() {
        winScreen.SetActive(false);
    }

    public void ShowWinScreen(PlayerInput playerInput) {
        this.playerInput = playerInput;

        playerInput.cameraHandler.ChangeCursor(false, true);

        winScreen.SetActive(true);
        hudUI.SetActive(false);
    }

    public void HideWinScreen() {
        playerInput.cameraHandler.ChangeCursor(true, true);

        winScreen.SetActive(false);
        hudUI.SetActive(true);
    }

    public void Continue(){
        HideWinScreen();
    }

    public void Quit(){
        SceneController.instance.LoadScene(0);
    }
}
