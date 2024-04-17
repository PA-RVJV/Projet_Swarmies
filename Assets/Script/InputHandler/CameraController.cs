using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.InputHandlers
{
    public class CameraController : MonoBehaviour
    {
        public float speed = 0.06f; // Vitesse de déplacement de la caméra
        public float zoomSpeed = 10.0f; // Vitesse de zoom de la caméra
        public float rotateSpeed= 0.1f; // Vitesse de rotation de la caméra
     
        public float maxHeight= 40f; // Hauteur maximale de la caméra
        public float minHeight= 8f; // Hauteur minimale de la caméra

        private Vector2 p1; // Position initiale du curseur pour la rotation
        private Vector2 p2; // Position finale du curseur pour la rotation

        private Camera _cam; // Référence à l'objet Camera
        
        public Terrain clampTo; // Terrain utilisé pour limiter le déplacement de la caméra
        private Vector3 _topLeftClamp; // Limite supérieure gauche pour le déplacement de la caméra
        private Vector3 _bottomRightClamp; // Limite inférieure droite pour le déplacement de la caméra
        
        void Start()
        {
            _cam = GetComponent<Camera>(); // Obtient le composant Camera de cet objet
            ComputeCameraBounds(); // Calcule les limites de déplacement de la caméra en fonction du terrain
        }

        void Update()
        {
            var speed = this.speed; // Utilise la vitesse de déplacement définie
            if(Input.GetKey(KeyCode.LeftShift))
            {
                speed = 0.06f; // Augmente la vitesse de déplacement si LeftShift est pressé
                zoomSpeed = 20.0f; // Augmente la vitesse de zoom si LeftShift est pressé
            }
            
            // Calcule le déplacement horizontal et vertical basé sur les entrées de l'utilisateur
            float hsp = transform.position.y * speed * Input.GetAxis("Horizontal");
            float vsp = transform.position.y * speed * Input.GetAxis("Vertical");
            
            // Calcule le déplacement de zoom basé sur la molette de la souris
            float scrollSp = Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

            // Limite le zoom pour ne pas dépasser les hauteurs maximale et minimale
            if ((transform.position.y >= maxHeight) && (scrollSp > 0))
            {
                scrollSp = 0;
            }
            else if ((transform.position.y <= minHeight) && (scrollSp < 0))
            {
                scrollSp = 0;
            }
            
            // Assure que le zoom ne dépasse pas les limites définies
            if((transform.position.y + scrollSp) > maxHeight)
            {
                scrollSp = maxHeight - transform.position.y;
            }
            else if((transform.position.y + scrollSp) < minHeight)
            {
                scrollSp = minHeight - transform.position.y;
            }
            
            // Calcule le déplacement total de la caméra
            Vector3 verticalMove = new Vector3(0, scrollSp, 0);
            Vector3 lateralMove = hsp * transform.right;
            Vector3 forwardMove = transform.forward;
            forwardMove.y = 0;
            forwardMove.Normalize();
            forwardMove *= vsp;

            Vector3 move = verticalMove + lateralMove + forwardMove;

            // Applique le déplacement en tenant compte des limites du terrain
            var camPos = transform.position;
            
            camPos += move;
            camPos.x = Mathf.Clamp(camPos.x, _topLeftClamp.x, _bottomRightClamp.x);
            camPos.z = Mathf.Clamp(camPos.z, _bottomRightClamp.z, _topLeftClamp.z); 

            transform.position = camPos;

            // Appelle la méthode de rotation de la caméra (actuellement non appelée dans Update)
        }

        private void getCameraRotation()
        {
            // Gère la rotation de la caméra avec le bouton du milieu de la souris
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
            // Calcule les limites de déplacement de la caméra en fonction de la taille du terrain
            // et de la position et de la rotation de la caméra pour éviter que la vue ne sorte du terrain
            
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
            var xPadding = Mathf.Tan(Mathf.Deg2Rad * zAngle) * camPosition.y;
            
            _topLeftClamp = new Vector3(
                x: terrainTransform.position.x * terrainScale.x * -1 - xPadding, 
                y: 0,
                z: meshHeight * terrainScale.z - zPadding
            );

            _bottomRightClamp = new Vector3(
                x: meshWidth * terrainScale.x + xPadding,
                y: 0,
                z: (terrainTransform.position.z * terrainScale.z + zPadding )* -1 
            );
        }

        public static Vector3 ProjectedPosition(Transform camera){
            var camRotation = camera.eulerAngles;
            var camPosition = camera.position;

            var zAngle = camRotation.z;
            var xAngle = camRotation.x;

            var zPadding = Mathf.Tan(Mathf.Deg2Rad * xAngle) * camPosition.y; 
            var xPadding = Mathf.Tan(Mathf.Deg2Rad * zAngle) * camPosition.y;

            return new Vector3(xPadding, camPosition.y, zPadding);
        }
    }
}

