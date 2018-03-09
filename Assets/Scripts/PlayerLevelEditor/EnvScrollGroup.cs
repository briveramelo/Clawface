using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class EnvScrollGroup : ScrollGroup {


    protected override string ResourcesPath { get { return Strings.Editor.ENV_OBJECTS_PATH; } }
    protected override string IconImagePath { get { return Strings.Editor.PROP_ICON_IMAGE_PREVIEW_PATH; } }
}
