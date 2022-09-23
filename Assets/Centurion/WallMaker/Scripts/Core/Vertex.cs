using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
namespace Centurion.WallMaker
{
    [ExecuteInEditMode]
    public class Vertex : MonoBehaviour,IEqualityComparer<Vertex>
    {
        //Bu noktanýn baðlý olduðu çizgiler
        public List<int> line_indices = new List<int>();

        public float local_x { get { return transform.localPosition.x; } }
        public float local_y { get { return transform.localPosition.z; } }

        public float x { get { return transform.position.x; } }
        public float y { get { return transform.position.z; } }

        public int id;

        private Vector3 old_position;
        public Wall wall;

        private void Start()
        {
            ReloadMesh();
        }
        public void AddLine(int i)
        {
            line_indices.Add(i);
        }
        public void AddLines(int[] lines_indices)
        {
            this.line_indices.AddRange(lines_indices);
        }
        public void RemoveLine(int i)
        {
            line_indices.Remove(i);
        }
        public int GetLineIndex(Vertex vertex)
        {
            return line_indices.Intersect(vertex.line_indices).FirstOrDefault();
        }
        public bool Equals(Vertex x, Vertex y)
        {
            return x == y;

        }
        public Vector2 GetV2Position()
        {
            return new Vector2(transform.position.x,transform.position.z);
        }
        public int GetHashCode(Vertex obj)
        {
            return id.GetHashCode();
        }
        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (old_position != transform.localPosition)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x,wall.height, transform.localPosition.z);
                    ReloadMesh();
                }
                old_position = transform.localPosition;
            }
        }
        public void ReloadMesh()
        {
            if (line_indices.Count > 0)
            {
                wall.BevelVertex(this);
                foreach (int i in line_indices)
                {
                    wall.BevelVertex(wall.lines[i].GetOther(this));
                }
            }
            wall.RefreshGameObjects();
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(Vertex))]
    [CanEditMultipleObjects]
    public class WallMakerEditor : Editor
    {
        public void OnSceneGUI()
        {
            var t = target as Vertex;
            var tr = t.transform;
            var pos = tr.position;
            // display an orange disc where the object is
            var color = new Color(1, 0.8f, 0.4f, 1);
            Handles.color = color;
            Handles.DrawWireDisc(pos, tr.up, 1.0f);
            // display object "value" in scene
            GUI.color = color;
            Handles.Label(pos, "1");
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Vertex t = target as Vertex;

            if (t == null)
                return;
            GameObject[] vertex_gos = Selection.gameObjects;
            if (vertex_gos.Length == 2)
            {
                if (vertex_gos[0].GetComponent<Vertex>() && vertex_gos[1].GetComponent<Vertex>())
                {
                    Vertex first_vertex = vertex_gos[0].GetComponent<Vertex>();
                    Vertex second_vertex = vertex_gos[1].GetComponent<Vertex>();

                    if (GUILayout.Button("Connect"))
                    {
                        t.wall.ConnectVertices(first_vertex, second_vertex);
                    }

                    if (GUILayout.Button("Remove Line"))
                    {
                        int common_item = first_vertex.GetLineIndex(second_vertex);
                        t.wall.RemoveLine(common_item);
                    }
                }
            }
            else if(vertex_gos.Length == 1)
            {
                if (GUILayout.Button("Clone"))
                {
                    t.wall.selected_vertex = t;
                    t.wall.NewVertex();


                }
                if (GUILayout.Button("Delete"))
                {
                    t.wall.selected_vertex = t;

                    t.wall.RemoveLines(t.line_indices.ToArray());
                    
                    t.wall.RemoveVertex(t);
                    DestroyImmediate(t.gameObject);
                }
            }
        }
    }
#endif
}
