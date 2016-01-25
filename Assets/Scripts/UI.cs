using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour {

	public Airplane plane;
	Text text;
	
	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		text.text = "Current Speed: " + ((plane.moveSpeed/1000)*333).ToString("0.0") + " km/h\n" +
			"Current Altitude: " + (plane.transform.position.y).ToString("0.0") + " m\n" +
				"Current Heading: " + getHeading().ToString("0") + "";
		
	}

	float getHeading() {
		//compute airplane heading out of the rigid body velocity

		float rad = Mathf.Atan (plane.rigidbody.velocity.z / plane.rigidbody.velocity.x);
		float deg = 180 / Mathf.PI * rad;
		deg = deg < 0 ? deg * -1 : deg;

		if (plane.rigidbody.velocity.z < 0 && plane.rigidbody.velocity.x < 0) {
			return (90 - deg) + 180;
		} else if (plane.rigidbody.velocity.z < 0 && plane.rigidbody.velocity.x >= 0) {
			return 90 + deg;

		} else if (plane.rigidbody.velocity.z >= 0 && plane.rigidbody.velocity.x < 0) {
			return 270 + deg;
		} else {
			return 90 - deg;
		}

	}
}
