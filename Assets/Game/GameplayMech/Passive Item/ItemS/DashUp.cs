using UnityEngine;

public class DashUp : PassiveItems
{
    protected override void ApplyModifier()
    {
        // Find the Playermovement component on the player GameObject
        Playermovement playerMovement = player.GetComponent<Playermovement>();

        if (playerMovement == null)
        {
            // Also check parent in case PlayerStats is on a child object
            playerMovement = player.GetComponentInParent<Playermovement>();
        }

        if (playerMovement != null)
        {
            // Dash distance = dashSpeed * dashDuration
            // Boost the duration by the item's multiplier percentage to increase distance
            playerMovement.dashDuration *= 1 + passiveItemsData.Multiplier / 100f;

            Debug.Log($"Dash distance boosted! New dash duration: {playerMovement.dashDuration}");
        }
        else
        {
            Debug.LogWarning("DashUp: Could not find Playermovement component on the player.");
        }
    }
}
