
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;



namespace AutoSmith.Patches;

[HarmonyPatch]
[HarmonyPatchCategory("ServerSide")]
public class BlockEntityAnvilPatch
{

    public static int SMITH_GRID_MAX_X = 16;
    public static int SMITH_GRID_MAX_Y = 6;
    public static int SMITH_GRID_MAX_Z = 16;
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockEntityAnvil), "OnUseOver", typeof(IPlayer), typeof(Vec3i), typeof(BlockSelection))]
    public static void BlockEntityAnvil_OnUseOver_Postfix(ref BlockEntityAnvil __instance, object[] __args)
    {
        __instance.Api.Logger.Notification("AutoSmith BlockEntityAnvilPatch.BlockEntityAnvil_OnUseOver_Postfix");

        IPlayer byPlayer = (IPlayer)__args[0];
        Vec3i voxelPos = (Vec3i)__args[1];
        BlockSelection blockSel = (BlockSelection)__args[2];

        if (byPlayer is null || blockSel is null)
            return;

        if (__instance.recipeVoxels == null)
            return;
        
        if (!__instance.CanWorkCurrent) 
            return;

        ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;
        if (slot.Itemstack == null)
            return;

        int toolMode = slot.Itemstack.Collectible.GetToolMode(slot, byPlayer, blockSel);

        if (toolMode != 6)
            return;

        AutoSmith(ref __instance);
        
        RegenMeshAndSelectionBoxes(__instance);
        __instance.Api.World.BlockAccessor.MarkBlockDirty(__instance.Pos);
        __instance.Api.World.BlockAccessor.MarkBlockEntityDirty(__instance.Pos);
        //slot.Itemstack.Collectible.DamageItem(__instance.Api.World, byPlayer.Entity, slot); // vanilla version will already handle this
        if (!HasAnyMetalVoxel(__instance))
        {
            clearWorkSpace(__instance);
            return;
        }
        __instance.CheckIfFinished(byPlayer);
        __instance.MarkDirty();
    }

    public static void AutoSmith(ref BlockEntityAnvil __instance)
    {
        
        // remove slag
        for (int y = SMITH_GRID_MAX_Y - 1; y >= 0; y--)
        for (int x = 0; x < SMITH_GRID_MAX_X; x++)
        for (int z = 0; z < SMITH_GRID_MAX_Z; z++)
        {
            if (__instance.Voxels[x, y, z] == (byte)EnumVoxelMaterial.Slag)
            {
                __instance.Voxels[x, y, z] = (byte)EnumVoxelMaterial.Empty;
                return; // only 1 action at a time
            }
        }

        for (int y = SMITH_GRID_MAX_Y - 1; y >= 0; y--)
        for (int x = 0; x < SMITH_GRID_MAX_X; x++)
        for (int z = 0; z < SMITH_GRID_MAX_Z; z++)
        {
            // find a metal voxel where we dont need one
            if (__instance.Voxels[x, y, z] == (byte)EnumVoxelMaterial.Metal && !__instance.recipeVoxels[x, y, z])
            {
                // listen im cringing too
                Vec3i needFillPos = null;
                for (int yf = SMITH_GRID_MAX_Y - 1; yf >= 0; yf--)
                for (int xf = 0; xf < SMITH_GRID_MAX_X; xf++)
                for (int zf = 0; zf < SMITH_GRID_MAX_Z; zf++)
                {
                    if (__instance.Voxels[xf, yf, zf] == (byte)EnumVoxelMaterial.Empty && __instance.recipeVoxels[xf, yf, zf])
                    {
                        needFillPos = new Vec3i(xf, yf, zf);
                    }
                }
                // fill if we did find somewhere we need to fill
                if (needFillPos != null)
                {
                    __instance.Voxels[needFillPos.X, needFillPos.Y, needFillPos.Z] = (byte)EnumVoxelMaterial.Metal;
                }
                // destroy where we took the voxel / or just plain destroy it
                __instance.Voxels[x, y, z] = (byte)EnumVoxelMaterial.Empty;
                return; // only 1 action at a time
            }
        }

    }

    #region Access to BlockEntityAnvil's private method

    [HarmonyReversePatch]
    [HarmonyPatch(typeof(BlockEntityAnvil), "RegenMeshAndSelectionBoxes")]
    public static void RegenMeshAndSelectionBoxes(object instance)
    {
        
    }
    
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(BlockEntityAnvil), "HasAnyMetalVoxel")]
    public static bool HasAnyMetalVoxel(object instance)
    {
        return true; // better to need to add an ingot than deleting work
    }

    [HarmonyReversePatch]
    [HarmonyPatch(typeof(BlockEntityAnvil), "clearWorkSpace")]
    public static void clearWorkSpace(object instance)
    {
        
    }
    
    #endregion

}