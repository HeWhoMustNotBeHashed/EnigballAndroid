using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip musicClip;
    public AudioMixerGroup musicMixerGroup;

    [Header("Fade Settings")]
    public float fadeInDuration = 2f;    
    public float fadeOutDuration = 2f;  

    [HideInInspector] public AudioLowPassFilter lowPassFilter;
    private AudioSource audioSource;
    private static BackgroundMusic instance;

    private enum FadeState { FadingIn, Playing, FadingOut }
    private FadeState fadeState = FadeState.FadingIn;
    private float fadeTimer;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = false;       
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = musicMixerGroup;
        audioSource.volume = 0f;       
        audioSource.Play();

        lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
        lowPassFilter.cutoffFrequency = 22000f;

        fadeTimer = 0f;
        fadeState = FadeState.FadingIn;
    }

    private void Update()
    {
        float clipLength = musicClip.length;
        float timeLeft = clipLength - audioSource.time;

        switch (fadeState)
        {
            case FadeState.FadingIn:
                fadeTimer += Time.deltaTime;
                audioSource.volume = Mathf.Clamp01(fadeTimer / fadeInDuration);
                if (fadeTimer >= fadeInDuration)
                    fadeState = FadeState.Playing;
                break;

            case FadeState.Playing:
                audioSource.volume = 1f;
                // Start fading out when close to end
                if (timeLeft <= fadeOutDuration)
                    fadeState = FadeState.FadingOut;
                break;

            case FadeState.FadingOut:
                audioSource.volume = Mathf.Clamp01(timeLeft / fadeOutDuration);
                // When clip ends, restart and fade back in
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                    fadeTimer = 0f;
                    fadeState = FadeState.FadingIn;
                }
                break;
        }
    }
}