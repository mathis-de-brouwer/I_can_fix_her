using UnityEngine;

public class SpinachPassiveItem : PassiveItems
{
    protected override void ApplyModifier()
    {
        player.currentMight *= 1 + passiveItemsData.Multiplier / 100f; 
    }
}
