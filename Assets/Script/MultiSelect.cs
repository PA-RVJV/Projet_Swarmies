using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS.InputHandlers
{
    // Classe statique MultiSelect pour aider avec la sélection multiple dans l'interface utilisateur.
    public static class MultiSelect
    {
        // Variable statique privée pour stocker une texture blanche utilisée pour dessiner les rectangles et les bordures.
        private static Texture2D _whiteTexture;

        // Propriété publique pour accéder à la texture blanche. Si elle n'existe pas, elle est créée.
        public static Texture2D WhiteTexture
        {
            get
            {
                if (_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(1, 1);
                    _whiteTexture.SetPixel(0, 0, Color.white);
                    _whiteTexture.Apply();
                }

                return _whiteTexture;
            }
        }

        // Méthode pour dessiner un rectangle plein sur l'écran avec une couleur spécifiée.
        public static void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, WhiteTexture);
        }

        // Méthode pour dessiner le bord d'un rectangle sur l'écran avec une épaisseur et une couleur spécifiées.
        public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            // Dessine les quatre côtés du rectangle pour créer une bordure.
            // HAUT
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // BAS
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
            // GAUCHE
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // DROITE
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        }

        // Méthode pour obtenir un rectangle de sélection à partir de deux positions à l'écran.
        public static Rect GetScreenRect(Vector3 screenPos1, Vector3 screenPos2)
        {
            // Ajuste les positions pour que le rectangle puisse être dessiné correctement dans tous les sens.
            screenPos1.y = Screen.height - screenPos1.y;
            screenPos2.y = Screen.height - screenPos2.y;
            
            // Détermine les coins inférieur droit et supérieur gauche du rectangle.
            Vector3 bottomRight = Vector3.Max(screenPos1, screenPos2);
            Vector3 topLeft = Vector3.Min(screenPos1, screenPos2);
            
            // Crée et retourne le rectangle.
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        // Méthode pour obtenir les limites en espace de vue à partir de deux positions à l'écran.
        public static Bounds GetVPBounds(Camera camera, Vector3 screenPos1, Vector3 screenPos2)
        {
            // Convertit les positions à l'écran en positions dans l'espace de vue.
            Vector3 viewportPosition1 = camera.ScreenToViewportPoint(screenPos1);
            Vector3 viewportPosition2 = camera.ScreenToViewportPoint(screenPos2);

            // Détermine les positions minimales et maximales pour créer les limites.
            Vector3 min = Vector3.Min(viewportPosition1, viewportPosition2);
            Vector3 max = Vector3.Max(viewportPosition1, viewportPosition2);

            // Ajuste les valeurs z pour les limites de la caméra.
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            // Crée et retourne les limites.
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            return bounds;
        }
    }
}