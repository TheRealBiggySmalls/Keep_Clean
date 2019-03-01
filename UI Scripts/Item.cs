using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

	public string type;
	public string identifier;
	public  int[] honeyCost;
	public int[] quantity;
	public Texture texture;

	public int tempCost;
	public int tempQuantity;

	//AllItems and textures can be in npcController


	public Item(string iden, Texture textur){
		identifier=iden;
		texture=textur;
	}

	public void assignValues(string typ, int[] honeyCos, int[] quantit){
		type=typ;
		honeyCost=honeyCos;
		quantity=quantit;
	}

	//generates a random cost based on the parameters
	public int generateHoneyCost(){
		tempCost= Random.Range(honeyCost[0],honeyCost[1]);
		return tempCost;
	}

	public int generateQuantity(){
		if(type!="unique"){
			tempQuantity = Random.Range(quantity[0],quantity[1]);
		}else{
			tempQuantity=1;
		}

		return tempQuantity;
	}
}
