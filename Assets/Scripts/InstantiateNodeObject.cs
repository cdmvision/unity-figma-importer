using System;
using Cdm.Figma.UI;
using UnityEngine;

public class InstantiateNodeObject : MonoBehaviour
{
    public Canvas canvas;
    public NodeObject doc;

    private void Awake()
    {
        Instantiate(doc, canvas.transform);
    }
}