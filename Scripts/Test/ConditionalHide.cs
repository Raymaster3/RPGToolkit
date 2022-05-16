using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalHide : PropertyAttribute
{
    public string conditionName;
    public bool invert;
    public ConditionalHide(string value, bool invert = false)
    {
        conditionName = value;
        this.invert = invert;
    }
}
