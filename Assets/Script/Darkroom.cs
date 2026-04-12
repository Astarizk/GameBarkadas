using UnityEngine;
using UnityEngine.Rendering.Universal;
public class Darkroom : MonoBehaviour
{
    private Light2D light;
    private GameObject player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            light.falloffIntensity = 1f;
            light.intensity = 0f; // simulate volumetric = 0
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            light.falloffIntensity = 0.5f;
            light.intensity = 1f; // simulate volumetric = 0.09
        }
    }
}
