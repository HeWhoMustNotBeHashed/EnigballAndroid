using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    
    public TextMeshProUGUI scoreTextDisplay;

    
    private int currentScore = 0;

    private void Start()
    {
        UpdateScoreUI();
    }

    
    public void AddScore(int pointsToAdd)
    {
        currentScore += pointsToAdd;
        UpdateScoreUI(); 

    }

    
    void UpdateScoreUI()
    {
        if (scoreTextDisplay != null)
        {
            scoreTextDisplay.text = "SCORE: " + currentScore.ToString();

        }
    }
}