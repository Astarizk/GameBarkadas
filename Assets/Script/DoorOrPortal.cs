using UnityEngine;
using UnityEngine.InputSystem;

public class DoorOrPortal : MonoBehaviour
{
    // Global variable so EnemyAI knows if we are currently hidden
    public static bool PlayerIsHidden = false;

    public GameManager Manager;
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

            // 2. Alert the Enemy that a portal was just used!
            EnemyAI[] allEnemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
            foreach (EnemyAI enemy in allEnemies)
            {
                // We send the enemy the portal's position, where it goes, and if it's a hiding spot
                enemy.OnPlayerUsedPortal(transform.position, destination, isHidingSpot);
            }

            // 3. Update our global hidden status
            // If we go IN to a cabinet, we are hidden. If we use a portal to go OUT, we are not!
            PlayerIsHidden = isHidingSpot; 
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