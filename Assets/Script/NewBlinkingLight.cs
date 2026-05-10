using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NewBlinkingLights : MonoBehaviour
{
    public float blinkInterval = 0.5f;

    public Light2D light2D;

    void Start()
    {

        InvokeRepeating(nameof(Blink), 0f, blinkInterval);
    }

    void Blink()
    {
        light2D.enabled = !light2D.enabled;
    }
}