using UnityEngine;
public interface IWeaponTargetable
{
    void TakeDamage(int damage);
    Transform Transform();
}