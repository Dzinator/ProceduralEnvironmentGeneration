using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Airplane : MonoBehaviour {

	Rigidbody rigid;

	Camera frontCamera;
	Camera rightCamera;
	Camera leftCamera;
	Camera mainCamera;

	bool accelerating = false;
	bool decelerating = false;

	public float moveSpeed = 500f;
	float turnSpeed = 75f;

	readonly float maxSpeed= 100000f;
	readonly float minSpeed = 100f;

	// Use this for initialization
	void Start () {
		frontCamera = (Camera) GameObject.Find ("Front Camera").GetComponent<Camera>();
		rightCamera = (Camera) GameObject.Find ("Right Camera").GetComponent<Camera>();
		leftCamera = (Camera) GameObject.Find ("Left Camera").GetComponent<Camera>();
		mainCamera = (Camera) GameObject.Find ("Main Camera").GetComponent<Camera>();

		rigid = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update(){
		/*Speed Control with camera fade in or out*/
		mainCamera.enabled = true;
		frontCamera.enabled = false;
		leftCamera.enabled = false;
		rightCamera.enabled = false;
		
		if (Input.GetKey (KeyCode.Q)) {
			moveSpeed += 5f;
			if(!accelerating && mainCamera.fieldOfView != 80 && moveSpeed < maxSpeed){
				StartCoroutine(cameraFadeOut());
			}
		}
		if (Input.GetKeyUp (KeyCode.Q) || moveSpeed >= maxSpeed)
			StartCoroutine (cameraStabilize ());//mainCamera.fieldOfView = 60f;

		if (Input.GetKey (KeyCode.Z)) {
			moveSpeed -= 5f;
			if(!decelerating && mainCamera.fieldOfView != 40 && moveSpeed > minSpeed){
				StartCoroutine(cameraFadeIn());
			}
		}
		if (Input.GetKeyUp (KeyCode.Z) || moveSpeed <= minSpeed)
			StartCoroutine (cameraStabilize ());// mainCamera.fieldOfView = 60f;

		
		if (moveSpeed < minSpeed)
			moveSpeed = minSpeed;
		else if (moveSpeed > maxSpeed)
			moveSpeed = maxSpeed;


		/*Camera Control*/
		if (Input.GetKey (KeyCode.C) || Input.GetKey (KeyCode.Keypad5)) {
			mainCamera.enabled = false;
			leftCamera.enabled = false;
			rightCamera.enabled = false;
			
			frontCamera.enabled = true;
		}
		if (Input.GetKey (KeyCode.Keypad4)) {
			mainCamera.enabled = false;
			frontCamera.enabled = false;
			rightCamera.enabled = false;
			
			leftCamera.enabled = true;
		}
		if (Input.GetKey (KeyCode.Keypad6)) {
			mainCamera.enabled = false;
			frontCamera.enabled = false;
			leftCamera.enabled = false;
			
			rightCamera.enabled = true;
		}

		//crashing the plane
		if (transform.position.y < 0)
			Time.timeScale = 0;
	}

	void FixedUpdate () {
		//Simulate arcade style airplane controls
		Quaternion newRot = Quaternion.identity;
		float roll = 0f;
		float yaw = 0f;
		float pitch = 0f;

		roll = Input.GetAxis("Roll") * (Time.deltaTime * turnSpeed);
		yaw = Input.GetAxis("Horizontal") * (Time.deltaTime * turnSpeed);
		pitch = Input.GetAxis("Vertical") * (Time.deltaTime * turnSpeed);

		newRot.eulerAngles = new Vector3 (pitch, yaw, roll);
		rigid.rotation *= newRot;

		Vector3 newPos = Vector3.forward;
		newPos = rigid.rotation * newPos;
		rigid.velocity = newPos * (Time.deltaTime * moveSpeed) ;


	
	}
	/*Camera coroutines*/

	IEnumerator cameraStabilize() {
		while(mainCamera.fieldOfView != 60) {
			if(mainCamera.fieldOfView > 60) {
				mainCamera.fieldOfView -= 1;
			}
			else {
				mainCamera.fieldOfView += 1;
			}
			yield return new WaitForFixedUpdate();
		}

	}

	IEnumerator cameraFadeOut(){
		for (float i = 60f; i <= 80f; i += 2f) {
			accelerating = true;
			mainCamera.fieldOfView = i;
			yield return new WaitForFixedUpdate();
			accelerating = false;
		}
	}

	IEnumerator cameraFadeIn(){
		for (float i = 60f; i >= 40f; i -= 2f) {
			decelerating = true;
			mainCamera.fieldOfView = i;
			yield return new WaitForFixedUpdate();
			decelerating = false;
		}
	}
}
