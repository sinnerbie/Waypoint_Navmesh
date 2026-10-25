using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    Transform goal; //Goal from A* algorithm
    [SerializeField] float speed = 5.0f;          //Velocidad del tanque
    [SerializeField] float accuracy = 1.0f;		 //Distancia por debajo de la cual considero que he alcanzado el nodo
    [SerializeField] float rotSpeed = 2.0f;       //Velocidad de rotacion del tanque
    public GraphManager wpManager; //All the graph points and links
    GameObject[] wps;            //Nodos del grafo
    GameObject currentNode;		 //El nodo mas cercano en cada momento
    int currentWP = 0;           //Indice a lo largo del camino que seguimos, no tiene que ver con el numero/posicion de cada WP
    Graph g;                     //Grafo

    public GameObject helipadWaypoint;
    public GameObject ruinsWaypoint;
    public GameObject selectedWaypoint;

    private bool alreadyFollowingPath = false;

    // Start is called before the first frame update
    void Start()
    {
        //Inicializar wps, g y currentNode
        g = wpManager.graph;
        wps = wpManager.waypoints;
        currentNode = GetClosestNode();
    }

    public void GoToHeli()
    {
        if (alreadyFollowingPath) return;
        currentWP = 0;
        if (g.AStar(currentNode, helipadWaypoint))
            g.printPath();
        else
            Debug.Log("Couldn't find path to Helipad");
    }

    public void GoToRuin()
    {
        if (alreadyFollowingPath) return;
        currentWP = 0;
        if (g.AStar(currentNode, ruinsWaypoint))
            g.printPath();
        else
            Debug.Log("Couldn't find path to Ruins");
    }

    public void GoToSelected()
    {
        if (alreadyFollowingPath || selectedWaypoint == null) return;
        currentWP = 0;
        if (g.AStar(currentNode, selectedWaypoint))
            g.printPath();
        else
            Debug.Log("Couldn't find path to selected waypoint");
    }

    
    void Update()
    {
        //Testear si no hay un camino definido (empty path) o currentWP esta ya al final del camino
        if (g.getPathLength() == 0) return;
        bool atEndOfPath = Vector3.Distance(transform.position, g.getPathPoint(g.getPathLength() - 1).transform.position) <= accuracy;
        if (atEndOfPath) { alreadyFollowingPath = false; return; }
        //Guardamos en currentNode el nodo que tenemos mas cercano en este momento
        currentNode = GetClosestNode();
        //Si estamos suficientemente cerca al nodo currentWP, pasamos a buscar el siguiente
        if (Vector3.Distance(transform.position, g.getPathPoint(currentWP).transform.position) <= accuracy)
            currentWP++;
        //Si aun no hemos llegado al final del camino, nos dirigimos al siguiente nodo
        if (transform.position != g.getPathPoint(g.getPathLength() - 1).transform.position)
        {
            alreadyFollowingPath = true;
            //Guardamos en goal el nuevo nodo a visitar
            goal = g.getPathPoint(currentWP).transform;
            //Miramos hacia nuestro destino (goal)
            Vector3 direction = goal.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            //Movemos el tanque
            transform.position = Vector3.MoveTowards(transform.position, goal.position, speed * Time.deltaTime);
        }
            
        g.debugDraw();

    }

    GameObject GetClosestNode()
    {
        GameObject closestNode = null;
        float distance = Mathf.Infinity;
        foreach (GameObject node in wps)
        {
            float distToNode = Vector3.Distance(transform.position, node.transform.position);
            if (distToNode < distance)
            {
                distance = distToNode;
                closestNode = node;
            }
        }
        return closestNode;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        if (goal != null)
            Gizmos.DrawLine(transform.position, goal.position);
    }
}
