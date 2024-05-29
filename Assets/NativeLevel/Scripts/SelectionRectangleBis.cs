using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SelectionRectangleDrawer : MonoBehaviour
{
    //set color via inspector for the selection rectangles filler color
    public Color selectionRectangleFillerColor;
    //set color via inspector for the selection rectangles border color
    public Color selectionRectangleBorderColor;
    //set selection rectangles  border thickness
    public int selectionRectangleBorderThickness = 2;
    private Texture2D _selectionRectangleFiller;
    private Texture2D _selectionRectangleBorder;
    private bool _drawSelectionRectangle;
    private float _x1, _x2, _y1, _y2;
    private Vector2Control pos1, pos2;
    void Start()
    {
        _selectionRectangleFiller = new Texture2D(1, 1);
        _selectionRectangleFiller.SetPixel(0, 0, selectionRectangleFillerColor);
        _selectionRectangleFiller.Apply();
        _selectionRectangleFiller.wrapMode = TextureWrapMode.Clamp;
        _selectionRectangleFiller.filterMode = FilterMode.Point;
        _selectionRectangleBorder = new Texture2D(1, 1);
        _selectionRectangleBorder.SetPixel(0, 0, selectionRectangleBorderColor);
        _selectionRectangleBorder.Apply();
        _selectionRectangleBorder.wrapMode = TextureWrapMode.Clamp;
        _selectionRectangleBorder.filterMode = FilterMode.Point;
    }
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            pos1 = Mouse.current.position;
        }
        
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            _drawSelectionRectangle = false;

        if (Mouse.current.leftButton.isPressed)
        {
            pos2 = Mouse.current.position;
            if (
                Mathf.Approximately(
                    0,
                    Vector2.Distance(new Vector2(pos1.x.value, pos1.y.value), new Vector2(pos2.x.value, pos2.y.value)))
            )
            {
                _drawSelectionRectangle = true;
            }
        }
        
    }
    private void OnGUI()
    {
        if (_drawSelectionRectangle)
            drawSelectionRectangle();
    }
    private void drawSelectionRectangle()
    {
        //check initial mouse position on X axis versus dragging mouse position
        if (pos1.x.value < pos2.x.value)
        {
            _x1 = pos1.x.value;
            _x2 = pos2.x.value;
        }
        else
        {
            _x1 = pos2.x.value;
            _x2 = pos1.x.value;
        }
        //check initial mouse position on Y axis versus dragging mouse position
        if (pos1.y.value < pos2.y.value)
        {
            _y1 = pos1.y.value;
            _y2 = pos2.y.value;
        }
        else
        {
            _y1 = pos2.y.value;
            _y2 = pos1.y.value;
        }
        //filler
        GUI.DrawTexture(new Rect(_x1, Screen.height - _y1, _x2 - _x1, _y1 - _y2), _selectionRectangleFiller, ScaleMode.StretchToFill);
        //top line
        GUI.DrawTexture(new Rect(_x1, Screen.height - _y1, _x2 - _x1, -selectionRectangleBorderThickness), _selectionRectangleBorder, ScaleMode.StretchToFill);
        //bottom line
        GUI.DrawTexture(new Rect(_x1, Screen.height - _y2, _x2 - _x1, selectionRectangleBorderThickness), _selectionRectangleBorder, ScaleMode.StretchToFill);
        //left line
        GUI.DrawTexture(new Rect(_x1, Screen.height - _y1, selectionRectangleBorderThickness, _y1 - _y2), _selectionRectangleBorder, ScaleMode.StretchToFill);
        //right line
        GUI.DrawTexture(new Rect(_x2, Screen.height - _y1, -selectionRectangleBorderThickness, _y1 - _y2), _selectionRectangleBorder, ScaleMode.StretchToFill);
    }
}