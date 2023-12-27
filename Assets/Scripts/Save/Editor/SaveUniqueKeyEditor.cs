using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

//[CustomPropertyDrawer(typeof(SaveUniqueKey))]
//public class SaveUniqueKeyEditor : PropertyDrawer
//{
//    public override VisualElement CreatePropertyGUI(SerializedProperty property)
//    {
//        // Create property container element.
//        var container = new VisualElement();

//        // Create property fields.
//        var uniqueKey = new PropertyField(property.FindPropertyRelative("uniqueKey"));

//        // Add fields to the container.
//        container.Add(uniqueKey);

//        return container;
//    }

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // Using BeginProperty / EndProperty on the parent property means that
//        // prefab override logic works on the entire property.
//        var key = property.FindPropertyRelative("uniqueKey");
//        EditorGUI.BeginProperty(position, label, property);
//        EditorGUI.PropertyField(position, key, label, true);
//        EditorGUI.EndProperty();

//        GUIContent btnTxt = new GUIContent("Regenerate Key");
//        var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.Width(200));
//        rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);
//        if(GUI.Button(rt, btnTxt, GUI.skin.button) || string.IsNullOrEmpty(key.stringValue))
//        {
//            key.stringValue = SaveUniqueKey.GenerateKey();
//        }

//        //SaveUniqueKey keyScript = (SaveUniqueKey)property.;
//    }
//}
