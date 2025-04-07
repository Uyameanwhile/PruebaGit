using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.JSGAONA.Unidad2.Scripts
{

    public class MeshInstancer : MonoBehaviour
    {
        [SerializeField] private Material material;
        [SerializeField] private Mesh mesh;
        [SerializeField] private int instancePerRow = 10;
        [SerializeField] private float spacing = 2f;
        [SerializeField] private bool useColor = false;
        [SerializeField] private Color colorBase;

        private List<Matrix4x4> matrices = new();
        private List<Vector4> colors = new();

        private void Start()
        {
            for(int x = 0; x < instancePerRow; x++)
            {
                for (int z = 0; z < instancePerRow; z++)
                {
                    Vector3 position = new (x * spacing, 0, z * spacing);
                    Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                    matrices.Add(matrix);

                    colors.Add(new Color(Random.value, Random.value, Random.value, 1));
                }
            }
        }
        private void Update()
        {
            int maxInstancesPerCall = 1023;

            for (int i = 0; i < matrices.Count; i =+maxInstancesPerCall)
            {
                int batchCount = Mathf.Min(maxInstancesPerCall, matrices.Count - i);
                Matrix4x4[] matrixArray = matrices.GetRange(i, batchCount).ToArray();

                MaterialPropertyBlock mpb = new();

                if (useColor)
                {
                    Vector4[] colorArray = colors.GetRange(i, batchCount).ToArray();
                    mpb.SetVectorArray("_BaseColor", colorArray);
                }
                else
                {
                    mpb.SetColor("_BaseColor", colorBase);
                }

                Graphics.RenderMeshInstanced(
                    new RenderParams(material)
                    {
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