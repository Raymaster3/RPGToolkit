using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventsHandler
{
    void onMouseEnter();
    void onMouseExit();
    void onMouseClick(int button);
}
