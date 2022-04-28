using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointsUI : MonoBehaviour
{


    void Awake()
    {

        //set last child position on the tower position

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Vector3 towerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            GameObject lastWaypoint = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject;
            lastWaypoint.transform.position = towerPosition;
        }
        
    }



    private void Start()
    {
        if (GameObject.Find("Waypoints Holder") != null)
        {
            transform.parent = GameObject.Find("Waypoints Holder").transform;
        }
    }


    void OnDrawGizmos()
    {
            
        // dispays waypoints and their paths
        Transform pathHolder = gameObject.transform;
        {
            Vector2 startPosition = pathHolder.GetChild(0).position;
            Gizmos.color = Color.red;
            Vector2 previousPosition = startPosition;

            for (int i = 0; i < pathHolder.childCount - 1; i++)
            {
                Transform waypoint = pathHolder.GetChild(i);
                Gizmos.DrawSphere(waypoint.position, .1f);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(previousPosition, waypoint.position);
                previousPosition = waypoint.position;
            }
        }

    }

}
