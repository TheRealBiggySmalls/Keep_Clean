using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeResourceText : MonoBehaviour {

	//updates the UI resources
	static GameObject foodResource, waterResource, honeyResource;
	void Awake(){
		foodResource = GameObject.FindGameObjectWithTag("Food");
		waterResource = GameObject.FindGameObjectWithTag("Water");
		honeyResource = GameObject.FindGameObjectWithTag("Honey"); 
	}
	public static void UpdateUIResources(int food, int water, int honey){

		if(foodResource==null||waterResource==null||honeyResource==null){
			foodResource = GameObject.FindGameObjectWithTag("Food");
			waterResource = GameObject.FindGameObjectWithTag("Water");
			honeyResource = GameObject.FindGameObjectWithTag("Honey"); 
		}
		foodResource.GetComponentInChildren<Text>().text = food.ToString();
		waterResource.GetComponentInChildren<Text>().text = water.ToString();
		honeyResource.GetComponentInChildren<Text>().text = honey.ToString();
	}

	public static void updateEndGameUI(){
		foodResource.GetComponentInChildren<Text>().text = "?";
		waterResource.GetComponentInChildren<Text>().text = "?";
		honeyResource.GetComponentInChildren<Text>().text = "?";
	}

	//CURRENTLY WORKS: - initially "updates" resource numbers at end of game start in map.
	//				   - additionally, updates resource numbers each time a new hex is set for the player.
	//				   - this ONLY applies to food and water as honey is not yet implemented
}
