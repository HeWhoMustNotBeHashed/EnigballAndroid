using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score UI")]
    public TextMeshProUGUI scoreTextDisplay;
    public TMP_Text highScoreDisplay;

    [Header("Combo UI")]
    public TextMeshProUGUI comboTextDisplay;
    public TextMeshProUGUI comboMultiplierDisplay; // separate "x4" big text

    [Header("Combo Settings")]
    public float comboResetTime = 2f;
    public int minComboToShow = 2;

    [Header("Punch Animation")]
    public float punchScale = 1.6f;       // How big it gets on pop
    public float punchDuration = 0.12f;   // How fast it pops out
    public float settleDuration = 0.2f;   // How fast it settles back

    private int currentScore = 0;
    private int comboCount = 0;
    private float comboTimer = 0f;
    private bool comboActive = false;

    // Punch animation state
    private float punchTimer = 0f;
    private bool isPunching = false;
    private bool isSettling = false;
    private Vector3 baseScale;

    private void Start()
    {
        UpdateScoreUI();

        if (comboTextDisplay != null)
        {
            baseScale = comboTextDisplay.transform.localScale;
            comboTextDisplay.gameObject.SetActive(false);
        }
        if (comboMultiplierDisplay != null)
            comboMultiplierDisplay.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (comboActive)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                ResetCombo();
        }

        HandlePunchAnimation();
    }

    public void AddScore(int pointsToAdd)
    {
        comboCount++;
        comboTimer = comboResetTime;
        comboActive = true;

        int multipliedPoints = pointsToAdd * comboCount;
        currentScore += multipliedPoints;

        UpdateScoreUI();
        UpdateComboUI();
        TriggerPunch();
    }

    private void TriggerPunch()
    {
        if (comboCount < minComboToShow) return;

        // snap to big scale then animate back
        if (comboTextDisplay != null)
            comboTextDisplay.transform.localScale = baseScale * punchScale;
        if (comboMultiplierDisplay != null)
            comboMultiplierDisplay.transform.localScale = baseScale * punchScale;

        punchTimer = 0f;
        isPunching = true;
        isSettling = false;
    }

    private void HandlePunchAnimation()
    {
        if (!isPunching && !isSettling) return;

        punchTimer += Time.unscaledDeltaTime;

        if (isPunching)
        {
            // Shrink from punchScale back to normal
            float t = Mathf.Clamp01(punchTimer / punchDuration);
            float scale = Mathf.Lerp(punchScale, 1f, EaseOutBack(t));

            SetComboScale(baseScale * scale);

            if (t >= 1f)
            {
                isPunching = false;
                isSettling = true;
                punchTimer = 0f;
            }
        }
        else if (isSettling)
        {
            // Slight overshoot settle
            float t = Mathf.Clamp01(punchTimer / settleDuration);
            float scale = Mathf.Lerp(0.95f, 1f, t);

            SetComboScale(baseScale * scale);

            if (t >= 1f)
            {
                isSettling = false;
                SetComboScale(baseScale);
            }
        }
    }

    private void SetComboScale(Vector3 scale)
    {
        if (comboTextDisplay != null)
            comboTextDisplay.transform.localScale = scale;
        if (comboMultiplierDisplay != null)
            comboMultiplierDisplay.transform.localScale = scale;
    }

    // springy overshoot feel
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private void UpdateComboUI()
    {
        if (comboCount >= minComboToShow)
        {
            if (comboTextDisplay != null)
            {
                comboTextDisplay.gameObject.SetActive(true);
                comboTextDisplay.text = "COMBO";
            }

            if (comboMultiplierDisplay != null)
            {
                comboMultiplierDisplay.gameObject.SetActive(true);
                comboMultiplierDisplay.text = "x" + comboCount.ToString();
            }

            // Color tiers
            Color comboColor;
            if (comboCount >= 10)
                comboColor = new Color(1f, 0.2f, 0.2f);        // Red
            else if (comboCount >= 5)
                comboColor = new Color(1f, 0.5f, 0f);           // Orange
            else
                comboColor = new Color(1f, 0.9f, 0f);           // Yellow

            if (comboTextDisplay != null)
                comboTextDisplay.color = comboColor;
            if (comboMultiplierDisplay != null)
                comboMultiplierDisplay.color = comboColor;
        }
        else
        {
            if (comboTextDisplay != null)
                comboTextDisplay.gameObject.SetActive(false);
            if (comboMultiplierDisplay != null)
                comboMultiplierDisplay.gameObject.SetActive(false);
        }
    }

    private void ResetCombo()
    {
        comboCount = 0;
        comboActive = false;
        comboTimer = 0f;
        isPunching = false;
        isSettling = false;

        if (comboTextDisplay != null)
            comboTextDisplay.gameObject.SetActive(false);
        if (comboMultiplierDisplay != null)
            comboMultiplierDisplay.gameObject.SetActive(false);
    }

    public void highScoreUpdate()
    {
        if (PlayerPrefs.HasKey("SavedHighScore"))
        {
            if (currentScore > PlayerPrefs.GetInt("SavedHighScore"))
                PlayerPrefs.SetInt("SavedHighScore", currentScore);
        }
        else
            PlayerPrefs.SetInt("SavedHighScore", currentScore);

        highScoreDisplay.text = "High Score: " + PlayerPrefs.GetInt("SavedHighScore").ToString();
    }

    void UpdateScoreUI()
    {
        if (scoreTextDisplay != null)
            scoreTextDisplay.text = "SCORE: " + currentScore.ToString();
    }
}