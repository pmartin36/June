using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Settings {
	public bool Music { get; set; }
	public bool SFX { get; set; }
	public bool Fullscreen { get; set; }

	public int HighestLevel { get; set; }
	public int HighScore { get; set ; }

	public Settings(bool music, bool sFX, bool fullscreen, int highestLevel, int highScore) {
		Music = music;
		SFX = sFX;
		Fullscreen = fullscreen;
		HighestLevel = highestLevel;
		HighScore = highScore;
	}

	public static Settings Load() {
		Settings s = Serializer<Settings>.Deserialize("Settings.bin");
		if( s == null ) {
			s = new Settings(true, true, true, 1, 0);
		}
		s.Fullscreen = Screen.fullScreen;
		return s;
	}

	public void Save() {
		Serializer<Settings>.Serialize(this, "Settings.bin");
	}
}

