using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerDead : MonoBehaviour
{

    private CinemachineCollisionImpulseSource impulseSource;
    public GameObject particles;
    public AudioClip explosionSound;
    public float soundVolume = 0.8f;


    
    private AudioMixerGroup sfxGroup;

    public GameManagerScript gameManagerScript;
    void Start()
    {
        AudioMixer mixer = Resources.Load<AudioMixer>("GameAudioMixer");
        if (mixer != null)
        {
            AudioMixerGroup[] groups = mixer.FindMatchingGroups("SFX");
            if (groups.Length > 0)
                sfxGroup = groups[0];
        }
        gameManagerScript = FindAnyObjectByType<GameManagerScript>();

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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Lava"))
        {
           
            Instantiate(particles, transform.position, Quaternion.identity);
            PlaySound(explosionSound);
            gameManagerScript.isDead();
            Destroy(gameObject);
            
            



        }
    }
}
