using System.Collections;
using UnityEngine;

public class EnemiesBehavior : LivingEntity
{
    [Header("Animations")]
    public float attackDistance;
    public float waitTime;
    public float secBetwenAttack;
    public float damage;
    public float moveSpeed;

    [Header("SpawnSettings")]
    public float timeToNextSpawn;
    public float chanceToSpawn;

    public int pointsForDeath;

    [Header("SpawnSettings")]
    public bool flipLookSide;

    //SameValue
    [HideInInspector] public float _damage;
    [HideInInspector] public float _chanceToSpawn;
    [HideInInspector] public float _timeToNextSpawn;
    [HideInInspector] public float _moveSpeed = 0;
    

    [HideInInspector]
    public bool isCanAttack = false;
    private bool isRotated = false;
    private bool isHasTarget;
    protected bool m_state = true;
    [HideInInspector] public bool isFrezed = false;
    private bool firstAttack = true;

    protected Animator animator;
    protected GameObject target;

    private Transform pathHolder;

    private float dstBtwMyTransformToPlayer;

    private float nextAttackTime;
    private float frameBeforePosX;
    private float rangeBetweenPrevPos;

    private Vector3[] waypoints;

    [HideInInspector]
    public int targetWaypointIndex;

    LivingEntity lE_target;
    protected override void Start()
    {
        base.Start();
        if (!isSummoned || _damage == 0 || _moveSpeed == 0)
        {
            _damage = damage;
            _moveSpeed = moveSpeed;
        }

        if (GameObject.FindGameObjectsWithTag("PathHolder") != null) // is their at least one pathHolder
        {


            animator = GetComponent<Animator>();

            //pathHolders massive
            GameObject[] waypointsArr = GameObject.FindGameObjectsWithTag("PathHolder");
            int waypointsLength = waypointsArr.Length;

            if (waypointsLength > 0)
            {
                //set a random spawn point
                int pathHolderInd = Random.Range(0, waypointsLength);
                pathHolder = waypointsArr[pathHolderInd].transform;
                waypoints = new Vector3[pathHolder.childCount];
                for (int i = 0; i < waypoints.Length; i++)
                {
                    waypoints[i] = pathHolder.GetChild(i).position;
                    waypoints[i] = new Vector3(waypoints[i].x, waypoints[i].y, transform.position.z);
                }

                Invoke("startMove", 1f);
                if (targetWaypointIndex == 0)
                {
                    transform.position = waypoints[targetWaypointIndex];
                }

                //move to the target
                animator.speed = _moveSpeed;
                animator.SetBool("isMoving", true);
                frameBeforePosX = transform.position.x;

                nextAttackTime = Time.time;
            }
        }
    }

    private void startMove()
    {
        StartCoroutine(movement(waypoints));
    }


    public virtual void Update()
    {
        hasTaget();
        if (isHasTarget == true)
        {
            //проверки возможности атаковать
            if (isCanAttack == true && isFrezed == false)
            {
                if (nextAttackTime < Time.time && firstAttack == false)
                {
                    //нанесение урон

                    animator.speed = 1 / secBetwenAttack;

                    nextAttackTime = Time.time + secBetwenAttack;
                    attackTarget();
                }
                else if (firstAttack == true && nextAttackTime < Time.time)
                {
                    firstAttack = false;
                    nextAttackTime = Time.time + secBetwenAttack;
                }
            }


            if (isCanAttack == false)
            {
                dstBtwMyTransformToPlayer = Vector2.SqrMagnitude(target.transform.position - transform.position);
                if (dstBtwMyTransformToPlayer < attackDistance)
                {
                    StopCoroutine("movement");
                    m_state = false;
                    animator.speed = 1 / secBetwenAttack;
                    animator.SetBool("isMoving", false);
                    isCanAttack = true;
                    animator.SetBool("IsAttack", true);
                }
            }



            rangeBetweenPrevPos = frameBeforePosX - transform.position.x;

            //swith watch side
            if (rangeBetweenPrevPos < 0)
            {
                isRotated = true;
                if (flipLookSide == false)
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else if (rangeBetweenPrevPos > 0)
            {
                isRotated = false;
                if (flipLookSide == false)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                }
            }
            else if (target.transform.position.x - transform.position.x > 0)
            {
                if (flipLookSide == false)
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else if (target.transform.position.x - transform.position.x < 0)
            {
                if (flipLookSide == false)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                }
            }


            //
            frameBeforePosX = transform.position.x;
        }
        else
        {
            StopCoroutine("movement");
            m_state = false;
            animator.SetBool("isMoving", false);
            isCanAttack = false;
            animator.SetBool("IsAttack", false);
            animator.speed = _moveSpeed;
        }
    }

    //проверка есть ли игрок на сцене
    protected void hasTaget()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            isHasTarget = true;
        }
        else
        {
            isHasTarget = false;
        }
    }


    IEnumerator movement(Vector3[] waypoints)
    {

        m_state = true;
        if (targetWaypointIndex == 0)
        {
            targetWaypointIndex++;
        }
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];

        while (m_state)
        {
            if (isFrezed == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, _moveSpeed * Time.deltaTime);
                if (transform.position == targetWaypoint)
                {
                    ++targetWaypointIndex;
                    if (targetWaypointIndex == waypoints.Length)
                    {
                        StopCoroutine("movement");
                        animator.SetBool("isMoving", false);
                        isCanAttack = true;
                        animator.speed = 1 / secBetwenAttack;
                        animator.SetBool("IsAttack", true);
                        break;
                    }
                    else
                    {
                        targetWaypoint = waypoints[targetWaypointIndex];
                    }
                    yield return new WaitForSeconds(waitTime);
                }
            }
           //проигрывать анимацию передвижения (*)
            yield return null;
        }
    }

    protected virtual void attackTarget()
    {
        target.GetComponent<IDamagable>().damageTaken(_damage);
    }

    protected override void Die()
    {
        base.Die();
        Spawner.currentPoints += pointsForDeath;
    }

}
