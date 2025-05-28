using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StatBarData
{
    public Slider bar;
    public Image fillImage;
    public Color lowColor;
    public Color fullColor;

    public StatBarData(Slider bar, Image fillImage, Color lowColor, Color fullColor)
    {
        this.bar = bar;
        this.fillImage = fillImage;
        this.lowColor = lowColor;
        this.fullColor = fullColor;
    }
}