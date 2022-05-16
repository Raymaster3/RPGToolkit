using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropertyNode : NodeData
{
    [SerializeReference] public ExposedProperty property;

    public override string getType()
    {
        return "Property Node";
    }
    public override NodeData createCopy()
    {
        return new PropertyNode
        {
            nodeGUID = nodeGUID,
            Position = Position,
            property = property.createDataCopy()
        };
    }

}
