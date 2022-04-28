using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamagable
{
    [Header("Health Settings")]
    public float startingHealth;
    protected bool dead;
    [HideInInspector] public float _health;

    int damageStateColor = 0;

    [Header("Damage coloring system")]
    public bool isChangingColor;
    public SpriteRenderer sprRenderer;
    public bool isSummoned = false;

    //очень плохая реализация изменение цвета при получение урона
    public Color colorToChange = new Color(Color.red.r * 0.8f,Color.red.g ,0.2f,Color.red.a-0.1f);

    public event System.Action onDeath;

    private Color initialColor;

    private float coefToLerp = 0;
    private float speedChangeColor = 5f;

    protected virtual void Start()
    {
        if (!isSummoned || _health == 0)
        {
            _health = startingHealth;
        }

        if (isChangingColor == true)
        { 
            initialColor = sprRenderer.material.color;
        }
    }

    public virtual void damageTaken(float damage)
    {
        print(_health);
        _health -= damage;
        if (isChangingColor == true && damageStateColor == 0)
        {
            coefToLerp = 0;
            StartCoroutine(changeColorDamageTakenIN());
        }
        if (_health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void TakeHit(float damage, Vector2 hitPoint, Vector2 hitDirection, Vector2 transformDirection)
    {
        damageTaken(damage);
        transform.position += new Vector3(transformDirection.x, transformDirection.y, 0);
    }

    public virtual void TakeHit(float damage, Vector2 hitPoint, Vector2 hitDirection)
    {
        damageTaken(damage);
    }



    [ContextMenu("Self Destruct")]
    protected virtual void Die()
    {
        dead = true;
        if (onDeath != null)
        {
            onDeath();
        }
        GameObject.Destroy(gameObject);
    }

    IEnumerator changeColorDamageTakenIN()
    {
        while (coefToLerp <= 1 && damageStateColor == 0)
        {
            coefToLerp += speedChangeColor * Time.deltaTime;
            sprRenderer.material.color = Color.Lerp(initialColor, colorToChange, coefToLerp);
            yield return null;
            if(coefToLerp >= 1)
            {
                damageStateColor = 1;
            }
        }
        while (coefToLerp > 0 && damageStateColor == 1)
        {
            coefToLerp -= speedChangeColor * Time.deltaTime * 2;
            coefToLerp = Mathf.Clamp01(coefToLerp);
            sprRenderer.material.color =  Color.Lerp(Color.white, colorToChange, coefToLerp);
            yield return null;
            if (coefToLerp <= 0)
            {
                damageStateColor = 2;
            }
        }
        if(damageStateColor == 2)
        {
            StopCoroutine(changeColorDamageTakenIN());
            damageStateColor = 0;
        }
    }
    
}
