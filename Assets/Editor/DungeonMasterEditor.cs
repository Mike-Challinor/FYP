using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonMasterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector UI
        DrawDefaultInspector();

        // Get reference to the DungeonGenerator script
        DungeonGenerator dungeonGenerator = (DungeonGenerator)target;

        // Add a button to the custom inspector
        if (GUILayout.Button("Generate Dungeon"))
        {
            // Call the method to start the coroutine
            dungeonGenerator.StartGenerateDungeonCoroutine();

            Debug.Log("Dungeon generation started!");
        }
    }
}
