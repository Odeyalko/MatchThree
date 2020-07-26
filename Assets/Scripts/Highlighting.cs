using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighting : MonoBehaviour
{
    private SpriteRenderer rend;
    private Color color;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        color = rend.material.color;
    }

    public void Hightlight()
    {
        rend.material.color = new Color(0.5f, 0.5f, 0.5f, 0) + color;
    }

    public void UnHighlight()
    {
        rend.material.color = color;
    }

}
