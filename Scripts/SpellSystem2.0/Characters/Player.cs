using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i <= abilities.Count; i++)
        {
            if (Input.GetKeyDown("" + i))
            {
                abilities[i-1]?.Cast(this, SpellsManager.getMouseWorldPos());
            }
        }
    }
}
