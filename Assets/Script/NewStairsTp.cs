using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewStairsTp : MonoBehaviour
{
    public static bool PlayerIsHidden = false;

    public bool isNewScene;
    public int Scene;
    public Transform destination;

    [Header("Teleport Settings")]
    public float teleportCooldown = 0.5f;

    [Header("Blink Effect")]
    public CanvasGroup fadeCanvas;
    public float blinkDuration = 0.15f;

    private GameObject player;

    // Shared cooldown for ALL teleporters
    private static bool canTeleport = true;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Auto-find CanvasGroup if not assigned
        if (fadeCanvas == null)
        {
            fadeCanvas = FindObjectOfType<CanvasGroup>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!canTeleport) return;

        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        canTeleport = false;

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Teleport or load scene
        if (!isNewScene)
        {
            player.transform.position = destination.position;
        }
        else
        {
            SceneManager.LoadScene(Scene);
        }

        // Small pause while dark
        yield return new WaitForSeconds(0.1f);

        // Fade back in
        yield return StartCoroutine(Fade(1f, 0f));

        // Wait before allowing teleport again
        yield return new WaitForSeconds(teleportCooldown);

        canTeleport = true;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeCanvas == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / blinkDuration);
            fadeCanvas.alpha = alpha;

            yield return null;
        }

        fadeCanvas.alpha = endAlpha;
    }
}