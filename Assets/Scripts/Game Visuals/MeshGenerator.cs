using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets
{
    public class MeshGenerator : MonoBehaviour
    {
        public static MeshGenerator Instance;
        [SerializeField] public List<Material> materials = new List<Material>();


        [SerializeField] private float edgeHeight;

        private void Awake()
        {
            Instance = this;
        }

        public Color getSqTypeColor(SquareType type)
        {
            switch (type)
            {
                case SquareType.Water:
                    return materials[0].color;
                case SquareType.Grass:
                    return materials[1].color;
                case SquareType.Sand:
                    return materials[2].color;
            }
            return Color.magenta;
        }

        private GameObject createMeshObject(Mesh m)
        {
            GameObject mesh = new GameObject("");
            mesh.AddComponent<MeshFilter>();
            mesh.AddComponent<MeshRenderer>();
            mesh.GetComponent<MeshFilter>().mesh = m;
            mesh.GetComponent<MeshRenderer>().materials = materials.ToArray();
            return mesh;

        }     

        public Mesh getSubBoardMesh(Board b)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<List<int>> triangles = new List<List<int>>();
            List<Vector2> uvs = new List<Vector2>();    
            triangles.Add(new List<int>());
            triangles.Add(new List<int>());
            triangles.Add(new List<int>());
            Mesh mesh = new Mesh();

            int c = 0;
            for (int x = 0; x < b.xsize; x++)
            {
                for (int z = 0; z < b.zsize; z++)
                {
                    Square s = b.getSquare(x, z);

                    vertices.AddRange(new List<Vector3>()
                    {
                        new Vector3(x-0.5f, 0, z-0.5f),
                        new Vector3(x+0.5f, 0, z-0.5f),
                        new Vector3(x-0.5f, 0, z+0.5f),
                        new Vector3(x+0.5f, 0, z+0.5f)
                    });

                    triangles[((int)s.type)-1].AddRange(new List<int>()
                    {
                        0+c, 2+c, 1+c, 2+c, 3+c, 1+c
                    });
                    c += 4;



                    float uvX = x / (b.xsize * 1f);
                    float uvY = z / (b.zsize * 1f);

                    uvs.Add(new Vector2(uvX, uvY));
                    uvs.Add(new Vector2(uvX + 1f / b.xsize, uvY));
                    uvs.Add(new Vector2(uvX + 1f / b.xsize, uvY + 1f / b.zsize));
                    uvs.Add(new Vector2(uvX, uvY + 1f / b.zsize));


                    if (s.getNeighbours().Count < 4)
                    {
                        List<Vector3> dir = new List<Vector3>();
                        if (s.getNeighbour(Vector3.back) == null) { dir.Add(Vector3.back); }
                        if (s.getNeighbour(Vector3.right) == null) { dir.Add(Vector3.right); }
                        if (s.getNeighbour(Vector3.forward) == null) { dir.Add(Vector3.forward); }
                        if (s.getNeighbour(Vector3.left) == null) { dir.Add(Vector3.left); }

                        if (dir.Count > 0) { addSides(ref vertices, triangles[(((int)s.type))-1], ref uvs, dir.ToArray(), s, b); }
                        c = vertices.Count;
                    }
                }
            }

            mesh.SetVertices(vertices);
            mesh.uv = uvs.ToArray();
            mesh.subMeshCount = triangles.Count;
            for (int i = 0; i < triangles.Count; i++)
            {
                mesh.SetTriangles(triangles[i], i);
            }
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;

        }

        private int addSides(ref List<Vector3> vertices, List<int> triangles, ref List<Vector2> uvs, Vector3[] dir, Square s, Board b)
        {

            int vc = vertices.Count;
            for (int i = 0; i < dir.Length; i++)
            {
                Vector3 v1 = Vector3.zero, v2 = Vector3.zero;
                float x = dir[i].x;
                float z = dir[i].z;
                if (x == 0)
                {
                    v1 = s.position + (new Vector3(z, 0, z) / 2);
                    v2 = s.position + (new Vector3(-z, 0, z) / 2);
                }
                if (z == 0)
                {
                    v1 = s.position + (new Vector3(x, 0, -x) / 2);
                    v2 = s.position + (new Vector3(x, 0, x) / 2);
                }

                //print(string.Format("{0} -> {1} , {2}", dir[i], v1, v2));

                vertices.AddRange(new List<Vector3>()
                {
                    v1,
                    v2,
                    v1 + Vector3.down * edgeHeight,
                    v2 + Vector3.down * edgeHeight,

                });

                triangles.AddRange(new List<int>()
                {
                    0 + vc, 1 + vc, 3 + vc, 0 + vc, 3 + vc, 2 + vc
                });

                float uvX = x / (b.xsize * 1f);
                float uvY = z / (b.zsize * 1f);

                uvs.Add(new Vector2(uvX, uvY));
                uvs.Add(new Vector2(uvX, uvY + 1f / b.zsize));
                uvs.Add(new Vector2(uvX + 1f / b.xsize, uvY + 1f / b.zsize));
                uvs.Add(new Vector2(uvX + 1f / b.xsize, uvY));

                vc += 4;

            }
            return vc;
        }

    }
}