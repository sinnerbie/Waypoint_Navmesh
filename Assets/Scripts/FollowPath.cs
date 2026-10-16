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
        if (g.AStar(currentNode, helipadWaypoint))
            Debug.Log("Going to Helipad");
        else
            Debug.Log("Couldn't find path to Helipad");
    }

    public void GoToRuin()
    {
        if (g.AStar(currentNode, ruinsWaypoint))
            Debug.Log("Going to Ruins");
        else
            Debug.Log("Couldn't find path to Ruins");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Testear si no hay un camino definido (empty path) o currentWP esta ya al final del camino
        if (g.getPathLength() == 0 || transform.position == g.getPathPoint(g.getPathLength() - 1).transform.position)
            return;
        else
            Debug.Log("Path Found");
        //Guardamos en currentNode el nodo que tenemos mas cercano en este momento
        currentNode = GetClosestNode();
        //Si estamos suficientemente cerca al nodo currentWP, pasamos a buscar el siguiente
        if (Vector3.Distance(transform.position, g.getPathPoint(currentWP).transform.position) <= accuracy)
            currentWP++;
        //Si aun no hemos llegado al final del camino, nos dirigimos al siguiente nodo
        if (transform.position != g.getPathPoint(g.getPathLength() - 1).transform.position)
        {
            Debug.Log("Going to next waypoint");
            //Guardamos en goal el nuevo nodo a visitar
            goal = g.getPathPoint(currentWP).transform;
            //Miramos hacia nuestro destino (goal)
            Quaternion targetRotation = Quaternion.LookRotation(goal.position);
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
}
