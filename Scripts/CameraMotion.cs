using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {

	Vector3 oldPosition;
	public GameObject target;
	// Use this for initialization
	void Start () {
		oldPosition = this.transform.position;
		target = GameObject.FindGameObjectWithTag("Player");
		//LateUpdate();
	}
	
	// Update is called once per frame
	void Update () {
		// TODO: Code to click-and-drag camera
		//		WASD
		//		Zoom in and out
		//LateUpdate();
		CheckIfCameraMoved();
	}

	/*void LateUpdate(){
		transform.LookAt(target.transform);
	}*/

	public void PanToHex(Hex hex){
		//TODO: move camera to hex
	}

	HexComponent[] hexes;
	void CheckIfCameraMoved(){
		if(oldPosition!= this.transform.position){
			//SOMETHING moved the camera
			oldPosition = this.transform.position;

			if(hexes == null){
				hexes = GameObject.FindObjectsOfType<HexComponent>();
			}

			//TODO: probably hexmap will have a dictionary of all these later
			foreach(HexComponent hex in hexes){
				hex.UpdatePosition();
			}
		}
	}
}
