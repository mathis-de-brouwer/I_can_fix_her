using System;
using UnityEngine;

public class KnifeBehavior : ProjectileWeaponBehavior
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
  
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += direction * weaponData.Speed * Time.deltaTime;  // Set the movement of the knife 
        
    }
}
