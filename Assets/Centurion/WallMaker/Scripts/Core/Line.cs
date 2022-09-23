using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Centurion.WallMaker
{
    [System.Serializable]
    public class Line 
    {
        public Vertex v1;
        public Vertex v2;

        public Vertices[] vertices;

        public Mesh beveled_mesh;

        public bool empty = false;

        public float thickness=1;
        public void GenerateMesh(float height)
        {
            if (beveled_mesh == null)
            {
                beveled_mesh = new Mesh();
                beveled_mesh.subMeshCount = 2;
            }

            Vector3[] mesh_vertices = new Vector3[] {
                new Vector3(vertices[0].v2.x,height,vertices[0].v2.z),
                new Vector3(vertices[1].v1.x,height,vertices[1].v1.z),
                new Vector3(v2.local_x, height, v2.local_y),
                new Vector3(vertices[1].v2.x,height,vertices[1].v2.z),
                new Vector3(vertices[0].v1.x,height,vertices[0].v1.z),
                new Vector3(v1.local_x, height, v1.local_y),

                new Vector3(vertices[0].v2.x,height,vertices[0].v2.z),
                new Vector3(vertices[1].v1.x,height,vertices[1].v1.z),
                new Vector3(v2.local_x, height, v2.local_y),
                new Vector3(vertices[1].v2.x,height,vertices[1].v2.z),
                new Vector3(vertices[0].v1.x,height,vertices[0].v1.z),
                new Vector3(v1.local_x, height, v1.local_y),
                new Vector3(vertices[0].v2.x,0,vertices[0].v2.z),
                new Vector3(vertices[1].v1.x,0,vertices[1].v1.z),
                new Vector3(vertices[1].v2.x,0,vertices[1].v2.z),
                new Vector3(vertices[0].v1.x,0,vertices[0].v1.z),

                new Vector3(vertices[0].v2.x,height,vertices[0].v2.z),
                new Vector3(vertices[1].v1.x,height,vertices[1].v1.z),
                new Vector3(v2.local_x, height, v2.local_y),
                new Vector3(vertices[1].v2.x,height,vertices[1].v2.z),
                new Vector3(vertices[0].v1.x,height,vertices[0].v1.z),
                new Vector3(v1.local_x, height, v1.local_y),
                new Vector3(vertices[0].v2.x,0,vertices[0].v2.z),
                new Vector3(vertices[1].v1.x,0,vertices[1].v1.z),
                new Vector3(vertices[1].v2.x,0,vertices[1].v2.z),
                new Vector3(vertices[0].v1.x,0,vertices[0].v1.z)
            };
            List<int> upper_mesh_indices = new List<int> {
                0,1,5,
                1,2,5,
                2,3,5,
                3,4,5
            };
            int[] wall_mesh_indices = new int[] {
                6,12,13,
                6,13,7,
                9,14,15,
                9,15,10
            };
            int[] open_v1_indices = new int[]
            {
                21,22,16,
                25,22,21,
                20,25,21
            };
            int[] open_v2_indices = new int[]
            {
                17,23,18,
                18,23,24,
                18,24,19

            };
            beveled_mesh.vertices = mesh_vertices;

            if (v1.line_indices.Count == 1)
            {
                upper_mesh_indices.AddRange(open_v1_indices);
            }
            if (v2.line_indices.Count == 1)
            {
                upper_mesh_indices.AddRange(open_v2_indices);
            }

            beveled_mesh.SetTriangles(upper_mesh_indices, 0);
            beveled_mesh.SetTriangles(wall_mesh_indices, 1);
            /*
            for (int i=0;i<mesh_vertices.Length;i++)
            {
                GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position=mesh_vertices[i];
            }
            */
            beveled_mesh.RecalculateNormals();
            beveled_mesh.RecalculateBounds();
        }

        public Line(Vertex v1,Vertex v2)
        {
            vertices = new Vertices[2];
            this.v1 = v1;
            this.v2 = v2;
        }
        public int GetVertexKey(Vertex vertex)
        {
            if (vertex.Equals(v1))
            {
                return 0;
            }
            else if (vertex.Equals(v2))
            {
                return 1;
            }
            return -1;
        }
        public Vertex GetOther(Vertex vertex)
        {
            if (vertex.Equals(v1))
            {
                return v2;
            }
            else if (vertex.Equals(v2))
            {
                return v1;
            }
            return null;
        }
        public float GetLenght()
        {
            return Vector2.Distance(v1.GetV2Position(), v2.GetV2Position());
        }
        public float GetSlope(Vertex vertex)
        {
            float slope;
            if (vertex.Equals(v2)) {
                slope = Vector2.SignedAngle(v2.GetV2Position() - v1.GetV2Position(), Vector2.right);
            }
            else
            {
                slope = Vector2.SignedAngle(v1.GetV2Position() - v2.GetV2Position(), Vector2.right);
            }
            if (slope < 0) slope += 360;
            return slope;
        }
        public bool GetParallel(int direction, out float a, out float b, out float c)
        {

            var dx = v2.local_x - v1.local_x;
            var dy = v2.local_y - v1.local_y;
            b = -dx;
            a = dy;
            c = -v2.local_x * dy + v2.local_y * dx;
            c = (Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2))* thickness + c * direction) * direction;
            if (dx == 0) return false;
            return true;
            
        }
        
    }
}
