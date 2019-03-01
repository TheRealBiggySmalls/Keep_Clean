using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class TurnOrganiser : MonoBehaviour {

	public GameObject turnCount;
	public bool firstEncounter;

	public int turnNumber; public static TurnOrganiser turnOrg; public bool endGame = false;
	private int starvationCount=0;

	void Awake(){
		if(turnOrg==null){
			turnOrg=this;
		}else if(turnOrg!=this){
			Destroy(gameObject);
		}
		//on turn org awake load all data
	}

	void Start(){
		turnNumber=1;
		firstEncounter=true;
	}

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/turnData.nut");
		TurnData data = new TurnData();

		//<<<-------------SAVING DATA--------------->>>
		data.firstEncounter=firstEncounter;
		data.turnNumber=turnNumber;
		loadedTurnNum=turnNumber;
		//<<<-------------END OF SAVING DATA--------------->>>

		//need a different file for each data
		bf.Serialize(file,data);
		file.Close();
	}
	private int loadedTurnNum, loadedFromTurnNumber;
	public void Load(){
		if(File.Exists(Application.persistentDataPath + "/turnData.nut")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/turnData.nut",FileMode.Open);
			TurnData data = (TurnData) bf.Deserialize(file);

			//<<<-------------LOADING DATA--------------->>>
			loadedFromTurnNumber = turnNumber;
			turnNumber = data.turnNumber;
			firstEncounter = data.firstEncounter;
			turnCount.GetComponentInChildren<Text>().text = "Day: " + turnNumber;
			//<<<-------------END OF LOADING DATA--------------->>>

			file.Close();

		}
	}
	
	// Update is called once per frame
	public void NextTurn(){

		if(endGame){
			npcController.npc.endShopKeeper();
			//for now nothing else happens
		}else{
			if(starvationCount==2){
				ScenarioManager.GameOver();//create game over
				return;
			}else if(Map.map.player.Food==0||Map.map.player.Water==0){
				starvationCount+=1;
			}else{
				starvationCount=0;
			}

			//it's in here as a safety net
			ApiaryOrganiser.apiaryOrg.reInitBeeDict();
			UniquesBackpack.backpack.createDict();
		
			//have each of these things store their turn which is then called in here
			Map.map.doTurn(); //at the moment map.doTurn() does nothing. Probably update fog of war or something in future
			int honeyUpdate = ApiaryOrganiser.apiaryOrg.doTurn();
			ApiaryOrganiser.apiaryOrg.reInitBeeDict(); //need before as may apply to character with scenarios

			Map.map.player.UpdateResources(0,0,honeyUpdate);//put this into where the apiary is opened to make game smoother
			Map.map.player.doTurn(turnNumber);

			//every third day the man comes. FOR NOW we treat it as a UI screen that pops up
			//happens after other updates so npc comes in front of events
			if(turnNumber%7==4){
				if(firstEncounter){
					npcController.npc.initNpc(firstEncounter);
					firstEncounter=false;
				}else{
					npcController.npc.openNpcScreen(false);
				}
			}

			//CAN USE PLACE BOY TO SET THEM ON THE SAME TILE
			ApiaryOrganiser.apiaryOrg.reInitBeeDict();

			turnNumber += 1;
			turnCount.GetComponentInChildren<Text>().text = "Day: " + turnNumber;}
			//what else do i need to do?
	}

	public void SaveAllData(){
		Save();
		ApiaryOrganiser.apiaryOrg.Save();
		UniquesBackpack.backpack.Save();
		Map.map.player.Save();
		ScenarioManager.instance.Save();
		HappinessMeter.instance.Save();
		Journal.j.Save();
	}

	public void LoadAllData(){
		Load();
		ApiaryOrganiser.apiaryOrg.Load(loadedFromTurnNumber, loadedTurnNum);
		UniquesBackpack.backpack.Load();
		Map.map.player.Load();
		ScenarioManager.instance.Load();
		HappinessMeter.instance.Load();
		Journal.j.Load();
	}

	public void startFresh(){
		try{
			File.Delete(Application.persistentDataPath + "/turnData.nut");
			File.Delete(Application.persistentDataPath + "/apiaryData.nut");
			File.Delete(Application.persistentDataPath + "/backpackData.nut");
			File.Delete(Application.persistentDataPath + "/playerData.nut");
			File.Delete(Application.persistentDataPath + "/scenarioData.nut");
			File.Delete(Application.persistentDataPath + "/meterData.nut");
			File.Delete(Application.persistentDataPath + "/journalData.nut");
		}catch{
			
		}
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		//SaveAllData(); //resets the previous saved data
	}

	public void fadeIn(GameObject obj){
		obj.GetComponentInChildren<Image>().canvasRenderer.SetAlpha(0.0f);
		obj.GetComponentInChildren<Image>().CrossFadeAlpha(1.0f,2.0f,false);
	}

	public void fadeOut(GameObject obj){
		obj.GetComponentInChildren<Image>().canvasRenderer.SetAlpha(1.0f);
		obj.GetComponentInChildren<Image>().CrossFadeAlpha(0.0f,2.0f,false);
	}
}



[Serializable]
class TurnData{
	public int turnNumber;
	public bool firstEncounter;
}