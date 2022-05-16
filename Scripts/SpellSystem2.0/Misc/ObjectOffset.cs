using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOffset : MonoBehaviour
{
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition += positionOffset;
        transform.localRotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
