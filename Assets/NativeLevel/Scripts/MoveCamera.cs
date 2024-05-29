using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

public class MoveCamera : MonoBehaviour
{
    public float scrollSpeed = 1f;
    public MeshFilter clampTo;
    private Camera _cam;
    private Vector2 _touch;
    private Gamepad _gamepad;
    private Vector3 _topLeftPos;
    private Vector3 _bottomRightPos;
    private WorldEventsScript _worldEventsScript;

    private void OnEnable()
    {
        _gamepad = Gamepad.current;
        _worldEventsScript = FindObjectOfType<WorldEventsScript>();
        _worldEventsScript.EventLevelChanged.AddListener(OnLevelChange);
    }

    private void OnDisable()
    {
        _gamepad = null;
        _worldEventsScript.EventLevelChanged.RemoveListener(OnLevelChange);
    }

    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
        ComputeCameraBounds();
    }
    
    void OnLevelChange(MeshFilter meshFilter)
    {
        clampTo = meshFilter;
        ComputeCameraBounds();
    }
    
    void ComputeCameraBounds()
    {
        var mesh = clampTo.mesh;
        var terrainTransform = clampTo.transform;
        var camTransform = _cam.transform;
        var camPosition = camTransform.position;
        var camRotation = camTransform.eulerAngles;
        var terrainScale = terrainTransform.lossyScale;
        var meshWidth = mesh.bounds.size.x;
        var meshHeight = mesh.bounds.size.z;
        
        // On tient compte de la rotation de la camera pour ajouter
        // ou enlever du rab sur le clamp de celle-ci par rapport au mesh
        var zAngle = camRotation.z;
        var xAngle = 90 - camRotation.x;
        var xAngleBas =  _cam.fieldOfView / 2 * xAngle;
        var zAngleDroit =  (_cam.fieldOfView / 2 * _cam.aspect) * zAngle;
        var zPadding = Mathf.Tan(Mathf.Deg2Rad * xAngle) * camPosition.y;
        var xPadding = Mathf.Tan(Mathf.Deg2Rad * zAngle) * camPosition.y;
        
        _topLeftPos = new Vector3(
            x: (meshWidth / 2f) * terrainScale.x * -1 - xPadding, 
            y: 0,
            z: (meshHeight / 2f) * terrainScale.z - zPadding
            );
        _bottomRightPos = new Vector3(
            x: (meshWidth / 2f) * terrainScale.x + xPadding,
            y: 0,
            z: ((meshHeight / 2f) * terrainScale.z + zPadding )* -1 
            );
    }
    
    // Update is called once per frame
    void Update()
    {
        _gamepad = Gamepad.current;
        
        var dx = 0f; // deplacement horizontal cette frame
        var dy = 0f; // deplacement vertical, devrait etre z, mais tous les input nous passe des y
        var pos = _cam.transform.position;
        Debug.DrawLine(_topLeftPos, _bottomRightPos, Color.red);

        #region Gamepad

        if (_gamepad != null)
        {
            var sx = _gamepad.leftStick.x.value;
            var sy = _gamepad.leftStick.y.value;
            if (sx != 0 || sy != 0)
            {
                dx = sx;
                dy = sy;
            }
        }

        #endregion
        
        #region mouse
        var mp = Mouse.current.position;
        var rect = _cam.pixelRect;

        if (mp.x.value is >= 0 and < 5)
        {
            dx = -scrollSpeed;
        }
        else if(mp.x.value  <= rect.width && mp.x.value > rect.width - 5)
        {
            dx = scrollSpeed;
        }

        if (mp.y.value is >= 0 and < 5)
        {
            dy = -scrollSpeed;
        }
        else if (mp.y.value <= rect.height && mp.y.value > rect.height - 15)
        {
            dy = scrollSpeed;
        }
        #endregion
        
        #region keyboard
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            dx = -scrollSpeed;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            dx = scrollSpeed;
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            dy = scrollSpeed;
        }
        else if (Keyboard.current.downArrowKey.isPressed)
        {
            dy = -scrollSpeed;
        }
        #endregion

        #region TouchScreen
        if(Input.touchSupported && Input.touchCount == 2)
        {
            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    this._touch = touch.position;
                    break;
                case TouchPhase.Moved:
                    dx = touch.position.x - _touch.x;
                    dy = touch.position.y - _touch.y;
                    break;
            }
        }
        #endregion

        #region LaptopTrackPad

        var dtp = Input.mouseScrollDelta;
        if (dtp.x != 0 || dtp.y != 0)
        {
            dx = -dtp.x;
            dy = dtp.y;
        }

        #endregion
        
        pos.x += dx;
        pos.z += dy;
        pos.x = Mathf.Clamp(pos.x, _topLeftPos.x, _bottomRightPos.x);
        pos.z = Mathf.Clamp(pos.z, _bottomRightPos.z, _topLeftPos.z);
        
        _cam.transform.position = pos;
    }
}
