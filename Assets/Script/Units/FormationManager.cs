using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FormationManager
{
    public static List<Vector3> GetFormationPositions(Vector3 targetPosition, int unitCount, float spacing, string formationType)
    {
        switch (formationType)
        {
            case "quinconce":
                return GetQuinconceFormation(targetPosition, unitCount, spacing);
            case "diamond":
                return GetDiamondFormation(targetPosition, unitCount, spacing);
            default:
                return GetGridFormation(targetPosition, unitCount, spacing);
        }
    }

    private static List<Vector3> GetGridFormation(Vector3 targetPosition, int unitCount, float spacing)
    {
        List<Vector3> positions = new List<Vector3>();
        int rows = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
        int cols = Mathf.CeilToInt(unitCount / (float)rows);
        Vector3 offset = new Vector3((cols - 1) * spacing / 2, 0, (rows - 1) * spacing / 2);

        for (int i = 0; i < unitCount; i++)
        {
            int row = i / cols;
            int col = i % cols;
            Vector3 position = targetPosition + new Vector3(col * spacing, 0, row * spacing) - offset;
            positions.Add(position);
        }

        return positions;
    }

    private static List<Vector3> GetQuinconceFormation(Vector3 targetPosition, int unitCount, float spacing)
    {
        List<Vector3> positions = new List<Vector3>();
        int rows = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
        int cols = Mathf.CeilToInt(unitCount / (float)rows);
        Vector3 offset = new Vector3((cols - 1) * spacing / 2, 0, (rows - 1) * spacing / 2);

        for (int i = 0; i < unitCount; i++)
        {
            int row = i / cols;
            int col = i % cols;
            float colOffset = (row % 2 == 0) ? 0 : spacing / 2;
            Vector3 position = targetPosition + new Vector3(col * spacing + colOffset, 0, row * spacing) - offset;
            positions.Add(position);
        }

        return positions;
    }

    private static List<Vector3> GetDiamondFormation(Vector3 targetPosition, int unitCount, float spacing)
    {
        List<Vector3> positions = new List<Vector3>();
        int layers = Mathf.CeilToInt((Mathf.Sqrt(1 + 8 * unitCount) - 1) / 2);  // Derivation from quadratic equation for layers
        int unitCounter = 0;

        for (int layer = 0; layer < layers; layer++)
        {
            int unitsInLayer = 2 * layer + 1;
            for (int i = 0; i < unitsInLayer && unitCounter < unitCount; i++)
            {
                float angle = i * (360.0f / unitsInLayer);
                Vector3 position = targetPosition + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), 0, Mathf.Sin(Mathf.Deg2Rad * angle)) * spacing * layer;
                positions.Add(position);
                unitCounter++;
            }
        }

        return positions;
    }
}