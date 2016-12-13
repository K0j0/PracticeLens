using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : MonoBehaviour {
	public static bool isActive;

	static Dictionary<Color, List<Path>> allPaths;

	public static void init(){
		allPaths = new Dictionary<Color, List<Path>> ();
	}

	public static void startPath(Vector3 pos, Color col) {
	}

	public static void addToPath(Vector3 pos, Color col) {
	}
}
