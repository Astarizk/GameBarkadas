using UnityEngine;

public class EnemyChaseSound : MonoBehaviour
{
    public AudioClip chaseSound;

    [Tooltip("Distance at which sound starts fading in.")]
    public float maxHearDistance = 15f;

    [Tooltip("Distance at which sound is at full volume.")]
    public float minHearDistance = 3f;

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
            targetVolume = 1f - Mathf.InverseLerp(minHearDistance, maxHearDistance, dist);
        }

        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
    }
}