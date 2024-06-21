using System;
using UnityEngine;

namespace Script.Callbacks
{
    public class ObjectDestroyNotifier : MonoBehaviour
    {
        public event Action<GameObject> OnObjectDestroyed;

        /// <summary>
        ///  Cette methode s'execute automatiquement quand notre gameobject
        ///  est detruit
        /// </summary>
        private void OnDestroy()
        {
            OnObjectDestroyed?.Invoke(gameObject);
        }
    }

}
