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

    private GameObject player;

    // Shared cooldown for ALL teleporters
    private static bool canTeleport = true;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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

        if (!isNewScene)
        {
            player.transform.position = destination.position;
        }
        else
        {
            SceneManager.LoadScene(Scene);
        }

        // Wait before allowing teleport again
        yield return new WaitForSeconds(teleportCooldown);

        canTeleport = true;
    }
}