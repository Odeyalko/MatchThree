using System.Collections;
using UnityEngine;

public class Handler : MonoBehaviour
{
    public static Handler instance;

    private Tile firstSelectedTile;
    private Tile secondSelectedTile;

    private float Speed = 1;
    private float movingAnimation = 0;

    private bool isShifting = false;
    private bool noMatch = false;

    private void Start()
    {
        instance = this;
    }

    public Tile SelectedTiles
    {
        set
        {
            if (!isShifting && !Board.instance.IsShifting())
            {
                if (firstSelectedTile == null)
                {
                    firstSelectedTile = value;
                    Highlight(value);
                }
                else if
                (
                    (firstSelectedTile.x + 1 == value.x
                    ||
                    firstSelectedTile.x - 1 == value.x)
                    &&
                    firstSelectedTile.y == value.y
                    ||
                    (firstSelectedTile.y + 1 == value.y
                    ||
                    firstSelectedTile.y - 1 == value.y)
                    &&
                    firstSelectedTile.x == value.x
                )
                {
                    secondSelectedTile = value;
                    Highlight(value);
                    noMatch = false;
                    Swap();
                }
                else
                {
                    UnHighlightAll();
                    Highlight(value);
                    firstSelectedTile = value;
                }
            }
        }
    }

    public void Highlight(Tile tile)
    {
        tile.GetComponent<Highlighting>().Hightlight();
    }

    private void UnHighlightAll()
    {
        firstSelectedTile.GetComponent<Highlighting>().UnHighlight();
        if (secondSelectedTile != null)
        {
            secondSelectedTile.GetComponent<Highlighting>().UnHighlight();
        }
    }

    private void Swap()
    {
        StartCoroutine(Move(firstSelectedTile.transform.position, secondSelectedTile.transform.position));
    }

    private IEnumerator Move(Vector2 pos, Vector2 target)
    {
        isShifting = true;
        movingAnimation = 0;

        int temp = firstSelectedTile.x;
        firstSelectedTile.x = secondSelectedTile.x;
        secondSelectedTile.x = temp;

        temp = firstSelectedTile.y;
        firstSelectedTile.y = secondSelectedTile.y;
        secondSelectedTile.y = temp;

        Board.instance.tiles[firstSelectedTile.x, firstSelectedTile.y] = firstSelectedTile.gameObject;
        Board.instance.tiles[secondSelectedTile.x, secondSelectedTile.y] = secondSelectedTile.gameObject;

        while (movingAnimation < 1)
        {
            firstSelectedTile.transform.position = Vector2.Lerp(firstSelectedTile.transform.position, target, movingAnimation);
            secondSelectedTile.transform.position = Vector2.Lerp(secondSelectedTile.transform.position, pos, movingAnimation);
            movingAnimation += Time.deltaTime;
            yield return null;
        }

        if (noMatch == false)
        {
            if (Board.instance.IsMatched() == false)
            {
                noMatch = true;
                Swap();
            }
        }

        if (movingAnimation >= 1)
        {
            UnHighlightAll();
            firstSelectedTile = null;
            secondSelectedTile = null;
            isShifting = false;
        }
    }
}