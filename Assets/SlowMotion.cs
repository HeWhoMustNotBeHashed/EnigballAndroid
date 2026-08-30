using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowMotion : MonoBehaviour
{
    private DragPhysics dragPhysicsScript;

    public Volume globalVolume;
    public float transitionSpeed = 8f;

    [Header("Chromatic Aberration")]
    public float maxAberration = 1f;
    private ChromaticAberration chromaticAberration;
    private float targetAberration = 0f;

    [Header("Vignette")]
    public float normalVignette = 0.3f;    
    public float slowMoVignette = 0.6f;    
    private Vignette vignette;
    private float targetVignette;

    [Header("Lens Distortion")]
    public float normalDistortion = 0f;
    public float slowMoDistortion = -0.3f;  
    private LensDistortion lensDistortion;
    private float targetDistortion;

    [Header("Color Adjustments")]
    public float normalSaturation = 0f;
    public float slowMoSaturation = -40f;   
    private ColorAdjustments colorAdjustments;
    private float targetSaturation;

    [Header("Low Pass Filter")]
    private AudioLowPassFilter lowPassFilter;
    public float normalCutoff = 22000f;
    public float slowMoCutoff = 800f;
    private float targetCutoff;

    public float slowMotionTime;
    public float startTime;
    public float fixedDeltaTime;

    void Start()
    {
        dragPhysicsScript = GetComponent<DragPhysics>();
        startTime = Time.timeScale;
        fixedDeltaTime = Time.fixedDeltaTime;

        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out lensDistortion);
        globalVolume.profile.TryGet(out colorAdjustments);

        // Set initial values
        targetVignette = normalVignette;
        targetDistortion = normalDistortion;
        targetSaturation = normalSaturation;
        targetCutoff = normalCutoff;

        BackgroundMusic musicManager = FindAnyObjectByType<BackgroundMusic>();
        if (musicManager != null)
            lowPassFilter = musicManager.lowPassFilter;
    }

    void Update()
    {


        // Touch input
        bool touchBegan = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        bool touchEnded = Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended ||
                                                    Input.GetTouch(0).phase == TouchPhase.Canceled);

        if (touchBegan && dragPhysicsScript.touchedGround)
        {
            startSlowMotion();
            targetAberration = maxAberration;
            targetVignette = slowMoVignette;
            targetDistortion = slowMoDistortion;
            targetSaturation = slowMoSaturation;
            targetCutoff = slowMoCutoff;
        }

        if (touchEnded || dragPhysicsScript.currentHealth == 0)
        {
            stopSlowMotion();
            targetAberration = 0f;
            targetVignette = normalVignette;
            targetDistortion = normalDistortion;
            targetSaturation = normalSaturation;
            targetCutoff = normalCutoff;
        }

        float t = Time.unscaledDeltaTime * transitionSpeed;

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, targetAberration, t);

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignette, t);

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, targetDistortion, t);

        if (colorAdjustments != null)
            colorAdjustments.saturation.value = Mathf.Lerp(colorAdjustments.saturation.value, targetSaturation, t);

        if (lowPassFilter != null)
            lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, targetCutoff, t);
    }

    private void startSlowMotion()
    {
        Time.timeScale = slowMotionTime;
        Time.fixedDeltaTime = fixedDeltaTime * slowMotionTime;
    }

    private void stopSlowMotion()
    {
        Time.timeScale = startTime;
        Time.fixedDeltaTime = fixedDeltaTime;
    }
}