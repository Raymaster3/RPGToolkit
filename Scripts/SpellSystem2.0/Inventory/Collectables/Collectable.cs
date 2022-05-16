using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour, IEventsHandler
{
    public void onMouseClick(int button)
    {

    }

    public void onMouseEnter()
    {
        GetComponent<HighlightPlus.HighlightEffect>().SetHighlighted(true);
    }

    public void onMouseExit()
    {
        GetComponent<HighlightPlus.HighlightEffect>().SetHighlighted(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
