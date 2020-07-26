using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelEditor : EditorWindow
{
    [MenuItem("Window/LevelEditor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Editor");
    }

    public EditorTile[,] tiles;

    public GameObject go;

    public string[] colorString = { "Yellow", "Green", "Violet", "Red" };

    public int selectedSprite;

    public int sizeX = 4;
    public int sizeY = 4;

    private bool isMapInitialised = false;

    private void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Size X", GUILayout.Width(50f));
        sizeX = EditorGUILayout.IntField(sizeX, GUILayout.Width(50f));

        EditorGUILayout.LabelField("Size Y", GUILayout.Width(50f));
        sizeY = EditorGUILayout.IntField(sizeY, GUILayout.Width(50f));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("CLICK IF YOU CHANGE SIZES!", GUILayout.Width(190f));
        if (GUILayout.Button("New Map", GUILayout.Width(70), GUILayout.Height(20)))
        {
            tiles = new EditorTile[sizeX, sizeY];
            isMapInitialised = true;
        }
        EditorGUILayout.EndHorizontal();

        selectedSprite = GUILayout.SelectionGrid(selectedSprite, colorString, 4);

        EditorGUILayout.BeginHorizontal();


        if (isMapInitialised)
        {
            for (int x = 0; x < sizeX; x++)
            {
                EditorGUILayout.BeginVertical();
                for (int y = sizeY - 1; y >= 0; y--)
                {
                    if (GUILayout.Button(GetColorString(x, y), GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        ToArray(x, y, selectedSprite);
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Save Level", GUILayout.Width(100), GUILayout.Height(20)))
        {
            SaveLevel();
        }
    }

    public void SaveLevel()
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "/StreamingAssets");
        writer.WriteLine(sizeX + "|" + sizeY);

        foreach (EditorTile item in tiles)
        {
            if (item == null)
            {
                Debug.LogError("Заполните все поля!");
            }
            else
            {
                writer.WriteLine(item.x + "|" + item.y + "|" + item.sprite);
            }
        }

        writer.Close();

        Debug.Log(this + " Cохранение в файл: " + Application.dataPath + "/StreamingAssets");
    }





    public string GetColorString(int x, int y)
    {
        if (tiles[x, y] != null)
        {
            return colorString[tiles[x, y].sprite];
        }
        else
        {
            return " ";
        }
    }

    public void ToArray(int x, int y, int sprite)
    {
        

        tiles[x, y] = new EditorTile(x, y, sprite);
    }
}

public class EditorTile
{
    public int x;
    public int y;
    public int sprite;

    public EditorTile(int X, int Y, int Sprite)
    {
        x = X;
        y = Y;
        sprite = Sprite;
    }


}
