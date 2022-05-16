using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class UpdatableList
{
    private List<IUpdatable> objects;

    public UpdatableList(List<IUpdatable> List) { objects = List; }
     
    public void Update()
    {
        foreach (IUpdatable upt in objects)
        {
            upt.Update();
        }
    }
}
