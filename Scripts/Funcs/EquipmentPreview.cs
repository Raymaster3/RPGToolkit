using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPreview : MonoBehaviour
{
    public static EquipmentPreview instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    public void updateVisual()
    {
        Transform playerObject = Player.instance.transform.GetChild(0);
        int i = 0;
        foreach (Transform t in playerObject) 
        {
            SkinnedMeshRenderer renderer = t.GetComponent<SkinnedMeshRenderer>();
            if (renderer == null) continue;

            Transform myChild = transform.GetChild(i);
            SkinnedMeshRenderer myChildRenderer = myChild.GetComponent<SkinnedMeshRenderer>();

            myChildRenderer.sharedMaterial = renderer.sharedMaterial;
            myChildRenderer.sharedMesh = renderer.sharedMesh;
            myChildRenderer.enabled = renderer.enabled;
            i++;
        }
    }
    public void Rotate(float angle)
    {
        transform.localRotation = Quaternion.Euler(0, transform.localRotation.y + angle, 0);
    }
    public void ResetTransform()
    {
        transform.localRotation = Quaternion.identity;
    }
}
