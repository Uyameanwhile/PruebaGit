using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayoDesactivador : MonoBehaviour
{
    [Header("Configuraci�n del Raycast")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask targetLayers = -1; // Por defecto todos los layers
    [SerializeField] private Transform shootPoint; // Punto desde donde sale el rayo
    [SerializeField] private float reactivationTime = 3f; // Tiempo para reactivar el objeto

    [Header("Efectos Visuales (Opcional)")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private float debugRayDuration = 1.0f;
    [SerializeField] private Color rayColor = Color.red;

    private void Start()
    {
        // Si no se asign� un punto de disparo, usar la posici�n del objeto actual
        if (shootPoint == null)
        {
            shootPoint = transform;
        }
    }

    // Este m�todo es el que se llamar� desde el bot�n
    public void DisparaRayo()
    {
        // Crear el rayo desde el punto de disparo en la direcci�n hacia adelante
        Ray ray = new Ray(shootPoint.position, shootPoint.forward);
        RaycastHit hit;

        // Mostrar el rayo en el editor (solo para depuraci�n)
        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, rayColor, debugRayDuration);
        }

        // Lanzar el rayo y comprobar si golpea algo
        if (Physics.Raycast(ray, out hit, maxDistance, targetLayers))
        {
            // Guardar referencia al objeto golpeado
            GameObject hitObject = hit.collider.gameObject;

            // Desactivar el objeto golpeado
            hitObject.SetActive(false);

            // Iniciar corrutina para reactivar el objeto despu�s de 3 segundos
            StartCoroutine(ReactivarObjeto(hitObject));
        }
    }

    private IEnumerator ReactivarObjeto(GameObject objeto)
    {
        // Esperar el tiempo especificado
        yield return new WaitForSeconds(reactivationTime);

        // Reactivar el objeto si a�n existe
        if (objeto != null)
        {
            objeto.SetActive(true);
        }
    }
}