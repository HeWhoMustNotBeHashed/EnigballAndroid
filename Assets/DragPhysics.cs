using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class DragPhysics : MonoBehaviour
{
    public GameObject forceParticles;
    public bool touchedGround = true;
    private bool healthZero = false;
    private bool isBurnedOut = false;

    public Slider healthSlider;
    public float maxHealth = 100f;
    public float drainSpeed = 25f;

    public float uiCatchUpSpeed = 15f;
    public float currentHealth;

    public LineRenderer lr;
    public int trajectoryPoints = 12;
    public float timeStep = 0.08f;
    public float power = 10f;
    public Rigidbody2D rb;
    public Vector2 minPower;
    public Vector2 maxPower;

    Vector3 startPoint;
    Vector3 endPoint;
    Vector2 force;
    public Camera cam;

    public Transform ballVisual;
    public float stretchModifier = 0.1f;
    public float maxStretch = 2f;
    private Vector3 originalScale;

    public float minDragDistance = 0.5f;  // Minimum drag to count as a real launch

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null) healthSlider.value = currentHealth;

        lr = GetComponent<LineRenderer>();
        lr.enabled = false;

        if (ballVisual != null)
            originalScale = ballVisual.localScale;
        else
        {
            Debug.LogWarning("Please assign the Ball Visual transform in the inspector", this);
            originalScale = transform.localScale;
        }
    }

    private void Update()
    {
        bool touchBegan = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        bool touchHeld = Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved ||
                                                      Input.GetTouch(0).phase == TouchPhase.Stationary);
        bool touchEnded = Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended ||
                                                      Input.GetTouch(0).phase == TouchPhase.Canceled);

        Vector2 touchScreenPos = Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;
        Vector3 touchWorldPos = cam.ScreenToWorldPoint(new Vector3(touchScreenPos.x, touchScreenPos.y, 0));
        touchWorldPos.z = 15;

        if (touchBegan)
        {
            startPoint = touchWorldPos;
            isBurnedOut = false;
        }

        if (touchHeld)
        {
            if (touchedGround && !healthZero && !isBurnedOut)
            {
                DrawTrajectory(touchWorldPos);
                ApplyStretch(touchWorldPos);

                currentHealth -= drainSpeed * Time.unscaledDeltaTime;
                currentHealth = Mathf.Max(currentHealth, 0f);

                if (currentHealth <= 0f)
                {
                    healthZero = true;
                    isBurnedOut = true;
                    lr.enabled = false;
                    ResetStretch();
                }
            }
        }

        if (touchEnded)
        {
            currentHealth = 0;
            if (!healthZero && !isBurnedOut)
                HandleLaunch(touchWorldPos);
            else
            {
                lr.enabled = false;
                ResetStretch();
            }

            isBurnedOut = false;
        }

        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(
                healthSlider.value,
                currentHealth,
                Time.unscaledDeltaTime * uiCatchUpSpeed
            );
        }
    }

    void HandleLaunch(Vector3 touchWorldPos)
    {
        endPoint = touchWorldPos;

        // If drag distance is too short, treat as a tap — don't launch
        float dragDistance = Vector2.Distance(startPoint, endPoint);
        if (dragDistance < minDragDistance)
        {
            lr.enabled = false;
            ResetStretch();
            return;  // Exit without setting touchedGround = false
        }

        force = new Vector2(
            Mathf.Clamp(startPoint.x - endPoint.x, minPower.x, maxPower.x),
            Mathf.Clamp(startPoint.y - endPoint.y, minPower.y, maxPower.y)
        );

        if (touchedGround)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(force * power, ForceMode2D.Impulse);
            Instantiate(forceParticles, transform.position, Quaternion.identity);
            touchedGround = false;
        }

        lr.enabled = false;
        ResetStretch();
    }

    void ApplyStretch(Vector3 touchWorldPos)
    {
        if (ballVisual == null) return;

        Vector2 dragDir = touchWorldPos - startPoint;
        float dragDistance = dragDir.magnitude;

        if (dragDistance > 0.05f)
        {
            float angle = Mathf.Atan2(dragDir.y, dragDir.x) * Mathf.Rad2Deg;
            ballVisual.rotation = Quaternion.Euler(0, 0, angle);

            float stretchFactor = Mathf.Min(dragDistance * stretchModifier, maxStretch - 1f);
            float newX = originalScale.x + (originalScale.x * stretchFactor);
            float newY = originalScale.y - (originalScale.y * stretchFactor * 0.5f);
            newY = Mathf.Max(newY, originalScale.y * 0.3f);

            ballVisual.localScale = new Vector3(newX, newY, originalScale.z);
        }
    }

    void ResetStretch()
    {
        if (ballVisual == null) return;
        ballVisual.rotation = Quaternion.identity;
        ballVisual.localScale = originalScale;
    }

    void DrawTrajectory(Vector3 touchWorldPos)
    {
        lr.enabled = true;
        lr.positionCount = trajectoryPoints;

        Vector2 previewForce = new Vector2(
            Mathf.Clamp(startPoint.x - touchWorldPos.x, minPower.x, maxPower.x),
            Mathf.Clamp(startPoint.y - touchWorldPos.y, minPower.y, maxPower.y)
        );

        Vector2 velocity = (previewForce * power) / rb.mass;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector2 point =
                (Vector2)transform.position +
                velocity * t +
                0.5f * Physics2D.gravity * rb.gravityScale * t * t;

            lr.SetPosition(i, point);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            touchedGround = true;

            if (Input.touchCount == 0)
            {
                healthZero = false;
                currentHealth = maxHealth;
            }
        }

        System.Random rand = new System.Random();
        if (collision.gameObject.CompareTag("Collider"))
        {
          
            touchedGround = true;

            ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
            if (scoreManager != null) scoreManager.AddScore(500);

            int direction = rand.Next(1, 5);
            float boost = 40f;

            switch (direction)
            {
                case 1: rb.AddForce(Vector2.up * boost, ForceMode2D.Impulse); break;
                case 2: rb.AddForce(Vector2.down * boost, ForceMode2D.Impulse); break;
                case 3: rb.AddForce(Vector2.right * boost, ForceMode2D.Impulse); break;
                case 4: rb.AddForce(Vector2.left * boost, ForceMode2D.Impulse); break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("pintuEnemy") || collision.CompareTag("abhinavEnemy"))
        {
            touchedGround = true;

            if (Input.touchCount == 0)
            {
                healthZero = false;
                currentHealth = maxHealth;
            }

            ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
            if (collision.CompareTag("Enemy"))
                if (scoreManager != null) scoreManager.AddScore(100);
                else if (collision.CompareTag("pintuEnemy"))
                    if (scoreManager != null) scoreManager.AddScore(300);
                    else if (collision.CompareTag("abhinavEnemy"))
                        if (scoreManager != null) scoreManager.AddScore(400);
        }
    }
}