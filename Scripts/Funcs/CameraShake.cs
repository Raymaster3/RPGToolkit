using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake
{
    public float amplitude;
    public float frequency;
    public float duration;

    private float timer = 0;
    private Vector3 originalPos;
    private Vector3 offset;

    public CameraShake (float _amplitude, float _frequency, float _duration)
    {
        amplitude = _amplitude;
        frequency = _frequency;
        duration = _duration;
        SpellsManager.instance.SubscribeFunction(Shake);
        originalPos = Camera.main.transform.localPosition;
    }

    private void Shake()
    {
        timer += Time.deltaTime;
        if(timer >= duration)
        {
            SpellsManager.instance.UnSubscribeFunction(Shake);
            Camera.main.transform.localPosition = originalPos;
            return;     // End timer
        }

        float xOffset = Mathf.Sin(timer * frequency) * amplitude;
        offset = new Vector2(xOffset, xOffset);
        Camera.main.transform.localPosition += offset;
    }
    
}
