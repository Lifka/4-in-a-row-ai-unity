using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSize : MonoBehaviour 
{
	Camera cam;
		
	void Awake () 
	{
		cam = GetComponent<Camera>();
		cam.orthographic = true;
	}
		
	void LateUpdate()
	{
		float maxY = Config.numRows + 2;

		cam.orthographicSize = maxY / 2f;
	}
}
