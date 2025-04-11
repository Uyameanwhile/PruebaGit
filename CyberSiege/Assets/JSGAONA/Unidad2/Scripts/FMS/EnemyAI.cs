using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Assets.JSGAONA.Unidad2.Scripts.FMS.Temple;
using System.Threading;
using System.Collections;

namespace Assets.JSGAONA.Unidad2.Scripts.FMS {

    [RequireComponent(typeof(NavMeshAgent))]
    // Se emplea este script para generar la IA del enemigo con NavMeshAgent
    public class EnemyAI : MonoBehaviour, IHackeable {

        private bool estaAnulado = false;
        private Coroutine rutinaDuracionAnulacion;

        // Variables visibles desde el inspector de Unity
        [Header("Adjust movement")]
        [SerializeField] private float speedMovement = 2.5f;
        [SerializeField] private float speedRotation = 120;
        [SerializeField] private LayerMask includeLayer;

        
        [Header("Adjust States Temple")]
        [SerializeField] private AIState currentState;
        [SerializeField] private AIBehaviorTemplate behaviorTemplate;

        public Transform player;

        // Variables ocultas desde el inspector de Unity
        private bool inRange = false;
        private float nextCheck = 0.25f;
        private NavMeshAgent agent;
        private Vector3 initialPoint;
        private Quaternion initialRotation;
        private readonly Dictionary<string, ActiveState> ActiveStates = new ();



        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, es el primer
        // metodo en ejecutarse, se realiza la asignacion de componentes
        private void Awake(){
            agent = GetComponent<NavMeshAgent>();
        }


        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, se ejecuta despues
        // de Awake, se realiza la asignacion de variables y configuracion del script
        private void Start() {
            agent.speed = speedMovement;
            initialPoint = transform.position;
            initialRotation = transform.rotation;

            // El pj dispone de un estado de reposo
            if (behaviorTemplate.DefaultState != null) {
                AddState(behaviorTemplate.DefaultState, behaviorTemplate.DefaultStateTemplate);
            }
            // El pj dispone de un estado de persecucion
            if (behaviorTemplate.ChaseState != null) {
                AddState(behaviorTemplate.ChaseState, behaviorTemplate.ChaseTemplate);
            }
            // El pj dispone de un estado de reinicio
            if (behaviorTemplate.ChaseState != null) {
                AddState(behaviorTemplate.ResetState, null);
            }
            // El pj dispone de un estado de ataque basico
            if (behaviorTemplate.BasicAttackState != null) {
                AddState(behaviorTemplate.BasicAttackState, null);
            }

            // Se recorre todos los estados y se los inicializa
            foreach (ActiveState activeState in ActiveStates.Values) {
                activeState.State.Initialize(this, activeState.StateTemplate);
            }
            // Se establece el estado por defecto
            currentState = GetDefaultState();
            currentState.Enter();
        }


        // Implementación del método de la interfaz IHackeable
        public void Anular(float duracion)
        {
            // Iniciar la corrutina que manejará la desactivación temporal
            StartCoroutine(DesactivarTemporalmente(duracion));
        }

        private IEnumerator DesactivarTemporalmente(float duracion)
        {
            // Desactivar el GameObject
            gameObject.SetActive(false);

            // Esperar la duración especificada
            yield return new WaitForSeconds(duracion);

            // Reactivar el GameObject
            gameObject.SetActive(true);
        }

        // Metodo de llamada de Unity, se llama en cada frame del computador
        // Se realiza la logica de control del enemigo mediante Estados
        private void Update() {
            // Si está anulado, no ejecutar comportamientos normales
            if (estaAnulado) return;

            // Se valida que el estado a ejecutar no sea nulo
            if (currentState != null) currentState.Execute();

            // El enemigo esta a rango del jugador

            if(inRange) {
                // Mira al jugador suavemente
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0;
                // Se voltea a ver simepre y cuando no exista movimiento
                if (direction != Vector3.zero) {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    float newSpeedRot = speedRotation * Time.deltaTime;
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, newSpeedRot);
                }
            }
        }


        // Se emplea este metodo para poder agregar estados al diccionario
        private void AddState(AIState state, AIStateTemplate template) {
            // Se crea un nuevo ActiveState
            ActiveState newState = new ActiveState {
                State = Instantiate(state),
                StateTemplate = template
            };
            ActiveStates.Add(state.name, newState);
        }


        // Metodo que permite cambiar el estado del enemigo
        public void ChangeState(AIState newState) {
            // Se valida que el estado actual no sea nulo para dejarlo
            if (currentState != null) {
                currentState.Exit();
            }
            // Se fija el nuevo estado
            currentState = newState;
            if (currentState != null) {
                currentState.Enter();
            }
        }


        // Metodo que sirve para detectar la fecuencia de actualizacion de cada estado
        public bool CheckInterval() {
            if (Time.timeSinceLevelLoad <= nextCheck) return false;
            // Se justa el tiempo para determinar el proximo chequeo
            nextCheck = Time.timeSinceLevelLoad + behaviorTemplate.CheckInterval;
            return true;
        }


        // Metodo que permite ajustar la velocidad de movimiento del personaje
        public void AdjustAgent(float speed, bool increase) {
            if(increase){
                agent.speed *= speed;
                agent.stoppingDistance = behaviorTemplate.MinDistanceFromTarget;
            }else{
                agent.speed /= speed;
                agent.stoppingDistance = 0;
            }
        }


        // Metodo que permite activar el estado de reset al momento de entrar
        public void ResetPositionEnemyAi() {
            agent.SetDestination(initialPoint);
        }


        // Permite obtener el estado por defecto: IDLE
        public AIState GetDefaultState() {
            return ActiveStates[behaviorTemplate.DefaultState.name].State;
        }


        // Se obtiene el estado de persecucion: CHASE
        public AIState GetChaseState(){
            // Se verifica si existe un estado de persecucion
            if (behaviorTemplate.ChaseState == null) return null;
            return ActiveStates[behaviorTemplate.ChaseState.name].State;
        }


        // Se obtiene el estado de reseteo: RESET
        public AIState GetResetState(){
            // Se verifica si existe un estado de persecucion
            if (behaviorTemplate.ResetState == null) return null;
            return ActiveStates[behaviorTemplate.ResetState.name].State;
        }


        // Se obtiene el estado de combat basico: BASICATTACK
        public AIState GetBasicAttackState(){
            // Se verifica si existe un estado de ataque basico
            if (behaviorTemplate.BasicAttackState == null) return null;
            return ActiveStates[behaviorTemplate.BasicAttackState.name].State;
        }


        // Se emplea este metodo para dteerminar la distancia entre en enemigo y el jugador
        public float GetDistance() {
            return Vector3.Distance(transform.position, player.position);
        }


        // Se emplea este metodo para detectar si exite algun obstaculo
        public bool DetectedObstacle(float distance) {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            return Physics.Raycast(transform.position, directionToPlayer, distance, includeLayer);
        }


        // Se emplea este metodo para poder obtener si el estado de Reset se a completado
        public bool GetResetEnemyAi() {
            // Espera a que se haya calculado el camino
            if (!agent.pathPending) {
                // Esta dentro del rango de parada
                if (agent.remainingDistance <= agent.stoppingDistance) {
                    // No hay mas camino o el agente se detuvo por completo
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                        transform.rotation = initialRotation;
                        return true;
                    }
                }
            }
            return false;
        }


        // Se emplea este metodo para gestionar el estado de persecucion
        public bool Chase(float distance) {
            // Se valida si el enemigo esta lo suficientemente cerca del jugador
            if(distance <= behaviorTemplate.MinDistanceFromTarget){
                // Cambiar al estado de reposo
                if(!inRange) {
                    agent.ResetPath();
                    inRange = true;
                }
                return false;
            }

            // Se valida si el personaje se alejado del jugador para proceder a seguirlo
            if(distance > behaviorTemplate.MaxDistanceFromTarget) {
                if(inRange) inRange = false;
                agent.SetDestination(player.position); // Regresar al estado de persecuion
                return true;
            }
            return false;
        }
    }
}
