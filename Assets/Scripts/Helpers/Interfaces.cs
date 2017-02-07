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

public interface ITriggerable
{
    void Notify(); // Tells the Triggerable to get ready
    void Activate(); // Activates the Triggerable
    void Deactivate(); // Deactivates the Triggerable
    void Wait(); // Tells the Triggerable to go into standby / wait

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