using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    void OnAttacked(float damage);
    void Damage(float amount);
    void Die();
}
