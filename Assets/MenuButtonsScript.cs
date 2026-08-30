using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonsScript : MonoBehaviour
{
    public void onStartClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void onExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }

    public void onGoBackClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void onGoInstructions()
    {
        SceneManager.LoadScene("HowToPlayScene");
    }
}
