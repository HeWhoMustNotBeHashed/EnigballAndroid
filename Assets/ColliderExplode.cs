using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;

public class ColliderExplode : MonoBehaviour
{
    public GameObject particles;
    public AudioClip explosionSound;
    public float soundVolume = 0.8f;

    [Header("Score")]
    public int pointsOnDestroy = 20;
    public GameObject damageNumberPrefab;

    private AudioMixerGroup sfxGroup;
    private ScoreManager scoreManager;

    void Start()
    {
        AudioMixer mixer = Resources.Load<AudioMixer>("GameAudioMixer");
        if (mixer != null)
        {
            AudioMixerGroup[] groups = mixer.FindMatchingGroups("SFX");
            if (groups.Length > 0)
                sfxGroup = groups[0];
        }

        scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = transform.position;
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = soundVolume;
        tempSource.outputAudioMixerGroup = sfxGroup;
        tempSource.Play();
        Destroy(tempGO, clip.length);
    }

    private void SpawnDamageNumber()
    {
        if (damageNumberPrefab == null) return;
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject dmgNum = Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity);
        dmgNum.GetComponent<DamageNumber>().Setup(pointsOnDestroy);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(particles, transform.position, Quaternion.identity);

            if (scoreManager != null)
                scoreManager.AddScore(pointsOnDestroy);

            SpawnDamageNumber();
            PlaySound(explosionSound);
            Destroy(gameObject);
        }
    }
}