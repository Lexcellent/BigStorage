using System;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using System.IO;
using ItemStatsSystem;

namespace BigStorage
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private bool _isInit = false;
        private Harmony? _harmony = null;
        public static int StorageCapacityIncrease { get; private set; } = 300;

        protected override void OnAfterSetup()
        {
            Debug.Log("BigStorage模组：OnAfterSetup方法被调用");
            if (!_isInit)
            {
                LoadConfig();
                Debug.Log("BigStorage模组：执行修补");
                _harmony = new Harmony("Lexcellent.BigStorage");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                _isInit = true;
                Debug.Log("BigStorage模组：修补完成");
            }
        }

        protected override void OnBeforeDeactivate()
        {
            Debug.Log("BigStorage模组：OnBeforeDeactivate方法被调用");
            if (_isInit)
            {
                Debug.Log("BigStorage模组：执行取消修补");
                if (_harmony != null)
                {
                    _harmony.UnpatchAll();
                }
                Debug.Log("BigStorage模组：执行取消修补完毕");
            }
        }

        private void LoadConfig()
        {
            try
            {
                string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "info.ini");
                if (File.Exists(configPath))
                {
                    string[] lines = File.ReadAllLines(configPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("StorageCapacity="))
                        {
                            string value = line.Substring("StorageCapacity=".Length).Trim();
                            if (int.TryParse(value, out int capacity))
                            {
                                StorageCapacityIncrease = capacity;
                                Debug.Log($"BigStorage模组：已从配置文件读取StorageCapacity值: {capacity}");
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("BigStorage模组：未找到info.ini文件，使用默认值");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"BigStorage模组：读取配置文件时出错：{e.Message}，使用默认值");
            }
        }
    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Capacity), MethodType.Getter)]
    public static class InventoryCapacityPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Inventory __instance, ref int __result)
        {
            try
            {
                if (__instance != null && __instance.DisplayNameKey == "UI_Inventory_Storage")
                {
                    Debug.Log($"BigStorage模组：容器名称:{__instance.DisplayName},原始容器容量：{__result}");
                    __result += ModBehaviour.StorageCapacityIncrease;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"BigStorage模组：错误：{e.Message}");
            }
        }
    }
}