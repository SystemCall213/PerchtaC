using Combat.Arena;
using UnityEditor;
using UnityEngine;

namespace Combat.Arena.Editor
{
    [CustomPropertyDrawer(typeof(RadialPositionSelectorData))]
    public class RadialPositionSelectorSwitcher : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty typeProp = property.FindPropertyRelative("type");
            SerializedProperty selectorProp = property.FindPropertyRelative("selector");

            Rect typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(typeRect, typeProp, label);
            if (EditorGUI.EndChangeCheck() || selectorProp.managedReferenceValue == null)
            {
                UpdateSelector(typeProp, selectorProp);
            }

            if (selectorProp.managedReferenceValue != null)
            {
                Rect selectorRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUI.GetPropertyHeight(selectorProp, true));
                
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(selectorRect, selectorProp, true);
                EditorGUI.indentLevel = indent;
            }

            EditorGUI.EndProperty();
        }

        private void UpdateSelector(SerializedProperty typeProp, SerializedProperty selectorProp)
        {
            RadialPositionSelectorType type = (RadialPositionSelectorType)typeProp.enumValueIndex;
            
            if (!RadialPositionSelectorData.SelectorMatchesType(selectorProp.managedReferenceValue as IRadialPositionSelector, type))
            {
                selectorProp.managedReferenceValue = RadialPositionSelectorData.CreateSelector(type);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty selectorProp = property.FindPropertyRelative("selector");
            float height = EditorGUIUtility.singleLineHeight;
            
            if (selectorProp.managedReferenceValue != null)
            {
                height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(selectorProp, true);
            }
            
            return height;
        }
    }
}