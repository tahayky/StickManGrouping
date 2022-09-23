using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
namespace Centurion.WallMaker
{
    public class Wall : MonoBehaviour
    {
        public List<Vertex> vertices;
        public List<Line> lines;

        public Vertex selected_vertex = null;

        public int vertex_count = 0;
        public Mode mode=Mode.ObjectMode;
        public bool edit_mode = false;
        [HideInInspector]public float height = 5;
        public float thickness = 5;
        public MeshBuilder mesh_builder;
        private void Start()
        {
            ChangeHeight();
            RefreshGameObjects();
        }
        void Reset()
        {
            vertices = new List<Vertex>();
            lines = new List<Line>();
            mesh_builder = new MeshBuilder(this);
        }
        public void OnModeChanged(Mode mode)
        {
            if (mode == this.mode)
                return;
            switch (mode)
            {
                case Mode.ObjectMode:
                    InvisibleVertices();
                    break;
                case Mode.EditMode:
                    VisibleVertices();
                    break;

            }
            this.mode = mode;
        }
        public void ChangeHeight()
        {
            for (int i=0;i<vertices.Count;i++)
            {
                vertices[i].transform.localPosition = new Vector3(vertices[i].transform.localPosition.x, height, vertices[i].transform.localPosition.z);
            }
        }
        public void ChangeThickness()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].thickness = thickness;
            }
            for (int i = 0; i < vertices.Count; i++)
            {
                mesh_builder.BevelVertex(vertices[i]);
            }
            mesh_builder.RefreshGameObjects(height);
        }
        public Vertex CreateVertex(int id)
        {
            GameObject vertex_go = new GameObject();
#if UNITY_EDITOR
            var iconContent = EditorGUIUtility.IconContent("Animation.Record@2x");
            EditorGUIUtility.SetIconForObject(vertex_go, (Texture2D)iconContent.image);
            DestroyImmediate(vertex_go.GetComponent<Collider>());
#endif
            Vertex vertex =vertex_go.AddComponent<Vertex>();
            vertex.id = id;
            vertex.wall = this;
            vertex_go.transform.SetParent(this.transform);
#if UNITY_EDITOR
            Selection.activeGameObject = vertex_go;
#endif
            return vertex;
        }
        public void BevelVertex(Vertex vertex)
        {
            mesh_builder.BevelVertex(vertex);
        }
        public int AddLine(Line line)
        {
            int last_index_of_null = lines.FindIndex(item=>item.empty);
            if (last_index_of_null != -1)
            {
                lines[last_index_of_null] = line;
                lines[last_index_of_null].empty = false;
                return last_index_of_null;
            }
            else
            {
                lines.Add(line);
                return lines.Count - 1;
            }

        }
        public void RemoveVertex(Vertex vertex)
        {
            vertices.Remove(vertex);
        }
        public void RemoveLine(int index)
        {
            Vertex[] vertices_of_line = new Vertex[] { lines[index].v1, lines[index].v2 };
            for (int i=0;i<vertices_of_line.Length;i++)
            {
                vertices_of_line[i].RemoveLine(index);
            }
            lines[index].empty = true;
            mesh_builder.RefreshGameObjects(vertices[0].transform.localPosition.y);
        }
        public void RemoveLines(int[] indices)
        {
            for (int k = 0; k < indices.Length; k++)
            {
                Vertex[] vertices_of_line = new Vertex[] { lines[indices[k]].v1, lines[indices[k]].v2 };
                for (int i = 0; i < vertices_of_line.Length; i++)
                {
                    vertices_of_line[i].RemoveLine(indices[k]);
                }
                lines[indices[k]].empty = true;
            }
            mesh_builder.RefreshGameObjects(vertices[0].transform.localPosition.y);
        }
        public Vertex NewVertex(Vector2 position)
        {
            return null;
        }
        public Vertex NewVertex()
        {
            Vertex vertex;
            if (selected_vertex != null)
            {
                vertex = CreateVertex(vertex_count);
                vertex.transform.localPosition = selected_vertex.transform.localPosition;
                vertex.transform.localPosition = new Vector3(vertex.transform.localPosition.x, height, vertex.transform.localPosition.z);
                AddVertex(vertex);
                Line line = new Line(selected_vertex, vertex);
                line.thickness = thickness;
                int index = AddLine(line);
                vertex.AddLine(index);
                selected_vertex.AddLine(index);
                BevelVertex(vertex);
                BevelVertex(selected_vertex);

            }
            else
            {
                vertex = CreateVertex(vertex_count);
                vertex.transform.localPosition = Vector3.zero;
                vertex.transform.localPosition = new Vector3(vertex.transform.localPosition.x, height, vertex.transform.localPosition.z);
                AddVertex(vertex);
            }
            RefreshGameObjects();
            vertex_count++;
            return vertex;
        }
        public void AddVertex(Vertex vertex)
        {
            vertices.Add(vertex);
        }
        public void ConnectVertices(Vertex first_vertex, Vertex second_vertex)
        {
            Line line = new Line(first_vertex, second_vertex);
            line.thickness = thickness;
            int index = AddLine(line);
            first_vertex.AddLine(index);
            second_vertex.AddLine(index);
            BevelVertex(first_vertex);
            BevelVertex(second_vertex);
            RefreshGameObjects();
        }
        public void RefreshGameObjects()
        {
            mesh_builder.RefreshGameObjects(height);
        }
        public void InvisibleVertices()
        {/*
            foreach (Vertex vert in vertices)
            {
                vert.gameObject.GetComponent<MeshRenderer>().enabled = false;
                vert.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }*/
        }
        public void VisibleVertices()
        {
            /*
            foreach (Vertex vert in vertices)
            {
                vert.gameObject.GetComponent<MeshRenderer>().enabled = true;
                vert.gameObject.hideFlags = HideFlags.None;
            }*/
        }
    }
}
