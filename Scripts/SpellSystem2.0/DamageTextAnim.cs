using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextAnim : MonoBehaviour
{
    [SerializeField] private float duration = 4f;
    [SerializeField] private float speed = 10f;
    [HideInInspector] public Vector3 targetPos;
    [HideInInspector] public Color textColor;
    private Vector2 offset;
    private TextMeshProUGUI text;

    [SerializeField] private float stepSize = .05f;
    [SerializeField] private float numSteps = 100;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        stepSize = duration / numSteps;

        for (int i = 0; i < numSteps; i++)
        {
            textColor.a = Mathf.Lerp(textColor.a, 0, i * 1 / numSteps);
            text.color = textColor;
            yield return new WaitForSeconds(stepSize);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Camera.main.WorldToScreenPoint(targetPos);
        transform.position = new Vector2(pos.x, transform.position.y);
        //transform.Translate(transform.up * speed * Time.deltaTime);
        transform.position = pos + offset;
        offset += new Vector2(0, speed * Time.deltaTime);
    }
}
