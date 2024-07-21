using UnityEngine;
using UnityEngine.AI;
using PS.Units.Enemy;
using System.Collections.Generic;
using System.Collections;

public class EnemyAttackWave : MonoBehaviour
{
    public Transform targetBuilding; // Référence au bâtiment cible
    public float radius = 5.0f; // Rayon de la zone circulaire autour du bâtiment
    public Transform attackTarget; // Position cible pour les unités ennemies
    public int initialUnitsToSendPerWave; // Nombre d'unités à envoyer par vague
    public int unitsToKeepInDefense; // Nombre d'unités à garder en défense
    
    private int currentUnitsToSendPerWave; // Nombre actuel d'unités à envoyer par vague
    private List<EnemyUnit> enemyUnits;
    private List<EnemyUnit> defendingUnits;
    
    private void Start()
    {
        enemyUnits = new List<EnemyUnit>();
        defendingUnits = new List<EnemyUnit>();
        
        // Pour chaque enfant de cet objet
        foreach (Transform enemy in transform)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            EnemyUnit enemyScript = enemy.GetComponent<EnemyUnit>();
            if (agent != null && enemyScript != null)
            {
                enemyUnits.Add(enemyScript);
            }
        }
        
        currentUnitsToSendPerWave = initialUnitsToSendPerWave;
        StartCoroutine(DefenseThenAttack());
    }

    private IEnumerator DefenseThenAttack()
    { 
        // Sélectionne un groupe d'unités pour la défense
        for (int i = 0; i < unitsToKeepInDefense && enemyUnits.Count > 0; i++)
        {
            defendingUnits.Add(enemyUnits[0]);
            enemyUnits.RemoveAt(0);
        }

        SetUnitsToDefend();
        
        // Envoie des vagues successives d'unités toutes les 3 minutes
        while (true)
        {
            UpdateAttackGroup();
            SendUnitsToAttack();
            yield return new WaitForSeconds(20); // Attend 3 minutes avant la prochaine vague
            currentUnitsToSendPerWave++; // Augmente le nombre d'unités par vague
        }
    }
    
    private void UpdateAttackGroup()
    {
        // Réinitialise la liste des unités d'attaque en excluant les unités de défense
        enemyUnits.Clear();
        
        foreach (Transform enemy in transform)
        {
            EnemyUnit enemyScript = enemy.GetComponent<EnemyUnit>();
            if (enemyScript != null && !defendingUnits.Contains(enemyScript))
            {
                enemyUnits.Add(enemyScript);
            }
        }
    }
    
    public void SendUnitsToAttack()
    {
        if (attackTarget == null)
        {
            attackTarget = FindNewAttackTarget();
            if (attackTarget == null)
            {
                Debug.LogWarning("No valid attack target found");
                return; // No valid targets
            }
        }
        
        int unitsSent = 0;
        for (int i = enemyUnits.Count - 1; i >= 0; i--)
        {
            if (unitsSent >= currentUnitsToSendPerWave) break;
            if (enemyUnits[i] != null && enemyUnits[i].aggroTarget == null)
            {
                enemyUnits[i].SetAttackTarget(attackTarget.position);
                unitsSent++;
            }
            else if (enemyUnits[i] == null)
            {
                enemyUnits.RemoveAt(i); // Remove destroyed units from the list
            }
        }
    }

    private Transform FindNewAttackTarget()
    {
        Collider[] playerUnits = Physics.OverlapSphere(transform.position, Mathf.Infinity, LayerMask.GetMask("PlayerUnits"));
        if (playerUnits.Length > 0)
        {
            return playerUnits[0].transform; // Select the first found player unit as the new target
        }
        return null;
    }
    
    public void SetUnitsToDefend()
    {
        foreach (var enemyUnit in defendingUnits)
        {
            if (enemyUnit != null)
            {
                enemyUnit.SetDefendTarget(GetRandomPositionAroundBuilding());
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