using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager
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
        int layer = 1;
        int unitsInLayer = 1;
        int unitsPlaced = 0;

        positions.Add(targetPosition); // Central position
        unitsPlaced++;

        while (unitsPlaced < unitCount)
        {
            int unitsInThisLayer = 4 * layer;
            for (int i = 0; i < unitsInThisLayer && unitsPlaced < unitCount; i++)
            {
                float angle = (i / (float)unitsInThisLayer) * 2 * Mathf.PI;
                Vector3 position = targetPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spacing * layer;
                positions.Add(position);
                unitsPlaced++;
            }
            layer++;
        }

        return positions;
    }
}
