using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace Assets.JSGAONA.Unidad2.Scripts {

    // Se emplea esta clse para poder instanciar objetos con GPU Instancing
    public class MeshInstancer : MonoBehaviour {
        
        // Variables visibles desde el inspector de Unity
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;
        [SerializeField] private int instancesPerRow = 10;
        [SerializeField] private float spacing = 2.0f;
        [SerializeField] private Color uniformColor = Color.green;
        [SerializeField] private Text modeText;
        [SerializeField] private bool useColor = true;

        // Variables ocultas desde el inspector de Unity
        private readonly List<Matrix4x4> matrices = new ();
        private readonly List<Vector4> colors = new ();
        


        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, se ejecuta despues
        // de Awake, se realiza la asignacion de variables y configuracion del script
        private void Start() {
            // Se valida si el dispositivo soporta instancing en GPU 
            if (SystemInfo.supportsInstancing) {
                // Crea una cuadricula de instancias (por ejemplo, 10x10) separadas por spacing unidades
                for (int x = 0; x < instancesPerRow; x++) {
                    for (int z = 0; z < instancesPerRow; z++) {
                        Vector3 position = new (x * spacing, 0, z * spacing);
                        Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                        matrices.Add(matrix);

                        // Se generan colores aleatorios
                        colors.Add(new Color(Random.value, Random.value, Random.value, 1));
                    }
                }
            
            // Se destruye el GameObject pra evitar errores
            } else {
                Destroy(gameObject);
            }
        }

        
        // Metodo de llamada de Unity, se llama en cada frame del computador
        // Se realiza la logica de renderizar en GPU las instncias
        private void Update() {
            //Unity solo permite instanciar hasta 1023 mallas en un solo draw call
            int maxInstancesPerCall = 1023;

            // Divide las instancias en lotes (batches) de hasta 1023
            // Extrae las matrices y colores para ese lote.
            for (int i = 0; i < matrices.Count; i += maxInstancesPerCall) {
                int batchCount = Mathf.Min(maxInstancesPerCall, matrices.Count - i);
                Matrix4x4[] matrixArray = matrices.GetRange(i, batchCount).ToArray();

                // Creacion del MaterialPropertyBlock para enviar los colores
                MaterialPropertyBlock mpb = new ();
                if (useColor) {
                    Vector4[] colorArray = colors.GetRange(i, batchCount).ToArray();
                    // Shader debe aceptar esto
                    mpb.SetVectorArray("_BaseColor", colorArray);
                } else {
                    mpb.SetColor("_BaseColor", uniformColor);
                }

                // Renderizado de las instancias con un solo draw call
                Graphics.RenderMeshInstanced(
                    new RenderParams(material) {
                        layer = gameObject.layer,
                        worldBounds = new Bounds(transform.position, Vector3.one * 1000),
                        shadowCastingMode = ShadowCastingMode.On,
                        receiveShadows = true,
                        matProps = mpb
                    },
                    mesh,
                    0,
                    matrixArray
                );
            }
        }
    }
}
