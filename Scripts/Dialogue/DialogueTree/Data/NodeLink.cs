using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeLink
{
    public string BaseNodeGUID;
    public string PortName;
    public string TargetNodeGUID;
    public int originPort = 0;  // Where im from    
    public int destinyPort = 0; // Where im going
}
