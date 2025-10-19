using System;
using System.Reflection;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace BigStorage
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public static int StorageCapacityIncrease { get; private set; } = 300;

        protected override void OnAfterSetup()
        {
            Debug.Log("BigStorage模组：OnAfterSetup方法被调用");
            LoadConfig();
        }

        void OnEnable()
        {
            TryHookStorage();
        }

        void OnDisable()
        {
            UnhookStorage();
        }

        private void TryHookStorage()
        {
            PlayerStorage.OnRecalculateStorageCapacity -= OnRecalculateStorageCapacity;
            PlayerStorage.OnRecalculateStorageCapacity += OnRecalculateStorageCapacity;
            Debug.Log("BigStorage模组：成功挂钩 OnRecalculateStorageCapacity 事件");
        }

        private void UnhookStorage()
        {
            PlayerStorage.OnRecalculateStorageCapacity -= OnRecalculateStorageCapacity;
            Debug.Log("BigStorage模组：取消挂钩 OnRecalculateStorageCapacity 事件");
        }

        private void OnRecalculateStorageCapacity(PlayerStorage.StorageCapacityCalculationHolder calculationHolder)
        {
            Debug.Log($"仓库默认容量：{PlayerStorage.Instance.DefaultCapacity}，holder容量：{calculationHolder.capacity}");
            calculationHolder.capacity += StorageCapacityIncrease;
        }

        private void LoadConfig()
        {
            try
            {
                string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "info.ini");
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
}