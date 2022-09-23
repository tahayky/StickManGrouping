using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Centurion.WallMaker
{
    [CustomEditor(typeof(Wall))]
    public class WallMakerEditor : Editor
    {
        SerializedProperty m_height;
        SerializedProperty m_thickness;
        void OnEnable()
        {
            m_height = serializedObject.FindProperty("height");
            m_thickness = serializedObject.FindProperty("thickness");
        }

        void OnDestroy()
        {

        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Wall t = target as Wall;

            if (t == null)
                return;
            Rect space_height = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(25);
            EditorGUILayout.EndHorizontal();
            Rect space_thickness = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(25);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(space_height, m_height, new GUIContent("Height"));
            if (EditorGUI.EndChangeCheck())
            {
                t.ChangeHeight();
            }
            EditorGUI.BeginChangeCheck();
            EditorGUI.Slider(space_thickness, m_thickness,0,1, new GUIContent("Thickness"));
            if (EditorGUI.EndChangeCheck())
            {
                t.ChangeThickness();
            }
            t.edit_mode = EditorGUILayout.BeginToggleGroup("Edit Mode", t.edit_mode);

            if (GUILayout.Button("New Vertex"))
            {
                t.NewVertex();
            }
            EditorGUILayout.EndToggleGroup();
            if (t.edit_mode)
                t.OnModeChanged(Mode.EditMode);
            else
                t.OnModeChanged(Mode.ObjectMode);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
