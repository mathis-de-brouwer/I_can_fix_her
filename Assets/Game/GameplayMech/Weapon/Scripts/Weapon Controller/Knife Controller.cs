using UnityEngine;

public class KnifeController : WeaponController
{
    [Header("Multi-shot")]
    [SerializeField] private int multiShotLevel = 3;
    [SerializeField] private int multiShotCount = 3;
    [SerializeField] private float multiShotSpreadDegrees = 12f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();

        Vector2 aim = GetAimDirection();
        int level = weaponData != null ? weaponData.Level : 1;

        int count = level >= multiShotLevel ? multiShotCount : 1;
        float spread = count > 1 ? multiShotSpreadDegrees : 0f;

        UiSfx.PlayKnifeThrow();

        for (int i = 0; i < count; i++)
        {
            float t = count == 1 ? 0.5f : (float)i / (count - 1);
            float angleOffset = Mathf.Lerp(-spread * 0.5f, spread * 0.5f, t);

            Vector2 dir = Quaternion.Euler(0f, 0f, angleOffset) * aim;

            GameObject spawnedKnife = Instantiate(weaponData.Prefab);
            spawnedKnife.transform.position = transform.position; // Makes the knife spawn at the same position as the parent layer aka the player
            spawnedKnife.GetComponent<KnifeBehavior>().DirectionChecker(dir); // this reference and set the direction the knife will go
        }
    }
}
