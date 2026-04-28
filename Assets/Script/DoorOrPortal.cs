using UnityEngine;
using UnityEngine.InputSystem;

public class DoorOrPortal : MonoBehaviour
{
    // Global variable so EnemyAI knows if we are currently hidden
    public static bool PlayerIsHidden = false;

    public Transform destination;

    [Header("Stealth Settings")]
    [Tooltip("Check this if this portal goes INTO a cabinet/hiding spot.")]
    public bool isHidingSpot = false;

    private GameObject player;
    private bool isPlayerInside = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (isPlayerInside && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // 1. Teleport the player
            player.transform.position = destination.position;

            // 3. Update hidden status
            PlayerIsHidden = isHidingSpot;

            // 2. Alert the Enemy that a portal was just used!
            EnemyAI[] allEnemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
            foreach (EnemyAI enemy in allEnemies)
            {
                enemy.OnPlayerUsedPortal(transform.position, destination, isHidingSpot);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}