using UnityEngine;
using UnityEngine.InputSystem;

public class DoorOrPortal : MonoBehaviour
{
    public Transform destination;
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
                player.transform.position = destination.position;
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