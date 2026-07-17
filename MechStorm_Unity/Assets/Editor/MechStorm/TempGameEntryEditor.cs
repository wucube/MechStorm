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
            EditorGUILayout.LabelField("Battle Debug", EditorStyles.boldLabel);

            var gameEntry = (TempGameEntry)target;
            if (Application.isPlaying && gameEntry.IsBattleReady)
            {
                EditorGUILayout.LabelField(
                    "Current Round",
                    gameEntry.CurrentRoundNumber.ToString());
                EditorGUILayout.LabelField(
                    "Current Faction",
                    gameEntry.CurrentFactionName);
                EditorGUILayout.LabelField(
                    "Current Unit",
                    gameEntry.CurrentUnitName);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Enter Play Mode and wait for BattleSession initialization.",
                    MessageType.Info);
            }

            using (new EditorGUI.DisabledScope(
                       !Application.isPlaying || !gameEntry.IsBattleReady))
            {
                if (GUILayout.Button("Log Current Battle State"))
                {
                    foreach (var selectedTarget in targets)
                    {
                        ((TempGameEntry)selectedTarget)
                            .LogCurrentBattleStateForDebug();
                    }
                }

                if (GUILayout.Button("Export Battle Debug JSON"))
                {
                    foreach (var selectedTarget in targets)
                    {
                        ((TempGameEntry)selectedTarget)
                            .ExportBattleDebugJsonForDebug();
                    }
                }

                if (GUILayout.Button("End Current Unit Action"))
                {
                    foreach (var selectedTarget in targets)
                    {
                        ((TempGameEntry)selectedTarget)
                            .EndCurrentUnitActionForDebug();
                    }
                }

                if (GUILayout.Button("Attack Current Opponent"))
                {
                    foreach (var selectedTarget in targets)
                    {
                        ((TempGameEntry)selectedTarget)
                            .AttackCurrentOpponentForDebug();
                    }
                }

                if (GUILayout.Button("Apply Debug Damage To PlayerA"))
                {
                    foreach (var selectedTarget in targets)
                    {
                        ((TempGameEntry)selectedTarget).ApplyDebugDamageToPlayerA();
                    }
                }
            }
        }
    }
}
