using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : EnemiesBehavior
{
    [Header("Summons Settings")]
    public GameObject summon;
    public float strengthCoef = 1f;
    public float spawnRadius = 1f;
    public int spawnCount = 3;

    [Header("Create Summons while walking")]
    public bool isCreatingSummonsWhileWalking = true;
    public float timeBetwenCreateSummons;
    private float timeToNextSummon;

    protected virtual void Awake()
    {
        if (timeBetwenCreateSummons == 0)
        {
            timeBetwenCreateSummons = secBetwenAttack;
        }
        timeToNextSummon = Time.time + secBetwenAttack + 1;
    }

    public override void Update()
    {
        base.Update();
        if (isCanAttack != true)
        {
            if (Time.time > timeToNextSummon)
            {
                isFrezed = true;

                StartCoroutine(summonWhileWalking());
                timeToNextSummon += secBetwenAttack + animator.speed;
            }
        }
    }

    IEnumerator summonWhileWalking()
    {
        isFrezed = true;
        animator.SetBool("IsAttack", true);
        animator.SetBool("isSpawn", true);
        animator.speed = 1 / secBetwenAttack;
        Invoke("castSpawnMethot", 1f);
        yield return new WaitForSeconds(1.2f);
        animator.speed = _moveSpeed;
        animator.SetBool("IsAttack", false);
        animator.SetBool("isSpawn", false);
        isFrezed = false;
    }

    private void castSpawnMethot()
    {
        spawnSummons(summon, spawnCount, spawnRadius, strengthCoef);
    }

    private void spawnSummons(GameObject summonObject, int summonsSpawnCount, float spawnRange, float summonPowerCoef)
    {
        float angleBetweenSpawn = 360f / summonsSpawnCount;
        float currentRadAngle;
        Vector3 positionToSpawn;
        GameObject currentSummon;
        EnemiesBehavior currentSummonBeh;
        for (int i = 0; i < summonsSpawnCount; i++)
        {
            //spawn summon
            currentRadAngle = angleBetweenSpawn * i * Mathf.Deg2Rad;
            positionToSpawn = new Vector3(spawnRange * Mathf.Cos(currentRadAngle) + transform.position.x, spawnRange * Mathf.Sin(currentRadAngle) + transform.position.y, transform.position.z - 0.001f);
            currentSummon = Instantiate(summonObject, positionToSpawn, transform.rotation, transform.parent) as GameObject;
            currentSummonBeh = currentSummon.GetComponent<EnemiesBehavior>();
            currentSummonBeh.targetWaypointIndex = targetWaypointIndex;
            currentSummonBeh.isSummoned = true;

            //set summonsStats;
            currentSummonBeh._damage = currentSummonBeh.damage * summonPowerCoef;
            currentSummonBeh._health = currentSummonBeh.startingHealth * summonPowerCoef;
            currentSummonBeh._moveSpeed = 1.05f * _moveSpeed * summonPowerCoef;

        }
    }

    protected override void attackTarget()
    {
        spawnSummons(summon, spawnCount, spawnRadius, strengthCoef);
    }
}
