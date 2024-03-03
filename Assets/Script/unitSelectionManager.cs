using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class unitSelectionManager : MonoBehaviour
{
    public static unitSelectionManager Instance { get; set; }
    
    // liste des unité active
    public List<GameObject> allUnitsList = new List<GameObject>();
    
    // liste des unité selectionner
    public List<GameObject> unitsSelected = new List<GameObject>();

    // layer pour les mesh cliquable
    public LayerMask clickable;
    
    // layer pour le terrain
    public LayerMask ground;
    
    // objet pour l'indicateur de position cliquer (sur terrain lors de déplacement d'unité)
    public GameObject groundMarker;
    
    // instancie objet cam (on y récupère la mainCaméra au start)
    Camera cam;
    
    private void Awake()
    { 
        // vérification de la sur-génération d'instance d'unitSelectionManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        // récupère la mainCaméra
        cam = Camera.main;
    }
    
    void Update()
    {
        // si clique clic gauche de la souris
        if (Input.GetMouseButtonDown(0))
        {
            // on récupère le hit
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                
            // Si on clic sur un objet du layer clickable (selection d'unité avec clique gauche de la souris
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                // si shift gauche est appuyer/maintenue
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // selection de plusieurs unité avec le clic (tant que shift-gauche est maintenu)
                    MultiSelect(hit.collider.gameObject);
                }
                else
                {
                    // selection d'une unité (si clic sur une autre unité, les autre sont déselectionné)
                    SelectByClicking(hit.collider.gameObject);
                }
            }
            else // Si on ne le fait pas
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    // si on clic sur n'importe quoi d'autre qu'une des unité
                    DeselectAll();
                }
            }
        }
        
        // si on clic sur le bouton droit de la souris
        if (Input.GetMouseButtonDown(1) && unitsSelected.Count>0)
        {
            
            // objet hit pour récupéré la position du clic
            RaycastHit hit;
            
            // récupère la position du clic de la souris
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            // vérifie le clic sur le terrain (objet du layer ground)
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {   
                // met a jour la position de l'objet groundMarker avec la pos du clic de la souris
                groundMarker.transform.position = hit.point;
                
                // desactive tout autre groundMarker et l'affiche
                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
            }
        }
    }
    
    // met a jour l'indicateur de selection pour l'unité concerner 
    private void SelectUnit(GameObject unit, bool isSelected)
    {
        TriggerSelectionIndicator(unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }
    
    // selectionne une unité avec le clic gauche de la souris
    private void SelectByClicking(GameObject unit)
    {
        // deselectionne toute les autre unité
        DeselectAll();
        
        // ajoute l'unité a la liste des unité selectionner
        unitsSelected.Add(unit);

        SelectUnit(unit, true);
    }
    
    // permet de selectionner plusieurs unité en maintenan le bouton shift gauche lors de clic gauche de la souris
    private void MultiSelect(GameObject unit)
    {
        // si l'unité selectionner n'est pas déja dans la liste selected, on l'ajoute, sinon on l'enlève
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            SelectUnit(unit, false);
            unitsSelected.Remove(unit);
        }
    }
    
    // méthode pour la selection par selectBox (script unitSelectionBox)
    public void DragSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
    }
    
    // nétoie la liste des unité selectionner (deselection de toute les unité) et met a jour les indicateur
    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            SelectUnit(unit, false);
        }
        groundMarker.SetActive(false);
        unitsSelected.Clear();
    }
    
    // méthode pour l'activation du script de mouvement
    // (séparé dans un autre script pour ne pas généré en boucle les agent AI navigation (problème de perf sinon)
    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        // active le script unitMoveController pour l'unité concerner
        unit.GetComponent<unitMoveController>().enabled = shouldMove;
    }
    
    // active l'indicateur de selection pour l'unité concerner
    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        unit.transform.GetChild(0).gameObject.SetActive(isVisible);
    }
    
    
    
    
    
    
}
