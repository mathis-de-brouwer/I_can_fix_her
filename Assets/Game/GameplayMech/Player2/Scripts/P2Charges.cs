using UnityEngine;

public class P2Charges : MonoBehaviour
{
    public float maxCharges = 10;
    public float currentCharges = 10;
    public float regenRate = 1f;

    void Update()
    {
        currentCharges = Mathf.Min(maxCharges, currentCharges + regenRate * Time.deltaTime);
    }

    public bool CanSpend(float cost)
    {
        return currentCharges >= cost;
    }

    public void Spend(float cost)
    {
        currentCharges -= cost;
    }
}
