using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float fadeDuration = 0.8f;

    private TextMeshPro tmp;
    private float timer;
    private Color startColor;
    private bool isSetup = false;

    private float scaleTimer;
    private float scaleDuration = 0.15f;

    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshPro>();
    }

    public void Setup(int points)
    {
        if (tmp == null) tmp = GetComponentInChildren<TextMeshPro>();
        tmp.text = "+" + points.ToString();
        startColor = tmp.color;
        timer = fadeDuration;
        scaleTimer = scaleDuration;
        transform.localScale = Vector3.one * 1.8f;  
        isSetup = true;
    }

    private void Update()
    {
        if (!isSetup || tmp == null) return;

        
        if (scaleTimer > 0f)
        {
            scaleTimer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(scaleTimer / scaleDuration);
            transform.localScale = Vector3.Lerp(Vector3.one * 1.8f, Vector3.one, t);
        }

       
        transform.position += new Vector3(0.3f, floatSpeed, 0f) * Time.deltaTime;

        
        timer -= Time.deltaTime;
        float alpha = Mathf.Clamp01(timer / fadeDuration);
        tmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer <= 0f)
            Destroy(gameObject);
    }
}