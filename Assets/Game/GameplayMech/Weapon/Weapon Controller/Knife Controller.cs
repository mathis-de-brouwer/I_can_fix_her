using UnityEngine;

public class KnifeController : WeaponController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedKnife = Instantiate(prefab);
        spawnedKnife.transform.position = transform.position; // Makes the knife spawn at the same position as the parent layer aka the player
        spawnedKnife.GetComponent<KnifeBehavior>().DirectionChecker(pm.lastMoveDirection); // this reference and set the direction the knife will go
    }

}
