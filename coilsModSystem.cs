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
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.ChatCommands.Create("coils")
            .WithDescription("coils test")
            .RequiresPrivilege(Privilege.chat)
            .HandleWith(args => TextCommandResult.Success("hello from coils"));
        
        api.Event.RegisterGameTickListener(dt =>
        {
            api.World.AllOnlinePlayers.Foreach(player =>
            {
                UpdateSpeedCoil(player);
            });
        }, 200);
    }

    public static void UpdateSpeedCoil(IPlayer player)
    {
        var item = player.InventoryManager.ActiveHotbarSlot?.Itemstack?.Collectible;

        if (item is ItemSpeedCoil)
        {
            player.Entity.Stats.Set("walkspeed", "speedcoil", 0.75f);
        } else
        {
            player.Entity.Stats.Remove("walkspeed", "speedcoil");
        }
    }


    public override void StartClientSide(ICoreClientAPI api)
    {
    }

}
