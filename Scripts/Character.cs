using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QPath;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Character {

	public int Food=500, Water=500, Honey=900; //REAL VALUES ARE: 250, 250, 0 maybe?
	public string Name = "Character";


	public delegate void CharacterMovedDelegate (Hex oldHex, Hex newHex);
	public CharacterMovedDelegate OnCharacterMoved; private UniquesBackpack backpack;

	public int storyProgress=0;
	/*
	//HAVE A VARIABLE THAT INCREASES YEILD CHANCE FROM TILES???
	public int hitPoints = 100;
	public int strength = 8;
	*/
	public Hex currentHex{get; protected set;}
	public Hex nextHex;

	public static bool storyTriggered = false;
	
	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerData.nut");
		playerData data = new playerData();

		//<<<-------------SAVING DATA--------------->>>
		data.q = currentHex.Q;
		data.r = currentHex.R;
		data.food = Food; data.water = Water; data.honey = Honey;
		data.storyProgress = storyProgress;

		//<<<-------------END OF SAVING DATA--------------->>>

		//need a different file for each data
		bf.Serialize(file,data);
		file.Close();
	}
	
	public void Load(){
		if(File.Exists(Application.persistentDataPath + "/playerData.nut")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerData.nut",FileMode.Open);
			playerData data = (playerData) bf.Deserialize(file);

			//<<<-------------LOADING DATA--------------->>>
			Food = data.food; Water = data.water; Honey = data.honey;
			storyProgress = data.storyProgress;
			for(int i=0;i<=storyProgress;i++){
				if(i==0){
					highlightStory(i);
				}else{
					highlightStory(i,i-1);
				}
			}

			ChangeResourceText.UpdateUIResources(Food,Water,Honey);

			Hex a = Map.map.GetHexAt(data.q,data.r);//store q and r of hex so it can be retreived
			SetHex(a);
			//<<<-------------END OF LOADING DATA--------------->>>

			file.Close();

		}
	}

	//store the move for the character and then do the things in here
	public void doTurn(int turnNum){
		UpdateDayResources();

		if(nextHex==currentHex||nextHex==null){
			ChangeResourceText.UpdateUIResources(Food,Water,Honey);
			return;
		}
		if(backpack==null){
			backpack = UniquesBackpack.backpack;
			storyProgress=0;
		}
		//set all the variables and stuff when the click happens, then call all the actual moving and updating of stuff here!
		//TODO: make it more clear when the player has selected a tile
		//have to pass as negative as we are taking these away as movement cost
		UpdateResources(-nextHex.foodCost,-nextHex.waterCost);

		//updates and resets event costs
		UpdateResources(ScenarioManager.foodResult,ScenarioManager.waterResult,ScenarioManager.honeyResult);
		ScenarioManager.honeyResult=0;ScenarioManager.waterResult=0;ScenarioManager.foodResult=0;

		float rand = Random.Range(0f,10f);
		
		highlightStory(storyProgress);

		if((turnNum%7!=4)){
			
			//HERE: hard code a check for if the nextHex is a story location
			if(currentHex.hexMap.storyLocations.Contains(nextHex)){ //roundabout way of doing it but we get what we want
				if(currentHex.hexMap.storyLocations.IndexOf(nextHex)==storyProgress){ //checks if the next is the one we have progressed up to
					//THIS WORKS: AS IN - the recognition triggers
					//ONCOMPLETION: add 1 to story progress from within scenario manager and unset storyTrigger
					if(storyProgress==0){
						ScenarioManager.ChooseEvent(nextHex, 1000); //TRIGGERS AGAIN
						storyTriggered=true;
					}else if(storyProgress==1){
						ScenarioManager.ChooseEvent(nextHex, 2000);
						storyTriggered=true;
					}else if(storyProgress==2){
						ScenarioManager.ChooseEvent(nextHex, 3000);
						storyTriggered=true;
					}else if(storyProgress==3){
						ScenarioManager.ChooseEvent(nextHex, 4000);
						storyTriggered=true;
					}else if(storyProgress==4){
						ScenarioManager.ChooseEvent(nextHex, 5000);
						storyTriggered=true;
					}//else game should be over 

				}else{
					int currentLoc = currentHex.hexMap.storyLocations.IndexOf(nextHex);
					if(currentLoc==0){ //can only be after
						ScenarioManager.ChooseEvent(nextHex,1002,true); //___1 is before, ___2 is after
					}else if(currentLoc==1){
						if(currentLoc>storyProgress){
							ScenarioManager.ChooseEvent(nextHex,2001,true); //have no completed the event before this one
						}else if(currentLoc<storyProgress){
							ScenarioManager.ChooseEvent(nextHex,2002,true); //have completed this story event
						}
					}else if(currentLoc==2){
						if(currentLoc>storyProgress){
							ScenarioManager.ChooseEvent(nextHex,3001,true);
						}else if(currentLoc<storyProgress){
							ScenarioManager.ChooseEvent(nextHex,3002,true);
						}
					}else if(currentLoc==3){
						if(currentLoc>storyProgress){
							ScenarioManager.ChooseEvent(nextHex,4001,true);
						}else if(currentLoc<storyProgress){
							ScenarioManager.ChooseEvent(nextHex,4002,true);
						}
					}else if(currentLoc==4){
						if(currentLoc>storyProgress){
							ScenarioManager.ChooseEvent(nextHex,5001,true);
						}else if(currentLoc<storyProgress){
							ScenarioManager.ChooseEvent(nextHex,5002,true);
						}
					}
					//TODO: add events here so it doesnt trigger agin if already visited
				}
			}else if(rand<4.1f){
				//happens a turn late but at least it happens
				ScenarioManager.ChooseEvent(nextHex);
			}
		}

		AudioManager.audioManager.playSound("footsteps");
		ChangeResourceText.UpdateUIResources(Food,Water,Honey);
		//TODO: play walking animation
		SetHex(nextHex);
		unHighlightTile(nextHex.hexMap.hexToGameObjectMap[nextHex]);

	}

	public void highlightStory(int highlight, int unhighlight=999){
		if(unhighlight!=999){
			unhighlightCurrentObjective(unhighlight);
		}
		highlightCurrentObjective(highlight);
	}

	private Color storyHighlightColour=Color.green, storyPreColor=Color.red;
	public void highlightCurrentObjective(int progress){
		if(progress>4){
			return;
		}
		Hex toHighlight = currentHex.hexMap.storyLocations[progress];
		GameObject obj = currentHex.hexMap.hexToGameObjectMap[toHighlight];
		obj.GetComponentInChildren<MeshRenderer>().material.color = storyPreColor;
	}

	public void unhighlightCurrentObjective(int progress){
		Hex toHighlight = currentHex.hexMap.storyLocations[progress];
		GameObject obj = currentHex.hexMap.hexToGameObjectMap[toHighlight];
		obj.GetComponentInChildren<MeshRenderer>().material.color = storyHighlightColour;
	}

	public void UpdateDayResources(){
		UpdateResources(-5,-5);
	}

	public void SetHex(Hex newHex){
		//check for water tiles etc here for movement
		//can add whatever code you want in here for movement restriction
		Hex oldHex = currentHex;
		currentHex = newHex;

		//NOW: factor in resource costs. Should put somewhere AFTER move so that moving to a 
		//tile can happen if you wont have enough resources to move after move

		if(OnCharacterMoved != null){
			OnCharacterMoved(oldHex, newHex);
		}
	}

	private Color currentSelectionColour;
	private GameObject oldTile;
	public void moveToHex(Hex destinationTile){
		//this is pretty messy, ideally character would not have information of these things
		Map map = destinationTile.hexMap;
		GameObject tile = map.hexToGameObjectMap[destinationTile];

		//in each of these create a new alert if tile violates one of their rules
		if(!checkIllegalTiles(destinationTile)){
			//AlertSection.NewAlert("Illegal Tile!","default",tile);
			return;
		}

		if(!checkTileInNeighbours(destinationTile, map)){
			//AlertSection.NewAlert("Tile not in neighbours!","default",map.hexToGameObjectMap[destinationTile]);
			return;
		}

		if(!checkResourcesSufficient(destinationTile)){
			//AlertSection.NewAlert("Resources insufficient for travel!","default",map.hexToGameObjectMap[destinationTile]);
			//These alerta are broken so we dont want them to trigger... maybe instead throw a sound or smn?
			return;
		}

		if(oldTile==null){
			updateTileSelections(tile);
		}else{
			updateTileSelections(oldTile, tile);
		}

		nextHex = destinationTile;
	}

	public void updateTileSelections(GameObject oldie, GameObject newTile){

		//unset old tile to its original colour
		//WE ARE CHANGING THE COLOUR OF THE ORIGINAL MATERIAL. NEED TO CHANGE MATERIAL OR SOMETHING
		unHighlightTile(oldie);

		//set new tile to the selection colour
		updateTileSelections(newTile);
	}

	public void updateTileSelections(GameObject tile){
		MeshRenderer mf = tile.GetComponentInChildren<MeshRenderer>();
		currentSelectionColour = mf.material.color;
		mf.material.color = Color.magenta;
		oldTile=tile;
	}

	public void unHighlightTile(GameObject tile){
		MeshRenderer mfOld = tile.GetComponentInChildren<MeshRenderer>();
		mfOld.material.color = currentSelectionColour;
		oldTile=null;
	}

	//checks tile is not illegal in any way
	public bool checkIllegalTiles(Hex hex){
		if(backpack!=null){
			if(hex.Elevation<=0f&&backpack.itemTruth["boat"]){//water movement should now be allowed
				return true;
			}else if(hex.Elevation<=0f&&!backpack.itemTruth["boat"]){
				return false;
			}
		}
		if(hex.Elevation<=0f){//water movement should now be allowed
			return false;
		}
		return true;
	}
	public bool checkTileInNeighbours(Hex hex, Map map){
		//need to write a new checkNeighbours function
		Hex[] hexes = map.getNeighbours(currentHex);
		foreach(Hex a in hexes){
			//checks if positions match
			if(a.Position()==hex.Position()){
				return true;
			}
		}
		return false;
	}

	public bool checkResourcesSufficient(Hex hex){
		//TODO: write scripts to assign varying costs of food and water based on hex mesh and top mesh
		if(hex.foodCost<=Food&&hex.waterCost<=Water){
			return true;
		}
		return false;
	}

	public void UpdateResources(int food, int water, int honey=0){
		if(Food+food<0){
			Food=0;
		}else{
			Food+=food;
		}

		if(Water+water<0){
			Water = 0;
		}else{
			Water +=water;
		}

		if(Honey+honey<0){
			Honey=0;
		}else{
			Honey +=honey;
		}
		ChangeResourceText.UpdateUIResources(Food,Water,Honey);//auto updates whenever there is a change
	}

}


[System.Serializable]
class playerData{
	public int food,water,honey;
	public int q,r;

	public int storyProgress;
}
