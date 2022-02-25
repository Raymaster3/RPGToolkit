using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Signal
{
    public Action callback;
    public int id;

    public Signal(Action c, int _id)
    {
        id = _id;
        callback = c;
    }
}
