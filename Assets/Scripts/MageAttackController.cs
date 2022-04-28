using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAttackController : MonoBehaviour
{

    float nextShootTime;

    public float msBetweenShoot;

    public baseProjectile m_b_a_projectile; //mage base attack projectile
    public Transform m_b_a_ProjectileSpawnTransform; // mage base attack projectile spawn pos

    void start()
    {
        //shoot cooldown
        nextShootTime = Time.time;
    }

    void Update()
    {
        //shoot input(temporarily)
        if (Input.GetMouseButton(0))            
        {
            shoot();
        }

    }

    public void shoot()
    {

        if (Time.time > nextShootTime)
        {

            //mage wand muzzle rotation where mouse position
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_b_a_ProjectileSpawnTransform.position;

            float muzzleAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            m_b_a_ProjectileSpawnTransform.rotation = Quaternion.Euler(m_b_a_ProjectileSpawnTransform.rotation.x, m_b_a_ProjectileSpawnTransform.rotation.y, muzzleAngle);

            //Attack    
            nextShootTime = Time.time + msBetweenShoot / 1000;
            baseProjectile newbaseProjectile = Instantiate(m_b_a_projectile, m_b_a_ProjectileSpawnTransform.position, m_b_a_ProjectileSpawnTransform.rotation) as baseProjectile;
            
        }
    }

}
