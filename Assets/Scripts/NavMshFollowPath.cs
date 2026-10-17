using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class NavMshFollowPath : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private GameObject helipadWypt;
    [SerializeField] private GameObject ruinsWypt;
    [SerializeField] private GameObject selectedWypt;

    private Transform target;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target == null) return;

        if (Vector3.Distance(transform.position, target.position) < 1)
        {   
            agent.isStopped = true; 
            target = null; 
        }
        else
        { 
            agent.isStopped = false; 
            agent.SetDestination(target.position); 
        }
    }

    public void GoToHeli()
    {
        target = helipadWypt.transform;
    }

    public void GoToRuins()
    {
        target = ruinsWypt.transform;
    }

    public void GoToSelected()
    {
        if (selectedWypt != null)
            target = selectedWypt.transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        if (target == null || agent == null) return;
        Vector3[] corners = agent.path.corners;

        for (int i = 0; i < corners.Length; i++)
            if (i != corners.Length - 1) Gizmos.DrawLine(corners[i], corners[i + 1]);
    }
}
