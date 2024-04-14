using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Units.Player;
using System;

namespace PS.InputHandlers
{
    public class InputManager : MonoBehaviour
    {
        
        public static InputManager instance; // Instance statique d'InputManager
        private RaycastHit hit; // stocke l'information du raycast.
        public List<WeakReference<Transform>> selectedUnits = new(); // Liste des unités sélectionnées.
        private bool isDragging = false; // Booléen de vérification sélection multiple en cour ou non.
        private Vector3 mousePos; // Position initiale de la souris lors du début de select.

        private void Awake()
        {
            instance = this;
        }
        
        void Start()
        {
            
        }

        // Dessine le rectangle de sélection sur l'interface a l'aide de la classe MultiSelect
        private void OnGUI()
        {
            if (isDragging)
            {
                // Crée et dessine le rectangle de sélection.un rectangle de sélection à partir de la position
                // initiale de la souris et la position actuelle.
                Rect rect = MultiSelect.GetScreenRect(mousePos, Input.mousePosition);
                MultiSelect.DrawScreenRect(rect, new Color(0f, 0f, 0f, 0.25f));
                
                // Dessine la bordure du rectangle de sélection.
                MultiSelect.DrawScreenRectBorder(rect, 3, Color.blue); 
            }
        }
        
        void Update()
        {
            
        }

        // Gère la séléction et le mouvement des unités basé sur les entrées de l'utilisateur.
        public void HandleUnitMovement()
        {
            // Vérifie si le bouton gauche de la souris est pressé.
            if (Input.GetMouseButtonDown(0))
            {
                mousePos = Input.mousePosition;
                
                // Crée un rayon partant de la caméra vers la position de la souris.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                // Vérifie si le rayon touche quelque chose.
                if (Physics.Raycast(ray, out hit))
                {
                    // Récupère le layer de l'objet touché.
                    int layerHit = hit.transform.gameObject.layer;

                    // Traite différemment selon le layer de l'objet touché.
                    switch (layerHit)
                    {
                        case 8: // Couche des unités amies.
                            // Sélectionne l'unité.
                            SelectUnit(hit.transform, Input.GetKey(KeyCode.LeftShift));
                            break;
                        case 9: // Couche des unités ennemies.
                            // Pourrait être utilisé pour attaquer ou cibler.
                            break;
                        default: // Si l'objet touché n'appartient à aucun des layers spécifiés.
                            // Commence une sélection multiple.
                            isDragging = true;
                            DeselectUnits();
                            break;
                    }
                }
            }

            // Vérifie si le bouton gauche de la souris est relâché.
            if (Input.GetMouseButtonUp(0))
            {
                // Vérifie chaque unité pour voir si elle est dans les bornes de sélection.
                foreach (Transform child in Player.PlayerManager.instance.playerUnits)
                {
                    foreach (Transform unit in child)
                    {
                        if (isWithinSelectionBounds(unit))
                        {
                            // Sélectionne l'unité si elle est dans la zone de sélection.
                            SelectUnit(unit, true);
                        }
                    }
                }
                // Termine la sélection multiple.
                isDragging = false;
            }

            // Vérifie si le bouton droit de la souris est pressé et si des unités sont sélectionnées.
            if (Input.GetMouseButtonDown(1) && HaveSelectedUnits())
            {
                // Crée un rayon partant de la caméra vers la position de la souris.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Vérifie si le rayon touche quelque chose.
                if (Physics.Raycast(ray, out hit))
                {
                    // Traite différemment selon le layer de l'objet touché.
                    switch (hit.transform.gameObject.layer)
                    {
                        case 8: // layer des unités amies.
                            break;
                        case 9: // layer des unités ennemies.
                            // Pourrait être utilisé pour attaquer ou cibler.
                            break;
                        default: // Si l'objet touché n'appartient à aucun des layers spécifiés.
                            // Déplace les unités sélectionnées vers le point touché.
                            foreach (var weakUnit in selectedUnits)
                            {
                                if(weakUnit.TryGetTarget(out Transform unit)) {
                                    PlayerUnit pU = unit.gameObject.GetComponent<PlayerUnit>();
                                    pU.MoveUnit(hit.point);
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
            selectedUnits.Add(new WeakReference<Transform>(unit));
            // Active un objet enfant spécifié de l'unité pour indiquer la sélection.
            unit.Find("Hightlight").gameObject.SetActive(true); // Attention à l'erreur de frappe : "Highlight".
        }

        // Désélectionne toutes les unités sélectionnées et désactive l'indicateur de sélection.
        private void DeselectUnits()
        { 
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                var sel = selectedUnits[i];
                if(sel.TryGetTarget(out Transform trans) && trans) {
                    trans.Find("Hightlight").gameObject.SetActive(false); // Attention à l'erreur de frappe : "Highlight".
                }
                
            }
            // Efface la liste des unités sélectionnées.
            selectedUnits.Clear();
        }

        // Vérifie si une unité se trouve dans les bornes de sélection.
        private bool isWithinSelectionBounds(Transform tf)
        {
            // Retourne faux si l'utilisateur n'est pas en train de faire une sélection multiple.
            if (!isDragging)
            {
                return false;
            }
            // Calcule les bornes de sélection en espace de vue.
            Camera cam = Camera.main;
            Bounds vpBounds = MultiSelect.GetVPBounds(cam, mousePos, Input.mousePosition);
            
            // Vérifie si la position de l'unité est à l'intérieur des bornes de sélection.
            return vpBounds.Contains(cam.WorldToViewportPoint(tf.position));
        }

        // Vérifie si des unités ont été sélectionnées.
        private bool HaveSelectedUnits()
        {
            // Retourne vrai si la liste des unités sélectionnées n'est pas vide.
            return selectedUnits.Count > 0;
        }
    }
}