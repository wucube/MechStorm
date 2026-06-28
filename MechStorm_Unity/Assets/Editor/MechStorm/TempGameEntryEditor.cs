using MechStorm.Presentation;
using UnityEditor;
using UnityEngine;

namespace MechStorm.Editor
{
    [CustomEditor(typeof(TempGameEntry))]
    public sealed class TempGameEntryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Apply Debug Damage To PlayerA"))
                {
                    foreach (var selectedTarget in targets)
                    {
                        ((TempGameEntry)selectedTarget).ApplyDebugDamageToPlayerA();
                    }
                }
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to apply debug damage.", MessageType.Info);
            }
        }
    }
}
