using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectionRectangleBehavior : MonoBehaviour
{
    public SelectedUnitsHolderBehavior selectedUnitsHolderBehavior;
    public RectTransform rectTransform;
    public Image image;

    private Vector2 _clickp;

    public readonly HashSet<WeakReference<GameObject>> selectable = new();
    private Camera _mainCamera;

    private void Start()
    {
        var rect = rectTransform.rect;
        rect.height = 0;
        image.gameObject.SetActive(false);

        _mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _clickp = Mouse.current.position.value;
            image.gameObject.SetActive(true);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _clickp = Vector2.zero;
            image.gameObject.SetActive(false);
            Debug.Log(selectable.Count);
        }

        if (Mouse.current.leftButton.isPressed && _clickp != Vector2.zero)
        {

            // change l'affichage du rectangle de selection
            image.transform.position = _clickp;
            var height = -(Mouse.current.position.value.y - _clickp.y);
            var width = Mouse.current.position.value.x - _clickp.x;

            var xscale = 1;
            var yscale = 1;

            if (width < 0) {
                xscale = -1;
                width *= -1;
            }

            if(height < 0) {
                yscale = -1;
                height *= -1;
            }

            rectTransform.localScale = new Vector3(xscale, yscale, 1);
            rectTransform.sizeDelta =
                new Vector2(
                    width, 
                    height
                );


            // met les unités dans le rectangle dans la selection
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, corners[0], _mainCamera, out Vector3 min);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, corners[2], _mainCamera, out Vector3 max);

            foreach (var sel in selectable) {
                if (sel.TryGetTarget(out GameObject tgtO)) {
                    Vector3 screenPoint = _mainCamera.WorldToScreenPoint(tgtO.transform.position);

                    if (screenPoint.x > min.x 
                    && screenPoint.x < max.x 
                    && screenPoint.y > min.y 
                    && screenPoint.y < max.y)
                    {
                        selectedUnitsHolderBehavior.AddSelected(tgtO);
                    }
                }
            }
        }
    }
    
    
}
