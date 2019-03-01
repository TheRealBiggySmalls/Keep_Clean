using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Update_CurrentFunction = Update_DetectModeStart;
	}
	//Generic variables
	Vector3 lastMousePosition; //from Input.mousePosition
	//Camera drag variables

	Vector3 lastMouseGroundPlanePosition, hitPos;

	delegate void UpdateFunc();

	private Character characterSelected;
	private Color previousColour;
	UpdateFunc Update_CurrentFunction;
	
	//CLICK is reserved for moving to a tile/exploring the map/interacting with things
	//moving mouse around is for exploring the world???
	// Update is called once per frame

	void Update(){
		//TODO: make it so camera cannot be dragged if a UI component is open

		if(Input.GetKeyDown(KeyCode.Escape)){
			CancelUpdateFunction();
		}
		Update_CurrentFunction();

		Update_ScrollZoom(); //want this to run every frame regardless

		lastMousePosition = Input.mousePosition;
	}

	void CancelUpdateFunction(){
		Update_CurrentFunction = Update_DetectModeStart;
		//Also do any cleanup and UI associated stuff
	}

	//detects current mouse situation. Only runs if already not in a mode
	void Update_DetectModeStart(){
		if(Input.GetMouseButtonDown(0)){
			//left mouse button went down. Doesn't do anything by itself we need more context
			//if mouse is down and mouse starts moving then we start a drag
		
		}else if(Input.GetMouseButtonUp(0)){
			//TODO: Are we clicking on a hex or a unit

		}else if(Input.GetMouseButton(0) && Input.mousePosition != lastMousePosition){
			//TODO: consider adding in some pixel jitter threshold
			//left mouse is being held down and camera is being dragged

			Update_CurrentFunction = Update_CameraDrag;
			lastMouseGroundPlanePosition = CheckHitPos(Input.mousePosition);
			Update_CurrentFunction();

		}else if(Input.GetMouseButtonUp(1)){//characterSelected!=null && ){
			//at the moment just want to move character if we are a right click
			Debug.Log("Updating character movement");
			//TODO: fix charactermovement updating multiple times
			Update_CharacterMovement();
		}
	}

	void Update_CharacterMovement(){

		//if this is called call updateCameraFollow or something so that the camera
		//follows the player as he moves

		//Raycast works fine. NotInNeighbours triggers it is called twice for whatever reason
		//so the moved to tile counts
		GameObject hitObject = GetHexHit();
		if(hitObject==null){
			Debug.Log("Raycast did not hit a game object: UpdateCharacterMovement");
			return;
		}

		if(hitObject.GetComponentInChildren<HexComponent>()!=null){
			Map map = hitObject.GetComponentInChildren<HexComponent>().hexMap;
			if(characterSelected==null){
				characterSelected = map.player;
			}
			
			//initialise character
			characterSelected.moveToHex(hitObject.GetComponentInChildren<HexComponent>().hex);

			//Create a new random event for that tile!
			//Currently randomises so events only occur about one third of the time

		}
		//TODO: Change mesh of neighbours to be highlighted for selection
		//THESE SHOULD BE HIGHLIGHTED AT ALL TIMES
	
		//maybe shouldnt set this here
		CancelUpdateFunction();
		return;
	}

	const float minX=17.0f, minZ=0.0f, maxX=35.0f,maxZ=10.0f;
	void Update_CameraDrag () {

		if(EventSystem.current.IsPointerOverGameObject()){ //over UI so we want to cancel
			CancelUpdateFunction();
			return;
		}
		
		if(Input.GetMouseButtonUp(0)){
			CancelUpdateFunction();
			return;
		}

		Vector3 hitPos = CheckHitPos(Input.mousePosition);

		Vector3 diff = hitPos-lastMouseGroundPlanePosition;
		Camera.main.transform.Translate(diff, Space.World);

		Vector3 p = Camera.main.transform.position;
		if(p.y<4.0f||p.y>4.0f){
			p.y=4.0f;
		} 
		//Locks drag within a certain area
		if(p.x<minX){
			p.x=minX;
		}else if(p.x>maxX){
			p.x=maxX;
		}
		if(p.z<minZ){
			p.z=minZ;
		}else if(p.z>maxZ){
			p.z=maxZ;
		}
		//This fixes the "drag-through-the-ground" error and locks world map to a specific area so you dont get to see the "void"
		Camera.main.transform.position = p;

		lastMouseGroundPlanePosition=hitPos=CheckHitPos(Input.mousePosition);
	}

	void Update_ScrollZoom(){
		return;
		//return; //DONE FOR NOW AS ZOOMING ISNT PARTICULARLY REQUIRED
		//FOR ZOOMING (Zoom to scrollwheel)
		float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
		float minHeight = 2;
		float maxHeight = 7;
		Vector3 hitPos = CheckHitPos(Input.mousePosition);
		
		if(Mathf.Abs(scrollAmount)>0.01f){

			//Move camera towards hitPos
			Vector3 dir = Camera.main.transform.position-hitPos;
			Vector3 p = Camera.main.transform.position;
			
			if(scrollAmount>0 || p.y<maxHeight-0.01f){
				Camera.main.transform.Translate(dir * scrollAmount, Space.World);
			}
			
			p = Camera.main.transform.position;
			if(p.y<minHeight){
				p.y=minHeight;
			}
			if(p.y>maxHeight){
				p.y=maxHeight;
			}
			//This SHOULD be fixing the "drag-through-the-ground" error
			Camera.main.transform.position = p;

			//Change camera angle when you get to the extremes
			float lowZoom = minHeight+3;
			float highZoom = maxHeight-3;

			//TODO: Fix bug where xooming in and pulling land makes you go through it
			//TODO: fix initial angle and y of camera so it doesn't look so weird
			Camera.main.transform.rotation=Quaternion.Euler(
					Mathf.Lerp(35, 90, (p.y/(maxHeight/1.5f))),
					Camera.main.transform.rotation.eulerAngles.y,
					Camera.main.transform.rotation.eulerAngles.z
				);
		}
	}

	Vector3 CheckHitPos(Vector3 mousePos){
		Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
		if(mouseRay.direction.y>=0){
			return Vector3.zero;
		}
		
		float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
		return mouseRay.origin + (mouseRay.direction * rayLength);
	}

	GameObject GetHexHit(){
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if( Physics.Raycast(mouseRay, out hitInfo) ) {
			GameObject ourHitObject = hitInfo.collider.transform.parent.gameObject;
			Debug.Log("Raycast hit: " + ourHitObject);
			return ourHitObject;
		}
		return null;
	}
}