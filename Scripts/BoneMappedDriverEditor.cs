using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(BoneMappedDriver))]
public class BoneMappedDriverEditor : Editor
{
        ReorderableList list;

        void OnEnable()
        {
                var property = serializedObject.FindProperty("BlendShapes");
                list = new ReorderableList(serializedObject, property, true, true, true, true);

                list.onAddCallback = (ReorderableList list) =>
                {
                        if (property.isArray)
                        {
                                int next = property.arraySize;
                                property.InsertArrayElementAtIndex(next);
                                var row = property.GetArrayElementAtIndex(next);
                                row.FindPropertyRelative("Name").stringValue = "";
                                row.FindPropertyRelative("Func").animationCurveValue = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
                                row.FindPropertyRelative("Mask").vector2Value = new Vector2(0.0f, 1.0f);
                        }
                };

                list.elementHeightCallback = (index) =>
                {
                        var e = property.GetArrayElementAtIndex(index);
                        return EditorGUI.GetPropertyHeight(e.FindPropertyRelative("Name")) +
                                EditorGUI.GetPropertyHeight(e.FindPropertyRelative("Func")) +
                                EditorGUI.GetPropertyHeight(e.FindPropertyRelative("Mask"));
                };

                list.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                        var e = property.GetArrayElementAtIndex(index);
                        rect.height = EditorGUI.GetPropertyHeight(e.FindPropertyRelative("Name"));
                        EditorGUI.PropertyField(rect, e.FindPropertyRelative("Name"));
                        rect.y += rect.height;
                        rect.height = EditorGUI.GetPropertyHeight(e.FindPropertyRelative("Func"));
                        EditorGUI.PropertyField(rect, e.FindPropertyRelative("Func"));
                        rect.y += rect.height;
                        rect.height = EditorGUI.GetPropertyHeight(e.FindPropertyRelative("Mask"));
                        EditorGUI.PropertyField(rect, e.FindPropertyRelative("Mask"), new GUIContent("Active Mask [X..Y]"));
                };

        }

        public override void OnInspectorGUI()
        {
                base.OnInspectorGUI();

                serializedObject.Update();
                list.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
        }
}
