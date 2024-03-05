using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Player;

namespace PS.InputHandlers
{
    public class InputManager : MonoBehaviour
    {
        // Instance statique pour accéder facilement à l'InputManager depuis d'autres scripts.
        public static InputManager instance;

        // Variable pour stocker l'information du raycast.
        private RaycastHit hit;

        // Liste pour garder une trace des unités sélectionnées.
        private List<Transform> selectedUnits = new List<Transform>();

        // Booléen pour vérifier si l'utilisateur est en train de faire une sélection multiple.
        private bool isDragging = false;

        // Position initiale de la souris lors du début du glissement.
        private Vector3 mousePos;
        
        void Start()
        {
            instance = this;
        }

        // Dessine le rectangle de sélection sur l'interface.
        private void OnGUI()
        {
            // Vérifie si l'utilisateur est en train de glisser pour la sélection multiple.
            if (isDragging)
            {
                // Crée un rectangle de sélection à partir de la position initiale de la souris et la position actuelle.
                Rect rect = MultiSelect.GetScreenRect(mousePos, Input.mousePosition);
                // Dessine le rectangle de sélection.
                MultiSelect.DrawScreenRect(rect, new Color(0f, 0f, 0f, 0.25f));
                // Dessine la bordure du rectangle de sélection.
                MultiSelect.DrawScreenRectBorder(rect, 3, Color.blue);
            }
        }
        
        void Update()
        {
            
        }

        // Gère le mouvement des unités basé sur les entrées de l'utilisateur.
        public void HandleUnitMovement()
        {
            // Vérifie si le bouton gauche de la souris est pressé.
            if (Input.GetMouseButtonDown(0))
            {
                // Enregistre la position initiale de la souris.
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
                foreach (Transform child in PlayerManager.instance.playerUnits)
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
                            foreach (Transform unit in selectedUnits)
                            {
                                PlayerUnit pU = unit.gameObject.GetComponent<PlayerUnit>();
                                pU.MoveUnit(hit.point);
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
            selectedUnits.Add(unit);
            // Active un objet enfant spécifié de l'unité pour indiquer la sélection.
            unit.Find("Hightlight").gameObject.SetActive(true); // Attention à l'erreur de frappe : "Highlight".
        }

        // Désélectionne toutes les unités sélectionnées et désactive l'indicateur de sélection.
        private void DeselectUnits()
        {
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                selectedUnits[i].Find("Hightlight").gameObject.SetActive(false); // Attention à l'erreur de frappe : "Highlight".
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