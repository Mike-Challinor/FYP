using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGeneratorEditor : Editor
{
    private bool tilemapSettingsFoldout = true;
    private bool cornerWallSettingsFoldout = true;
    private bool roomSettingsFoldout = true;
    private bool generatorSettingsFoldout = true;

    // Nested foldouts for walls
    private bool horizontalWallsFoldout = true;
    private bool verticalWallsFoldout = true;

    // Serialized properties
    private SerializedProperty m_horizontalWallsTopUpper;
    private SerializedProperty m_horizontalWallsBottom;
    private SerializedProperty m_verticalWallsLeft;
    private SerializedProperty m_verticalWallsRight;
    private SerializedProperty m_bottomRightCornerWall;
    private SerializedProperty m_bottomLeftCornerWall;
    private SerializedProperty m_bottomLeftCornerWallUpper;
    private SerializedProperty m_bottomRightCornerWallUpper;


    private void OnEnable()
    {
        // Initialize serialized properties
        m_horizontalWallsTopUpper = serializedObject.FindProperty("m_horizontalWallsTopUpper");
        m_horizontalWallsBottom = serializedObject.FindProperty("m_horizontalWallsBottom");
        m_verticalWallsLeft = serializedObject.FindProperty("m_verticalWallsLeft");
        m_verticalWallsRight = serializedObject.FindProperty("m_verticalWallsRight");
        m_bottomRightCornerWall = serializedObject.FindProperty("m_bottomRightCornerWall");
        m_bottomLeftCornerWall = serializedObject.FindProperty("m_bottomLeftCornerWall");
        m_bottomLeftCornerWallUpper = serializedObject.FindProperty("m_bottomLeftCornerWallUpper");
        m_bottomRightCornerWallUpper = serializedObject.FindProperty("m_bottomRightCornerWallUpper");
    }

    public override void OnInspectorGUI()
    {
        // Reference to the script
        RoomGenerator roomGenerator = (RoomGenerator)target;

        serializedObject.Update();

        // Tilemap Settings Foldout
        tilemapSettingsFoldout = EditorGUILayout.Foldout(tilemapSettingsFoldout, "Tilemap Settings", true);
        if (tilemapSettingsFoldout)
        {
            EditorGUI.indentLevel++; // Indent the elements inside the foldout

            roomGenerator.m_tileMap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", roomGenerator.m_tileMap, typeof(Tilemap), true);

            // Horizontal Walls Foldout (nested)
            horizontalWallsFoldout = EditorGUILayout.Foldout(horizontalWallsFoldout, "Horizontal Walls", true);
            if (horizontalWallsFoldout)
            {
                EditorGUI.indentLevel++; // Indent the elements inside the foldout

                EditorGUILayout.PropertyField(m_horizontalWallsTopUpper, new GUIContent("Top Upper"), true);
                EditorGUILayout.PropertyField(m_horizontalWallsBottom, new GUIContent("Bottom"), true);

                EditorGUI.indentLevel--; // Reset indentation after the foldout content
            }

            // Vertical Walls Foldout (nested)
            verticalWallsFoldout = EditorGUILayout.Foldout(verticalWallsFoldout, "Vertical Walls", true);
            if (verticalWallsFoldout)
            {
                EditorGUI.indentLevel++; // Indent the elements inside the foldout

                EditorGUILayout.PropertyField(m_verticalWallsLeft, new GUIContent("Left"), true);
                EditorGUILayout.PropertyField(m_verticalWallsRight, new GUIContent("Right"), true);

                EditorGUI.indentLevel--; // Reset indentation after the foldout content
            }

            // Corner Wall Settings Foldout
            cornerWallSettingsFoldout = EditorGUILayout.Foldout(cornerWallSettingsFoldout, "Corner Wall Settings", true);
            if (cornerWallSettingsFoldout)
            {
                EditorGUI.indentLevel++; // Indent the elements inside the foldout

                EditorGUILayout.PropertyField(m_bottomRightCornerWall, new GUIContent("Bottom Right"), true);
                EditorGUILayout.PropertyField(m_bottomLeftCornerWall, new GUIContent("Bottom Left"), true);
                EditorGUILayout.PropertyField(m_bottomLeftCornerWallUpper, new GUIContent("Bottom Left Corner Upper"), true);
                EditorGUILayout.PropertyField(m_bottomRightCornerWallUpper, new GUIContent("Bottom Right Corner Upper"), true);

                roomGenerator.m_topLeftCornerWall = (TileBase)EditorGUILayout.ObjectField("Top Left", roomGenerator.m_topLeftCornerWall, typeof(TileBase), true);
                roomGenerator.m_topLeftCornerWallUpper = (TileBase)EditorGUILayout.ObjectField("Top Left Upper", roomGenerator.m_topLeftCornerWallUpper, typeof(TileBase), true);
                roomGenerator.m_topLeftCornerWallLower = (TileBase)EditorGUILayout.ObjectField("Top Left Lower", roomGenerator.m_topLeftCornerWallLower, typeof(TileBase), true);
                roomGenerator.m_topRightCornerWall = (TileBase)EditorGUILayout.ObjectField("Top Right", roomGenerator.m_topRightCornerWall, typeof(TileBase), true);
                roomGenerator.m_insideWallCornerLeftUpper = (TileBase)EditorGUILayout.ObjectField("Inside Left Upper", roomGenerator.m_insideWallCornerLeftUpper, typeof(TileBase), true);
                roomGenerator.m_insideWallCornerLeftLower = (TileBase)EditorGUILayout.ObjectField("Inside Left Lower", roomGenerator.m_insideWallCornerLeftLower, typeof(TileBase), true);
                roomGenerator.m_insideWallCornerRightUpper = (TileBase)EditorGUILayout.ObjectField("Inside Right Upper", roomGenerator.m_insideWallCornerRightUpper, typeof(TileBase), true);
                roomGenerator.m_insideWallCornerRightLower = (TileBase)EditorGUILayout.ObjectField("Inside Right Lower", roomGenerator.m_insideWallCornerRightLower, typeof(TileBase), true);
                roomGenerator.m_verticalDoorwayTopLeft = (TileBase)EditorGUILayout.ObjectField("Vertical Doorway corner top left", roomGenerator.m_verticalDoorwayTopLeft, typeof(TileBase), true);
                roomGenerator.m_verticalDoorwayTopRight = (TileBase)EditorGUILayout.ObjectField("Vertical Doorway corner top right", roomGenerator.m_verticalDoorwayTopRight, typeof(TileBase), true);
                
                EditorGUI.indentLevel--; // Reset indentation after the foldout content
            }

            EditorGUI.indentLevel--; // Reset indentation after the entire "Tilemap Settings" section
        }

        // Room Settings Foldout
        roomSettingsFoldout = EditorGUILayout.Foldout(roomSettingsFoldout, "Room Settings", true);
        if (roomSettingsFoldout)
        {
            // Ensure indentation level is reset to 0 before Room Settings
            EditorGUI.indentLevel = 0;

            EditorGUI.indentLevel++; // Indent the elements inside the foldout

            roomGenerator.m_width = EditorGUILayout.IntField("Width", roomGenerator.m_width);
            roomGenerator.m_height = EditorGUILayout.IntField("Height", roomGenerator.m_height);
            roomGenerator.m_xLocation = EditorGUILayout.IntField("X Location", roomGenerator.m_xLocation);
            roomGenerator.m_yLocation = EditorGUILayout.IntField("Y Location", roomGenerator.m_yLocation);
            roomGenerator.m_numberOfDoorways = EditorGUILayout.IntField("Number of Doorways", roomGenerator.m_numberOfDoorways);

            EditorGUI.indentLevel--; // Reset indentation after the foldout content
        }

        // Generator Settings Foldout
        generatorSettingsFoldout = EditorGUILayout.Foldout(generatorSettingsFoldout, "Generator Settings", true);
        if (generatorSettingsFoldout)
        {
            // Ensure indentation level is reset to 0 before Room Settings
            EditorGUI.indentLevel = 0;

            EditorGUI.indentLevel++; // Indent the elements inside the foldout

            roomGenerator.m_drawTime = EditorGUILayout.FloatField("Draw Time", roomGenerator.m_drawTime);
            roomGenerator.m_isDrawing = EditorGUILayout.Toggle("Draw Time", roomGenerator.m_isDrawing);

            EditorGUI.indentLevel--; // Reset indentation after the foldout content
        }

        serializedObject.ApplyModifiedProperties();

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(roomGenerator);
        }
    }
}
