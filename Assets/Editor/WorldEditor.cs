using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(World.World))]
    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
    public class WorldEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var world = (World.World)target;
            DrawDefaultInspector();

            var guiStyle = new GUIStyle {
                normal = {
                    textColor = Color.green
                },
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Editor Stuff", guiStyle);
            if (GUILayout.Button("Gen Chunks For Editor")) {
                world.GenChunksForEditor();
            }


            if (GUILayout.Button("Destroy Chunks For Editor")) {
                world.ClearChunksFromEditor();
            }
        }
    }
}