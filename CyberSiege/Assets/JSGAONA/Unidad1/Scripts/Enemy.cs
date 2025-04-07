using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Assets.JSGAONA.Unidad1.Scripts
{
    // Componentes requeridos para que funcione el script
    [RequireComponent(typeof(NavMeshAgent))]

    // Este script se emplea para gestionar la logica de los enemigos
    public class Enemy : MonoBehaviour
    {
        // Variables visibles desde el inspector de Unity
        [SerializeField] private float chaseDistance = 0.5f;
        [SerializeField] private float updateInterval = 0.25f;
        [SerializeField] private float minimumDistance = 1.5f; // Distancia m�nima
        [SerializeField] private LayerMask obstacleLayer; // Capa para detectar obst�culos
        [SerializeField] private float eyeHeight = 1.5f; // Altura de los "ojos" del enemigo

        // Variables ocultas desde el inspector de Unity
        private bool isPlayerInRange = false;
        private bool hasLineOfSight = false; // Variable para la l�nea de visi�n
        private NavMeshAgent agent;
        private Transform player;
        private Vector3 initialPosition; // Para almacenar la posici�n inicial
        private bool isReturningToStart = false; // Para controlar el regreso a la posici�n inicial
        public Transform Player { set => player = value; }

        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, es el primer
        // metodo en ejecutarse, se realiza la asignacion de componentes
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, se ejecuta despues
        // de Awake, se realiza la asignacion de variables y configuracion del script
        private void Start()
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            initialPosition = transform.position; // Guardar la posici�n inicial
            StartCoroutine(UpdateEnemyBehavior());
        }

        // Metodo de llamada de Unity, se llama en el momento de que el GameObject es destruido
        private void OnDestroy()
        {
            StopCoroutine(UpdateEnemyBehavior());
        }

        // Metodo de llamada de Unity, se activa cuando el renderizador del objeto entra en el campo
        // de vision de la camara activa
        private void OnBecameVisible()
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        }

        // Metodo de llamada de Unity, se activa cuando el renderizador del objeto sale del campo de
        // vision de la camara.
        private void OnBecameInvisible()
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }

        // Verifica si hay l�nea de visi�n directa hasta el jugador
        private bool CheckLineOfSight()
        {
            if (player == null)
                return false;

            // Determinar posiciones de "ojos"
            Vector3 enemyEyePosition = transform.position + Vector3.up * eyeHeight;
            Vector3 playerEyePosition = player.position + Vector3.up * eyeHeight;

            // Direcci�n y distancia al jugador
            Vector3 directionToPlayer = playerEyePosition - enemyEyePosition;
            float distanceToPlayer = directionToPlayer.magnitude;

            // Solo verificar si est� dentro del rango de persecuci�n
            if (distanceToPlayer <= chaseDistance)
            {
                // Visualizaci�n del rayo para depuraci�n
                Debug.DrawRay(enemyEyePosition, directionToPlayer, hasLineOfSight ? Color.green : Color.red, updateInterval);

                // Verificar colisi�n con objetos
                RaycastHit hit;
                if (Physics.Raycast(enemyEyePosition, directionToPlayer.normalized, out hit, distanceToPlayer))
                {
                    // Verificar si lo que golpe� el rayo es el jugador o parte del jugador
                    if (hit.transform == player || hit.transform.IsChildOf(player))
                    {
                        return true; // El rayo golpe� al jugador, hay l�nea de visi�n
                    }
                    return false; // El rayo golpe� algo que no es el jugador
                }

                // Si no golpe� nada, probablemente hay l�nea de visi�n clara
                return true;
            }
            return false;
        }

        // Coroutine para optimizar las actualizaciones
        private IEnumerator UpdateEnemyBehavior()
        {
            // Mientras sea verdad, se ejecuta indefinidamente
            while (true)
            {
                if (player != null)
                {
                    // Verificar l�nea de visi�n primero
                    hasLineOfSight = CheckLineOfSight();

                    float distance = Vector3.Distance(transform.position, player.position);

                    // Solo est� en rango si tiene l�nea de visi�n directa
                    isPlayerInRange = distance <= chaseDistance && hasLineOfSight;

                    // ****************** 1) Enemigos que mantiene su distancia ****************** //
                    if (isPlayerInRange)
                    {
                        if (distance <= minimumDistance)
                        {
                            // Detener al agente si est� demasiado cerca
                            agent.ResetPath();
                            isReturningToStart = false;
                        }
                        else
                        {
                            // Perseguir al jugador
                            agent.SetDestination(player.position);
                            isReturningToStart = false;
                        }
                    }
                    else if (!isReturningToStart)
                    {
                        // Si el jugador est� fuera del rango o no hay l�nea de visi�n
                        agent.SetDestination(initialPosition);
                        isReturningToStart = true;
                    }
                }

                yield return new WaitForSeconds(updateInterval);
            }
        }

        // Metodo de llamada de Unity, se llama en cada frame del computador
        // Se simplifica para evitar problemas
        private void Update()
        {
            // Verificar si ha vuelto a la posici�n inicial
            if (isReturningToStart && Vector3.Distance(transform.position, initialPosition) < 0.1f)
            {
                isReturningToStart = false;
                agent.ResetPath(); // Detener al agente cuando llega a la posici�n inicial
            }
        }

        // M�todo para visualizar el rango de detecci�n en el editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, minimumDistance);
        }
    }
}