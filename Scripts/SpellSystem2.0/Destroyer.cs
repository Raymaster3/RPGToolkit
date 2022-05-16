using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float lifeTime;
    public Action OnDestroy;
    // Start is called before the first frame update
    void Start()
    {
        //Destroy(gameObject, lifeTime);
        StartCoroutine(DestroyCoroutine());
    }
    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(lifeTime-1.5f);
        OnDestroy?.Invoke();
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
