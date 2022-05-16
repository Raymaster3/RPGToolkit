using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "RPGToolkit/Abilities/Dash", order = 1)]
public class DashAbility : Ability
{
    [Header("Dash Properties")]
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] bool teleport; // We want it to travel the distance or just tp
    [SerializeField] float dashSpeed;

    private Vector3 targetPos;

    protected override void OnStartAiming()
    {
    }
    protected override void OnEndAim()
    {

    }
    protected override void Aiming()
    {
    }

    protected override void NextCast()
    {
        // Actual dash
        if (spendResource())
        {
            SoundManager.instance.PlaySoundOnPlayer(castSound);
            if (castAnimation != null)
            {
                AnimationsManager.instance.PlaySkillAnim(Caster, "SkillsAnimBaseLayer", castAnimation, 1, loopCastAnim);
            }

            Vector3 clickPos = SpellsManager.getMouseWorldPos();
            targetPos = new Vector3(clickPos.x, Caster.transform.position.y, clickPos.z);
            Vector3 dir = targetPos - Caster.transform.position;

            if (dir.magnitude > maxDistance)
                targetPos = Caster.transform.position + dir.normalized * maxDistance;
            else if (dir.magnitude < minDistance)
                targetPos = Caster.transform.position + dir.normalized * minDistance;

            Caster.StopMoving();
            // We can dash
            if (teleport)
                Caster.transform.position = targetPos;
            else
            {
                Caster.BlockMovement();
                SpellsManager.instance.SubscribeFunction(MoveTo);
            }
        }
    }

    private void MoveTo()
    {
        Vector3 distance = targetPos - Caster.transform.position;
        Rigidbody rig = Caster.transform.GetComponent<Rigidbody>();
        if (distance.magnitude < .5f)
        {
            SpellsManager.instance.UnSubscribeFunction(MoveTo);
            rig.velocity = Vector3.zero;
            Caster.UnBlockMovement();
            AnimationsManager.instance.StopLoopAnim(Caster);
            return;
        }
        rig.velocity = distance.normalized * dashSpeed;
    }
}
