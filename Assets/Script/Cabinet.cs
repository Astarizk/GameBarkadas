using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class Cabinet : MonoBehaviour
{
    private GameObject player;
    private bool isHidden = false;
    private Light2D light;
    private bool isPlayerInside = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject lightObj = GameObject.FindGameObjectWithTag("Light");
        if (lightObj != null)
        {
            light = lightObj.GetComponent<Light2D>();
        };
    }

    private void Update()
    {
        /*
        if (isPlayerInside && Keyboard.current.eKey.wasPressedThisFrame && isHidden == false)
        {
            player.transform.position = destination.position;
            isHidden = true;
        }
        if (isPlayerInside && Keyboard.current.eKey.wasPressedThisFrame && isHidden == true)
        {
            player.transform.position = destination.position;
            isHidden = false;
        }*/
        if (light == null) return;


       /* if (isHidden == true)
        {
            light.falloffIntensity = 0f;
            light.intensity = 0f; // simulate volumetric = 0
            Debug.Log("Player is hiding");
        }
        else
        {
            light.falloffIntensity = 1f;
            light.intensity = 0.09f; // simulate volumetric = 0.09
            Debug.Log("Player is NOT hiding");
        }*/
    }
    private void FixedUpdate()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
            light.falloffIntensity = 1f;
            light.intensity = 0f; // simulate volumetric = 0
            Debug.Log("Player is hiding");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
            isHidden = false;
            light.falloffIntensity = 0.5f;
            light.intensity = 1f; // simulate volumetric = 0.09
        }
    }
}