using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackWave : MonoBehaviour
{
    public Transform targetBuilding; // Référence au bâtiment cible
    public float radius = 5.0f; // Rayon de la zone circulaire autour du bâtiment

    private void Start()
    {
        // Pour chaque enfant de cet objet
        foreach (Transform enemy in transform)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                // Calculer une position aléatoire autour du bâtiment
                Vector3 randomPosition = GetRandomPositionAroundBuilding();
                // Définir la destination de l'agent
                agent.SetDestination(randomPosition);
            }
        }
    }

    private Vector3 GetRandomPositionAroundBuilding()
    {
        // Calculer un angle aléatoire
        float angle = Random.Range(0, 2 * Mathf.PI);
        // Calculer la position aléatoire sur le cercle
        Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        // Retourner la position finale autour du bâtiment
        return targetBuilding.position + position;
    }
}