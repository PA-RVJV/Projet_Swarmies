using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 0.06f;
    public float zoomSpeed = 10.0f;
    public float rotateSpeed= 0.1f;
 
    public float maxHeight= 40f;
    public float minHeight= 8f;

    private Vector2 p1;
    private Vector2 p2;

    private Camera _cam;
    
    public Terrain clampTo; // le mesh auquel on veut se clamp
    private Vector3 _topLeftClamp;
    private Vector3 _bottomRightClamp;
    
    void Start()
    {
        _cam = GetComponent<Camera>();
        ComputeCameraBounds();
    }

    void Update()
    {
        var speed = this.speed;
        if(Input.GetKey(KeyCode.LeftShift))
        {
            speed = 0.06f;
            zoomSpeed = 20.0f;
        }
        
        float hsp = transform.position.y * speed * Input.GetAxis("Horizontal");
        float vsp = transform.position.y * speed * Input.GetAxis("Vertical");
        float scrollSp = Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        if ((transform.position.y >= maxHeight) && (scrollSp > 0))
        {
            scrollSp = 0;
        }
        else if ((transform.position.y <= minHeight) && (scrollSp < 0))
        {
            scrollSp = 0;
        }
        
        if((transform.position.y + scrollSp) > maxHeight)
        {
            scrollSp = maxHeight - transform.position.y;
        }
        else if((transform.position.y + scrollSp) < minHeight)
        {
            scrollSp = minHeight - transform.position.y;
        }
        Vector3 verticalMove = new Vector3(0, scrollSp, 0);
        Vector3 lateralMove = hsp * transform.right;
        Vector3 forwardMove = transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= vsp;

        Vector3 move = verticalMove + lateralMove + forwardMove;

        var camPos = transform.position;
        
        camPos += move;
        camPos.x = Mathf.Clamp(camPos.x, _topLeftClamp.x, _bottomRightClamp.x);
        camPos.z = Mathf.Clamp(camPos.z, _bottomRightClamp.z, _topLeftClamp.z); 

        transform.position = camPos;

        // getCameraRotation();
    }

    private void getCameraRotation()
    {
        if (Input.GetMouseButtonDown(2))
        {
            p1 = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            p2 = Input.mousePosition;

            float dx = (p2 - p1).x * rotateSpeed;
            float dy = (p2 - p1).y * rotateSpeed;

            transform.rotation *= Quaternion.Euler(new Vector3(0, dx, 0));
            transform.transform.rotation *= Quaternion.Euler(new Vector3(-dy, 0, 0));
            p1 = p2;
        }
    }

    private void ComputeCameraBounds()
    {
        var terrainTransform = clampTo.transform;
        var camTransform = _cam.transform;
        var camPosition = camTransform.position;
        var camRotation = camTransform.eulerAngles;
        var terrainScale = terrainTransform.lossyScale;
        var meshWidth = clampTo.terrainData.size.x;
        var meshHeight = clampTo.terrainData.size.z;
        
        // On tient compte de la rotation de la camera pour ajouter
        // ou enlever du rab sur le clamp de celle-ci par rapport au mesh
        var zAngle = camRotation.z;
        var xAngle = camRotation.x;
        var xAngleBas =  _cam.fieldOfView / 2 * xAngle;
        var zAngleDroit =  (_cam.fieldOfView / 2 * _cam.aspect) * zAngle;
        
        // on rajoute du bourrage sur le clamp de la camera qui correspond a son angle
        // ici c'est le coté opposé qu'on veut vu qu'on a l'angle et le coté adjacent
        // du coup tangente. un angle de zero est une camera horizontale, un angle de 90
        // est une camera braquee sur le sol
        var zPadding = Mathf.Tan(Mathf.Deg2Rad * (90 - xAngle)) * camPosition.y; 
        var xPadding = Mathf.Tan(Mathf.Deg2Rad * (zAngle)) * camPosition.y;
        
        _topLeftClamp = new Vector3(
            x: (terrainTransform.position.x) * terrainScale.x * -1 - xPadding, 
            y: 0,
            z: (meshHeight) * terrainScale.z - zPadding
        );
        _bottomRightClamp = new Vector3(
            x: (meshWidth) * terrainScale.x + xPadding,
            y: 0,
            z: ((terrainTransform.position.z) * terrainScale.z + zPadding )* -1 
        );
    }
}
