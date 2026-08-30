using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject gamePanel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void isDead()
    {
        Debug.Log("DEADDDDDDDD!");
        gamePanel.SetActive(true);
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void quit()
    {
        Application.Quit();
    }
}
