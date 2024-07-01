using UnityEngine;
using UnityEngine.AI;

public class FlockAgent : MonoBehaviour
{
    public float separationWeight = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        // Find all nearby agents (including self)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);

        foreach (Collider collider in colliders)
        {
            if (collider && collider != this.GetComponent<Collider>())
            {
                Vector3 direction = transform.position - collider.transform.position;
                float distance = direction.magnitude;

                if (distance > 0)
                {
                    // Separation: Move away from nearby agents
                    separation += direction.normalized / distance;

                    // Alignment: Align with nearby agents' heading
                    alignment += collider.transform.forward;

                    // Cohesion: Move towards the center of nearby agents
                    cohesion += direction;
                }
            }
        }

        // Apply weights to behaviors
        Vector3 moveDirection = (separation * separationWeight +
                                 alignment * alignmentWeight +
                                 cohesion * cohesionWeight).normalized;

        // Move the agent
        if (moveDirection != Vector3.zero)
        {
            agent.Move(moveDirection * Time.deltaTime * agent.speed);
        }
    }
}