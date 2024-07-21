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
        // Dictionnaire pour stocker les icônes chargées
        private Dictionary<string, Sprite> iconDictionary = new Dictionary<string, Sprite>();
        
        //public VisualTreeAsset uxmlVisualTree;
        public UIDocument uiDocument;
        public bool IsOverSomeButton { get; private set; }
    
        public GameRules gameRules;
        
        private List<(GameObject, UnitActionsEnum)> _currentActions = new();
        
        private Button pausePlayButton;
        
        // Start is called before the first frame update
        private void Start()
        {
            Assert.IsNotNull(uiDocument);
            LoadIcons();
            //uiDocument.rootVisualElement
            //uiDocument.rootVisualElement.Clear();
            //uiDocument.rootVisualElement.Add(container);
        }
    
    
        private Button AddButton(UnitActionsEnum action, GameObject[] sObjects)
        {
            var button = new Button() { text = GetText.Get(action) };
            switch (action)
            {
                case UnitActionsEnum.ConstruireCaserne:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(iconDictionary["Caserne"]));
                    break;
                case UnitActionsEnum.ConstruireEntrepot:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(iconDictionary["Entrepot"]));
                    break;
                case UnitActionsEnum.ConvertirEnWarriors:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(iconDictionary["Warrior"]));
                    break;
                case UnitActionsEnum.ConvertirEnShooters:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(iconDictionary["Shooter"]));
                    break;
                case UnitActionsEnum.ConvertirEnHealers:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(iconDictionary["Healer"]));
                    break;
                case UnitActionsEnum.ConvertirEnTanks:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(iconDictionary["Tank"]));
                    break;
                case UnitActionsEnum.PausePlayProduction:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(iconDictionary["PausePlay"]));
                    break;
                case UnitActionsEnum.Demolir:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(iconDictionary["Dead"]));
                    break;
            }
            
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
        
        // Méthode pour charger les icônes
        void LoadIcons()
        {
            // Charge toutes les icônes du dossier spécifié
            Sprite[] icons = Resources.LoadAll<Sprite>( "IconButton");
    
            // Ajoute chaque icône au dictionnaire
            foreach (Sprite icon in icons)
            {
                iconDictionary.Add(icon.name, icon);
            }
        }
        
        private Texture2D SpriteToTexture(Sprite sprite)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                    (int)sprite.textureRect.y,
                    (int)sprite.textureRect.width,
                    (int)sprite.textureRect.height);
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }
            else
            {
                return sprite.texture;
            }
        }
    }
}
