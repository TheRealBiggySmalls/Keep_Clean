using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertSection : MonoBehaviour {

	//store an enum of alerts in here with unique IDS
	//store a dict for alertTextIds to actual strings
	
	private static AlertSection instance;
	public GameObject alertFab;
	//points to game object with vertical layout group
	public GameObject alertList;

	// Use this for initialization
	void Start () {
		instance = this;
		if(alertList==null){
			alertList = this.gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//default parameters
	public static void NewAlert(string alertTextId, string iconId="", GameObject focusGameObject=null){
		
		//string alertText = "test"; //change this to be dictionary of enums or whatever
		GameObject alertGo = (GameObject)Instantiate(instance.alertFab);
		alertGo.GetComponentInChildren<Text>().text = alertTextId;
		//TODO: set icon
		//TODO: cache reference to game object to focus on
		alertGo.transform.SetParent(instance.alertList.transform);
		alertGo.transform.localScale=Vector3.one;
	}

	public static void NewAlert(string alertText, GameObject focusGameObject=null){
		NewAlert(alertText, "", focusGameObject);
	}
}
