using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BlinkingLightsEffect : MonoBehaviour
{
    private Light2D light;
    private Coroutine blinkCoroutine;

    private void Awake()
    {
        GameObject lightObj = GameObject.FindGameObjectWithTag("Light");

        if (lightObj != null)
        {
            light = lightObj.GetComponent<Light2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkLight());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }

            light.intensity = 1f; // reset light
        }
    }

    IEnumerator BlinkLight()
    {
        while (true)
        {
            light.intensity = 0f; // off
            yield return new WaitForSeconds(0.2f);

            light.intensity = 1f; // on
            yield return new WaitForSeconds(0.2f);
        }
    }
}