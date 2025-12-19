using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMakerWindow : EditorWindow
{
    GenerateGrid generator;
    
    [MenuItem("Grid/Grid Window")]
    public static void ShowExample()
    {
        GridWindow wnd = GetWindow<GridWindow>();
        wnd.titleContent = new GUIContent("Grid Window");
    }

    public void OnGUI()
    {
        generator = (GenerateGrid)EditorGUILayout.ObjectField(
            "Target Grid",
            generator,
            typeof(GenerateGrid),
            true);
        
        if(GUILayout.Button("Remake Grid"))
        {
            generator.GenerateNewGrid();
        }

        if(GUILayout.Button("Delete Grid"))
        {
            generator.ClearGrid();
        }
    }
}