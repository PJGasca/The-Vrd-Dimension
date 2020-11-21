using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectColor : MonoBehaviour
{
    [SerializeField]
    private Color manualColor;
    public Color randomColor;

    // Start is called before the first frame update
    void Start()
    {
        if (manualColor.a != 0)
        {
            randomColor = manualColor;
        }
        else if (randomColor.a == 0) // stop it from re-randomizing the color in Play
        {
            float r = Random.Range(0, 255);
            float g = Random.Range(0, 255);
            float b = Random.Range(0, 255);
            randomColor = new Color(r, g, b, 1);
        }
    }
}
