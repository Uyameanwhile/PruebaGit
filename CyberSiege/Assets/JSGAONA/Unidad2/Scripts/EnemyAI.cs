using Assets.JSGAONA.Unidad2.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    [SerializeField] private AIState currentState;

public float GetDistance()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    public bool DetecteObstacle()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        return Physics.Raycast(transform.position, directionToPlayer);
    }

      
        
}
