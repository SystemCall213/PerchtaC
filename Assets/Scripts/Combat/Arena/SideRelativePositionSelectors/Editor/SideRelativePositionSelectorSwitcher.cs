using Combat.Arena.SideRelativePositionSelectors;
using UnityEditor;
using UnityEngine;

namespace Combat.Arena.SideRelativePositionSelectors.Editor
{
    [CustomPropertyDrawer(typeof(SideRelativePositionSelectorData))]
    public class SideRelativePositionSelectorSwitcher : PropertyDrawer
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
            SideRelativePositionSelectorType type = (SideRelativePositionSelectorType)typeProp.enumValueIndex;
            
            if (!SideRelativePositionSelectorData.SelectorMatchesType(selectorProp.managedReferenceValue as ISideRelativePositionSelector, type))
            {
                selectorProp.managedReferenceValue = SideRelativePositionSelectorData.CreateSelector(type);
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
