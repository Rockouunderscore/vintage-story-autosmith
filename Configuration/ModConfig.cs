
using Vintagestory.API.Common;

namespace AutoSmith.Configuration;

public static class ModConfig
{
    public static AutoSmithConfig ReadConfig(ICoreAPI api)
    {
        try
        {
            AutoSmithConfig config = LoadConfig(api);
            
            if (config == null)
            {
                api.Logger.Debug("Generating configuration for Auto Smith..");
                
                SaveOrCreateConfig(api);
                
                config = LoadConfig(api);
            }
            SaveOrCreateConfig(api, config);
            
            return config;
        }
        catch
        {
            SaveOrCreateConfig(api);
            
            return LoadConfig(api);
        }
    }
    private static AutoSmithConfig LoadConfig(ICoreAPI api)
    {
        return api.LoadModConfig<AutoSmithConfig>(ModConstants.ConfigFileName);
    }
    private static void SaveOrCreateConfig(ICoreAPI api, AutoSmithConfig config = default)
    {
        api.StoreModConfig(config ?? new AutoSmithConfig(), ModConstants.ConfigFileName);
    }
}