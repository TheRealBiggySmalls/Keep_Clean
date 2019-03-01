using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HappinessMeter : MonoBehaviour {

	public Sprite sad, happy, cumming;
	public int happiness;
	public static GameObject meter; private Image background, fill, emoji; public static HappinessMeter instance;
	// Use this for initialization
	void Start () {
		instance = this;
		meter=gameObject;
		happiness=0;
		init();
	}

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/meterData.nut");
		MeterData data = new MeterData();

		//<<<-------------SAVING DATA--------------->>>
		data.currentAmount = happiness;
		//<<<-------------END OF SAVING DATA--------------->>>

		//need a different file for each data
		bf.Serialize(file,data);
		file.Close();
	}
	public void Load(){
		if(File.Exists(Application.persistentDataPath + "/turnData.nut")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/meterData.nut",FileMode.Open);
			MeterData data = (MeterData) bf.Deserialize(file);

			//<<<-------------LOADING DATA--------------->>>
			happiness=0;
			fillMeter(data.currentAmount);
			//<<<-------------END OF LOADING DATA--------------->>>

			file.Close();

		}
	}

	private void init(){
		Image[] imgs = meter.GetComponentsInChildren<Image>();
		background = imgs[0];	fill=imgs[1];	emoji=imgs[2];
	}	
	
	// Update is called once per frame
	public void fillMeter(int amount){
		if(happiness>=0&&happiness<=100){
			if(happiness+amount<0){
				happiness=0;
			}else if(happiness+amount>100){
				happiness=100;
			}else{
				happiness+=amount;
			}
			updateHandlePos(amount);
		}
		fill.fillAmount = happiness/100f;
	}
	int cont = 20;
	private void updateHandlePos(int amount){
		if(happiness>80){ 
			emoji.sprite=cumming;
		}else if(happiness>40){
			emoji.sprite=happy;
		}else if(happiness>-1){
			emoji.sprite=sad;
		}
		Vector3 old = emoji.transform.position;
		old.x += (amount + cont*happiness/(100));
		emoji.transform.position = old;
	}
}

[System.Serializable]
class MeterData{
	public int currentAmount;
}
