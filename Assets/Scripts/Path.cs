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
	Color startColor;
	Sound mySound;
	GameObject vfx;

	public static void init_static(){
		allPaths = new Dictionary<Color, List<Path>> ();
	}

	public void init_local(Vector3 startPos, Color col){
		cam = Camera.main;
		line = GetComponent<LineRenderer> ();
		line.SetPosition (0, startPos);
		lastPos = startPos;
		startColor = col;

		// Start playing sound
		print("Play New Sound");
		string audioId = Main.getAudioIdForColor (col);
		mySound = AudioManager.Main.PlayNewSound(audioId, true);

		if (audioId == "Audio/sound_clip_1") {
			vfx = Instantiate (Resources.Load ("VFX/vfx_1"), Vector3.zero, Quaternion.identity) as GameObject;
		} else if (audioId == "Audio/sound_clip_2") {
			vfx = Instantiate (Resources.Load ("VFX/vfx_2"), Vector3.zero, Quaternion.identity) as GameObject;
		} else if (audioId == "Audio/sound_clip_3") {
			vfx = Instantiate (Resources.Load ("VFX/vfx_1"), Vector3.zero, Quaternion.identity) as GameObject;
		} else if (audioId == "Audio/sound_clip_4") {
			vfx = Instantiate (Resources.Load ("VFX/vfx_2"), Vector3.zero, Quaternion.identity) as GameObject;
		} else if (audioId == "Audio/sound_clip_5") {
			vfx = Instantiate (Resources.Load ("VFX/vfx_5"), Vector3.zero, Quaternion.identity) as GameObject;
		}
		vfx.transform.SetParent (transform);

		initialized = true;

		StartCoroutine (makePath ());
	}

	IEnumerator makePath(){
		bool onSameColor = true;
		while (Input.touchCount == 1 && onSameColor) {			
			// get world pos of finger
			Vector3 pos = Input.touches [0].position;
			pos = cam.ScreenToWorldPoint (new Vector3 (pos.x, pos.y, 7.5f));

			// Stop path on color change of projected point
			if (Main.lastColorPressed != startColor) {
				//print (">>> Color Change <<<");
				onSameColor = false;
				break;
			}

			// Have to be a minimum distance apart to grow path
			if (Vector3.Distance (pos, lastPos) > .2f) {
                line.numPositions = ++pointCount;
                line.SetPosition (pointCount - 1, pos);
				lastPos = pos;
			}
			vfx.transform.position = lastPos;

			yield return null;
		}
		StartCoroutine( burnDown ());
	}

	IEnumerator burnDown(){
		while (pointCount > 0) {
			line.numPositions = --pointCount;

            if(pointCount >= 1)
            {
                lastPos = line.GetPosition(pointCount-1);
			    vfx.transform.position = lastPos;
            }

//			yield return new WaitForEndOfFrame ();
			yield return new WaitForSeconds (.25f);
		}

		mySound.playing = false;
		mySound.Finish ();
		Destroy (gameObject);
	}

	public static void startPath(Vector3 pos, Color col) {
		isActive = true;

		// create path prefab
		GameObject pathGO = Instantiate(Resources.Load("Path"), Vector3.zero, Quaternion.identity) as GameObject;
		Path path = pathGO.GetComponent<Path> ();
		path.init_local (pos, col);
	}

	public static bool isCurrPath(Path p){
		return currPath == p;
	}
}
