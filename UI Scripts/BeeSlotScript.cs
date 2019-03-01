using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeeSlotScript : MonoBehaviour {
	
	public Texture defaultTexture;
	public Texture oldTexture;
	//clicking on the bee slot we only want to deactivate it
	public void removeBee(RawImage image){
		//not sure if we need to get component in children	
		if(image.enabled==true){
			oldTexture=image.texture;
			image.texture=defaultTexture;
			image.enabled=false;
		}
	}

	public void addBee(RawImage image, Texture texture){
		oldTexture = texture;
		image.texture = texture;
		image.enabled=true;
	}
}
