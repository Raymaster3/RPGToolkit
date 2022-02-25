using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsManager : MonoBehaviour
{
    public static SpellsManager instance;

    private Action Subscribers;
    [SerializeField] private Ability toCast;

    private Vector3 gizmoPos, gizmoSize;
    private float radius;
    private bool drawGizmosCube = false, drawGizmosSphere;

    public Character target;  // Temporal para pruebas
    public Color gizmosColor;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        toCast = ScriptableObject.Instantiate(toCast);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            toCast.Cast(target, getMouseWorldPos());
        }
        Subscribers?.Invoke();
    }
    
    public void BlockAbilities(Character caster)
    {
        foreach (Ability a in caster.Abilities)
        {
            if (a.getState() != AbilityState.Idle) continue;
            if (caster.GetType() == typeof(Player))
                UIManager.instance.BlockAbility(a.index);
            a.setState(AbilityState.Blocked);
        }
    }
    public void UnBlockAbilities(Character caster)
    {
        foreach (Ability a in caster.Abilities)
        {
            if (a.getState() != AbilityState.Blocked) continue;
            if (caster.GetType() == typeof(Player))
                UIManager.instance.UnBlockAbility(a.index);
            a.setState(AbilityState.Idle);
        }
    }

    public void SubscribeFunction(Action func)
    {
        Subscribers += func;
    }
    public void UnSubscribeFunction(Action func)
    {
        Subscribers -= func;
    }
    // For debug only
    public void setUpGizmosCube (Vector3 pos, Vector3 size)
    {
        gizmoPos = pos;
        gizmoSize = size;
        drawGizmosCube = true;
    }
    public void setUpGizmosSphere(Vector3 pos, float radi)
    {
        radius = radi;
        gizmoPos = pos;
        drawGizmosSphere = true;
    }
    public void resetGizmos()
    {
        drawGizmosCube = false;
        drawGizmosSphere = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        if (drawGizmosCube)
        {
            Gizmos.DrawCube(gizmoPos, gizmoSize);
        }
        if (drawGizmosSphere)
        {
            Gizmos.DrawSphere(gizmoPos, radius);
        }
    }
    public static Vector3 getMouseWorldPos()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, float.PositiveInfinity,LayerMask.GetMask("Terrain"));
        return hit.point;
    }
}
