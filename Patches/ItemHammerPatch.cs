
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace AutoSmith.Patches;

[HarmonyPatch]
[HarmonyPatchCategory("ServerSide")]
public class ItemHammerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemHammer), nameof(ItemHammer.OnLoaded), typeof(ICoreAPI))]
    public static void ItemHammerPatch_OnLoaded_Postfix(ref ItemHammer __instance, ref SkillItem[] ___toolModes, object[] __args)
    {
        
        ICoreAPI api = __args[0] as ICoreAPI;
        
        ICoreClientAPI capi = api as ICoreClientAPI;
        if (capi == null)
            return;
        
        SkillItem[] oldToolModes = ObjectCacheUtil.TryGet<SkillItem[]>(api, "hammerToolModes");
        if (oldToolModes == null)
            return;
        
        api.Logger.Notification("AutoSmith ItemHammerPatch.ItemHammerPatch_OnLoaded_Postfix");
        
        ObjectCacheUtil.Delete(api, "hammerToolModes");
        ___toolModes = ObjectCacheUtil.GetOrCreate<SkillItem[]>(api, "hammerToolModes", () =>
        {
            SkillItem[] newToolModes = new SkillItem[]
            {
                oldToolModes[0],
                oldToolModes[1],
                oldToolModes[2],
                oldToolModes[3],
                oldToolModes[4],
                oldToolModes[5],
                // this might break in the future if other tool modes are added
                // but this postfix method is called for each hammer (i think)
                // so it adds multiple auto modes so making it more future-proof adds like 8 auto modes...
                // if you know of a better way to do this, please let me know
                
                new SkillItem()
                {
                    Code = new AssetLocation("auto"),
                    Name = Lang.Get("Automatic")
                }.WithLetterIcon(capi, "A")
            };

            return newToolModes;
        });

    }
    
}