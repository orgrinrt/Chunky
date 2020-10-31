using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace Chunky.Shared
{
    public class Chunky
    {
        public static GlobalConfig Config = new GlobalConfig(); // TODO: Serialize/deserialize from disk
        public static string GlobalConfigPath;
        public static string SavedConfigsPath;

        static Chunky()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            
            GlobalConfigPath = Path.Combine(appDataPath, "Chunky", "global.conf");
            SavedConfigsPath = Path.Combine(appDataPath, "Configs");
        }

        public static ChunkyConfig32bit LoadConfig(string pathToConfig, bool treatAsName = false)
        {
            ChunkyConfig32bit result = new ChunkyConfig32bit();

            if (treatAsName) pathToConfig = Path.Combine(SavedConfigsPath, pathToConfig);
            
            try
            {
                result = FileHelper.DeserializeFromJson<ChunkyConfig32bit>(pathToConfig);
            }
            catch (Exception e)
            { 
                Print.Line(e);
                Print.Line(ConsoleColor.Yellow, "Returning a default config as a fallback.");
            }

            return result;
        }

        public static void SaveConfig(string pathToConfig, bool treatAsName = false)
        {
            if (string.IsNullOrEmpty(pathToConfig)) pathToConfig = GlobalConfigPath;

            if (treatAsName) pathToConfig = Path.Combine(SavedConfigsPath, pathToConfig);
            
            FileHelper.SerializeAsJson(pathToConfig, Config);
        }
        
        public static void LoadGlobalConfig(string pathToConfig = null)
        {
            if (string.IsNullOrEmpty(pathToConfig)) pathToConfig = GlobalConfigPath;
            
            try
            {
                FileHelper.DeserializeFromJson<GlobalConfig>(pathToConfig);
            }
            catch (Exception e)
            { 
                Print.Line(e);
                Print.Line(ConsoleColor.Yellow, "No changes made to current config.");
            }
        }

        public static void SaveGlobalConfig(string pathToConfig = null)
        {
            if (string.IsNullOrEmpty(pathToConfig)) pathToConfig = GlobalConfigPath;
            
            FileHelper.SerializeAsJson(pathToConfig, Config);
        }

        public static void ResetGlobalConfig()
        {
            Config = new GlobalConfig();
        }
    }
}