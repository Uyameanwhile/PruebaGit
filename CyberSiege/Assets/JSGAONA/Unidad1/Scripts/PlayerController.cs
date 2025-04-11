
using UnityEngine;
using System.Collections;

namespace Assets.JSGAONA.Unidad1.Scripts {

    // Se emplea para obligar al GameObject asignar el componente para su correcto funcionamiento
    [RequireComponent(typeof(CharacterController))]

    // Se emplea este script para gestionar el controlador del personaje jugable
    public class PlayerController : MonoBehaviour {
        
        // Variables visibles desde el inspector de Unity
        [Header("Adjust movement")]
        [SerializeField] private float speedMove;
        [SerializeField] private float speedRotation;
        [SerializeField] private float jumpForce = 3.0f;
        [SerializeField] private int maxJumpCount = 1;
        [SerializeField]  private float minFallVelocity = -2;
        [SerializeField] [Range(0, 5)] private float gravityMultiplier = 1;
        [SerializeField] private Joystick joystick;

        [Header("Adjust to gorund")]
        [SerializeField] private float radiusDetectedGround = 0.2f;
        [SerializeField] private float groundCheckDistance = 0.0f;
        [SerializeField] private LayerMask ignoreLayer;

        [Header("Ajust Dash")]
        [SerializeField] private float dashWallDetec = 0.7f;
        [SerializeField] private float offsetTop = 0.25f;
        [SerializeField] private float offsetBottom = 0.25f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashDistance = 5f;
        [SerializeField] private float dashCooldown = 1f;

        // Variables ocultas desde el inspector de Unity
        public bool enableControl = true;
        private bool onGround = false;
        private bool isDashing = false;
        private bool dashInCooldown = false;
        private int currentJumpCount = 0;
        private float fallVelocity = 0;
        private readonly float gravity = -9.8f;
        private Vector3 dirMove;
        private Vector3 positionFoot;
        private CharacterController charController;



        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, es el primer
        // metodo en ejecutarse, se realiza la asignacion de componentes
        private void Awake(){
            charController = GetComponent<CharacterController>();
        }
        
        
        // Metodo de llamada de Unity, se llama en cada frame del computador
        // Se realiza la logica de control del personaje jugable
        private void Update() {
            if(enableControl) {
                // No se esta realiznado un dash
                if(!isDashing) {
                    dirMove = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;

                    // Esta conectado a tierra
                    if(onGround){
                        fallVelocity = Mathf.Max(minFallVelocity, fallVelocity + gravity * Time.deltaTime);
                    
                    // No esta conectado a tierra, por ende se esta en el aire
                    }else {
                        fallVelocity += gravity  * gravityMultiplier * Time.deltaTime;
                    }

                    // Rotacion del personaje
                    if (dirMove != Vector3.zero) {
                        Quaternion targetRotation = Quaternion.LookRotation(dirMove);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speedRotation);
                    }
                    dirMove *= speedMove;
                    dirMove.y = fallVelocity;
                }
            }
            charController.Move(dirMove * Time.deltaTime);
        }


        // Metodo de llamada de Unity, se llama en cada actualizacion constante 0.02 seg
        // Se realiza la logica de gestion de fisicas del motor
        private void FixedUpdate() {
            positionFoot = transform.position + Vector3.down * groundCheckDistance;
            onGround = Physics.CheckSphere(positionFoot, radiusDetectedGround, ~ignoreLayer);
        }


        // Metodo publico que se emplea para gestionar los saltos del personaje
        public void Jump() {
            // Esta conectado a tierra
            if(onGround) {
                onGround = false;
                CountJump(false);
            
            // No esta conectado a tierra, por ende se esta en el aire
            }else{
                // Se valida si el contador de saltos a superado los permitidos
                if(currentJumpCount < maxJumpCount) {
                    dirMove.y = 0;
                    CountJump(true);
                }
            }
        }


        // Metodo que se utiliza para poder controlar el contador de los saltos
        private void CountJump(bool accumulate) {
            currentJumpCount = accumulate ? (currentJumpCount + 1): 1;
            fallVelocity = jumpForce;
        }


        // Metodo que se emplea para poder generar el dash del personaje
        public void Dash() {
            // Se valida que no se este realizando un dash, o el dash este en enfiramiento
            if(!isDashing && !dashInCooldown) {
                StartCoroutine(StartDash());
            }
        }


        // Coroutina que permite dar inicio al movimiento de desplazamiento
        private IEnumerator StartDash() {
            // Lanza un rayo desde el personaje hacia alfrente en la direccion donde esta mirando
            if(DetectWallDuringDash()) yield break;
            dashInCooldown = true;
            isDashing = true;
            fallVelocity = 0;
            // Establece la direccion del dash
            Vector3 dashVector = dashDistance * transform.forward;
            float elapsedTime = 0;
            // Ciclo repetitivo que permite dar movimiento al dash
            while (elapsedTime < dashDuration) {
                // Se valida si el pj ha detectado una pared
                if (DetectWallDuringDash()) {
                    // Se sale del efecto de dash
                    break;
                }
                // Aplicar la nueva posicion
                dirMove = dashVector / dashDuration;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            dirMove = Vector3.zero;
            // Se asigna la posicion objetivo al finalizar el dash
            isDashing = false;
            // Espera que termine el tiempo de enfriamiento del dash
            yield return new WaitForSeconds(dashCooldown);
            dashInCooldown = false;
        }


        // Se emplea este metodo para verificar si existe una pared durante el dash
        private bool DetectWallDuringDash(){
            // Se instancia la direccion de los rayos hacia arriba y abajo del personaje
            Vector3 rayOriginTop = transform.position + (Vector3.up * offsetTop);
            Vector3 rayOriginBot = transform.position + (Vector3.down * offsetBottom);
            // Lanza un rayo desde el personaje hacia alfrente en la direccion donde esta mirando
            return HitDetectedWallOnDash(rayOriginTop) || HitDetectedWallOnDash(rayOriginBot);
        }


        // Se emplea este metodo para verificar la existencia de pared si un raycast lo detecta
        private bool HitDetectedWallOnDash(Vector3 originRay){
            // permite verificar una pared en funcion del dash
            return Physics.Raycast(originRay, transform.forward, dashWallDetec, ~ignoreLayer);
        }


        public void StopMovement()
        {
            enableControl = false; // Desactiva el control del jugador

            // Movimiento lento hacia x = 0
            Vector3 targetPosition = new Vector3(0, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 2f * Time.deltaTime);
        }


#if UNITY_EDITOR
        // Metodo de llamada de Unity, se emplea para visualizar en escena, acciones del codigo
        private void OnDrawGizmos() {
            // Se genera el gizmos para visualizar la posicion de los pies, validar piso
            Gizmos.color = Color.green;
            positionFoot = transform.position + Vector3.down * groundCheckDistance;
            Gizmos.DrawSphere(positionFoot, radiusDetectedGround);

            // Se genera el gizmos para verificar los raycast del dash
            Vector3 rayOriginTop = transform.position + Vector3.up * offsetTop;
            Vector3 rayOriginBot = transform.position + Vector3.down * offsetBottom;

            // Establecer el color del Gizmo
            Gizmos.color = Color.red;
            Gizmos.DrawRay(rayOriginTop, transform.forward * dashWallDetec);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(rayOriginBot, transform.forward * dashWallDetec);
        }
    #endif
    }
}
