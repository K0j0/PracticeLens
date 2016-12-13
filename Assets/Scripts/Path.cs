using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : MonoBehaviour {
	public static bool isActive;
	static Dictionary<Color, List<Path>> allPaths;
	static Path currPath;

	LineRenderer line;
	bool initialized = false;
	Vector3 lastPos;
	Camera cam;
	int pointCount = 0;

	public static void init_static(){
		allPaths = new Dictionary<Color, List<Path>> ();
	}

	public void init_local(Vector3 startPos){
		cam = Camera.main;
		line = GetComponent<LineRenderer> ();
		line.SetPosition (0, startPos);
		lastPos = startPos;

		initialized = true;

		StartCoroutine (makePath ());
	}

	IEnumerator makePath(){
		while (Input.touchCount == 1) {
			// get world pos of finger
			Vector3 pos = Input.touches [0].position;
			pos = cam.ScreenToWorldPoint (new Vector3 (pos.x, pos.y, 5));

			// Have to be a minimum distance apart to grow path
			if (Vector3.Distance (pos, lastPos) > .2f) {
				line.SetVertexCount (++pointCount);
				line.SetPosition (pointCount - 1, pos);
			}

			yield return null;
		}
		
	}

	public static void startPath(Vector3 pos, Color col) {
		// create path prefab
		GameObject pathGO = Instantiate(Resources.Load("Path"), Vector3.zero, Quaternion.identity) as GameObject;
		Path path = pathGO.GetComponent<Path> ();
		path.init_local (pos);
	}

	public static void addToPath(Vector3 pos, Color col) {
	}

	public static bool isCurrPath(Path p){
		return currPath == p;
	}
}
