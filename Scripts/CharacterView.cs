using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour {

	Vector3 oldPosition;
	Vector3 newPosition;
	//Vector3 currentVelocity;
	float smoothTime = 1f;

	void Start(){
		oldPosition = newPosition = this.transform.position;
	}

	public void OnCharacterMoved(Hex oldHex, Hex newHex) {
		
		//animate moving from one hex to the other
		oldPosition = oldHex.PositionFromCamera();
		newPosition = newHex.PositionFromCamera();

		//executes once when we know character has moved. Could go somewhere else but dont
		//wanna break it
		//TODO: change colour to darker material once it has been explored
		Map map = oldHex.hexMap;
		map.deleteOldSelectables();
		map.highlightSelectableTiles(newHex);
	}

	void Update(){
		this.transform.position = Vector3.Lerp(this.transform.position, newPosition, smoothTime);
	}
	
}

