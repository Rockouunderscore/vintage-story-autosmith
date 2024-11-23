
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace AutoSmith
{
    public class AutoSmithModSystem : ModSystem
    {
        private Harmony _harmony;

        public override void Start(ICoreAPI api)
        {
            api.Logger.Notification("Hello from Auto Smith!");
            
            _harmony = new Harmony("autosmith");
            
            _harmony.PatchCategory("BothSide");
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Logger.Notification("Hello from Auto Smith mod server side");
            
            _harmony.PatchCategory("ServerSide");
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            api.Logger.Notification("Hello from Auto Smith mod client side");
            
            _harmony.PatchCategory("ClientSide");
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            _harmony.UnpatchAll();
        }
    }
}
