using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Required for Coroutines

public class RoomPortal : MonoBehaviour
{
    [Header("Destination")]
    [Tooltip("Drag the destination Transform (e.g., the other door) here.")]
    public Transform targetDestination;

    [Header("Settings")]
    public bool requireKeyPress = false;

    private GameObject _player;
    private bool _playerInside = false;
    
    // A static flag shared across all portals to prevent infinite bouncing
    private static bool _isTeleporting = false; 

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // Do nothing if the player isn't inside, we don't require a key, or we are currently teleporting
        if (!_playerInside || !requireKeyPress || _isTeleporting) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Teleport();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        _playerInside = true;

        // If we don't require a key, and we aren't already teleporting, go immediately
        if (!requireKeyPress && !_isTeleporting)
        {
            Teleport();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInside = false;
        }
    }

    private void Teleport()
    {
        if (_player == null || targetDestination == null) 
        {
            Debug.LogWarning("Player or Target Destination is missing!");
            return;
        }

        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        // Lock all portals from triggering
        _isTeleporting = true; 

        // Move the player to the exact position of the target destination
        _player.transform.position = targetDestination.position;

        // Wait a tiny fraction of a second before allowing portals to work again
        // This prevents the destination portal from immediately sending you back
        yield return new WaitForSeconds(0.1f);

        _isTeleporting = false; 
        _playerInside = false;
    }
}