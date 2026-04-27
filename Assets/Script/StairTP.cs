using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StairTP : MonoBehaviour
{
    // Global variable so EnemyAI knows if we are currently hidden
    public static bool PlayerIsHidden = false;
    public bool isNewScene;
    public int Scene;
    public Transform destination;

    [Header("Stealth Settings")]
    [Tooltip("Check this if this portal goes INTO a cabinet/hiding spot.")]
    public bool isHidingSpot = false;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isNewScene == false)
            {
                Debug.Log("Teleport");
                player.transform.position = destination.position;
            }
            if (isNewScene == true)
            {
                SceneManager.LoadScene(Scene);
            }
        }
    }


}