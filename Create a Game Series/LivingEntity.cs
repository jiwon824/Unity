using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamaneable
{
    public float startingHealth;
    protected float health;
    protected bool dead; // 죽었는지 아닌지 계속 확인

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        // Do some stuff here with hit
        TakeDamage(damage);
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead) // 'health가 0 이하' && '아직 죽지 않았을때' 
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}
