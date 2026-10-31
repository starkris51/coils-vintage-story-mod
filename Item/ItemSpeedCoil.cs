using System.Collections.Generic;
using Vintagestory.API.Common;

class ItemSpeedCoil : Item
{
    private static readonly AssetLocation[] HumSounds =
    {
        new AssetLocation("game:sounds/effect/translocate-idle"),
        new AssetLocation("game:sounds/effect/tempstab-low"),
        new AssetLocation("game:sounds/block/riftward"),
    };

    // In memory only; Entity.Attributes is saved and ElapsedMilliseconds resets on restart
    private readonly Dictionary<long, long> nextHumByEntity = new();

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
    }

    public override void OnHeldIdle(ItemSlot slot, EntityAgent byEntity)
    {
        base.OnHeldIdle(slot, byEntity);

        if (api.Side != EnumAppSide.Server) return;

        long now = api.World.ElapsedMilliseconds;
        if (nextHumByEntity.TryGetValue(byEntity.EntityId, out long nextHum) && now < nextHum) return;

        var sound = HumSounds[api.World.Rand.Next(HumSounds.Length)];
        api.World.PlaySoundAt(sound, byEntity, null, true, 16, 0.5f);

        nextHumByEntity[byEntity.EntityId] = now + 3000 + api.World.Rand.Next(3000);
    }
}
