using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Script.Units
{

    public class ButtonHoverListener : MonoBehaviour, IPointerEnterHandler
    {
        
        private Vector3 _originalLocalPosition;
        private Vector3 _offset = new Vector3(0, 1, 0);

        private Transform _highlight;
        private void Start()
        {
            var hl = transform.Find("Hightlight");
            if (hl)
                _highlight = hl;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_highlight)
            {
                MoveUp(_highlight);
            }
        }
        public void OnPointerLeave(PointerEventData eventData)
        {
            if(_highlight)
                ResetPosition(_highlight);
        }
        
        private void MoveUp(Transform hl)
        {
            hl.localPosition += _offset;
        }

        private void ResetPosition(Transform hl)
        {
            hl.localPosition -= _offset;
        }
    }
    
}