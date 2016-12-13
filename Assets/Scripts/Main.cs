using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main : MonoBehaviour {

	public GameObject cube1;
	public GameObject cube2;

	public GameObject plane;
	WebCamTexture deviceCam;

	GameObject wall;
	GameObject wParent;
	Camera cam;

	Quaternion baseRotation;

	void Start() {
		cam = Camera.main;

//		Input.gyro.enabled = true;
		foreach (var cams in WebCamTexture.devices) {
			print ("[snap] Camera Names: " + cams.name);
		}

		deviceCam = new WebCamTexture (WebCamTexture.devices[0].name, 108, 192, 60);
//		deviceCam = new WebCamTexture ();
		plane.GetComponent<Renderer> ().material.mainTexture = deviceCam;

		deviceCam.Play ();

		// TODO: Dynamically create a mesh that will be size of viewport, assign camera texture to that
		makeMesh();
	}

	void makeMesh(){
		Vector3 tr = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, 20));
		Vector3 bl = cam.ScreenToWorldPoint(new Vector3(0, 0, 20));
		print ("[snap] w: " + cam.pixelWidth + ", h: " + cam.pixelHeight);
		print ("[snap] TR: " + tr);
		print ("[snap] BL: " + bl);


		wall = new GameObject("Wall");
		wall.transform.position = new Vector3 (0, 0, 10); // 10
//		wall.transform.localScale = new Vector3 (-1, 1, 1);

		Renderer r =  wall.AddComponent<MeshRenderer> ();
		r.material.mainTexture = deviceCam;
		Mesh m = wall.AddComponent<MeshFilter> ().mesh;
		Vector3[] vertices = new Vector3[4];

//		float w = tr.x;
//		float h = tr.y;
//		vertices[0] = new Vector3(bl.x, bl.y, 0);
//		vertices[1] = new Vector3(tr.x, bl.y, 0);
//		vertices[2] = new Vector3(bl.x, tr.y, 0);
//		vertices[3] = new Vector3(tr.x, tr.y, 0);

		/*
		 * TEST
		 */ 
		float w = tr.x - bl.x;
		float h = tr.y - bl.y;
		// try swapping width and height
		float tmp = w;
		w = h;
		h = tmp;
		print ("[snap] W x H " + w + " x " + h);

		vertices[0] = new Vector3(bl.x, bl.y, 0);
		vertices[1] = new Vector3(bl.x + w, bl.y, 0);
		vertices[2] = new Vector3(bl.x, bl.y + h, 0);
		vertices[3] = new Vector3(bl.x + w, bl.y + h, 0);
		/*
		 * END TEST
		 */ 

		m.vertices = vertices;

		int[] tri = new int[6];

		tri[0] = 0;
		tri[1] = 2;
		tri[2] = 1;

		tri[3] = 2;
		tri[4] = 3;
		tri[5] = 1;

		m.triangles = tri;

		Vector3[] normals = new Vector3[4];

		normals[0] = -Vector3.forward;
		normals[1] = -Vector3.forward;
		normals[2] = -Vector3.forward;
		normals[3] = -Vector3.forward;

		m.normals = normals;

		Vector2[] uv = new Vector2[4];

		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(1, 0);
		uv[2] = new Vector2(0, 1);
		uv[3] = new Vector2(1, 1);

		m.uv = uv;

		// Center the mesh
		print("[snap] Wall Bounds: " + r.bounds.center + " | " + r.bounds.extents);
		wParent = new GameObject("WallParent");
		wParent.transform.position = r.bounds.center;
		wall.transform.SetParent (wParent.transform);
		wParent.transform.position = new Vector3 (0, 0, 10);
		wParent.transform.localEulerAngles = new Vector3(0, 0, -90);
		wall.transform.localEulerAngles = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
//		print (Input.acceleration.ToString("F3"));
//		print("Gyro: " + Input.gyro.attitude.ToString("F3"));

//		cube1.transform.Translate (Input.acceleration.x, 0, -Input.acceleration.z);
//		cube2.transform.localRotation = Quaternion.Euler(Input.acceleration.z * 90, 0, Input.acceleration.x * 90);
//		cube2.transform.localRotation = Quaternion.Euler(0, 0, Input.acceleration.x * 90);

//		cube2.transform.rotation = Input.gyro.attitude;

//		wall.transform.rotation = baseRotation * Quaternion.AngleAxis(deviceCam.videoRotationAngle, Vector3.up);
	}

	public void ui_RotX(int val){
		wParent.transform.Rotate (10 * val, 0, 0);
	}

	public void ui_RotY(int val){
		wParent.transform.Rotate (0, 10 * val, 0);
	}

	public void ui_RotZ(int val){
		wParent.transform.Rotate (0, 0, 10 * val);
	}
}
