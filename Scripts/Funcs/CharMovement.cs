using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMovement : MonoBehaviour
{

    private bool moving = false;
    private Vector3 targetPos; Vector3 myTarget; Vector3 dir;
    private Rigidbody rig;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1)) {
            // Move
            targetPos = SpellsManager.getMouseWorldPos();
            myTarget = new Vector3(targetPos.x, transform.position.y, targetPos.z); 
            
            moving = true;
        }
    }
    private void FixedUpdate()
    {
        if (moving)
        {
            Move(targetPos);
        }
    }
    private void Move(Vector3 pos)
    {
        float speed = Player.instance.getStatByName("Speed").getValue();
        dir = myTarget - transform.position;
        if (dir.magnitude <= .1) // Close enough
        {
            rig.velocity = Vector3.zero;
            moving = false;
            return;
        }
        transform.LookAt(myTarget);
        rig.velocity = dir.normalized * speed;
    }
}
