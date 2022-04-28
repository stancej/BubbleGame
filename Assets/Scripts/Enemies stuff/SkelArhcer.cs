using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelArhcer : EnemiesBehavior
{
    [Header("shoot settings")]
    public GameObject arrow;
    public Transform muzzle;
    public float arrowSpeed;
    public float shootAngle;

    private GameObject currentArrow;
    private float rangeToTarget;
    private bool isAttacked = false;
    private void Awake()
    {
        secBetwenAttack += 2;
    }

    public override void Update()
    {
        base.Update();
        if (isCanAttack == true && isAttacked == false)
        {
            secBetwenAttack -= (1 + (secBetwenAttack - 2));
            isAttacked = true;
        }
        else if(isAttacked == true && isCanAttack == false) 
        {
            secBetwenAttack += 2;
            isAttacked = false;
        }
        
    }

    protected override void attackTarget()
    {

        rangeToTarget = Vector2.SqrMagnitude(muzzle.position - target.transform.position);


        //instantiate arrow
        currentArrow = Instantiate(arrow, muzzle.position, transform.rotation);
        currentArrow.GetComponent<Arrow>().infoAboutTarget(rangeToTarget, target.transform.position - muzzle.position, target.transform.position, shootAngle, arrowSpeed);
    }
}
