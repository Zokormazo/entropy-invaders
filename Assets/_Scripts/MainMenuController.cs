using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * MainMenuController: Controls main menu action & events
 */

public class MainMenuController : MonoBehaviour {

    // Run ASAP.
    void Awake() {
        Screen.SetResolution(400, 600, false); // Set Fixed resolution on window mode
    }

    // Exit Game
    public void ExitGame() {
        Application.Quit();
    }

    // Play Game
    public void PlayGame() {
        SceneManager.LoadScene(1);
    }
}
