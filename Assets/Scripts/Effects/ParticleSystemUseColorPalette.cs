using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemUseColorPalette : UseColorPaletteBase {

    [SerializeField] ParticleSystem ps;
    [SerializeField] PropertyType property;

    public override void UpdateColor()
    {
        switch (property)
        {
            case PropertyType.StartColor:
                ParticleSystem.MainModule main = ps.main;
                main.startColor = TargetColor;
                break;

            default:
                break;
        }
    }

    enum PropertyType
    {
        StartColor
    }
}
