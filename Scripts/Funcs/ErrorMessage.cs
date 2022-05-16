using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorMessage : MonoBehaviour
{
    [HideInInspector] public string message;
    [HideInInspector] public Color color = Color.red;
    [HideInInspector] public float duration = 1.5f;
    [HideInInspector] public ErrorDuration errDur = ErrorDuration.Short;

    private TextMeshProUGUI text;
    
    [SerializeField] private float stepSize = .05f;
    [SerializeField] private float numSteps = 100;
    [SerializeField] private float displacement = 50;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.color = color;
        text.text = message;

        if (errDur == ErrorDuration.Short) duration = 3f;
        else
        {
            numSteps = 300;
            duration = 8f;
        }

        stepSize = duration / numSteps;
        float curDisplacement = transform.position.y + displacement;

        for (int i = 0; i < numSteps; i++) 
        {
            color.a = Mathf.Lerp(color.a, 0, i * 1 / numSteps);
            float y = Mathf.Lerp(transform.position.y, curDisplacement, i * 1 / numSteps);
            transform.position = new Vector2(transform.position.x, y);
            text.color = color;
            yield return new WaitForSeconds(stepSize);
        }
        Destroy(gameObject);
    }
}
public enum ErrorDuration
{
    Short,
    Long
}
