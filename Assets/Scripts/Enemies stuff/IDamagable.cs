using UnityEngine;

public interface IDamagable
{

    void TakeHit(float damage, Vector2 hitPoint, Vector2 hitDirection, Vector2 transformDirection);

    void TakeHit(float damage, Vector2 hitPoint, Vector2 hitDirection);

    void damageTaken(float damage);

}
