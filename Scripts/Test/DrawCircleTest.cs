using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircleTest : MonoBehaviour
{
    public LineRenderer lRenderer;
    public Vector3 originPoint;
    // Start is called before the first frame update
    void Start()
    {
        lRenderer = GetComponent<LineRenderer>();
        DrawCircle(transform.position, transform.forward, 5, 10, 360);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DrawCircle(Vector3 origin, Vector3 direction, float radius, float steps, float angle ,float minDistance = 0)
    {
        /*Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 startingDir = direction * radius;
        Vector3 angledVector = rotation * startingDir;*/

        Vector3 startingDir = direction * radius;
        List<float> angles = GetAngles(50, 0, angle, -angle/2);
        lRenderer.positionCount = angles.Count;

        int i = 0;
        if (angle < 360) { 
            lRenderer.SetPosition(0, origin);
            lRenderer.positionCount = angles.Count+1;
            i = 1;
        }
        foreach (float _angle in angles)
        {
            Quaternion rotation = Quaternion.AngleAxis(_angle, Vector3.up);
            Vector3 angledVector = rotation * startingDir;
            lRenderer.SetPosition(i, origin + angledVector);
            i++;
        }
    }
    private List<float> GetAngles(float n, float minAngle, float maxAngle, float angleOffset = 0)
    {
        List<float> angles = new List<float>();
        minAngle += angleOffset;
        maxAngle += angleOffset;
        for (int i = 0; i < n; i++)
        {
            angles.Add(Mathf.Lerp(minAngle, maxAngle, i/n));
        }
        angles.Add(maxAngle);
        return angles;
    }
}
