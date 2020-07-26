using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Board : MonoBehaviour
{
    public static Board instance;

    public List<Sprite> sprites;

    public GameObject[,] tiles;
    private List<GameObject> tilesToDestroy;

    public int gridSizeX; //размер сетки
    public int gridSizeY; //размер сетки

    public float xOffset;
    public float yOffset;

    public GameObject tile; //префаб тайла

    private int choosenSpriteNumber;
    private Sprite fillMatchChecker;

    private int sameCount = 1;
    private bool isTilesMatched = false;
    private bool matched = false;

    void Start()
    {
        instance = this;
        Load();
        tilesToDestroy = new List<GameObject>();
    }

    private void Load()
    {
        if (!File.Exists(Application.dataPath + "/StreamingAssets"))
        {
            FillGrid();
            return;
        }

        StreamReader reader = new StreamReader(Application.dataPath + "/StreamingAssets");

        string[] sizes = reader.ReadLine().Split(new char[] { '|' });

        gridSizeX = int.Parse(sizes[0]);
        gridSizeY = int.Parse(sizes[1]);

        tiles = new GameObject[gridSizeX, gridSizeY];

        while (!reader.EndOfStream)
        {
            ReadAndCreateFromFile(reader.ReadLine());
        }

        reader.Close();
    }

    private void ReadAndCreateFromFile(string tileData)
    {
        string[] data = tileData.Split(new char[] { '|' });
        choosenSpriteNumber = int.Parse(data[2]);
        CreateTile(int.Parse(data[0]), int.Parse(data[1]));
    }

    public void FillGrid()
    {
        tiles = new GameObject[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                choosenSpriteNumber = Random.Range(0, sprites.Count);
                fillMatchChecker = sprites[choosenSpriteNumber];

                if (
                    x >= 2                                           // если спрайт второй от левого края
                    &&                                               // и
                    fillMatchChecker == GetSpriteByMatrix(x - 1, y)  // он равен предыдущему по горизонтали
                    &&                                               // и
                    fillMatchChecker == GetSpriteByMatrix(x - 2, y)  // он равен пред-предыдущему по горизонтали
                    ||                                               // или
                    y >= 2                                           // если спрайт второй от нижнего края
                    &&                                               // и
                    fillMatchChecker == GetSpriteByMatrix(x, y - 1)  // он равен предыдущему по вертикали
                    &&                                               // и
                    fillMatchChecker == GetSpriteByMatrix(x, y - 2)  // он равен пред-предыдущему по вертикали
                   )
                {
                    DoCircleSpriteNumber();
                }

                CreateTile(x, y);
            }
        }
    }

    private Sprite GetSpriteByMatrix(int x, int y)
    {
        return tiles[x, y].GetComponent<SpriteRenderer>().sprite;
    }

    private void DoCircleSpriteNumber()
    {
        if (choosenSpriteNumber < sprites.Count - 1)
        {
            choosenSpriteNumber++;
        }
        else
        {
            choosenSpriteNumber = 0;
        }
    }

    public IEnumerator CheckMatch()
    {
        tilesToDestroy.Clear();

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                if (x < gridSizeX)
                {
                    if (tiles[x, y] != null)
                    {
                        if (CheckTileHorizontal(x, y))
                        {
                            tilesToDestroy.Add(tiles[x, y]);
                            x += sameCount;
                        }
                    }

                    sameCount = 1;
                }
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (y < gridSizeY)
                {
                    if (tiles[x, y] != null)
                    {
                        if (CheckTileVertical(x, y))
                        {
                            tilesToDestroy.Add(tiles[x, y]);
                            y += sameCount;
                        }
                    }

                    sameCount = 1;
                }
            }
        }

        if (tilesToDestroy.Count > 0)
        {
            Score.instance.AddToScore(tilesToDestroy.Count);
            isTilesMatched = true;
            matched = true;
        }
        else
        {
            isTilesMatched = false;
        }

        foreach (var item in tilesToDestroy)
        {
            item.GetComponent<Tile>().Deactivate();
        }

        if (isTilesMatched)
        {
            if (Falling())
            {
                yield return new WaitForSeconds(2f);
                IsMatched();
            }
        }
    }

    public bool IsMatched()
    {
        matched = false;
        StartCoroutine(CheckMatch());
        return matched;
    }

    public bool IsShifting()
    {
        return matched;
    }

    private bool CheckTileHorizontal(int x, int y)
    {
        bool isSame = false;
        if (x < gridSizeX - 1)
        {
            if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite)
            {
                sameCount++;

                if (x + 1 < gridSizeX)
                {
                    CheckTileHorizontal(x + 1, y);
                }

                if (sameCount >= 3)
                {
                    isSame = true;
                    tilesToDestroy.Add(tiles[x + 1, y]);
                }
            }
        }

        return isSame;
    }

    private bool CheckTileVertical(int x, int y)
    {
        bool isSame = false;
        if (y < gridSizeY - 1)
        {
            if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == tiles[x, y + 1].GetComponent<SpriteRenderer>().sprite)
            {
                sameCount++;

                if (y + 1 < gridSizeY)
                {
                    CheckTileVertical(x, y + 1);
                }

                if (sameCount >= 3)
                {
                    isSame = true;
                    tilesToDestroy.Add(tiles[x, y + 1]);
                }
            }
        }
        return isSame;
    }

    public bool Falling()
    {
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                if (!tiles[x, y].GetComponent<Tile>().IsActive)
                {
                    CheckNextNotNull(x, y);
                }
            }
        }
        return true;
    }

    private void CheckNextNotNull(int x, int y)
    {
        if (y < gridSizeY)
        {
            for (int i = y; i < gridSizeY; i++)
            {
                if (i == gridSizeY - 1 && (tiles[x, i] == null || !tiles[x, i].GetComponent<Tile>().IsActive))
                {
                    choosenSpriteNumber = Random.Range(0, sprites.Count);
                    Destroy(tiles[x, i]);
                    CreateTile(x, i);
                    Moving(tiles[x, y], tiles[x, i]);
                }

                if (tiles[x, i].GetComponent<Tile>().IsActive)
                {
                    Moving(tiles[x, y], tiles[x, i]);
                    return;
                }
            }
        }
    }

    private GameObject CreateTile(int x, int y)
    {
        GameObject newTile = Instantiate(tile, new Vector3(transform.position.x + x * xOffset, transform.position.y + y * yOffset, 0), Quaternion.identity, transform);
        newTile.GetComponent<SpriteRenderer>().sprite = sprites[choosenSpriteNumber];
        newTile.GetComponent<Tile>().x = x;
        newTile.GetComponent<Tile>().y = y;
        tiles[x, y] = newTile;

        return newTile;
    }
    
    public void Move(GameObject target, GameObject falling)
    {
        Vector2 pos1 = target.transform.position;
        Vector2 pos2 = falling.transform.position;

        falling.transform.position = pos1;
        target.transform.position = pos2;
    }

    public void Moving(GameObject second, GameObject first)
    {
        int temp = first.GetComponent<Tile>().x;
        first.GetComponent<Tile>().x = second.GetComponent<Tile>().x;
        second.GetComponent<Tile>().x = temp;

        temp = first.GetComponent<Tile>().y;
        first.GetComponent<Tile>().y = second.GetComponent<Tile>().y;
        second.GetComponent<Tile>().y = temp;

        tiles[second.GetComponent<Tile>().x, second.GetComponent<Tile>().y] = second.gameObject;
        tiles[first.GetComponent<Tile>().x, first.GetComponent<Tile>().y] = first.gameObject;

        Move(second, first);
    }
}