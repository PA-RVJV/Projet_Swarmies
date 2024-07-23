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
        private Button _pausePlayButton;
        // Dictionnaire pour stocker les icônes chargées
        private readonly Dictionary<string, Sprite> _iconDictionary = new();
        
        
        // Tooltips
        public string tooltipText = "This is a tooltip";
        private Label _tooltipLabel;

        // Start is called before the first frame update
        private void Start()
        {
            Assert.IsNotNull(uiDocument);
            LoadIcons();
            CreateToolTip();
            //uiDocument.rootVisualElement
            //uiDocument.rootVisualElement.Clear();
            //uiDocument.rootVisualElement.Add(container);
        }
    
    
        private Button AddButton(UnitActionsEnum action, GameObject[] sObjects)
        {
            var button = new Button() { /*text = GetText.Get(action)*/ };
            switch (action)
            {
                case UnitActionsEnum.ConstruireCaserne:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(_iconDictionary["Caserne"]));
                    break;
                case UnitActionsEnum.ConstruireEntrepot:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(_iconDictionary["Entrepot"]));
                    break;
                case UnitActionsEnum.ConvertirEnWarriors:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(_iconDictionary["Warrior"]));
                    break;
                case UnitActionsEnum.ConvertirEnShooters:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(_iconDictionary["Shooter"]));
                    break;
                case UnitActionsEnum.ConvertirEnHealers:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(_iconDictionary["Healer"]));
                    break;
                case UnitActionsEnum.ConvertirEnTanks:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(_iconDictionary["Tank"]));
                    break;
                case UnitActionsEnum.PausePlayProduction:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(_iconDictionary["PausePlay"]));
                    break;
                case UnitActionsEnum.Demolir:
                    button.style.backgroundImage = new StyleBackground(SpriteToTexture(_iconDictionary["Dead"]));
                    break;
            }
            
            button.AddToClassList("swarmies-button");
            button.RegisterCallback<MouseEnterEvent>((_) => _OnUI(_, action));
            button.RegisterCallback<MouseLeaveEvent>(_OutUI);
            button.RegisterCallback<DetachFromPanelEvent>(_DestroyButton);
            button.RegisterCallback<MouseMoveEvent>(MoveTooltip);
            
            button.clickable.clicked += () => ButtonOnclicked(action, sObjects);
            
            return button;
        }
    
        private void _OnUI(MouseEnterEvent _, UnitActionsEnum action)
        {
            IsOverSomeButton = true;
            ShowTooltip(_, GetText.Get(action));
        }
        private void _OutUI(MouseLeaveEvent _)
        {
            IsOverSomeButton = false;
            HideTooltip(_);
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
                _iconDictionary.Add(icon.name, icon);
            }
        }
        
        private Texture2D SpriteToTexture(Sprite sprite)
        {
            if (!Mathf.Approximately(sprite.rect.width, sprite.texture.width))
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

        /// <summary>
        /// Gestion du tooltip sur les boutons
        /// </summary>
        private void CreateToolTip()
        {
            // Get the root of the UI
            var root = uiDocument.rootVisualElement;

            // Create the tooltip label
            _tooltipLabel = new Label
            {
                style =
                {
                    position = Position.Absolute,
                    visibility = Visibility.Hidden,
                }
            };
            _tooltipLabel.AddToClassList("tooltip");
            root.Add(_tooltipLabel);
        }
        
        private void ShowTooltip(MouseEnterEvent evt, string text)
        {
            _tooltipLabel.text = text;
            _tooltipLabel.style.visibility = Visibility.Visible;
        }

        private void HideTooltip(MouseLeaveEvent evt)
        {
            _tooltipLabel.style.visibility = Visibility.Hidden;
        }

        private void MoveTooltip(MouseMoveEvent evt)
        {
            _tooltipLabel.style.left = evt.mousePosition.x + 10;
            _tooltipLabel.style.top = evt.mousePosition.y + 10;
        }
    }
}
