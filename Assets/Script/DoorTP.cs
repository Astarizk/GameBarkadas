using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoorTp : MonoBehaviour
{
    public GameManager Manager;

    // Which key opens THIS door
    public bool requiresKey1;
    public bool requiresKey2;
    public int Scene;
    public int Scene2;
    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Check correct key
            if (requiresKey1 && Manager.gameItem1Picked)
            {
                SceneManager.LoadScene(Scene);
            }
            else if (requiresKey2 && Manager.gameItem2Picked)
            {
                SceneManager.LoadScene(Scene2);
            }
        }
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