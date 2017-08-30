// LevelEditorStyles.cs
// Author: Aaron

using UnityEngine;

/// <summary>
/// Class to store level editor GUIStyles.
/// </summary>
public class LevelEditorStyles
{
    /// <summary>
    /// Header label style.
    /// </summary>
	public static GUIStyle Header = new GUIStyle ()
    {
		active = new GUIStyleState ()
        {

		},
		alignment = TextAnchor.UpperCenter,
		//border = new RectOffset
//		clipping = TextClipping.Clip,
//		//contentOffset
//		//fixedHeight
//		//fixedWidth
//		//focused
//		//font = 
//		fontSize = 24,
//		fontStyle = FontStyle.Bold,
//		//hover
//		//imagePosition
//		//lineHeight
//		margin = new RectOffset (0, 0, 16, 16),
//		name = "LevelEditorHeader",
//		//normal
//		//onActive
//		//onFocused
//		//onHover
//		//onNormal
//		//overflow = new RectOffset() {
//		//	bottom = 16
//		//},
//		//padding
//		//richText
//		stretchHeight = true,
//		stretchWidth = true,
//		wordWrap = false
	};

    /// <summary>
    /// Normal button style.
    /// </summary>
    public static GUIStyle NormalButton = new GUIStyle ()
    {
        alignment = TextAnchor.MiddleCenter,
    };

    /// <summary>
    /// Selected button style.
    /// </summary>
	public static GUIStyle SelectedButton = new GUIStyle ()
    {
		normal = new GUIStyleState () {
            background = Texture2D.blackTexture,
            textColor = Color.white
        },
        alignment = TextAnchor.MiddleCenter,
	};
}
