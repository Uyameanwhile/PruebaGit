using System.Collections;
using System.Collections.Generic;
using Assets.JSGAONA.Unidad1.Scripts;
using UnityEngine;
using UnityEngine.UI;

// Interfaz para dispositivos que pueden ser anulados
public interface IHackeable
{
    void Anular(float duracion);
}

namespace Assets.JSGAONA.Unidad2.Scripts
{

    public class PlayerCombat : MonoBehaviour
    {

        [SerializeField] private int maxLifePoint = 250;
        [SerializeField] private int maxResourcePoint = 100;

        [SerializeField] private Slider sliderLifePoint;
        [SerializeField] private Slider sliderResourcePoint;

        // Variables para la habilidad Pulso de Anulación
        [SerializeField] private float rangoPulsoAnulacion = 10f;
        [SerializeField] private int costePulsoAnulacion = 35;
        [SerializeField] private float duracionAnulacion = 5f;
        [SerializeField] private LayerMask capasAfectables;
        [SerializeField] private int maxObjetosDetectables = 50;

        private bool isAlive = true;
        public int currentLifePoint;
        private int currentResourcePoint;
        private PlayerController playerController;
        private RaycastHit[] hitResultsBuffer;
        private Collider[] collidersBuffer;


        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            hitResultsBuffer = new RaycastHit[1]; // Buffer para el raycast
            collidersBuffer = new Collider[maxObjetosDetectables]; // Buffer para los colliders
        }


        private void Start()
        {
            currentLifePoint = maxLifePoint;
            currentResourcePoint = maxResourcePoint;

            // Mi barra de salud y recurso es min 0 y max es el de la variable
            sliderLifePoint.minValue = 0;
            sliderResourcePoint.minValue = 0;

            sliderLifePoint.maxValue = maxLifePoint;
            sliderResourcePoint.maxValue = maxResourcePoint;

            sliderLifePoint.value = maxLifePoint;
            sliderResourcePoint.value = maxResourcePoint;
        }

        public void AddLifePoints(int amount)
        {
            if (!isAlive) return;

            currentLifePoint += amount;
            currentLifePoint = Mathf.Clamp(currentLifePoint, 0, maxLifePoint); // Asegurarse de que no exceda el máximo
            sliderLifePoint.value = currentLifePoint; // Actualizar el slider

        }

        public void TakeDamage(int amount)
        {
            if (!isAlive) return;

            currentLifePoint -= amount;
            sliderLifePoint.value = Mathf.Clamp(currentLifePoint, 0, maxLifePoint);

            // Me he quedado sin vida
            if (currentLifePoint <= 0)
            {
                currentLifePoint = 0;
                isAlive = false;
                playerController.StopMovement();
                // Accion de morir o reinicio del pj
            }
            else
            {
                // Animacion de recibir daño
            }
        }


        public void TakeResource(int amount)
        {
            currentResourcePoint -= amount;
            sliderResourcePoint.value = Mathf.Clamp(currentResourcePoint, 0, maxResourcePoint);

            // Me he quedado sin recurso
            if (currentResourcePoint <= 0)
            {
                currentResourcePoint = 0;
                // Accion de morir o reinicio del pj
            }
        }

        /// <summary>
        /// Habilidad definitiva: Pulso de Anulación
        /// Hackea todos los dispositivos electrónicos en un radio determinado
        /// si están en línea de visión directa
        /// </summary>
        /// <returns>True si la habilidad se activó correctamente</returns>
        public bool PulsoDeAnulacion()
        {
            // Verificar si hay suficiente recurso para usar la habilidad
            if (currentResourcePoint < costePulsoAnulacion || !isAlive)
            {
                Debug.Log("Recurso insuficiente o jugador muerto. No se puede usar Pulso de Anulación.");
                return false;
            }

            // Consumir el recurso
            TakeResource(costePulsoAnulacion);

            // Posición desde donde se origina el pulso
            Vector3 origen = transform.position;

            // Detectar objetos dentro del rango usando SphereCastNonAlloc
            int numColliders = Physics.OverlapSphereNonAlloc(
                origen,
                rangoPulsoAnulacion,
                collidersBuffer,
                capasAfectables
            );

            // Recorrer todos los objetos detectados
            for (int i = 0; i < numColliders; i++)
            {
                Collider collider = collidersBuffer[i];

                // Ignorar al propio jugador
                if (collider.gameObject == gameObject)
                    continue;

                // Verificar línea de visión directa
                Vector3 direccion = collider.transform.position - origen;
                float distancia = direccion.magnitude;

                // Normalizar la dirección para el raycast
                direccion.Normalize();

                // Verificar si hay obstáculos entre el jugador y el objetivo
                int hitsCount = Physics.RaycastNonAlloc(
                    origen,
                    direccion,
                    hitResultsBuffer,
                    distancia,
                    ~capasAfectables // Invertir la capa para detectar obstáculos
                );

                // Si hitsCount == 0, no hay obstáculos en la línea de visión
                bool hayLineaDeVision = (hitsCount == 0);

                if (hayLineaDeVision)
                {
                    // Buscar componentes que implementen la interfaz IHackeable
                    IHackeable[] dispositivos = collider.GetComponentsInChildren<IHackeable>();

                    // Anular cada dispositivo encontrado
                    foreach (IHackeable dispositivo in dispositivos)
                    {
                        dispositivo.Anular(duracionAnulacion);
                    }
                }
            }

            // Efecto visual o sonido del pulso (a implementar)
            // TODO: Añadir efectos visuales o feedback

            Debug.Log("Pulso de Anulación activado. Rango: " + rangoPulsoAnulacion);
            return true;
        }
    }
}