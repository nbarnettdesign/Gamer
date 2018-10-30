using UnityEditor;
using System.Collections.Generic;
using UnityEngine;


[CustomPropertyDrawer(typeof(AxisKeys))]
public class AxisKeysDrawer : PropertyDrawer {

    //adjust inspector visuals

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //ensure override works on entire property
        EditorGUI.BeginProperty(position, label, property);

        //don't indent
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        //label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //Set position rects
        Rect posLabel = new Rect(position.x, position.y, 15, position.height);
        Rect posField = new Rect(position.x + 20, position.y, 50, position.height);
        Rect negLabel = new Rect(position.x + 75, position.y, 15, position.height);
        Rect negField = new Rect(position.x + 90, position.y, 50, position.height);

        //Set labels
        GUIContent posGUI = new GUIContent("+");
        GUIContent negGUI = new GUIContent("-");

        //Draw fields
        EditorGUI.LabelField(posLabel, posGUI);
        EditorGUI.PropertyField(posField, property.FindPropertyRelative("positive"), GUIContent.none);
        EditorGUI.LabelField(negLabel, negGUI);
        EditorGUI.PropertyField(negField, property.FindPropertyRelative("negative"), GUIContent.none);


        //Reset idnent
        EditorGUI.indentLevel = indent;

        //end property
        EditorGUI.EndProperty();
    }
}
