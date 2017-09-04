using UnityEngine;
using PaintCraft.Tools;
using UnityEditor;
using System.Reflection;


namespace PaintCraft.Editor{
    [CustomPropertyDrawer(typeof(PointColor))]
    public class PointColorPropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FieldInfo fieldInfo = property.serializedObject.targetObject.GetType().GetField(property.name, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            PointColor c = (PointColor)fieldInfo.GetValue(property.serializedObject.targetObject);
            Color newColor = EditorGUI.ColorField(position, label, c.Color);
            if (newColor != c.Color){
                c.Color = newColor;
                property.serializedObject.UpdateIfDirtyOrScript();
                property.serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(property.serializedObject.targetObject);
            }

        }
    }
}