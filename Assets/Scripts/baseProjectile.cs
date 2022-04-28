using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseProjectile : MonoBehaviour
{

    public float lifeTime = 3;
    public float speed = 10;
    public float pushEnemy = 15f;
    public LayerMask collisionMask;

    protected float movementDst;


    [SerializeField] private float coef = 8;
    [SerializeField] private float skinWidth = 0.75f;

    private void Awake()
    {
        //set std base damage

        //group up bullets under a bulletsHolder 
        if (GameObject.FindGameObjectWithTag("Trash") != null)
        {
            GameObject projectileStorage = GameObject.FindGameObjectWithTag("Trash");
            Transform projectileStorageT = projectileStorage.GetComponent<Transform>();
            transform.parent = projectileStorageT;
        }
    }

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    //setProjSpeet
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }


    private void Update()
    {
        //transform position
        calculeteMovementDistance();

        //check the collision
        checkCollison(movementDst);

    }


    void checkCollison(float moveDst)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, skinWidth / coef, collisionMask);
        if (hit.collider != null)
        {
            OnHitObject(hit.collider, hit.point);
        }
    }


    private void OnHitObject(Collider2D c, Vector2 hitPoint)
    {
        IDamagable damageableObject = c.GetComponent<IDamagable>();
        if (damageableObject != null)
        {
            Vector2 pushDir = new Vector2(transform.position.x, transform.position.y).normalized;
            Vector2 newPush = new Vector2(pushDir.x * pushEnemy, pushDir.y * pushEnemy);
            TakeHit(hitPoint,newPush,damageableObject);

        }

        GameObject.Destroy(gameObject);
    }

    protected virtual void TakeHit(Vector2 hitPoint,Vector2 push,IDamagable damageObj)
    {
        damageObj.TakeHit(Mage.mageAttackDamage, hitPoint, transform.position, push);
    }

    protected virtual void calculeteMovementDistance()
    {
        transform.Translate(Vector3.right *  Time.deltaTime *  speed);
    }
}

