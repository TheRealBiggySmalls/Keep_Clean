using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBeeName : MonoBehaviour {

	private GameObject popUp;
	public GameObject prefab;
	private GameObject bee;

	// Use this for initialization
	void Start () {
		bee = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseOver(){
		Debug.Log("RUNNING");
		popUp = (GameObject) Instantiate(prefab,bee.transform.position, Quaternion.identity,bee.transform);
		popUp.GetComponentInChildren<Text>().text = bee.name;
	}

	void OnMouseExit(){
		Destroy(popUp);
	}
}
