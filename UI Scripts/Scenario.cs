using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;	

public class Scenario : MonoBehaviour {

	private float time = 2.0f, startAlpha, endAlpha, currentAlpha;
	public bool fading=false;

	void Start(){
		//gameObject.GetComponentInChildren<CanvasRenderer>().SetAlpha(1);
		//startAlpha = gameObject.GetComponentInChildren<CanvasRenderer>().GetAlpha();
	}
	void Update(){
		if(gameObject.GetComponentInChildren<CanvasRenderer>().GetAlpha()<0.02f){
			Destroy(gameObject);
		}
		/*if(fading){
			currentAlpha = Mathf.Lerp(startAlpha, 0, time);
			gameObject.GetComponent<CanvasRenderer>().SetAlpha(currentAlpha);
		}*/
	}

	public void OptionOne(){
		if(Character.storyTriggered){
			ScenarioManager.ContinueStory(1);
		}else{
			Close();
			ScenarioManager.DisplayResults(1);
		}
	}

	public void OptionTwo(){
		if(Character.storyTriggered){
			ScenarioManager.ContinueStory(2);
		}else{
			Close();
			ScenarioManager.DisplayResults(2);
		}
	}

	public void Close(){
		Destroy(gameObject);
	}

	
	public void setFade(bool value){
		for (int childIndex = 0; childIndex < gameObject.transform.childCount; childIndex++){
            Transform child = gameObject.transform.GetChild(childIndex);           
            fadeOut(child.gameObject);
        }
	}

	public void fadeOut(GameObject obj){
		obj.GetComponentInChildren<Image>().canvasRenderer.SetAlpha(1.0f);
		//obj.GetComponentInChildren<Renderer>().CrossFadeAlpha(0.0f,2.0f,false);
	}

	//list option outcomes in here
}
