using System;
using UnityEngine;

public class KnifeBehavior : ProjectileWeaponBehavior
{
    KnifeController kc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        kc = FindAnyObjectByType<KnifeController>();
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += direction * kc.speed * Time.deltaTime;  // Set the movement of the knife 
        
    }
}
