using Vintagestory.API.Common;

class ItemRegenCoil : Item
{
    // Played once by CoilsModSystem.UpdateSpeedCoil when the coil becomes active
    public static readonly AssetLocation EquipSound = new("coils:sounds/gravitycoil");

    // Health restored per 200ms tick while held
    public const float HealPerTick = 0.1f;
}
