using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MySelectionDropper : SelectionDropper
{
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(evt => {
            Debug.Log("Hola");
            DragAndDrop.SetGenericData("Indexes", 1);
        });

    }
    private void Test()
    {

    }
}
