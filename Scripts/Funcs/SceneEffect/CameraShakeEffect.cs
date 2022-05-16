using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraShake", menuName = "RPGToolkit/SceneEffects/CameraShake", order = 1)]
public class CameraShakeEffect : SceneEffect
{
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;
    [SerializeField] private float duration;
    public override void StartEffect()
    {
        new CameraShake(amplitude, frequency, duration);
    }
}
