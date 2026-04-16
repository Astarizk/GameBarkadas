using UnityEngine;
using UnityEngine.InputSystem;

public class Ladder : MonoBehaviour
{
    public GameManager Manager;
    public Transform destination;

    private GameObject player;
    private bool isPlayerInside = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure it has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (isPlayerInside && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (Manager != null && Manager.LadderItem)
            {
                TeleportPlayer();
            }
            else
            {
                Debug.Log("You need the ladder item!");
            }
        }
    }

    void TeleportPlayer()
    {
        if (destination != null && player != null)
        {
            player.transform.position = destination.position;
        }
        else
        {
            Debug.LogError("Missing destination or player!");
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