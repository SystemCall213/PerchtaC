using Glyph.Glyph_HoldPoint;
using UnityEditor;
using Installers;

namespace Installers.Editor
{
    [CustomEditor(typeof(CombatInstaller))]
    public class CombatInstallerEditor : UnityEditor.Editor
    {
        private SerializedProperty combatArena;
        private SerializedProperty combatScenario;
        private SerializedProperty glyphSystemType;
        private SerializedProperty glyphSOInstaller;
        private SerializedProperty glyphFollowerPrefab;
        private SerializedProperty glyphLinesSOInstaller;

        private void OnEnable()
        {
            combatArena = serializedObject.FindProperty("combatArena");
            combatScenario = serializedObject.FindProperty("combatScenario");
            glyphSystemType = serializedObject.FindProperty("glyphSystemType");
            glyphSOInstaller = serializedObject.FindProperty("glyphSOInstaller");
            glyphFollowerPrefab = serializedObject.FindProperty("glyphFollowerPrefab");
            glyphLinesSOInstaller = serializedObject.FindProperty("glyphLinesSOInstaller");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(combatArena);
            EditorGUILayout.PropertyField(combatScenario);
            EditorGUILayout.PropertyField(glyphSystemType);

            if (glyphSystemType.enumValueIndex == (int)GlyphSystemType.BlindPainting)
            {
                EditorGUILayout.PropertyField(glyphSOInstaller);
            }
            else if (glyphSystemType.enumValueIndex == (int)GlyphSystemType.HoldPoint)
            {
                EditorGUILayout.PropertyField(glyphFollowerPrefab);
                EditorGUILayout.PropertyField(glyphLinesSOInstaller);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
