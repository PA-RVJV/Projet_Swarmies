using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public bool showMinimapMarkers = true;
    public Camera camera_;
    public List<WeakReference<GameObject>> playerUnitsOnMinimap;
    public List<WeakReference<GameObject>> allyUnitsOnMinimap;
    public List<WeakReference<GameObject>> enemyUnitsOnMinimap;
    public GameObject playerUnits;
    public GameObject allyUnits;
    public GameObject enemyUnits;

    public Sprite minimapMarkerSprite;

    void Start()
    {
        if(!showMinimapMarkers)
            return;

        foreach(Transform category in playerUnits.transform) {
            foreach(Transform unit in category) {
                // playerUnitsOnMinimap.Add( 
                //     new WeakReference<GameObject>(unit.gameObject)
                // );
                // var hoverCanvas = new GameObject("MapMarkerCanvas");
                // hoverCanvas.transform.parent = unit.transform;
                // var canvas = hoverCanvas.AddComponent<Canvas>();
                // canvas.worldCamera = camera_;
                // //Transform hoverCanvas = unit.Find("UnitStatDisplay");
                
                // var mapMarker = new GameObject("MapMarker");
                // mapMarker.transform.parent = hoverCanvas.transform;

                // var image = mapMarker.AddComponent<Image>();
                // image.sprite = minimapMarkerSprite;
                // image.color = Color.blue;
                // image.raycastTarget = false;

                // var rectTransform = image.GetComponent<RectTransform>();
                // rectTransform.eulerAngles = new Vector3(90, 0, 0);
                // rectTransform.localScale = new Vector3(20, 8.5f, 1);
                // rectTransform.localPosition = Vector3.zero;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        camera_.transform.position = Camera.main.transform.position;
    }
}