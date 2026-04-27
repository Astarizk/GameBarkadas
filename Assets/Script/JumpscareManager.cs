using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{
    public Image jumpscareImage;
    public AudioClip jumpscareSound;
    public GameObject gameOverCanvas;

    private AudioSource audioSource;
    private bool triggered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (jumpscareImage != null) jumpscareImage.gameObject.SetActive(false);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
    }

    public void TriggerJumpscare()
    {
        if (triggered) return;
        triggered = true;
        Time.timeScale = 0f;
        AudioListener.pause = false; // make sure audio still plays when timeScale = 0
        StartCoroutine(PlayJumpscare());
    }

    private System.Collections.IEnumerator PlayJumpscare()
    {
        if (jumpscareImage != null) jumpscareImage.gameObject.SetActive(true);

        if (audioSource != null && jumpscareSound != null)
        {
            audioSource.ignoreListenerPause = true;
            audioSource.clip = jumpscareSound;
            audioSource.Play();
        }

        float duration = jumpscareSound != null ? jumpscareSound.length : 2f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (jumpscareImage != null) jumpscareImage.gameObject.SetActive(false);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
    }
}