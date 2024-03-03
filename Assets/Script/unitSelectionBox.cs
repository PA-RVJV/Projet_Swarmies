using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe pour gérer la sélection de unités via une boîte de sélection visuelle
public class unitSelectionBox : MonoBehaviour
{
    // Référence à la caméra principale du jeu
    Camera myCam;

    // Référence à l'élément d'UI qui représentera visuellement la boîte de sélection
    [SerializeField]
    RectTransform boxVisual;

    // Rectangle définissant la zone de sélection
    Rect selectionBox;

    // Positions de départ et de fin pour le dessin de la boîte de sélection
    Vector2 startPosition;
    Vector2 endPosition;

    // Initialisation
    private void Start()
    {
        // Récupère la caméra principale
        myCam = Camera.main;
        // Initialise les positions de départ et de fin
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        // Dessine l'aspect visuel de la boîte de sélection (sera vide initialement)
        DrawVisual();
    }

    // Mise à jour appelée une fois par frame
    private void Update()
    {
        // Si le bouton gauche de la souris est pressé
        if (Input.GetMouseButtonDown(0))
        {
            // Enregistre la position de départ de la souris
            startPosition = Input.mousePosition;
            
            // Réinitialise la sélectionBox
            selectionBox = new Rect();
        }
        
        // Si le bouton gauche de la souris est maintenu
        if (Input.GetMouseButton(0))
        {
            // Désélectionne toutes les unités si la boîte a une taille non nulle et sélectionne les unités
            if (boxVisual.rect.width > 0 || boxVisual.rect.height > 0)
            {
                unitSelectionManager.Instance.DeselectAll();
                SelectUnits();
            }
            
            // Met à jour la position de fin avec la position actuelle de la souris
            endPosition = Input.mousePosition;
            // Dessine l'aspect visuel et la sélection réelle de la boîte
            DrawVisual();
            DrawSelection();
        }
        
        // Quand le bouton gauche de la souris est relâché
        if (Input.GetMouseButtonUp(0))
        {
            // Finalise la sélection des unités
            SelectUnits();
 
            // Réinitialise les positions de départ et de fin
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            // Efface l'aspect visuel de la boîte de sélection
            DrawVisual();
        }
    }
 
    // Dessine l'aspect visuel de la boîte de sélection
    void DrawVisual()
    {
        // Calcule la position de départ et de fin
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;
 
        // Calcule le centre de la boîte
        Vector2 boxCenter = (boxStart + boxEnd) / 2;
 
        // Définit la position du rectangle visuel basé sur son centre
        boxVisual.position = boxCenter;
 
        // Calcule et définit la taille de la boîte visuelle
        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
        boxVisual.sizeDelta = boxSize;
    }
 
    // Calcule la sélection réelle en fonction de la position de la souris
    void DrawSelection()
    {
        // Ajuste les bornes min et max de la sélectionBox en fonction de la position de la souris
        if (Input.mousePosition.x < startPosition.x)
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }

        if (Input.mousePosition.y < startPosition.y)
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }
 
    // Sélectionne les unités à l'intérieur de la boîte de sélection
    void SelectUnits()
    {
        // Parcourt toutes les unités disponibles
        foreach (var unit in unitSelectionManager.Instance.allUnitsList)
        {
            // Si l'unité se trouve à l'intérieur de la boîte de sélection
            if (selectionBox.Contains(myCam.WorldToScreenPoint(unit.transform.position)))
            {
                // Sélectionne l'unité
                unitSelectionManager.Instance.DragSelect(unit);
            }
        }
    }
}