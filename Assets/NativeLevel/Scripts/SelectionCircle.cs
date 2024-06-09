using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircle : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public int subdivisions = 10;

    public float radius = 1f;

    public MeshRenderer attachedMesh;
    // Start is called before the first frame update
    void Start()
    {
        float angleStep = 2 * Mathf.PI / subdivisions;

        lineRenderer.positionCount = subdivisions;
        for (int i = 0; i < subdivisions; i++)
        {
            float xPosition = radius * Mathf.Cos(i * angleStep);
            float zPosition = radius * Mathf.Sin(i * angleStep);
            lineRenderer.SetPosition(i, new Vector3(xPosition, 0, zPosition));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}

/**
* Gere la souris qui passe sur un collider d'unité
*/
public class MouseSelectableBehavior : MonoBehaviour
{
    public GameObject selectionCirclePrefab;
    public SelectedUnitsHolderBehavior selectedUnitsHolderBehavior;
    [Tooltip("When this is selected, clear the current selection")] 
    public bool clearOtherOnSelection = true;
    
    private GameObject _selectionCircle;

    public void DestroyCircle()
    {
        if(_selectionCircle)
            Destroy(_selectionCircle);
    }
    
    private void OnMouseEnter()
    {
        Debug.Log("enter");
        _selectionCircle = Instantiate(selectionCirclePrefab, transform);
        Vector3 currentPosition = _selectionCircle.transform.position;
        _selectionCircle.transform.position = currentPosition + new Vector3(0, 0.25f, 0);
    }

    private void OnMouseExit()
    {
        Debug.Log("leave");
        if(selectedUnitsHolderBehavior.IsSelected(gameObject))
            return;
        
        DestroyCircle();
    }

    private void OnMouseUp()
    {
        if(clearOtherOnSelection)
            selectedUnitsHolderBehavior.ClearSelection();
        selectedUnitsHolderBehavior.AddSelected(gameObject);
    }

    
}

/**
 * Gere de garder en memoire un tableau d'unités
 * sélectionnées via clic ou rectangle ou autre.
 */
public class SelectedUnitsHolderBehavior : MonoBehaviour
{
    private readonly HashSet<WeakReference<GameObject>> _selected = new();


    public void AddSelected(GameObject o)
    {
        _selected.Add(new WeakReference<GameObject>(o));
    }

    public void ClearSelection()
    {
        foreach (var weakReference in _selected)
        {
            // redondant vu que le circle est descendant
            // de l'unité et qu'on la detruit?
            if (weakReference.TryGetTarget(out GameObject tgtO))
                tgtO.GetComponent<MouseSelectableBehavior>().DestroyCircle();
        }
        _selected.Clear();
        
    }

    public bool IsSelected(GameObject o)
    {
        foreach (var weakReference in _selected)
        {
            if (weakReference.TryGetTarget(out GameObject tgtO) && o == tgtO)
                return true;
        }

        return false;
    }
}

