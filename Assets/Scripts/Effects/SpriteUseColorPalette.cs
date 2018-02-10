using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUseColorPalette : UseColorPaletteBase
{
    public override void UpdateColor()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = TargetColor;
    }
}
