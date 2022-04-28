using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : baseProjectile
{

    //const
    float spaceFallingSpeed = 10;

    private float shootAngle;
    private float timeToFlight;
    private float rangeToTarget;
    private float perpendicularAngle;

    private Vector2 movementVector;
    private Vector2 targetPos;
    private Vector2 directionVect;
    private Vector2 spaceFallSpeedDir;

    protected override void Start()
    {
        base.Start();
        timeToFlight = (2 * speed * Mathf.Sin(shootAngle * Mathf.Deg2Rad)) / spaceFallingSpeed;


        //transform rotation;
        float corner;
        corner = (Mathf.Atan2(transform.position.y - targetPos.y, transform.position.x - targetPos.x) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Euler(0, 0, corner);
        directionVect = new Vector2(movementVector.x * speed , movementVector.y * speed );

    }

    public void infoAboutTarget(float _rangeToTarget, Vector2 _movementVector, Vector2 _targetPos, float _shootAngle,float _speed)
    {
        rangeToTarget = _rangeToTarget;
        movementVector = _movementVector;
        targetPos = _targetPos;
        shootAngle = _shootAngle;
        speed = _speed;
    }

    protected override void calculeteMovementDistance()
    {
        transform.position += new Vector3(directionVect.x * Time.deltaTime,directionVect.y * Time.deltaTime ,0);
    }
}
