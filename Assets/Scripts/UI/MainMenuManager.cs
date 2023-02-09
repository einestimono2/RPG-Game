using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void Start() {
        // StartGame();
    }

    public void QuitGame(){
        Debug.Log("Good Bye!");
        Application.Quit();
    }

    public void StartGame(){
        SceneController.instance.LoadScene(1);
    }
}
