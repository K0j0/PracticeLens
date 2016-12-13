using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main : MonoBehaviour {

	public Transform camParent;
	public Transform camLead;
//	public GameObject cube1;
	public GameObject cube2;

	public GameObject plane;
	WebCamTexture deviceCam;

	GameObject wall;
	GameObject wParent;
	Camera cam;

	Quaternion baseRotation;

	void Start() {
		cam = Camera.main;

		Input.gyro.enabled = true;
		foreach (var cams in WebCamTexture.devices) {
			print ("[snap] Camera Names: " + cams.name);
		}

		deviceCam = new WebCamTexture ();
		plane.GetComponent<Renderer> ().material.mainTexture = deviceCam;

		deviceCam.Play ();

		makeMesh();

		baseRotation = Quaternion.identity;
//		baseRotation = Input.gyro.attitude;
		print("Gyro Start: (" + (baseRotation.eulerAngles.x * Mathf.Rad2Deg).ToString("F3") + ", " +
								(baseRotation.eulerAngles.y * Mathf.Rad2Deg).ToString("F3") + ", " +
								(baseRotation.eulerAngles.z * Mathf.Rad2Deg).ToString("F3") + ")");

	}

	void makeMesh(){
		float distFromCam = 20;
		float camWallWorldZ = 10;

		Vector3 tr = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, distFromCam));
		Vector3 bl = cam.ScreenToWorldPoint(new Vector3(0, 0, distFromCam));
		Vector3 center = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth/2, cam.pixelHeight/2, distFromCam));
		print ("[snap] w: " + cam.pixelWidth + ", h: " + cam.pixelHeight);
		print ("[snap] TR: " + tr);
		print ("[snap] BL: " + bl);
		print ("[snap] center: " + center);


		wall = new GameObject("Wall");
		wall.transform.position = new Vector3 (0, 0, camWallWorldZ);
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
		wParent.transform.position = new Vector3 (center.x, center.y, camWallWorldZ);
		wParent.transform.localEulerAngles = new Vector3(0, 0, -90);
		wall.transform.localEulerAngles = Vector3.zero;

		wParent.transform.SetParent (camParent);
	}
	
	// Update is called once per frame
	void Update () {
//		print (Input.acceleration.ToString("F3"));
//		print("Gyro Update: (" + (Input.gyro.attitude.eulerAngles.ToString()));

//		cube1.transform.Translate (Input.acceleration.x, 0, -Input.acceleration.z);
//		cube2.transform.localRotation = Quaternion.Euler(Input.acceleration.z * 90, 0, Input.acceleration.x * 90);
//		cube2.transform.localRotation = Quaternion.Euler(0, 0, Input.acceleration.x * 90);

		cube2.transform.rotation = Input.gyro.attitude * baseRotation;
		camParent.rotation = Input.gyro.attitude * baseRotation;

		// Input
		if (Input.touchCount > 0) {
			Vector3 pos = Input.touches [0].position;
			pos = cam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 90));

//			Debug.DrawRay (cam.transform.position, pos, Color.red, 5); // works
			Debug.DrawRay (pos, cam.transform.position - pos, Color.green, 5);
			RaycastHit hit;
//			if (!Physics.Raycast(cam.ScreenPointToRay(pos), out hit)) // works
			if (!Physics.Raycast(pos, cam.transform.position - pos, out hit)) // Inverting ray since physics won't trigger with origin within collider
				return;

//			print ("here 1");
			Renderer rend = hit.transform.GetComponent<Renderer>();
			Collider meshCollider = hit.collider as Collider;
			if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
				return;

			Texture2D tex = rend.material.mainTexture as Texture2D;
			Vector2 pixelUV = hit.textureCoord;
			print ("Here 2. Hit point: " + hit.point + ". Tex coord: " + hit.textureCoord );
			pixelUV.x *= tex.width;
			pixelUV.y *= tex.height;
//			tex.SetPixel(, Color.black);
//			tex.Apply();
			print ("Hit bubble: " + tex.GetPixel((int)pixelUV.x, (int)pixelUV.y));
		}
	}

	public void ui_RotX(int val){
		wParent.transform.Rotate (10 * val, 0, 0);
	}

	public void ui_RotY(int val){
//		wParent.transform.Rotate (0, 10 * val, 0);
		wParent.transform.Translate (0, 0, val);
	}

	public void ui_RotZ(int val){
		wParent.transform.Rotate (0, 0, 10 * val);
	}
}
