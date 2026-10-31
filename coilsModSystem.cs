using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace coils;

public class CoilsModSystem : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.Logger.Notification("coils loaded on " + api.Side);
        api.RegisterItemClass("coils.ItemSpeedCoil", typeof(ItemSpeedCoil));
        api.RegisterItemClass("coils.ItemGravityCoil", typeof(ItemGravityCoil));
        api.RegisterItemClass("coils.ItemRegenCoil", typeof(ItemRegenCoil));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.ChatCommands.Create("coils")
            .WithDescription("coils test")
            .RequiresPrivilege(Privilege.chat)
            .HandleWith(args => TextCommandResult.Success("Hello welcome to my coil mod, use the coils!!"));
        
        api.Event.RegisterGameTickListener(dt =>
        {
            api.World.AllOnlinePlayers.Foreach(player =>
            {
                UpdateSpeedCoil(player);
            });
        }, 200);

        api.Event.PlayerDisconnect += player => regenCoilPlayers.Remove(player.PlayerUID);
    }

    // Regen has no stat modifier to check, so track who has it active for the equip sound
    private static readonly HashSet<string> regenCoilPlayers = new();

    public static void UpdateSpeedCoil(IPlayer player)
    {
        var item = player.InventoryManager.ActiveHotbarSlot?.Itemstack?.Collectible;
        bool wasActiveSpeedCoil = player.Entity.Stats["walkspeed"].ValuesByKey.ContainsKey("speedcoil");
        bool wasActiveGravityCoil = player.Entity.Stats["jumpHeightMul"].ValuesByKey.ContainsKey("gravitycoil");

        bool wasActiveRegenCoil = regenCoilPlayers.Contains(player.PlayerUID);

        if (item is ItemSpeedCoil)
        {
            if (!wasActiveSpeedCoil)
            {
                player.Entity.Stats.Set("walkspeed", "speedcoil", 1f);
                player.Entity.World.PlaySoundAt(ItemSpeedCoil.EquipSound, player.Entity, null, false, 16, 1f);
            }
        } else if (wasActiveSpeedCoil)
        {
            player.Entity.Stats.Remove("walkspeed", "speedcoil");
        }

        if (item is ItemGravityCoil)
        {
            if (!wasActiveGravityCoil)
            {
                player.Entity.Stats.Set("jumpHeightMul", "gravitycoil", 2f);
                player.Entity.World.PlaySoundAt(ItemGravityCoil.EquipSound, player.Entity, null, false, 16, 1f);
            }
        } else if (wasActiveGravityCoil)
        {
            player.Entity.Stats.Remove("jumpHeightMul", "gravitycoil");
        }

        if (item is ItemRegenCoil)
        {
            if (!wasActiveRegenCoil)
            {
                regenCoilPlayers.Add(player.PlayerUID);
                player.Entity.World.PlaySoundAt(ItemRegenCoil.EquipSound, player.Entity, null, false, 16, 1f);
            }

            player.Entity.ReceiveDamage(new DamageSource
            {
                Source = EnumDamageSource.Internal,
                Type = EnumDamageType.Heal
            }, ItemRegenCoil.HealPerTick);
        } else if (wasActiveRegenCoil)
        {
            regenCoilPlayers.Remove(player.PlayerUID);
        }
    }


    public override void StartClientSide(ICoreClientAPI api)
    {
    }

}
