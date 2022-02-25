using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextAnim : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private float speed = 10f;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, duration + 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime);
    }
}
