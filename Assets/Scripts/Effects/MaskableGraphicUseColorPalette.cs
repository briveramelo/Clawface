using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MaskableGraphicUseColorPalette : UseColorPaletteBase {

    [SerializeField] MaskableGraphic maskableGraphic;
    MaskableGraphic MaskableGraphic {
        get {
            if (maskableGraphic==null) {
                maskableGraphic = GetComponent<MaskableGraphic>();
            }
            return maskableGraphic;
        }
    }


    public override void UpdateColor() {
        if (MaskableGraphic != null) {
            MaskableGraphic.color = TargetColor;
        }
    }

}
