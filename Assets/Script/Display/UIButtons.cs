using System;
using System.Collections.Generic;
using System.Linq;
using Script.Systems;
using Unity.Assertions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Script.Display
{
    public class UIButtons : MonoBehaviour
    {
        //public VisualTreeAsset uxmlVisualTree;
        public UIDocument uiDocument;
        public bool IsOverSomeButton { get; private set; }
    
        public GameRules gameRules;
        
        private List<(GameObject, UnitActionsEnum)> _currentActions = new();
        
        // Start is called before the first frame update
        private void Start()
        {
            Assert.IsNotNull(uiDocument);
            
            //uiDocument.rootVisualElement
            //uiDocument.rootVisualElement.Clear();
            //uiDocument.rootVisualElement.Add(container);
        }
    
    
        private Button AddButton(UnitActionsEnum action, GameObject[] sObjects)
        {
            var button = new Button() { text = GetText.Get(action) };
            button.AddToClassList("swarmies-button");
            button.RegisterCallback<MouseEnterEvent>(_OnUI);
            button.RegisterCallback<MouseLeaveEvent>(_OutUI);
            button.RegisterCallback<DetachFromPanelEvent>(_DestroyButton);
            
            button.clickable.clicked += () => ButtonOnclicked(action, sObjects);
            
            return button;
        }
    
        private void _OnUI(MouseEnterEvent _)
        {
            IsOverSomeButton = true;
        }
        private void _OutUI(MouseLeaveEvent _)
        {
            IsOverSomeButton = false;
        }

        private void _DestroyButton(DetachFromPanelEvent _)
        {
            IsOverSomeButton = false;
        }
    
        private void ButtonOnclicked(UnitActionsEnum action, GameObject[] sObjects)
        {
            gameRules.DealWithAction(action, sObjects);
        }
    
        public void SetButtons(List<(GameObject, UnitActionsEnum)> actions)
        {
            if (_currentActions.SequenceEqual(actions))
                return;
            
            var root = uiDocument.rootVisualElement;
            var container = root.Q<VisualElement>("SwarmiesActions");
            
            Assert.IsNotNull(container);
            
            container.Clear();
            foreach (var action in actions)
            {
                container.Add(AddButton(action.Item2, new []{action.Item1}));
            }
    
            _currentActions = actions;
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 300, 20), IsOverSomeButton.ToString());
        }
    }
}
