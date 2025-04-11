using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Assets.JSGAONA.Unidad1.Scripts {

    // Componentes requeridos para que funcione el script
    [RequireComponent(typeof(NavMeshAgent))]

    // Este script se emplea para gestionar la logica de los enemigos
    public class Enemy : MonoBehaviour {

        public GameObject projectilePrefab; // Prefab del proyectil



        // Variables visibles desde el inspector de Unity
        [SerializeField] private float speedRotation = 120;
        [SerializeField] private float chaseDistance = 3.5f;
        [SerializeField] private float approachDistance = 0.5f;
        [SerializeField] private float updateInterval  = 0.25f;

        [SerializeField] private LayerMask includeLayer;

        // Variables ocultas desde el inspector de Unity
        public bool isPlayerInRange = false;
        public bool inRange = false;
        private NavMeshAgent agent;
        public Transform player;
        public Transform Player { set => player = value; }



        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, es el primer
        // metodo en ejecutarse, se realiza la asignacion de componentes
        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }


        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, se ejecuta despues
        // de Awake, se realiza la asignacion de variables y configuracion del script
        private void Start() {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            if(player != null) StartCoroutine(UpdateEnemyBehavior());
        }
        
        // Metodo de llamada de Unity, se llama en el momento de que el GameObject es destruido
        private void OnDestroy() {
            if(player != null) StopCoroutine(UpdateEnemyBehavior());
        }


        // Metodo de llamada de Unity, se activa cuando el renderizador del objeto entra en el campo
        // de vision de la camara activa
        private void OnBecameVisible() {
            enabled = true;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        }


        // Metodo de llamada de Unity, se activa cuando el renderizador del objeto sale del campo de
        // vision de la camara.
        private void OnBecameInvisible() {
            enabled = false;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }


        // Metodo de llamada de Unity, se llama en cada frame del computador
        // Se realiza la logica de control del enemigo
        private void Update() {
            // Se valida que exista una referencia valida del personaje a seguir y este esta a rango
            if (isPlayerInRange) {
                // Mira al jugador suavemente
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0; // para evitar rotación vertical
                if (direction != Vector3.zero) {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,
                        speedRotation * Time.deltaTime);
                }
                // El enemigo esta en rango de alcance del personaje
                if(inRange) {

                    agent.ResetPath();
                    if (Time.time % 1 < Time.deltaTime) { AttackPlayer(); }

                } else {
                    agent.SetDestination(player.position);
                }
            }
        }
        private void AttackPlayer()
        {
            // Instanciar el proyectil y dirigirlo hacia el jugador
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Vector3 direction = (player.position - transform.position).normalized;
            projectile.GetComponent<Rigidbody>().velocity = direction * 10f; // Ajustar velocidad según prefieras
        }


        // Coroutine para optimizar las actualizaciones
        private IEnumerator UpdateEnemyBehavior() {
            // Mientras sea verdad, se ejecuta indefinidamente
            while (true) {
                float distance = Vector3.Distance(transform.position, player.position);
                bool playerDetected = distance <= chaseDistance;
                inRange = distance <= approachDistance;

                // El jugador a ingresado al radio de persecucion
                if(playerDetected) {
                    Vector3 directionToPlayer = (player.position - transform.position).normalized;
                    isPlayerInRange = !Physics.Raycast(transform.position, directionToPlayer,
                        distance, includeLayer);
                }

                yield return new WaitForSeconds(updateInterval);
            }
        }


    #if UNITY_EDITOR
        // Metodo de llamada de Unity, se emplea para visualizar en escena, acciones del codigo
        private void OnDrawGizmos() {
            // Se genera el gizmos para visualizar la posicion de los pies, validar piso
            Gizmos.color = isPlayerInRange ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    #endif
    }
}