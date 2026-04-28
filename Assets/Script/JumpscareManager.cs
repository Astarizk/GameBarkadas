using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{
    public Image jumpscareImage;
    public AudioClip jumpscareSound;
    public GameObject gameOverCanvas;

    private AudioSource audioSource;
    private bool triggered = false;
    private bool isPlaying = false;
    private float elapsed = 0f;
    private float duration = 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        if (jumpscareImage != null) jumpscareImage.gameObject.SetActive(false);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
    }

    public void TriggerJumpscare()
    {
        if (triggered) return;
        triggered = true;

        if (jumpscareImage != null) jumpscareImage.gameObject.SetActive(true);

        // Stop chase sound on enemy
        EnemyChaseSound chaseSound = FindFirstObjectByType<EnemyChaseSound>();
        if (chaseSound != null) chaseSound.StopChaseSound();

        // Play sound — must set these BEFORE timeScale = 0
        if (audioSource == null) Debug.LogError("NO AUDIOSOURCE ON JUMPSCAREMANAGER!");
        else if (jumpscareSound == null) Debug.LogError("NO AUDIO CLIP ASSIGNED!");
        else
        {
            audioSource.ignoreListenerPause = true;
            audioSource.PlayOneShot(jumpscareSound);
            Debug.Log("Sound played: " + jumpscareSound.name);
        }

        duration = jumpscareSound != null ? jumpscareSound.length : 2f;
        elapsed = 0f;
        isPlaying = true;

        Time.timeScale = 0f;
        AudioListener.pause = false;
    }

    // Use Update with unscaledDeltaTime instead of coroutine
    // because coroutines can be unreliable when timeScale = 0
    private void Update()
    {
        if (!isPlaying) return;

        elapsed += Time.unscaledDeltaTime;

        if (elapsed >= duration)
        {
            isPlaying = false;
            if (jumpscareImage != null) jumpscareImage.gameObject.SetActive(false);
            if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        }
    }
}