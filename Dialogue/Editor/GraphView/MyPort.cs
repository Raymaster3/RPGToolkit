using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MyPort : Port
{
    public Action OnConnected = null;
    protected MyPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
    {
    }

}
public class MyBlackBoardField : BlackboardField
{
    public int index;
    public override void OnSelected()
    {

        DialogueTreeView.selectedIndexes.Add(index);
        base.OnSelected();
    }
    public override void OnUnselected()
    {
        DialogueTreeView.selectedIndexes.Remove(index);
        base.OnUnselected();
    }
}
