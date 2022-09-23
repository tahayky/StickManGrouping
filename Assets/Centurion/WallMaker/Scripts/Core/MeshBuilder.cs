using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
namespace Centurion.WallMaker
{
    [System.Serializable]
    public class MeshBuilder
    {
        public GameObject parent_go;
        public List<GameObject> line_gos;

        public Wall wall;

        public MeshBuilder(Wall wall)
        {
            parent_go = wall.gameObject;
            line_gos = new List<GameObject>();
            this.wall = wall;
        }

        public void RefreshGameObjects(float height)
        {
            if (line_gos.Count == wall.lines.Count)
            {
                for (int i = 0; i < wall.lines.Count; i++)
                {
                    if (!wall.lines[i].empty&&line_gos[i]!=null)
                    {
                        if (wall.lines[i].beveled_mesh == null)
                        {
                            wall.lines[i].GenerateMesh(height);

                            line_gos[i].GetComponent<MeshFilter>().sharedMesh = wall.lines[i].beveled_mesh;
                            line_gos[i].GetComponent<MeshCollider>().sharedMesh = wall.lines[i].beveled_mesh;
                        }
                        if (line_gos[i].GetComponent<MeshFilter>().sharedMesh == null)
                        {
                            line_gos[i].GetComponent<MeshFilter>().sharedMesh = wall.lines[i].beveled_mesh;
                            line_gos[i].GetComponent<MeshCollider>().sharedMesh = wall.lines[i].beveled_mesh;
                        }
                    }else if (wall.lines[i].empty && line_gos[i] != null)
                    {
                        UnityEngine.Object.DestroyImmediate(line_gos[i]);                 
                    }
                    else if (!wall.lines[i].empty && line_gos[i] == null)
                    {
                        Material upper_side_material = Resources.Load<Material>("Materials/UpperSide");
                        Material wall_side_material = Resources.Load<Material>("Materials/Wall");

                        GameObject new_go = new GameObject($"Line_{i}", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                        new_go.transform.SetParent(parent_go.transform);
                        new_go.transform.localPosition = Vector3.zero;
                        new_go.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { upper_side_material, wall_side_material };
                        //new_go.hideFlags = HideFlags.HideInHierarchy;
                        line_gos[i] = new_go;
                    }


                }
            }
            else if (line_gos.Count < wall.lines.Count)
            {
                Material upper_side_material = Resources.Load<Material>("Materials/UpperSide");
                Material wall_side_material = Resources.Load<Material>("Materials/Wall");
                for (int i = 0; i < wall.lines.Count; i++)
                {
                    if (i >= line_gos.Count)
                    {
                        GameObject new_go = new GameObject($"Line_{i}", typeof(MeshFilter), typeof(MeshRenderer));
                        new_go.transform.SetParent(parent_go.transform);
                        new_go.transform.localPosition = Vector3.zero;
                        new_go.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { upper_side_material, wall_side_material };
                        new_go.AddComponent<MeshCollider>();
                        //new_go.hideFlags = HideFlags.HideInHierarchy;
                        line_gos.Add(new_go);
                        

                    }


                    if (wall.lines[i].beveled_mesh == null) wall.lines[i].GenerateMesh(height);

                    line_gos[i].GetComponent<MeshFilter>().sharedMesh = wall.lines[i].beveled_mesh;
                    line_gos[i].GetComponent<MeshCollider>().sharedMesh = wall.lines[i].beveled_mesh;
                }
            }
            else if (line_gos.Count > wall.lines.Count)
            {
                line_gos.RemoveRange(line_gos.Count, line_gos.Count - wall.lines.Count);
            }


        }
        public void BevelVertex(Vertex vertex)
        {
            if (!wall.gameObject.activeInHierarchy) return;
            if (vertex.line_indices.Count == 1)
            {
                SetOffset(wall.lines[vertex.line_indices[0]], vertex);
            }
            else if (vertex.line_indices.Count > 1)
            {
                int[] line_indices_by_slope = vertex.line_indices.OrderBy(item => wall.lines[item].GetSlope(vertex)).Reverse().ToArray();

                for (int i = 0; i < line_indices_by_slope.Length; i++)
                {
                    int index_1 = i;
                    int index_2 = i + 1;

                    if (i == line_indices_by_slope.Length - 1) index_2 = 0;
                    SetIntersection(wall.lines[line_indices_by_slope[index_1]], wall.lines[line_indices_by_slope[index_2]], vertex);
                    
                }
            }
            for (int i = 0; i < vertex.line_indices.Count; i++) wall.lines[vertex.line_indices[i]].GenerateMesh(vertex.transform.localPosition.y);

        }
        private void SetIntersection(Line l1,Line l2,Vertex vertex)
        {
            int vertex_key_1 = l1.GetVertexKey(vertex);
            int vertex_key_2 = l2.GetVertexKey(vertex);

            float angle = Mathf.Abs(l1.GetSlope(vertex) - l2.GetSlope(vertex));

            if (Mathf.RoundToInt(angle) == 180)
            {
                float u1=0;
                float v1=0;
                if (vertex_key_1 == 0)
                {
                    if (l1.GetParallel(1, out float a1, out float b1, out float c1))
                    {
                        u1 = -a1 / Mathf.Sqrt(Mathf.Pow(a1, 2) + Mathf.Pow(b1, 2)) + vertex.local_x;
                        v1 = -(u1 * a1 + c1) / b1;

                    }
                    else
                    {
                        u1 = vertex.local_x + (-1) * Mathf.Sign(a1)*l1.thickness;
                        v1 = vertex.local_y;
                    }
                }
                else if (vertex_key_1 == 1)
                {
                    if (l1.GetParallel(-1,  out float a1, out float b1, out float c1))
                    {
                        u1 = a1 / Mathf.Sqrt(Mathf.Pow(a1, 2) + Mathf.Pow(b1, 2)) + vertex.local_x;
                        v1 = -(u1 * a1 + c1) / b1;
                    }
                    else
                    {
                        u1 = vertex.local_x + (-1)*Mathf.Sign(-a1)*l1.thickness;
                        v1 = vertex.local_y;
                    }
                }
                l1.vertices[vertex_key_1].v2 = new Vector3(u1, 0, v1);
                l2.vertices[vertex_key_2].v1 = new Vector3(u1, 0, v1);
                return;
            }
            float x=0;
            float y=0;
            if (vertex_key_1 == 0&& vertex_key_2 == 0)
            {
                bool con_1 = l1.GetParallel(1, out float a1, out float b1, out float c1);
                bool con_2 = l2.GetParallel(-1,  out float a2, out float b2, out float c2);
                //Debug.Log(vertex.id + "" + con_1 + ""+ con_2);
                if (con_1 && con_2)
                {
                    x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
                    y = (a2 * x + c2) / (-b2);
                }
                else if (!con_1 && con_2)
                {
                    x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
                    y = (a2 * x + c2) / (-b2);
                }
                else if (con_1 && !con_2)
                {
                    x = (b2 * c1 - b1 * c2) / (a2 * b1 - a1 * b2);
                    y = (a1 * x + c1) / (-b1);
                }

            }
            else if (vertex_key_1 == 1 && vertex_key_2 == 0)
            {
                bool con_1 = l1.GetParallel(-1,  out float a1, out float b1, out float c1);
                bool con_2 = l2.GetParallel(-1,  out float a2, out float b2, out float c2);
                //Debug.Log(vertex.id +"" + con_1 + "" + con_2);
                if (con_1 && con_2)
                {
                    x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
                    y = (a2 * x + c2) / (-b2);
                }
                else if (!con_1 && con_2)
                {
                    x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
                    y = (a2 * x + c2) / (-b2);
                }
                else if (con_1 && !con_2)
                {
                    x = (b2 * c1 - b1 * c2) / (a2 * b1 - a1 * b2);
                    y = (a1 * x + c1) / (-b1);
                }


            }
            else if (vertex_key_1 == 0 && vertex_key_2 == 1)
            {
                bool con_1 = l1.GetParallel(1,  out float a1, out float b1, out float c1);
                bool con_2 = l2.GetParallel(1, out float a2, out float b2, out float c2);
                //Debug.Log(vertex.id + "" + con_1 + "" + con_2);
                if (con_1 && con_2)
                {
                    x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
                    y = (a2 * x + c2) / (-b2);
                }
                else if (!con_1 && con_2)
                {
                    x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
                    y = (a2 * x + c2) / (-b2);
                }
                else if (con_1 && !con_2)
                {
                    x = (b2 * c1 - b1 * c2) / (a2 * b1 - a1 * b2);
                    y = (a1 * x + c1) / (-b1);
                }

            }
            else if (vertex_key_1 == 1 && vertex_key_2 == 1)
            {
                bool con_1 = l1.GetParallel(-1,  out float a1, out float b1, out float c1);
                bool con_2 = l2.GetParallel(1,  out float a2, out float b2, out float c2);
               // Debug.Log(vertex.id + "" + con_1 + "" + con_2);
                if (con_1 && con_2)
                {
                    x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
                    y = (a2 * x + c2) / (-b2);
                }
                else if (!con_1 && con_2)
                {
                    x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
                    y = (a2 * x + c2) / (-b2);
                }
                else if (con_1 && !con_2)
                {
                    x = (b2 * c1 - b1 * c2) / (a2 * b1 - a1 * b2);
                    y = (a1 * x + c1) / (-b1);
                }
            }
            l1.vertices[vertex_key_1].v2 = new Vector3(x, 0, y);
            l2.vertices[vertex_key_2].v1 = new Vector3(x, 0, y);
        }
        public void SetOffset(Line l1, Vertex vertex)
        {

            int vertex_key = l1.GetVertexKey(vertex);
            float u1 = 0, v1 = 0;
            float u2 = 0, v2 = 0;
            if (vertex_key == 0)
            {
                bool con_1 = l1.GetParallel(1,  out float a1, out float b1, out float c1);
                bool con_2 = l1.GetParallel(-1,  out float a2, out float b2, out float c2);
                if (con_1 && con_2)
                {
                    u1 = -a1 * l1.thickness / Mathf.Sqrt(Mathf.Pow(a1, 2) + Mathf.Pow(b1, 2)) + vertex.local_x;
                    v1 = -(u1 * a1 + c1) / b1;
                    u2 = a2 * l1.thickness / Mathf.Sqrt(Mathf.Pow(a2, 2) + Mathf.Pow(b2, 2)) + vertex.local_x;
                    v2 = -(u2 * a2 + c2) / b2;
                }
                else 
                {
                    u1 = vertex.local_x + (-1) * Mathf.Sign(a1) * l1.thickness;
                    v1 = vertex.local_y ;
                    u2 = vertex.local_x + (-1) * Mathf.Sign(-a1) * l1.thickness;
                    v2 = vertex.local_y ;
                }
                l1.vertices[vertex_key].v2 = new Vector3(u1, 0, v1);
                l1.vertices[vertex_key].v1 = new Vector3(u2, 0, v2);
            }
            else if(vertex_key == 1)
            {
                bool con_1 = l1.GetParallel(-1,  out float a1, out float b1, out float c1);
                bool con_2 = l1.GetParallel(1,  out float a2, out float b2, out float c2);
                if (con_1 && con_2)
                {
                    u1 = a1 *l1.thickness/ Mathf.Sqrt(Mathf.Pow(a1, 2) + Mathf.Pow(b1, 2)) + vertex.local_x;
                    v1 = -(u1 * a1 + c1) / b1;
                    u2 = -a2 * l1.thickness / Mathf.Sqrt(Mathf.Pow(a2, 2) + Mathf.Pow(b2, 2)) + vertex.local_x;
                    v2 = -(u2 * a2 + c2) / b2;

                }
                else
                {
                    u1 = vertex.local_x + (-1) * Mathf.Sign(-a1) * l1.thickness;
                    v1 = vertex.local_y;
                    u2 = vertex.local_x + (-1) * Mathf.Sign(a1) * l1.thickness;
                    v2 = vertex.local_y;
                }
                l1.vertices[vertex_key].v2 = new Vector3(u1, 0, v1);
                l1.vertices[vertex_key].v1 = new Vector3(u2, 0, v2);
            }
        }
    }
}
