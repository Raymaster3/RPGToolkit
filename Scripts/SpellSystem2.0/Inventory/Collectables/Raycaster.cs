using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    public static Raycaster instance;

    [HideInInspector] public bool rightClick;
    [HideInInspector] public bool leftClick;
    [HideInInspector] public bool openCharWindowAction;
    [HideInInspector] public bool escape;
    [HideInInspector] public Vector2 mousePos;
    [HideInInspector] public bool[] keyNumbers;



    private RaycastHit hit;

    private Collider curHitColl;

    void Awake()
    {
        instance = this;
        keyNumbers = new bool[10];
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
        HandleEvents();
    }

    protected virtual void HandleInputs()
    {
        leftClick = Input.GetMouseButtonDown(0);
        rightClick = Input.GetMouseButtonDown(1);
        escape = Input.GetKeyDown(KeyCode.Escape);

        openCharWindowAction = Input.GetKeyDown("i");
        mousePos = Input.mousePosition;
        for (int i = 1; i < 10; i++)
        {
            keyNumbers[i] = Input.GetKeyDown("" + i);
        }
    }

    private void HandleEvents()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            IEventsHandler[] handlers = hit.collider.GetComponents<IEventsHandler>();

            if (handlers == null || handlers.Length == 0) // This object has no EventHandlers
            {
                if (leftClick)
                {
                    SpellsManager.instance.target = null;
                    UIManager.instance.closeSelctedCharWindow();
                }
                if (curHitColl != null) { 
                    IEventsHandler[] hs = curHitColl.GetComponents<IEventsHandler>();
                    foreach (IEventsHandler h in hs)
                        h.onMouseExit();
                    curHitColl = null;
                }
                return;
            }
            if (curHitColl == null)
            {
                curHitColl = hit.collider;
                foreach (IEventsHandler handler in handlers)
                    handler.onMouseEnter();
                return;
            }
            if (hit.collider == curHitColl)
            {
                if (leftClick)
                {
                    foreach(IEventsHandler handler in handlers)
                        handler.onMouseClick(0);
                }
                if (rightClick)
                {
                    foreach (IEventsHandler handler in handlers)
                        handler.onMouseClick(1);
                }
                return;
            }
            IEventsHandler[] curHandlers = curHitColl.GetComponents<IEventsHandler>();
            foreach(IEventsHandler curHandler in curHandlers)
            {
                curHandler.onMouseExit();
            }
            curHitColl = hit.collider;
        }
    }
}
