using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class unitMoveController : MonoBehaviour
{
    // class pour la gestion des mouvements de l'unité
    // déplacement gérer avec le package AI navigation (pas sur qu'on est le droit d'utiliser des package mais bon)
    
    // instancie objet cam (on y récupère la mainCaméra au start)
    Camera cam;

    // instancie objet NavMeshAgent pour le déplacement (package AI navigation) 
    UnityEngine.AI.NavMeshAgent agent;
    
    // instancie objet pour le terrain
    public LayerMask ground;
    
    void Start()
    {
        // récupère la mainCaméra
        cam = Camera.main;
        
        // récupère le navMeshAgent sur  la scene
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    
    void Update()
    {
        // vérifie le clic du bouton droit de la souris
        if (Input.GetMouseButtonDown(1))
        {
            // objet hit pour récupéré la position du clic
            RaycastHit hit;
            
            // récupère la position du clic de la souris
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                
            // vérifie le clic sur le terrain (objet ground)
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                // définis la destination de l'agent avec la pos du point cliquer pour deplacer le mesh
                agent.SetDestination(hit.point);
            }
        }
    }
}
