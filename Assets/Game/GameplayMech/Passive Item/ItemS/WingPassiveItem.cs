using UnityEngine;

public class WingPassiveItem : PassiveItems
{
    protected override void ApplyModifier()
    {
        player.currentMovementSpeed *= 1 + passiveItemsData.Multiplier / 100f; //this formula multiplies movementsSpeed of the player based of the value of the multiplier 
    }
}
