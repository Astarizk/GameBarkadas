using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StairTP : MonoBehaviour
{
    public static bool PlayerIsHidden = false;
    public bool isNewScene;
    public int Scene;
    public Transform destination;

    [Header("Stealth Settings")]
    [Tooltip("Check this if this portal goes INTO a cabinet/hiding spot.")]
    public bool isHidingSpot = false;

    [Header("Blink Effect")]
    public CanvasGroup fadeCanvas;
    public float blinkDuration = 0.15f;

    private GameObject player;
    private LayerMovement playerMovement; //  Replace with your movement script name
    private bool isTeleporting = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<LayerMovement>(); //  Same here
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

        Debug.Log("canMove BEFORE lock: " + playerMovement.canMove);
        if (playerMovement != null) playerMovement.canMove = false;
        Debug.Log("canMove AFTER lock: " + playerMovement.canMove);

        yield return StartCoroutine(Fade(0f, 1f));

        if (!isNewScene)
        {
            player.transform.position = destination.position;
        }
        else
        {
            SceneManager.LoadScene(Scene);
        }

        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Fade(1f, 0f));

        Debug.Log("canMove BEFORE restore: " + playerMovement.canMove);
        if (playerMovement != null) playerMovement.canMove = true;
        Debug.Log("canMove AFTER restore: " + playerMovement.canMove);

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