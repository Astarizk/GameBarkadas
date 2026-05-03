using UnityEngine;
using UnityEngine.InputSystem;

public class ViewObject : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";
    public float interactionRange = 2f;

    [Header("Object To Show/Hide")]
    public GameObject targetObject;

    private Transform player;
    private bool isShown = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null || targetObject == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool playerNearby = distance <= interactionRange;

        // Movement check
        bool isWalking =
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.dKey.isPressed ||
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.sKey.isPressed;

        // Hide if player moves
        if (isShown && isWalking)
        {
            isShown = false;
            targetObject.SetActive(false);
        }

        // Toggle with E only if nearby
        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            isShown = !isShown;
            targetObject.SetActive(isShown);
        }

        // Force hide if player leaves range
        if (!playerNearby && isShown)
        {
            isShown = false;
            targetObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}