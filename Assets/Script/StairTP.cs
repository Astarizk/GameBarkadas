using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StairTP : MonoBehaviour
{
    // Global variable so EnemyAI knows if we are currently hidden
    public static bool PlayerIsHidden = false;

    public bool isNewScene;
    public int Scene;
    public Transform destination;

    [Header("Stealth Settings")]
    [Tooltip("Check this if this portal goes INTO a cabinet/hiding spot.")]
    public bool isHidingSpot = false;

    [Header("Blink Effect")]
    public CanvasGroup fadeCanvas; // Assign black UI image with CanvasGroup
    public float blinkDuration = 0.15f;

    private GameObject player;
    private bool isTeleporting = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(BlinkAndTeleport());
        }
    }

    IEnumerator BlinkAndTeleport()
    {
        isTeleporting = true;

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Teleport or change scene
        if (!isNewScene)
        {
            Debug.Log("Teleport");
            player.transform.position = destination.position;
        }
        else
        {
            SceneManager.LoadScene(Scene);
        }

        // Small delay while screen is dark
        yield return new WaitForSeconds(0.1f);

        // Fade back in
        yield return StartCoroutine(Fade(1f, 0f));

        isTeleporting = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
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