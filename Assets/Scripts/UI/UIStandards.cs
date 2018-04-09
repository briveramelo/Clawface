using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class UIStandards : MonoBehaviour {

    [SerializeField] private List<FontAdapter> fontAdapters;
    private void OnEnable() {
        FontAdapters = fontAdapters;
    }

    public void ApplyFontSizes() {
        FontAdapters = fontAdapters;
        (FindObjectsOfTypeAll(typeof(FontSizer)) as FontSizer[]).ToList().ForEach(sizer => {
            sizer.Apply();
        });
    }
    private static List<FontAdapter> FontAdapters;
    public static int GetFontSize(TextStyle style) {
        FontAdapter adapter = FontAdapters.Find(thisAdapter => thisAdapter.textStyle == style);
        if (adapter!=null) {
            return adapter.fontSize;
        }
        return 20;
    }
}
