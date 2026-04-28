using UnityEngine;

public class EnemyChaseSound : MonoBehaviour
{
    public AudioClip chaseSound;

    [Header("Distance Settings")]
    [Tooltip("Distance at which the sound starts to be heard (very quiet).")]
    public float maxHearDistance = 15f;

    [Tooltip("Distance at which the sound reaches its maximum volume.")]
    public float minHearDistance = 3f;

    [Header("Volume & Fading")]
    [Tooltip("The maximum volume the sound can reach (0.0 to 1.0).")]
    [Range(0f, 1f)] 
    public float maxVolume = 1f;

    [Tooltip("How smoothly the volume fades in/out.")]
    public float fadeSpeed = 2f;

    private Transform player;
    private AudioSource audioSource;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = chaseSound;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    public void StopChaseSound()
    {
        if (audioSource != null)
        {
            audioSource.volume = 0f;
            audioSource.Stop();
        }
    }

    private void Update()
    {
        if (player == null || audioSource == null) return;

        bool isChasing = !DoorOrPortal.PlayerIsHidden;
        float targetVolume = 0f;

        if (isChasing)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            
            // Calculate how close the player is as a percentage (0 to 1)
            float distanceRatio = 1f - Mathf.InverseLerp(minHearDistance, maxHearDistance, dist);
            
            // Multiply the ratio by your chosen maxVolume
            targetVolume = distanceRatio * maxVolume;
        }

        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
    }
}