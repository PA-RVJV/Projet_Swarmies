using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.InputHandlers
{
    public class SelectionGroupManager : MonoBehaviour
    {
        private readonly Dictionary<int, List<WeakReference<Transform>>> selectionGroups = new();

        public void CreateGroup(int groupNumber, List<WeakReference<Transform>> selectedUnits)
        {
            if (selectionGroups.TryGetValue(groupNumber, out var group))
            {
                group.Clear();
            }
            else
            {
                selectionGroups[groupNumber] = new List<WeakReference<Transform>>();
            }

            foreach (var unit in selectedUnits)
            {
                if (unit.TryGetTarget(out Transform transform) && transform)
                {
                    selectionGroups[groupNumber].Add(new WeakReference<Transform>(transform));
                }
            }
        }

        public List<WeakReference<Transform>> GetGroup(int groupNumber)
        {
            if (selectionGroups.TryGetValue(groupNumber, out var group))
            {
                return group;
            }
            return null;
        }
    }
}
