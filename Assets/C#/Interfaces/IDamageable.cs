using System.Collections;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, Vector3 instigator, Animator instAnim, float dieAnimTime);
    void Die(Animator instAnim, float dieAnimTime);
}
