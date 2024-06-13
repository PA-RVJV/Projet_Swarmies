using System.Collections.Generic;
using UnityEngine;
using PS.Units.Player;
using System;
using PS.Player;
using Script;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace PS.InputHandlers
{
    public class InputManager : MonoBehaviour
    {
        public PlayerManager playerManager;
        public readonly List<WeakReference<Transform>> SelectedUnits = new(); // Liste des unités sélectionnées.
        public UIButtons uiButtons;
        public LayerMask layerMask;
        
        private RaycastHit _hit; // stocke l'information du raycast.
        private bool _isDragging = false; // Booléen de vérification sélection multiple en cour ou non.
        private Vector3 _mousePos; // Position initiale de la souris lors du début de select.
        private Camera _cam;


        private void Awake()
        {
            _cam = Camera.main;
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
        }
        
        void Update()
        {
            if (SelectedUnits.Count == 1)
            {
                Transform unit;
                if (SelectedUnits[0].TryGetTarget(out unit))
                {
                    //Debug.Log(unit);
                    if (unit.parent.name == "Workers")
                    {
                        uiButtons.SetButtons(new List<UnitActionsEnum>{UnitActionsEnum.Construire});
                    }
                }
            }
            else
            {
                uiButtons.SetButtons(new List<UnitActionsEnum>());
            }
        }

        // Gère la séléction et le mouvement des unités basé sur les entrées de l'utilisateur.
        public void HandleUnitMovement()
        {
            // Vérifie si le bouton gauche de la souris est pressé.
            if (Input.GetMouseButtonDown(0))
            {
                _mousePos = Input.mousePosition;
                
                // Crée un rayon partant de la caméra vers la position de la souris.
                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
                
                // Vérifie si le rayon touche quelque chose.
                if (Physics.Raycast(ray, out _hit, Mathf.Infinity, layerMask: layerMask))
                {
                    // Récupère le layer de l'objet touché.
                    int layerHit = _hit.transform.gameObject.layer;

                    // Traite différemment selon le layer de l'objet touché.
                    switch (layerHit)
                    {
                        case 8: // Couche des unités amies.
                            // Sélectionne l'unité.
                            SelectUnit(_hit.transform, Input.GetKey(KeyCode.LeftShift));
                            break;
                        case 9: // Couche des unités ennemies.
                            // Pourrait être utilisé pour attaquer ou cibler.
                            break;
                        default: // Si l'objet touché n'appartient à aucun des layers spécifiés.
                            // Commence une sélection multiple.
                            _isDragging = true;
                            DeselectUnits();
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
                // Crée un rayon partant de la caméra vers la position de la souris.
                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
                // Vérifie si le rayon touche quelque chose.
                if (Physics.Raycast(ray, out _hit))
                {
                    // Traite différemment selon le layer de l'objet touché.
                    switch (_hit.transform.gameObject.layer)
                    {
                        case 8: // layer des unités amies.
                            break;
                        case 9: // layer des unités ennemies.
                            // Pourrait être utilisé pour attaquer ou cibler.
                            break;
                        default: // Si l'objet touché n'appartient à aucun des layers spécifiés.
                            // Déplace les unités sélectionnées vers le point touché.
                            foreach (var weakUnit in SelectedUnits)
                            {
                                if(weakUnit.TryGetTarget(out Transform unit) && unit) 
                                {
                                    PlayerUnit pU = unit.gameObject.GetComponent<PlayerUnit>();
                                    pU.MoveUnit(_hit.point);
                                }
                            }
                            break;
                    }
                }
            }
        }
        
        // Sélectionne une unité et active un objet enfant spécifié pour indiquer la sélection.
        private void SelectUnit(Transform unit, bool canMultiselect = false)
        {
            // Désélectionne toutes les unités si la multisélection n'est pas autorisée.
            if (!canMultiselect)
            {
                DeselectUnits();
            }
            // Ajoute l'unité à la liste des unités sélectionnées.
            SelectedUnits.Add(new WeakReference<Transform>(unit));
            // Active un objet enfant spécifié de l'unité pour indiquer la sélection.
            unit.Find("Hightlight").gameObject.SetActive(true); // Attention à l'erreur de frappe : "Highlight".
        }

        // Désélectionne toutes les unités sélectionnées et désactive l'indicateur de sélection.
        private void DeselectUnits()
        { 
            for (int i = 0; i < SelectedUnits.Count; i++)
            {
                var sel = SelectedUnits[i];
                if(sel.TryGetTarget(out Transform trans) && trans) {
                    trans.Find("Hightlight").gameObject.SetActive(false); // Attention à l'erreur de frappe : "Highlight".
                }
            }
            // Efface la liste des unités sélectionnées.
            SelectedUnits.Clear();
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
    }
}
