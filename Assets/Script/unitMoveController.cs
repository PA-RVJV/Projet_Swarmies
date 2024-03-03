using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class unitMoveController : MonoBehaviour
{
    Camera cam;

    UnityEngine.AI.NavMeshAgent agent;

    public LayerMask ground;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                
            // Si on clic sur un objet clickable
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
