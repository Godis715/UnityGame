using UnityEngine;
using System.Collections;

public class UIScript : MonoBehaviour {

	public Transform camera;
	public Transform canvas;
	public Transform point;

	void Start () {
		
	}

	void Update () {
		Vector3 N = new Vector3(0, 0, 1);
		N = camera.localRotation * N;
		point.position = camera.position + N;
		float D = -Vector3.Dot(N, canvas.position);
	}
}
