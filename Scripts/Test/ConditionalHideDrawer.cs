using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHide))]
public class ConditionalHideDrawer : PropertyDrawer
{
    bool showField = true;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHide attribute = (ConditionalHide)this.attribute;
        SerializedProperty prop = property.serializedObject.FindProperty(attribute.conditionName);
        
        if (prop.propertyType == SerializedPropertyType.ObjectReference)
        {
            showField = prop.objectReferenceValue != null;
        }else
            showField = prop.boolValue;

        if (attribute.invert) showField = !showField;

        if (showField)
            EditorGUI.PropertyField(position, property, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (showField)
            return EditorGUI.GetPropertyHeight(property);
        else
            return -EditorGUIUtility.standardVerticalSpacing;
    }
}
