using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMakerWindow : EditorWindow
{
    GenerateGrid generator;
    EnemyBehavior enemy;
    
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

        enemy = (EnemyBehavior)EditorGUILayout.ObjectField("Test Enemy", enemy, typeof(EnemyBehavior), true);
        
        if(GUILayout.Button("Remake Grid"))
        {
            generator.GenerateNewGrid();
        }

        if(GUILayout.Button("Delete Grid"))
        {
            generator.ClearGrid();
        }

        if(GUILayout.Button("Make Path"))
        {
            enemy.TestCalculationOfPath();
        }
    }
}