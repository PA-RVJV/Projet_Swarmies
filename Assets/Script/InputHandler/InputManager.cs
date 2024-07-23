using System.Collections.Generic;
using UnityEngine;
using PS.Units.Player;
using System;
using PS.Player;
using Script;
using Script.Display;
using Script.Systems;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace PS.InputHandlers
{
    public class InputManager : MonoBehaviour
    {
        public PlayerManager playerManager;
        public readonly List<WeakReference<Transform>> SelectedUnits = new(); // Liste des unités sélectionnées.
        public UIButtons uiButtons;
        public LayerMask layerMask;
        public GameRules GameRules;
        public SelectionGroupManager selectionGroupManager; // Référence au gestionnaire de groupes de sélection
        
        private RaycastHit _hit; // stocke l'information du raycast.
        private bool _isDragging = false; // Booléen de vérification sélection multiple en cour ou non.
        private Vector3 _mousePos; // Position initiale de la souris lors du début de select.
        private Camera _cam;
        private CameraController _camController;
        private UnityEngine.UI.GraphicRaycaster _graphicRaycaster;
        private EventSystem _eventSystem;

        private float doubleClickTime = .2f;
        private float lastClickTime; 
        Transform unit;

        private void Awake()
        {
            _cam = Camera.main;
            _graphicRaycaster = FindObjectOfType<UnityEngine.UI.GraphicRaycaster>();
            _eventSystem = FindObjectOfType<EventSystem>();
        }

        private void Start()
        {
            _camController = _cam.GetComponent<CameraController>();
            selectionGroupManager = GetComponent<SelectionGroupManager>();
        }
        
        // Dessine le rectangle de sélection sur l'interface a l'aide de la classe MultiSelect
        private void OnGUI()
        {
            if (_isDragging)
            {
                // Crée et dessine le rectangle de sélection.un rectangle de sélection à partir de la position
                // initiale de la souris et la position actuelle.
                Rect rect = MultiSelect.GetScreenRect(_mousePos, Input.mousePosition);
                MultiSelect.DrawScreenRect(rect, new Color(0f, 0f, 0f, 0.25f));
                
                // Dessine la bordure du rectangle de sélection.
                MultiSelect.DrawScreenRectBorder(rect, 3, Color.blue); 
            }
            
            // Capture des événements clavier pour les combinaisons de touches
            Event e = Event.current;
            if (e != null && e.isKey)
            {
                HandleSelectionGroups(e);
            }
        }
        
        void Update()
        {
            if (SelectedUnits.Count >= 1)
            {
                var dico = new List<(GameObject, UnitActionsEnum)>();
                foreach (var selectedUnit in SelectedUnits)
                {
                    if (selectedUnit.TryGetTarget(out unit))
                    {
                        if(!unit)
                            continue;
                        
                        //Debug.Log(unit);
                        foreach (var action in GameRules.YieldActions(unit))
                        {
                            dico.Add((unit.gameObject, action));
                        }
                    }
                }
                
                uiButtons.SetButtons(dico);
            }
            else
            {
                uiButtons.SetButtons(new ());
            }

            if (Input.GetMouseButtonDown(0))
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickTime)
                {
                    if (SelectedUnits.Count == 1)
                    {
                        if (SelectedUnits[0].TryGetTarget(out unit))
                        {
                            Debug.Log("Double click");
                            _camController.SetTarget(unit);
                        }
                    }
                }

                lastClickTime = Time.time;
            }
        }
        
        private void HandleSelectionGroups(Event e)
        {
            KeyCode[] azertyKeys = {
                KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
                KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
            };

            bool isCtrlPressed = e.control;
            //Debug.Log("Ctrl pressed: " + isCtrlPressed);

            if (isCtrlPressed)
            {
                for (int i = 0; i < azertyKeys.Length; i++)
                {
                    if (e.keyCode == azertyKeys[i] && e.type == EventType.KeyDown)
                    {
                        Debug.Log("Creating selection group: " + i);
                        // Créer un groupe de sélection
                        selectionGroupManager.CreateGroup(i, SelectedUnits);
                        e.Use(); // Consomme l'événement pour éviter les conflits
                    }
                }
            }
            else
            {
                for (int i = 0; i < azertyKeys.Length; i++)
                {
                    if (e.keyCode == azertyKeys[i] && e.type == EventType.KeyDown)
                    {
                        Debug.Log("Recalling selection group: " + i);
                        // Rappeler un groupe de sélection
                        var group = selectionGroupManager.GetGroup(i);
                        if (group != null)
                        {
                            DeselectUnits();
                            foreach (var unit in group)
                            {
                                if (unit.TryGetTarget(out Transform transform) && transform)
                                {
                                    SelectUnit(transform, true);
                                }
                            }
                        }
                        e.Use(); // Consomme l'événement pour éviter les conflits
                    }
                }
            }
        }
        
        // Gère la séléction et le mouvement des unités basé sur les entrées de l'utilisateur.
        public void HandleUnitMovement()
        {
            // Vérifie si le bouton gauche de la souris est pressé.
            if (Input.GetMouseButtonDown(0))
            {
                // Check if the pointer is over a UI element
                if (IsPointerOverInteractableUI())
                {
                    // If the pointer is over a UI element, skip the raycast
                    return;
                }

                _mousePos = Input.mousePosition;
                
                // Crée un rayon partant de la caméra vers la position de la souris.
                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
                
                // Vérifie si le rayon touche quelque chose.
                if (Physics.Raycast(ray, out _hit, Mathf.Infinity, layerMask: layerMask))
                {
                    // Récupère le layer de l'objet touché.
                    int layerHit = _hit.transform.gameObject.layer;
                    Debug.Log("Hit object: " + _hit.collider.gameObject.name);
                    Debug.Log("Hit object layer: " + LayerMask.LayerToName(layerHit));

                    string layerName = LayerMask.LayerToName(layerHit);
                    
                    // Traite différemment selon le layer de l'objet touché.
                    switch (layerName)
                    {
                        case "Ground":
                            _isDragging = true;
                            DeselectUnits();
                            break;
                        case "PlayerUnits": // Couche des unités amies.
                            // Sélectionne l'unité.
                            SelectUnit(_hit.transform, Input.GetKey(KeyCode.LeftShift));
                            break;
                        case "EnemyUnits": // Couche des unités ennemies.
                            // Pourrait être utilisé pour attaquer ou cibler.
                            break;
                        default: // Si l'objet touché n'appartient à aucun des layers spécifiés.
                            // Commence une sélection multiple.
                            break;
                    }
                }
            }

            // Vérifie si le bouton gauche de la souris est relâché.
            if (Input.GetMouseButtonUp(0))
            {
                // Vérifie chaque unité pour voir si elle est dans les bornes de sélection.
                foreach (Transform child in playerManager.playerUnits)
                {
                    foreach (Transform unit in child)
                    {
                        if (IsWithinSelectionBounds(unit))
                        {
                            // Sélectionne l'unité si elle est dans la zone de sélection.
                            SelectUnit(unit, true);
                        }
                    }
                }
                // Termine la sélection multiple.
                _isDragging = false;
            }

            // Vérifie si le bouton droit de la souris est pressé et si des unités sont sélectionnées.
            if (Input.GetMouseButtonDown(1) && HaveSelectedUnits())
            {
                HandleRightClick(Input.mousePosition);
            } 
            // ou alors qu'on a deux doigts pressés
            else if (Input.touchCount == 2) {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                if (touchZero.phase == TouchPhase.Ended && touchOne.phase == TouchPhase.Ended)
                {
                    float touchTime = Mathf.Max(touchZero.deltaTime, touchOne.deltaTime);

                    if (touchTime < 0.2f)
                    {
                        // Two fingers tap detected
                        Vector2 touchCenter = (touchZero.position + touchOne.position) / 2;

                        HandleRightClick(touchCenter);
                    }
                }
            }
        }
        
        // Sélectionne une unité et active un objet enfant spécifié pour indiquer la sélection.
        private void SelectUnit(Transform unit, bool canMultiselect = false)
        {
            Assert.IsNotNull(unit);
            
            // Désélectionne toutes les unités si la multisélection n'est pas autorisée.
            if (!canMultiselect)
            {
                DeselectUnits();
            }
            // Ajoute l'unité à la liste des unités sélectionnées.
            SelectedUnits.Add(new WeakReference<Transform>(unit));
            // Active un objet enfant spécifié de l'unité pour indiquer la sélection.
            Transform t = unit.Find("Hightlight");
            if (!t)
            {
                Debug.LogWarning("Le gameobject "+unit.name+" n'a pas de descendant Hightlight");
                return;
            }
            t.gameObject.SetActive(true); // Attention à l'erreur de frappe : "Highlight".
        }

        // Désélectionne toutes les unités sélectionnées et désactive l'indicateur de sélection.
        private void DeselectUnits()
        {
            for (int i = 0; i < SelectedUnits.Count; i++)
            {
                var sel = SelectedUnits[i];
                if(sel.TryGetTarget(out Transform trans) && trans) {
                    Transform t = trans.Find("Hightlight"); // Attention à l'erreur de frappe : "Highlight".
                    if (!t)
                    {
                        Debug.LogWarning("Le gameobject "+unit.name+" n'a pas de descendant Hightlight");
                        continue;
                    }

                    t.gameObject.SetActive(false);
                }
            }
            // Efface la liste des unités sélectionnées.
            SelectedUnits.Clear();
        }


        private void HandleRightClick(Vector2 position)
        {
            // Crée un rayon partant de la caméra vers la position de la souris.
                Ray ray = _cam.ScreenPointToRay(position);
                
                // Vérifie si le rayon touche quelque chose, en ignorant le Layer "ResourceZone"
                int ignoreResourceZoneMask = ~LayerMask.GetMask("ResourceZone");
                if (Physics.Raycast(ray, out _hit, Mathf.Infinity, ignoreResourceZoneMask))
                {
                    // Traite différemment selon le layer de l'objet touché.
                    switch (_hit.transform.gameObject.layer)
                    {
                        case 5:
                            break;
                        case 8: // layer des unités amies.
                            break;
                        case 9: // layer des unités ennemies.
                            // Pourrait être utilisé pour attaquer ou cibler.
                            foreach (var weakUnit in SelectedUnits)
                            {
                                if(weakUnit.TryGetTarget(out Transform unit) && unit) 
                                {
                                    PlayerUnit pU = unit.gameObject.GetComponent<PlayerUnit>();
                                    pU.TargetEnemy(_hit.transform);
                                }
                            }
                            
                            break;
                        default:
                            if (SelectedUnits.Count > 1)
                            {
                                string currentFormation = "quinconce";
                                List<Vector3> formationPositions = FormationManager.GetFormationPositions(_hit.point, SelectedUnits.Count, 0.6f, currentFormation);
                                int index = 0;
                                foreach (var weakUnit in SelectedUnits)
                                {
                                    if (weakUnit.TryGetTarget(out Transform unit) && unit)
                                    {
                                        PlayerUnit pU = unit.gameObject.GetComponent<PlayerUnit>();
                                        pU.MoveUnit(formationPositions[index]);
                                        index++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var weakUnit in SelectedUnits)
                                {
                                    if (weakUnit.TryGetTarget(out Transform unit) && unit)
                                    {
                                        PlayerUnit pU = unit.gameObject.GetComponent<PlayerUnit>();
                                        pU.MoveUnit(_hit.point);
                                    }
                                }
                            }
                            break;
                    }
                }
        }

        // Vérifie si une unité se trouve dans les bornes de sélection.
        private bool IsWithinSelectionBounds(Transform tf)
        {
            // Retourne faux si l'utilisateur n'est pas en train de faire une sélection multiple.
            if (!_isDragging)
            {
                return false;
            }
            // Calcule les bornes de sélection en espace de vue.
            Bounds vpBounds = MultiSelect.GetVPBounds(_cam, _mousePos, Input.mousePosition);
            
            // Vérifie si la position de l'unité est à l'intérieur des bornes de sélection.
            return vpBounds.Contains(_cam.WorldToViewportPoint(tf.position));
        }

        // Vérifie si des unités ont été sélectionnées.
        private bool HaveSelectedUnits()
        {
            // Retourne vrai si la liste des unités sélectionnées n'est pas vide.
            return SelectedUnits.Count > 0;
        }

        // Custom method to check if the pointer is over an interactable UI element
        private bool IsPointerOverInteractableUI()
        {
            return uiButtons.IsOverSomeButton;
        }

        
        private bool IsPointerOverUIButton()
        {
            PointerEventData eventData = new PointerEventData(_eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Button>() != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
