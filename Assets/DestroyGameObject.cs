using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;

public class DestroyGameObject : MonoBehaviour
{
    public float upwardBoostForce = 15f;
    private CinemachineCollisionImpulseSource impulseSource;
    public GameObject particles;

    public AudioClip explosionSound, pintuSound;
    public float soundVolume = 0.8f;

    [Header("Score")]
    public int pointsOnDestroy = 10;        
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

        impulseSource = GetComponent<CinemachineCollisionImpulseSource>();
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
        Debug.Log("SpawnDamageNumber called, prefab is: " + damageNumberPrefab);
        if (damageNumberPrefab == null)
        {
            Debug.Log("damageNumberPrefab is NULL");
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject dmgNum = Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity);
        Debug.Log("Instantiated: " + dmgNum.name);
        DamageNumber dn = dmgNum.GetComponent<DamageNumber>();
        Debug.Log("DamageNumber component: " + dn);
        dn.Setup(pointsOnDestroy);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("Trigger hit by: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected");

            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);
                playerRb.AddForce(Vector2.up * upwardBoostForce, ForceMode2D.Impulse);
            }

            impulseSource.GenerateImpulse();
            Debug.Log("Before particles");
            Instantiate(particles, transform.position, Quaternion.identity);
            Debug.Log("Before score");
            if (scoreManager != null)
                scoreManager.AddScore(pointsOnDestroy);
            Debug.Log("Before damage number");
            SpawnDamageNumber();
            Debug.Log("Before destroy");
            PlaySound(explosionSound);
            Destroy(gameObject);
            Debug.Log("After destroy");
            PlaySound(pintuSound);
        }
    }
}