using CoreLoop.Interfaces;
using UnityEditor;
using UnityEngine;

namespace CoreLoop.Editor
{
    [CustomPropertyDrawer(typeof(SceneStateSelection))]
    public class SceneStateSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty stateTypeProp = property.FindPropertyRelative("stateType");
            SerializedProperty payloadProp = property.FindPropertyRelative("payload");

            Rect stateTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(stateTypeRect, stateTypeProp, label);
            bool stateTypeChanged = EditorGUI.EndChangeCheck();

            SceneStateType stateType = (SceneStateType)stateTypeProp.intValue;
            if (stateTypeChanged || PayloadNeedsUpdate(payloadProp, stateType))
            {
                UpdatePayload(payloadProp, stateType);
            }

            if (payloadProp.managedReferenceValue != null)
            {
                Rect payloadRect = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUI.GetPropertyHeight(payloadProp, true));

                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(payloadRect, payloadProp, true);
                EditorGUI.indentLevel = indent;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty payloadProp = property.FindPropertyRelative("payload");
            float height = EditorGUIUtility.singleLineHeight;

            if (payloadProp.managedReferenceValue != null)
            {
                height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(payloadProp, true);
            }

            return height;
        }

        private static bool PayloadNeedsUpdate(SerializedProperty payloadProp, SceneStateType stateType)
        {
            if (stateType == SceneStateType.None)
            {
                return payloadProp.managedReferenceValue != null;
            }

            if (!SceneStateRegistry.TryGetDescriptor(stateType, out var descriptor))
            {
                return payloadProp.managedReferenceValue != null;
            }

            if (!descriptor.HasPayload)
            {
                return payloadProp.managedReferenceValue != null;
            }

            return !SceneStateSelection.PayloadMatchesState(payloadProp.managedReferenceValue as IStatePayload, stateType);
        }

        private static void UpdatePayload(SerializedProperty payloadProp, SceneStateType stateType)
        {
            payloadProp.managedReferenceValue = SceneStateSelection.CreatePayload(stateType);
        }
    }
}