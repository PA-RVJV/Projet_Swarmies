using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.Units
{
    public class UnitStatTypes : ScriptableObject
    {
        [System.Serializable]
        public class Base
        {
            public float cost, attack, attackRange, aggroRange, health, armor;
        }
    }
}

