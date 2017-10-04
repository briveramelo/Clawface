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

    void StopVerticalMovement();
}

public interface IStunnable
{
    void Stun();
}

public interface IDamageable
{
    void TakeDamage(Damager damager);
    float GetHealth();
}

public interface IModifiable
{
    void Multiply(CharacterStatType statType, float statMultiplier);
    void Add(CharacterStatType statType, int statAddend);
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

public interface ISpawnable {
    bool HasWillBeenWritten();
    void RegisterDeathEvent(OnDeath onDeath);
    void WarpToNavMesh(Vector3  position);
}



public delegate void OnDeath();