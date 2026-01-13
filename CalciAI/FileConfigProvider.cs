using CalciAI.Models;
using System.Collections.Generic;
using System.IO;

namespace CalciAI
{
    public static class FileConfigProvider
    {
        private static readonly Dictionary<string, object> CachedConfig = new();
        private static readonly object SyncObj = new();

        public static bool Exists(string name)
        {
            var fileName = $"{name}{(CommonUtils.EnvironmentName == EnvironmentNames.Development ? ".dev" : "")}";

            var file = CommonUtils.ConfigFolder + fileName + ".json";

            return File.Exists(file);
        }

        public static T Load<T>(string name) where T : ISetting
        {
            if (CachedConfig.ContainsKey(name))
            {
                return (T)CachedConfig[name];
            }

            lock (SyncObj)
            {
                if (CachedConfig.ContainsKey(name))
                {
                    return (T)CachedConfig[name];
                }

                var fileName = $"{name}{(CommonUtils.EnvironmentName == EnvironmentNames.Development ? ".dev" : "")}";

                var file = CommonUtils.ConfigFolder + fileName + ".json";
                if (!File.Exists(file))
                {
                    throw new FileNotFoundException("File not found", fileName);
                }

                var obj = System.Text.Json.JsonSerializer.Deserialize<T>(File.ReadAllText(file));
                CachedConfig.TryAdd(name, obj);
                return obj;
            }
        }

        public static void Update<T>(T value) where T : ISetting
        {
            Update(typeof(T).Name, value);
        }

        public static void Update<T>(string name, T value) where T : ISetting
        {
            var settingName = $"{name}{(CommonUtils.EnvironmentName == EnvironmentNames.Development ? ".dev" : "")}";

            var file = CommonUtils.ConfigFolder + settingName + ".json";
            lock (SyncObj)
            {
                File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(value));

                if (CachedConfig.ContainsKey(name))
                {
                    CachedConfig[name] = value;
                }
                else
                {
                    CachedConfig.Add(name, value);
                }
            }
        }
    }
}
