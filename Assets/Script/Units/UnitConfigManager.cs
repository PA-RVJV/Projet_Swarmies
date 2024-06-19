using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Units
{
    public class UnitConfigManager : MonoBehaviour
    {
        public List<UnitConfig> unitConfigs;

        private Dictionary<string, UnitConfig> configDictionary;

        void Awake()
        {
            configDictionary = new Dictionary<string, UnitConfig>();
            foreach (var config in unitConfigs)
            {
                configDictionary[config.unitType] = config;
            }
        }

        public UnitConfig GetConfig(string unitType)
        {
            if (configDictionary.TryGetValue(unitType, out var config))
            {
                return config;
            }
            return null;
        }
    } 
}

