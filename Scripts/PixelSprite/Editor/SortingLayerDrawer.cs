using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SortingLayerAttribute))]
public class SortingLayerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        List<string> layers = new List<string>();
        foreach (SortingLayer item in SortingLayer.layers)
        {
            layers.Add(item.name);
        }   
        int s = layers.Contains(property.stringValue) ? EditorGUI.Popup(position, label.text, layers.IndexOf(property.stringValue), layers.ToArray(), EditorStyles.popup) : 0;
        property.stringValue = layers[s];
    }
}
