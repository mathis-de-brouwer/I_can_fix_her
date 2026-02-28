using UnityEngine;

public class OrbitController : WeaponController
{
    private GameObject _currentOrbit;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();

        // Destroy the old orbit before spawning a new one
        if (_currentOrbit != null)
            Destroy(_currentOrbit);

        _currentOrbit = Instantiate(weaponData.Prefab, transform.position, Quaternion.identity);
    }
}