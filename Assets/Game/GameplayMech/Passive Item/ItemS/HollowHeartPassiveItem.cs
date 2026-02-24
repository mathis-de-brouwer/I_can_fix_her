public class HollowHeartPassiveItem : PassiveItems
{
    protected override void ApplyModifier()
    {
        player.currentHealth *= 1 + passiveItemsData.Multiplier / 100f;
    }
}