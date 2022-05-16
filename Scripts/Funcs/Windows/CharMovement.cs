using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMovement : MonoBehaviour
{

    private bool moving = false;
    private bool canMove = true;
    private Vector3 targetPos; Vector3 myTarget; Vector3 dir;
    private Rigidbody rig;
    private Animator anim;
    private Transform visual;
    public bool debug;

    bool validPos;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        visual = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1) && canMove) {
            // Move
            targetPos = SpellsManager.getMouseWorldPos(out validPos, true);
            if (validPos)
            {
                myTarget = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                resetCharacterTransform();
                moving = true;
            }
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
            resetCharacterTransform();
            anim.SetFloat("WalkSpeed", 0);
            moving = false;
            return;
        }
        visual.LookAt(myTarget);
        anim.SetFloat("WalkSpeed", speed);
        rig.velocity = dir.normalized * speed;
    }
    public void UpdateAnimation()
    {
        anim.SetFloat("WalkSpeed", rig.velocity.magnitude);
    }
    private void resetCharacterTransform()
    {
        //transform.GetChild(0).transform.localRotation = new Quaternion(0, 0, 0, 0);
    }
    private void OnDrawGizmos()
    {
        if (debug)
            Gizmos.DrawSphere(targetPos, 1);
    }
    public void StopMoving()
    {
        moving = false;
        rig.velocity = Vector3.zero;
        anim.SetFloat("WalkSpeed", 0);
        resetCharacterTransform();
    }
    public void BlockMovement()
    {
        canMove = false;
    }
    public void UnBlockMovement()
    {
        canMove = true;
    }
}
