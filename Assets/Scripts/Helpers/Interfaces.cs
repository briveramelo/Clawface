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
    void TakeDamage(float damage);
}

public interface IModifiable
{
    void Modify(StatType statType, float statMultiplier);
    void Modify(StatType statType, int statAddend);
}

public interface IUnlockable
{
    void Unlock();

}

public interface ICollectable{
    GameObject Collect();
}

public interface ISkinnable
{
    bool IsSkinnable();
    GameObject DeSkin();    
}

public interface ICodexLoggable {
    CodexType GetCodexType();
}