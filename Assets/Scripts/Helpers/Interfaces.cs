using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    void AddDecayingForce(Vector3 force, float decay=0.1f);
    bool IsGrounded();
    void SetMovementMode(MovementMode mode);
    Vector3 GetForward();
    Quaternion GetRotation();
}

public interface IStunnable
{
    void Stun();
}

public interface IDamageable
{
    void TakeDamage(float damage);
    float GetHealth();
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

public interface ISpawnable {
    bool HasWillBeenWritten();
    void RegisterDeathEvent(OnDeath onDeath);
}
public delegate void OnDeath();