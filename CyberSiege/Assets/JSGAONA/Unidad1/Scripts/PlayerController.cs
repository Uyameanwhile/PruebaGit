using UnityEngine;
using System.Collections;
namespace Assets.JSGAONA.Unidad1.Scripts
{

    // Se emplea para obligar al GameObject asignar el componente para su correcto funcionamiento
    [RequireComponent(typeof(CharacterController))]

    // Se emplea este script para gestionar el controlador del personaje jugable
    public class PlayerController : MonoBehaviour
    {


        [Header("Adjust movement")]
        [SerializeField] private float speedMove;
        [SerializeField] private float speedRotation;
        [SerializeField] private float jumpForce = 3.0f;
        [SerializeField] private int maxJumpCount = 1;
        [SerializeField] private float minFallVelocity = -2;
        [SerializeField][Range(0, 5)] private float gravityMultiplier = 1;
        [SerializeField] private Joystick joystick;

        [Header("Adjust to gorund")]
        [SerializeField] private float radiusDetectedGround = 0.2f;
        [SerializeField] private float groundCheckDistance = 0.0f;
        [SerializeField] private LayerMask ignoreLayer;

        [Header("Dash Settings")]
        [SerializeField] private float dashDistance = 5.0f;
        [SerializeField] private float dashDuration = 0.5f;
        [SerializeField] private float dashCooldown = 3.0f;
        [SerializeField] private float dashObstacleCheckDistance = 5.5f; // Slightly longer than dash distance to check for obstacles

        // Variables ocultas desde el inspector de Unity
        private readonly float gravity = -9.8f;
        private int currentJumpCount = 0;
        public bool onGround = false;
        public float fallVelocity = 0;
        private Vector3 dirMove;
        private Vector3 positionFoot;
        private CharacterController charController;

        // Variables para el sistema de dash
        private bool isDashing = false;
        private bool canDash = true;
        private float originalGravity;


        // Tag del enemigo para detectar la colisión
        


        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, es el primer
        // metodo en ejecutarse, se realiza la asignacion de componentes
        private void Awake()
        {
            charController = GetComponent<CharacterController>();
        }


        // Metodo de llamada de Unity, se llama en cada frame del computador
        // Se realiza la logica de control del personaje jugable
        private void Update()
        {
            // No procesar movimiento normal si está en dash
            if (!isDashing)
            {
                dirMove = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;

                // ****************** 2) Mecánica de dash o movimiento rápido ****************** //

                // Esta conectado a tierra
                if (onGround)
                {
                    fallVelocity = Mathf.Max(minFallVelocity, fallVelocity + gravity * Time.deltaTime);

                    // No esta conectado a tierra, por ende se esta en el aire
                }
                else
                {
                    fallVelocity += gravity * gravityMultiplier * Time.deltaTime;
                }

                // Rotacion del personaje
                if (dirMove != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dirMove);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speedRotation);
                }

                dirMove *= speedMove;
                dirMove.y = fallVelocity;
                charController.Move(dirMove * Time.deltaTime);
            }
        }


        // Metodo de llamada de Unity, se llama en cada actualizacion constante 0.02 seg
        // Se realiza la logica de gestion de fisicas del motor
        private void FixedUpdate()
        {
            positionFoot = transform.position + Vector3.down * groundCheckDistance;
            onGround = Physics.CheckSphere(positionFoot, radiusDetectedGround, ~ignoreLayer);
        }


        // Metodo publico que se emplea para gestionar los saltos del personaje
        public void Jump()
        {
            // No permitir saltos durante el dash
            if (isDashing)
                return;

            // Esta conectado a tierra
            if (onGround)
            {
                onGround = false;
                CountJump(false);

                // No esta conectado a tierra, por ende se esta en el aire
            }
            else
            {
                // Se valida si el contador de saltos a superado los permitidos
                if (currentJumpCount < maxJumpCount)
                {
                    dirMove.y = 0;
                    CountJump(true);
                }
            }
        }


        // Metodo que se utiliza para poder controlar el contador de los saltos
        private void CountJump(bool accumulate)
        {
            currentJumpCount = accumulate ? (currentJumpCount + 1) : 1;
            fallVelocity = jumpForce;
        }

        // Método público para iniciar el dash
        public void Dash()
        {
            // Verificar si puede hacer dash (no está en dash y no está en cooldown)
            if (canDash && !isDashing)
            {
                // Comprobar si hay obstáculos frente al personaje
                if (CheckForObstacles())
                {
                    // Si hay obstáculos, no ejecutar el dash
                    return;
                }

                // Iniciar la coroutine del dash
                StartCoroutine(PerformDash());
            }
        }

        // Método para comprobar obstáculos frente al personaje
        private bool CheckForObstacles()
        {
            // Usar Raycast para detectar obstáculos
            Ray ray = new Ray(transform.position, transform.forward);
            bool hitObstacle = Physics.Raycast(ray, out RaycastHit hit, dashObstacleCheckDistance, ~ignoreLayer);

            Debug.DrawRay(transform.position, transform.forward * dashObstacleCheckDistance, hitObstacle ? Color.red : Color.green, 1.0f);

            return hitObstacle;
        }

        // Coroutine para gestionar el dash
        private IEnumerator PerformDash()
        {
            // Configurar el estado de dash
            isDashing = true;
            canDash = false;

            // Guardar la velocidad vertical actual
            originalGravity = fallVelocity;

            // Calcular velocidad de dash para lograr la distancia en el tiempo establecido
            float dashSpeed = dashDistance / dashDuration;
            Vector3 dashDirection = transform.forward;

            // Ejecutar el dash durante el tiempo especificado
            float dashTime = 0;
            while (dashTime < dashDuration)
            {
                // Mover el personaje en la dirección del dash
                Vector3 dashMove = dashDirection * dashSpeed * Time.deltaTime;

                // Mantener la posición vertical (sin efecto de gravedad)
                dashMove.y = 0;

                // Aplicar el movimiento
                charController.Move(dashMove);

                dashTime += Time.deltaTime;
                yield return null;
            }

            // Restaurar el estado normal después del dash
            isDashing = false;

            // Restaurar la velocidad vertical a como estaba antes del dash
            fallVelocity = originalGravity;

            // Iniciar el cooldown
            StartCoroutine(DashCooldown());
        }

        // Coroutine para gestionar el cooldown del dash
        private IEnumerator DashCooldown()
        {
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }


#if UNITY_EDITOR
        // Metodo de llamada de Unity, se emplea para visualizar en escena, acciones del codigo
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            positionFoot = transform.position + Vector3.down * groundCheckDistance;
            Gizmos.DrawSphere(positionFoot, radiusDetectedGround);

            // Visualizar la verificación de obstáculos para el dash
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * dashObstacleCheckDistance);
        }
#endif
    }
}