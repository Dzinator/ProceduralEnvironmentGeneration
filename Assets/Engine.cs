using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour {

	public Airplane plane;
	public GUIText text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		text.text = "Current speed: " + plane.moveSpeed + "km/h";
	
	}
}
