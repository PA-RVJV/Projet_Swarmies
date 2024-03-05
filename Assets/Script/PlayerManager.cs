using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PS.InputHandlers;

namespace PS.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;

        public Transform playerUnits;
        void Start()
        {
            instance = this;
        }
        
        void Update()
        {
            InputManager.instance.HandleUnitMovement();
        }
    }
}

