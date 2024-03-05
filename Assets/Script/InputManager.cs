using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Player;

namespace PS.InputHandlers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        private RaycastHit hit;

        private List<Transform> selectedUnits = new List<Transform>();

        private bool isDragging = false;

        private Vector3 mousePos;
        
        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        private void OnGUI()
        {
            if (isDragging)
            {
                Rect rect = MultiSelect.GetScreenRect(mousePos, Input.mousePosition);
                MultiSelect.DrawScreenRect(rect, new Color(0f, 0f, 0f, 0.25f));
                MultiSelect.DrawScreenRectBorder(rect, 3, Color.blue);
            }
        }
        // Update is called once per frame
        void Update()
        {
        
        }

        public void HandleUnitMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mousePos = Input.mousePosition;
                // creation du ray
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // verif si on touche quelque chose
                if (Physics.Raycast(ray, out hit))
                {
                    int layerHit = hit.transform.gameObject.layer;

                    switch (layerHit)
                    {
                        case 8: // units layer
                            // do something
                            SelectUnit(hit.transform, Input.GetKey(KeyCode.LeftShift));
                            break;
                        case 9: // ennemy units layer
                            // attack or set target
                            break;
                        default: // if none of the above happens
                            // do something
                            isDragging = true;
                            DeselectUnits();
                            break;
                    }
                }
                // si il fait on procède
            }

            if (Input.GetMouseButtonUp(0))
            {
                foreach (Transform child in Player.PlayerManager.instance.playerUnits)
                {
                    foreach (Transform unit in child)
                    {
                        if (isWithinSelectionBounds(unit))
                        {
                            SelectUnit(unit, true);
                        }
                    }
                }
                isDragging = false;
            }

            if (Input.GetMouseButtonDown(1) && HaveSelectedUnits())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // verif si on touche quelque chose
                if (Physics.Raycast(ray, out hit))
                {
                    int layerHit = hit.transform.gameObject.layer;

                    switch (layerHit)
                    {
                        case 8: // units layer
                            // do something
                            break;
                        case 9: // ennemy units layer
                            // attack or set target
                            break;
                        default: // if none of the above happens
                            // do something
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
        private void SelectUnit(Transform unit, bool canMultiselect = false)
        {
            if (!canMultiselect)
            {
                DeselectUnits();
            }
            selectedUnits.Add(unit);
            // lets set an obje on the unit called Highlight
            unit.Find("Hightlight").gameObject.SetActive(true);
        }

        private void DeselectUnits()
        {
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                selectedUnits[i].Find("Hightlight").gameObject.SetActive(false);
            }
            selectedUnits.Clear();
        }

        private bool isWithinSelectionBounds(Transform tf)
        {
            if (!isDragging)
            {
                return false;
            }

            Camera cam = Camera.main;
            Bounds vpBounds = MultiSelect.GetVPBounds(cam, mousePos, Input.mousePosition);
            return vpBounds.Contains(cam.WorldToViewportPoint(tf.position));
        }

        private bool HaveSelectedUnits()
        {
            if (selectedUnits.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

