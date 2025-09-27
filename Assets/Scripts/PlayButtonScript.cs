using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonScript : MonoBehaviour
{
    // This function will be called when the PlayButton is clicked
    public void LoadGameplayScene()
    {
        // Debug to make sure the button click works
        Debug.Log("Play button clicked!");

        // Load the gameplay scene by name
        SceneManager.LoadScene("MainMenuPlay"); // <-- EXACT name of your target scene
    }
}
