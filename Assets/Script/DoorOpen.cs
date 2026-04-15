using UnityEngine;
using UnityEngine.InputSystem;

public class DoorOpen : MonoBehaviour
{
    public GameManager Manager;

    // Which key opens THIS door
    public bool requiresKey1;
    public bool requiresKey2;
    public GameObject Door;
    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Check correct key
            if (requiresKey1 && Manager.gameItem1Picked)
            {
                OpenDoor();
            }
            else if (requiresKey2 && Manager.gameItem2Picked)
            {
                OpenDoor();
            }
        }
    }

    void OpenDoor()
    {
        Debug.Log("Door opened!");
        Destroy(gameObject);
        Destroy(Door);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}