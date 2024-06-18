using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PS.InputHandlers
{
    public class CameraController : MonoBehaviour
    {
        public float keyboardSpeed,
            dragSpeed,
            screenEdgeSpeed,
            screenEdgeBorderSize,
            mouseRotationSpeed,
            followMoveSpeed,
            followRotationSpeed,
            minHeight, 
            maxHeight,
            zoomSensitivity,
            zoomSmoothing,
            mapLimitSmoothing;

        public Vector2 mapLimits, rotationLimits;
        public Vector3 followOffset;
        
        public Terrain clampTo; // Terrain utilisé pour limiter le déplacement de la caméra
        private Vector3 _topLeftClamp; // Limite supérieure gauche pour le déplacement de la caméra
        private Vector3 _bottomRightClamp; // Limite inférieure droite pour le déplacement de la caméra
        
        private Transform targetToFollow;
        private float zoomAmount = 1, yaw, pitch;
        KeyCode dragKey = KeyCode.Mouse2;
        KeyCode rotationKey = KeyCode.Mouse1;
        private Transform mainTransform;
        LayerMask groundMask;
        
        private bool isRotating = false;
        private float rotationKeyHeldTime = 0f;
        private float rotationDelay = 0.2f; // Délai de 0.5 seconde

        private Camera _cam; // Référence à l'objet Camera
        
        void Start()
        {
            mainTransform = transform;
            groundMask = LayerMask.GetMask("Ground");
            pitch = mainTransform.eulerAngles.x;
            _cam = GetComponent<Camera>();
        }

        void Update()
        {
            if (!targetToFollow)
            {
                Move();
            }
            else
            {
                FollowTarget();
            }

            Rotation();
            HeightCalculation();
            ComputeCameraBounds();
            LimitPosition();

            if (Input.GetKey(KeyCode.Space))
            {
                ResetTarget();
            }
        }

        void Move()
        {
            if (Input.GetKey(dragKey))
            {
                // clic et drag
                Vector3 desiredDragMove =
                    new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * dragSpeed;
                desiredDragMove = Quaternion.Euler(new Vector3(0, mainTransform.eulerAngles.y, 0)) * desiredDragMove *
                                  Time.deltaTime;
                desiredDragMove = mainTransform.InverseTransformDirection(desiredDragMove);
                
                mainTransform.Translate(desiredDragMove, Space.Self);
                ResetTarget();
                
            }
            else
            {
                // touche du clavier
                Vector3 desiredMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                desiredMove *= keyboardSpeed;
                desiredMove = Quaternion.Euler(new Vector3(0, mainTransform.eulerAngles.y, 0)) * desiredMove *
                              Time.deltaTime;
                desiredMove = mainTransform.InverseTransformDirection(desiredMove);
                
                mainTransform.Translate(desiredMove, Space.Self);
                
                // souris sur les bord de l'écran
                Vector3 desiredEdgeMove = new Vector3();
                Vector3 mousePos = Input.mousePosition;

                Rect leftRect = new Rect(0, 0, screenEdgeBorderSize, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorderSize, 0, screenEdgeBorderSize, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorderSize, Screen.width, screenEdgeBorderSize);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorderSize);
                
                desiredEdgeMove.x = leftRect.Contains(mousePos) ? -1 : rightRect.Contains(mousePos) ? 1 : 0;
                desiredEdgeMove.z = upRect.Contains(mousePos) ? 1 : downRect.Contains(mousePos) ? -1 : 0;

                desiredEdgeMove *= screenEdgeSpeed;
                desiredEdgeMove *= Time.deltaTime;
                desiredEdgeMove = Quaternion.Euler(new Vector3(0, mainTransform.eulerAngles.y, 0)) * desiredEdgeMove;
                desiredEdgeMove = mainTransform.InverseTransformDirection(desiredEdgeMove);
                
                mainTransform.Translate(desiredEdgeMove, Space.Self);
                ResetTarget();
            }
        }

        void Rotation()
        {
            if (Input.GetKey(rotationKey))
            {
                rotationKeyHeldTime += Time.deltaTime;

                if (rotationKeyHeldTime >= rotationDelay)
                {
                    isRotating = true;
                }

                if (isRotating)
                {
                    yaw += mouseRotationSpeed * Input.GetAxis("Mouse X");
                    pitch -= mouseRotationSpeed * Input.GetAxis("Mouse Y");

                    pitch = Mathf.Clamp(pitch, rotationLimits.x, rotationLimits.y);

                    mainTransform.eulerAngles = new Vector3(pitch, yaw, 0);
                    ResetTarget();
                }
            }
            else
            {
                rotationKeyHeldTime = 0f;
                isRotating = false;
            }
        }

        void LimitPosition()
        {
            mainTransform.position = Vector3.Lerp(mainTransform.position, new Vector3(
                    Mathf.Clamp(mainTransform.position.x, _topLeftClamp.x, _bottomRightClamp.x), mainTransform.position.y,
                    Mathf.Clamp(mainTransform.position.z, _bottomRightClamp.z, _topLeftClamp.z)),
                    Time.deltaTime * mapLimitSmoothing);

        }

        void HeightCalculation()
        {
            zoomAmount += -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSensitivity;
            zoomAmount = Mathf.Clamp01(zoomAmount);

            float distanceToGround = DistanceToGround();
            float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomAmount);

            mainTransform.position = Vector3.Lerp(mainTransform.position,
                new Vector3(mainTransform.position.x, targetHeight + distanceToGround, mainTransform.position.z),
                Time.deltaTime * zoomSmoothing);
        }

        private float DistanceToGround()
        {
            Ray ray = new Ray(mainTransform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {
                return hit.point.y; 
            }

            return 0;
        }

        
        void FollowTarget() // TODO besoin de setup un radius pour que la camera reste a distance de la cible et n'aille pas se foutre juste au dessus
        {
            // partie déplacement
            Vector3 targetPos =
                new Vector3(targetToFollow.position.x, mainTransform.position.y, targetToFollow.position.z) +
                followOffset;
            mainTransform.position =
                Vector3.MoveTowards(mainTransform.position, targetPos, Time.deltaTime * followMoveSpeed);
            
            // partie rotation
            if (followRotationSpeed > 0 && !Input.GetKey(rotationKey))
            {
                Vector3 targetDirection = (targetToFollow.position - mainTransform.position).normalized;
                Quaternion targetRotation = Quaternion.Lerp(mainTransform.rotation, Quaternion.LookRotation(targetDirection), followRotationSpeed * Time.deltaTime);
                mainTransform.rotation = targetRotation;

                pitch = mainTransform.eulerAngles.x;
                yaw = mainTransform.eulerAngles.y;
            }
        }

        public void SetTarget(Transform target)
        {
            targetToFollow = target;
        }

        public void ResetTarget()
        {
            targetToFollow = null;
        }

        private void ComputeCameraBounds()
        {
            // Calcule les limites de déplacement de la caméra en fonction de la taille du terrain
            // et de la position et de la rotation de la caméra pour éviter que la vue ne sorte du terrain
            
            var terrainTransform = clampTo.transform;
            var camTransform = mainTransform;
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

