using System.Collections.Generic;
using UnityEngine;

public class OrbitController : WeaponController
{
    [Header("Orbit scaling")]
    [SerializeField] private int orbitCountPerLevel = 1;

    private readonly List<GameObject> _orbits = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        RefreshOrbitCount();
    }

    protected override void Update()
    {
        base.Update();
        RefreshOrbitCount();
    }

    protected override void Attack()
    {
        base.Attack();
    }

    private void RefreshOrbitCount()
    {
        if (weaponData == null || weaponData.Prefab == null)
            return;

        int level = Mathf.Max(1, weaponData.Level);
        int perLevel = Mathf.Max(1, orbitCountPerLevel);
        int desiredCount = level * perLevel;

        while (_orbits.Count < desiredCount)
        {
            GameObject orbit = Instantiate(weaponData.Prefab, transform.position, Quaternion.identity);
            _orbits.Add(orbit);

            UiSfx.PlayOrbitSpawn();
        }

        while (_orbits.Count > desiredCount)
        {
            int lastIndex = _orbits.Count - 1;
            GameObject orbit = _orbits[lastIndex];
            _orbits.RemoveAt(lastIndex);

            if (orbit != null)
                Destroy(orbit);
        }

        ApplyEvenSpacing();
    }

    private void ApplyEvenSpacing()
    {
        int total = _orbits.Count;
        for (int i = 0; i < total; i++)
        {
            GameObject orbitObj = _orbits[i];
            if (orbitObj == null)
                continue;

            if (orbitObj.TryGetComponent(out OrbitBehavior orbitBehavior))
                orbitBehavior.InitializeOrbit(i, total);
        }
    }
}