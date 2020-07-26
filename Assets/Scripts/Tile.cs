using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool isSelected;
    private bool isActive;

    public int x;
    public int y;

    public bool IsActive { get => isActive;}

    private void Start()
    {
        isSelected = false;
        isActive = true;
    }

    private void OnMouseDown()
    {
        isSelected = true;
        Handler.instance.SelectedTiles = this;
    }
       
    public void Deactivate()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        isActive = false;
    }
}