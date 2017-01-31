using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    void AddExternalForce(Vector3 force);
}

public interface IStunnable
{
    void Stun();
}

public interface IDamageable
{
    void TakeDamage(int damage);
}

public interface IModifiable
{
    void Modify(StatType statType, float multiplier);
    void Modify(StatType statType, int addend);
}

public interface IUnlockable
{
    void Unlock();

}

public interface ICollectable
{
    void Collect();
}