using UnityEngine;
using System.Collections;

public class SplatoonMenu : MonoBehaviour {

	public Texture2D menu;
	public Texture2D sliderYellow;
	public Texture2D sliderRed;
	public Texture2D sliderGreen;
	public Texture2D sliderBlue;

	void OnGUI () {
		GUI.DrawTexture (new Rect (20, 20, menu.width, menu.height), menu);

		Vector4 scores = SplatManagerSystem.instance.scores + new Vector4(0.1f,0.1f,0.1f,0.1f);
		float totalScores = scores.x + scores.y + scores.z + scores.w;
		int yelowScore = (int)( 512 * ( scores.x / totalScores ) );
		int redScore = (int)( 512 * ( scores.y / totalScores ) );
		int greenScore = (int)( 512 * ( scores.z / totalScores ) );
		int blueScore = (int)( 512 * ( scores.w / totalScores ) );

		GUI.DrawTexture (new Rect (20 + menu.width + 20, 20, yelowScore, 30), sliderYellow);
		GUI.DrawTexture (new Rect (20 + menu.width + 20, 60, redScore, 30), sliderRed);
		GUI.DrawTexture (new Rect (20 + menu.width + 20, 100, greenScore, 30), sliderGreen);
		GUI.DrawTexture (new Rect (20 + menu.width + 20, 140, blueScore, 30), sliderBlue);

	}
}
