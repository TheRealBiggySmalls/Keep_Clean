using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ScenarioManager : MonoBehaviour {

	public static int foodResult, waterResult, honeyResult, beeQuantity;
	public Texture defaultTexture;
	private Character boi;
	public GameObject preFab; public GameObject deathPrefab; public GameObject endGame; public GameObject load; public GameObject tutorial;
	public GameObject closePrefab; public GameObject beeClosePrefab; public GameObject happinessClosePrefab; private bool happiness;
	private static string HeaderText, EventText, OptionOne, OptionTwo, CloseText;

	public GameObject apiary; private UniquesBackpack backpack; private GameObject currentEvent;

	public static ScenarioManager instance;
	private List<int> eventsThatHaveOccured;
	public int ScenarioId;

	//store a delegate function that stores the current outcome
	//OR can just have an if statement that stores the fun bois in there with a million elifs

	private static Vector3 defaultPos = Vector3.zero;

	void Awake(){
		if(instance==null){
			instance=this;
		}else if(instance!=this){
			Destroy(gameObject);
		}
	}

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/scenarioData.nut");
		ScenarioData data = new ScenarioData();

		//<<<-------------SAVING DATA--------------->>>
		data.occuredEvents = eventsThatHaveOccured;
		//<<<-------------END OF SAVING DATA--------------->>>

		//need a different file for each data
		bf.Serialize(file,data);
		file.Close();
	}
	
	public void Load(){
		if(File.Exists(Application.persistentDataPath + "/scenarioData.nut")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/scenarioData.nut",FileMode.Open);
			ScenarioData data = (ScenarioData) bf.Deserialize(file);

			//<<<-------------LOADING DATA--------------->>>
			eventsThatHaveOccured = data.occuredEvents;
			//<<<-------------END OF LOADING DATA--------------->>>

			file.Close();

		}
	}

	void Start(){	//so we only need to for loop once to find each items truth instead of having to iterate every time
		HeaderText = "DEFAULT HEADER";
		EventText = "DEFAULT EVENT";
		OptionOne = "FIRST OPTION";
		OptionTwo = "SECOND OPTION";
		ScenarioId = 999;
		backpack = UniquesBackpack.backpack;
		eventsThatHaveOccured = new List<int>();
		boi = Map.map.player;
		StartLoadScreen();
	}

	public void StartLoadScreen(){
		GameObject load  = (GameObject) Instantiate(instance.load);
		Button buttons = load.GetComponentInChildren<Button>();
		
		if(File.Exists(Application.persistentDataPath + "/turnData.nut")){
			buttons.onClick.AddListener(delegate{TurnOrganiser.turnOrg.LoadAllData();});
			buttons.GetComponentInChildren<Text>().text = "Load";
		}else{
			buttons.onClick.AddListener(delegate{StartTutorial();}); //starts tutorial if this is "Start" game and not "Load"
		}

		buttons.onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});
		//buttons.onClick.AddListener(delegate{load.GetComponentInChildren<Scenario>().setFade(true);});
		buttons.onClick.AddListener(delegate{Destroy(load);});
		load.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		load.transform.localScale=Vector3.one;
		//Setting position to be correct
		defaultPos.y = Screen.height/2;
		defaultPos.x = Screen.width/2;
		load.transform.position = defaultPos;
	}

	public void StartTutorial(){
		GameObject load  = (GameObject) Instantiate(instance.tutorial);
		Button buttons = load.GetComponentInChildren<Button>();

		buttons.onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});
		buttons.onClick.AddListener(delegate{Destroy(load);});

		load.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		load.transform.localScale=Vector3.one;
		//Setting position to be correct
		defaultPos.y = Screen.height/2;
		defaultPos.x = Screen.width/2;
		load.transform.position = defaultPos;
	}

	public static void ChooseEvent(Hex hex, int id = 0, bool end=false){

		int rand;
		if(id==0){
			rand = (int) Random.Range(1,12);
		}else{
			rand = id;
		}
		instance.yuckyIf(rand, hex);
		if(!end){
			instance.CreateEvent();
		}
	}
	public static void GameOver(){
		GameObject gameOver  = (GameObject) Instantiate(instance.deathPrefab);

		AudioManager.audioManager.playSound("death");

		Button[] buttons = gameOver.GetComponentsInChildren<Button>();
		if(buttons[0].name=="OptionOne"){
			buttons[0].onClick.AddListener(delegate{TurnOrganiser.turnOrg.LoadAllData();});
			buttons[1].onClick.AddListener(delegate{TurnOrganiser.turnOrg.startFresh();});
		}else{
			buttons[1].onClick.AddListener(delegate{TurnOrganiser.turnOrg.LoadAllData();});
			buttons[0].onClick.AddListener(delegate{TurnOrganiser.turnOrg.startFresh();});
		}
		buttons[1].onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});
		buttons[0].onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});
		buttons[1].onClick.AddListener(delegate{Destroy(gameOver);});
		buttons[0].onClick.AddListener(delegate{Destroy(gameOver);});

		TurnOrganiser.turnOrg.fadeIn(gameOver);

		gameOver.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		gameOver.transform.localScale=Vector3.one;
		//Setting position to be correct
		defaultPos.y = Screen.height/2;
		defaultPos.x = Screen.width/2;
		gameOver.transform.position = defaultPos;
	}

	public void CreateEvent(){
		GameObject tileEvent  = (GameObject) Instantiate(instance.preFab);
		currentEvent = tileEvent;
		Text[] texts = tileEvent.GetComponentsInChildren<Text>();

		//reinit dict every time
		backpack.createDict();
		
		//sets header and event text
		if(texts[0].name=="HeaderText"){
			texts[0].text = HeaderText;
			texts[1].text = EventText;
		}else{
			texts[0].text = EventText;
			texts[1].text = HeaderText;
		}

		Button[] buttons = tileEvent.GetComponentsInChildren<Button>();
		if(buttons[0].name=="OptionOne"){
			buttons[0].GetComponentInChildren<Text>().text = OptionOne;
			buttons[1].GetComponentInChildren<Text>().text = OptionTwo;
		}else{
			buttons[0].GetComponentInChildren<Text>().text = OptionTwo;
			buttons[1].GetComponentInChildren<Text>().text = OptionOne;
		}
		buttons[0].onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});
		buttons[1].onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});
		tileEvent.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		tileEvent.transform.localScale=Vector3.one;

		//Setting position to be correct
		defaultPos.y = Screen.height/2;
		defaultPos.x = Screen.width/2;
		tileEvent.transform.position = defaultPos;
	}

	public void SetStoryTexts(){//sets text again for events
		Text[] texts = currentEvent.GetComponentsInChildren<Text>();
		if(texts[0].name=="HeaderText"){
			texts[0].text = HeaderText;
			texts[1].text = EventText;
		}else{
			texts[0].text = EventText;
			texts[1].text = HeaderText;
		}

		Button[] buttons = currentEvent.GetComponentsInChildren<Button>();
		if(buttons[0].name=="OptionOne"){
			buttons[0].GetComponentInChildren<Text>().text = OptionOne;
			buttons[1].GetComponentInChildren<Text>().text = OptionTwo;
		}else{
			buttons[0].GetComponentInChildren<Text>().text = OptionTwo;
			buttons[1].GetComponentInChildren<Text>().text = OptionOne;
		}
		//buttons[0].onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});//probably not necessary here
		//buttons[1].onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});
	}

	public static void DisplayResults(int option){
		Texture texture = instance.yuckyOutcomeIf(option);
		//HAVE THIS RETURN NULL if no bee OR a STRING NAME IF BEE

		if(instance.happiness){//happiness is set true for events that dont give bee or resource results
			
			//IN HERE SOMEWHERE UPDATE THE HAPPINESS METER

			GameObject outcome = (GameObject) Instantiate(instance.happinessClosePrefab);
			Text[] texts = outcome.GetComponentsInChildren<Text>();

			//sets header and event text
			if(texts[0].name=="HeaderText"){
				texts[0].text = HeaderText;
				texts[1].text = EventText;
			}else{
				texts[0].text = EventText;
				texts[1].text = HeaderText;
			}
			//sets button text
			outcome.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = CloseText;
			outcome.GetComponentInChildren<Button>().onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});


			outcome.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
			outcome.transform.localScale=Vector3.one;

			defaultPos.y = Screen.height/2;
			defaultPos.x = Screen.width/2;
			outcome.transform.position = defaultPos;

			instance.happiness=false;

		}else if(texture==null){
			//creates the regular outcome for an event if no texture is returned
			GameObject outcome = (GameObject) Instantiate(instance.closePrefab);
			Text[] texts = outcome.GetComponentsInChildren<Text>();

			//sets header and event text
			if(texts[0].name=="HeaderText"){
				texts[0].text = HeaderText;
				texts[1].text = EventText;
			}else{
				texts[0].text = EventText;
				texts[1].text = HeaderText;
			}

			//sets resources gained text
			texts[2].text = waterResult.ToString();
			texts[3].text = foodResult.ToString();
			texts[4].text = honeyResult.ToString();
		
			//sets button text
			outcome.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = CloseText;
			outcome.GetComponentInChildren<Button>().onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});


			outcome.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
			outcome.transform.localScale=Vector3.one;

			defaultPos.y = Screen.height/2;
			defaultPos.x = Screen.width/2;
			outcome.transform.position = defaultPos;

			Map.map.player.UpdateResources(waterResult,foodResult,honeyResult);
		}else{
			//creates the regular outcome for an event if no texture is returned
			GameObject outcome = (GameObject) Instantiate(instance.beeClosePrefab);
			Text[] texts = outcome.GetComponentsInChildren<Text>();

			//sets header and event text
			if(texts[0].name=="HeaderText"){
				texts[0].text = HeaderText;
				texts[1].text = EventText;
			}else{
				texts[0].text = EventText;
				texts[1].text = HeaderText;
			}

			//TODO: hide bee pedestal in UI OR set it to a question mark and hide text

			//sets bee texture and quantity
			RawImage bee = outcome.GetComponentsInChildren<RawImage>()[1];
			if(texture.name=="questionMark"){
				texts[2].enabled=false; //disable text
			}else{
				texts[2].text = beeQuantity.ToString();
			}
			
			bee.texture=texture;

			//sets button text
			outcome.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = CloseText;
			outcome.GetComponentInChildren<Button>().onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});

			outcome.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
			outcome.transform.localScale=Vector3.one;

			defaultPos.y = Screen.height/2;
			defaultPos.x = Screen.width/2;
			outcome.transform.position = defaultPos;

			//IT ALL WORKS FUCK YEAH
			instance.apiary.GetComponentInChildren<ApiaryOrganiser>().addToBeeQuantity(texture, beeQuantity);
		}
		instance.currentEvent=null;
	}

	public void EndStoryEvent(Texture texture, bool add=true){ //specifically for story

		Destroy(currentEvent); //closes old event
		if(texture==null){
			texture=defaultTexture;
		}

		//creates the regular outcome for an event if no texture is returned
		GameObject outcome = (GameObject) Instantiate(instance.beeClosePrefab);
		Text[] texts = outcome.GetComponentsInChildren<Text>();

		//sets header and event text
		if(texts[0].name=="HeaderText"){
			texts[0].text = HeaderText;
			texts[1].text = EventText;
		}else{
			texts[0].text = EventText;
			texts[1].text = HeaderText;
		}

		RawImage bee = outcome.GetComponentsInChildren<RawImage>()[1];
		if(texture.name=="questionMark"){
			texts[2].enabled=false; //disable text
		}else if(add){
			
			texts[2].text = beeQuantity.ToString();
			instance.apiary.GetComponentInChildren<ApiaryOrganiser>().addToBeeQuantity(texture, beeQuantity);
		}else if(!add){
			texts[2].enabled=false;
		}

		bee.texture=texture;
		
		//sets button text
		outcome.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = CloseText;
		outcome.GetComponentInChildren<Button>().onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});

		outcome.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		outcome.transform.localScale=Vector3.one;

		defaultPos.y = Screen.height/2;
		defaultPos.x = Screen.width/2;
		outcome.transform.position = defaultPos;

		Character.storyTriggered=false; //reset this

	}


	//HERE WE HAVE A CHANGETEXT FUNCTION WHICH DOESNT DELETE OR RECREATE ANYTHING BUT IT DOES CHANGE TEXT
	//WE HAVE OPTION CHOSEN EACH TIME: WE HAVE SCENARIO ID: CREATE CHAIN OF EVENTS FOR EACH STORY TRIGGER
	int storyChain;
	public static void ContinueStory(int option){
		if(instance.ScenarioId==1000){
			instance.storyEventOne(option);
		}else if(instance.ScenarioId==2000){
			instance.storyEventTwo(option);
		}else if(instance.ScenarioId==3000){
			instance.storyEventThree(option);
		}else if(instance.ScenarioId==4000){
			instance.storyEventFour(option);
		}else if(instance.ScenarioId==5000){
			instance.storyEventFive(option);
		}
	}	


	private float mountain=0.8f, snow=0.61f,forest=0.2f,grassland=0.16f;
	public bool identifyTile(Hex hex, string tile){
		float ele = hex.Elevation;
		if(ele>=mountain){
			if(tile=="mountain"){
				return true;
			}
			return false;
		}else if(ele>=snow&&ele<mountain){
			if(tile=="snow"){
				return true;
			}
			return false;
		}else if(ele>=forest&&ele<snow){
			if(tile=="forest"){
				return true;
			}
			return false;
		}else if(ele>=grassland&&ele<forest){
			if(tile=="grassland"){
				return true;
			}
			return false;
		}else if(ele<grassland&&ele>0){
			if(tile=="desert"){
				return true;
			}
			return false;
		}
		return true;
	}


	//Here create a bunch of scenarios. Or maybe store them in a different file called Events

	//STORE ALL SCENARIOS IN HERE AS FUNCTIONS
	//FUNCTIONS SET IDS ETC
	//FUNCTION FOR SETTING TEXT AND SEPERATE ONE FOR OUTCOMES
	//Having certain high level bees unlocks new events (and makes old events not occur any more)

	public void ScenarioOne(){
		ScenarioId=1;
		HeaderText = "What's that sound?";
		EventText = "You hear a strange buzzing in the distance...";
		OptionOne = "Investigate!";
		OptionTwo = "It's probably nothing.";
	}
	public Texture ScenarioOneOutcome(int outcome){
		if(outcome==1){
			honeyResult = Random.Range(10,20);
			//HERE: add in some randomness
			EventText = "You stumble across a wild hive filled with bland bees and are able to harvest some of their honey!";
			CloseText = "Huzzah!";
			/*
			EventText = "You stumble across a wild hive filled with irregular bees but soon realize they are hyper aggressive! You are chased away and drop some of your supplies in the escape!";
			CloseText = "Oh no!"; 
			*/
		}else if(outcome==2){
			EventText = "You think it's best to leave whatever it is alone. Who knows what's in these woods!";
			CloseText = "Carry on.";
		}
		return null;
	}


	public void ScenarioTwo(){
		ScenarioId=2;
		HeaderText = "Abandoned cottage ahead!";
		EventText = "Coming over a small hill you are confronted by a small, abandoned cottage that lies in your path. It looks to be in relatively good condition for how old it must be.";
		OptionOne = "Go inside!";
		OptionTwo = "Not worth exploring.";
	}
	public Texture ScenarioTwoOutcome(int outcome){
		if(outcome==1){
			waterResult = Random.Range(8,20);
			foodResult = Random.Range(10,25);
			EventText = "You enter the cottage and find a small number of well-preserved supplies!";
			CloseText = "Hooray!";
			/*
			EventText = "You enter the cottage but find the insides in ruins. Whatever wealth was once here is long gone.";
			CloseText = "Move on."; 
			*/
		}else if(outcome==2){
			EventText = "Whatever goods it once held will be long gone, no use checking inside.";
			CloseText = "Okay.";
		}
		return null;
	}


	//make most of these about "you see a strange new hive in the woods/swamp/mountains etc" and the options give different ways to approach the bees
	//each with different success rates and depending on the bee types they are more difficult to pass or require different tactics etc.
	public void ScenarioThree(){
		ScenarioId=3;
		HeaderText = "Bees!";
		EventText = "You stumble across a bright red hive, glowing with anger and activity. You could surely use some of these on your farm. How do you approach retreiving them?";
		OptionOne = "Sneak up to them...";
		if(backpack.itemTruth["sword"]){
			OptionTwo = "Draw your shiny sword!";
		}else{
			OptionTwo = "Charge the nest!";
		}
	}
	public Texture ScenarioThreeOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==1){
			EventText = "Sneaking was the wrong move. The bees spotted you and attacked before you could get close enough to their hive. You were forced to run away!";
			CloseText = "To hell with them!";
			//dont set texture here it will still be null
		}else if(outcome==2&&backpack.itemTruth["sword"]){

			beeQuantity = Random.Range(1,5);
			EventText = "You made the right move - your shiny sword stunned the bees and froze them with fear! You successfully nabbed some before the hive could react!";
			CloseText = "Hooray!";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(5,7));
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="warrior"){
					texture=text.GetComponentInChildren<RawImage>().texture;
					break;
				}
			}
			
		}else if(outcome==2){
			beeQuantity = Random.Range(1,5);
			EventText = "The bees were caught off guard for a second but soon reacted and chased you away. Something tells you this approach could work...";
			CloseText = "Hmm...";
		}
		return texture;
	}

	public void ScenarioFour(){
		ScenarioId=4;
		HeaderText = "Bees!";
		EventText = "You approach a cold area and see some bees busy at work. However, as you approach they quickly retreat and hide inside their nest.";
		OptionOne = "Try to take some";
		if(backpack.itemTruth["robot"]){
			OptionTwo = "Boot up Compliment-o-bot";
		}else{
			OptionTwo = "Shower them with compliments";
		}
	}

	public Texture ScenarioFourOutcome(int outcome){
		Texture texture=defaultTexture;//SET IT TO BE SOME DEFAULT TEXTURE HERE
		if(outcome==1){
			EventText = "You try to get some out of the hive but it is covered in ice and unyeilding! You walk away unsuccessful.";
			CloseText = "Next time!";
			//dont set texture here it will still be null
		}else if(outcome==2&&backpack.itemTruth["robot"]){

			beeQuantity = Random.Range(1,5);
			EventText = "As your trusty companion starts praising the hive and how fashionable its' inhabitants look, you notice a few bees creeping out to listen to you. Before long a few settle into your backpack and decide to follow you back to the farm!";
			CloseText = "Nice moves buddy!";
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="icy"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
		//not today
		}else if(outcome==2){
			EventText = "You stand there shouting nice words at the hive for some time. Every now and then you think you spot some activity but no bees come out of the hive.";
			CloseText = "Not this time.";
		}
		return texture;
	}


	public void ScenarioFive(){ //ROSE IS FOR FOREST BEES
		ScenarioId=5;
		HeaderText = "Bees!";
		EventText = "The forest ascends on all sides of you. It must go high enough to touch the sky! A sudden wave of tranquility overcomes you and you realise you are frozen in your path. A small swarm of deep green bees wait in front of you, expectantly...";
		OptionTwo = "What do you want?";
		if(backpack.itemTruth["rose"]){
			OptionOne = "Offer them a rose";
		}else{
			OptionOne = "Show them your bee collection";
		}
	}
	public Texture ScenarioFiveOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==1&&backpack.itemTruth["rose"]){
			EventText = "The bees float away and you try to follow them with your head but cant. Your muscles eventually loosen and you find yourself able to move again. Turning to follow your assialants you see them slowly floating away with your rose, and a few that have stayed behind with you.";
			CloseText = "New friends!";
			beeQuantity = Random.Range(1,5);
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(5,7));
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="forest"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
		}else if(outcome==1){
			EventText = "The bees are not impressed and leave.";
			CloseText = "Come back!";
		}else if(outcome==2){
			EventText = "You black out and find yourself on the outskirts of the forest.";
			CloseText = "What happened?";
		}
		return texture;
	}


	public void ScenarioSix(){ //PICKAXE IS FOR STONE BEES
		ScenarioId=6;
		HeaderText = "Bees!";
		EventText = "You enter a small chasm with a large rock cliff face to your left, long drained of all its precious ores. After a few steps you soon notice a flat tone emminating from the wall itself.";
		OptionTwo = "Touch the stone";
		if(backpack.itemTruth["pickaxe"]){
			OptionOne = "Chip at it with the pickaxe";
		}else{
			OptionOne = "Punch the rock face";
		}
	}
	public Texture ScenarioSixOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==1&&backpack.itemTruth["pickaxe"]){
			EventText = "Lightly knocking into the rock loosens a large plate of bassalt which falls to the ground. Behind it a large swarm is revealed, barely distinguishable from the rock itself, and moving at a snails pace. They do not object when you remove a few.";
			CloseText = "These are very heavy!";
			beeQuantity = Random.Range(1,5);
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(5,7));
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="stone"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
		}else if(outcome==1){
			EventText = "What did you expect?";
			CloseText = "Ouch";
		}else if(outcome==2){
			EventText = "As your palm touches the stone you feel a wave of relief overcome you. Clear vibrations are emminating from it.";
			CloseText = "I'll be back, rock.";
		}
		return texture;
	}


	public void ScenarioSeven(){ //BOAT IS FOR OCEAN BEES
		ScenarioId=7;
		HeaderText = "Bees!";
		EventText = "Wandering around the island you spot strange shapes oscillating around the waves";
		OptionTwo = "Swim out";
		if(backpack.itemTruth["boat"]){
			OptionOne = "Investigate with your dingy";
		}else{
			OptionOne = "Try to make out the shapes";
		}
	}
	public Texture ScenarioSevenOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==1&&backpack.itemTruth["boat"]){
			EventText = "Your rickety old boat calmly moors the tides as you row out. Looking around you barely can make out a bustling hive - filled with dark blue bees, hard to see amongst the depth of the ocean. You calmly collect a few and they dont seem to object as you take them back to shore with you.";
			CloseText = "Yeehaw!";
			beeQuantity = Random.Range(1,5);
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(5,7));
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="ocean"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
			//dont set texture here it will still be null
		}else if(outcome==1){
			EventText = "Squinting you notice the figures dip in and out of the tides. You're fairly sure it's not just your imagination...";
			CloseText = "Hmm...";
		//not today
		}else if(outcome==2){
			EventText = "Stripping down too your togs and diving into the ocean you manage to brave the first few sets of waves but before long the gentle tide has carried you straight back to shore.";
			CloseText = "Blast!";
		}
		return texture;
	}

	public void ScenarioSeventy(){ //BOAT IS FOR OCEAN BEES
		ScenarioId=70;
		HeaderText = "Bees!";
		EventText = "While rowing around you have spotted some shapes oscillating around the water";
		OptionOne = "Row over";
		OptionTwo = "Ignore Them";
	}
	public Texture ScenarioSeventyOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==1){
			EventText = "Your rickety old boat calmly moors the waves as you row over. Looking around you barely can make out a bustling hive - filled with dark blue bees, hard to see amongst the depth of the ocean. You calmly collect a few and they dont seem to object as you take them back to shore with you.";
			CloseText = "Yeehaw!";
			beeQuantity = Random.Range(1,5);
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(6,9));
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="ocean"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
			//dont set texture here it will still be null
		}else if(outcome==2){
			EventText = "The Ocean can do strange things to a man... You decide to move on.";
			CloseText = "The Ocean Stirs...";
		}
		return texture;
	}

	public void ScenarioEight(){ //SHOES IS FOR OCEAN BEES
		ScenarioId=8;
		HeaderText = "Bees!";
		EventText = "From a nearby beach you can see unusual bees busy at work";
		OptionOne = "Call out the them";
		if(backpack.itemTruth["shoe"]){
			OptionTwo = "Adorn your sneak proof shoes";
		}else{
			OptionTwo = "Walk up to them";
		}
	}
	public Texture ScenarioEightOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==2&&backpack.itemTruth["shoe"]){
			EventText = "Your special shoes cancel out all sounds from the squeaky sand and you are able to reach the bees without startling them. they happily come with you when gathered.";
			CloseText = "Yeet!";
			beeQuantity = Random.Range(1,5);
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(5,7));
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="shore"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
			//dont set texture here it will still be null
		}else if(outcome==2){
			EventText = "You start to approach them before realising this is no regular sand... its SQUEAKY sand! The bees are startled by the sound and disperse.";
			CloseText = "Hmm...";
		//not today
		}else if(outcome==1){
			EventText = "On hearing you they disperse, and even after waiting for some time they show no signs of returning.";
			CloseText = "Hmm...";
		}
		return texture;
	}

	public void ScenarioEighty(){ //SHOES IS FOR OCEAN BEES
		ScenarioId=80;
		HeaderText = "Bees!";
		EventText = "From a nearby beach you can see unusual bees busy at work. You dock on the beach as the waves silently roll you in.";
		OptionOne = "Call out the them";
		if(backpack.itemTruth["shoe"]){
			OptionTwo = "Adorn your sneak proof shoes";
		}else{
			OptionTwo = "Walk up to them";
		}
	}
	public Texture ScenarioEightyOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==2&&backpack.itemTruth["shoe"]){
			EventText = "Your special shoes cancel out all sounds from the squeaky sand and you are able to reach the bees without startling them. they happily come with you when gathered.";
			CloseText = "Hell yeah!";
			beeQuantity = Random.Range(1,5);
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(5,7));
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="shore"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
			//dont set texture here it will still be null
		}else if(outcome==2){
			EventText = "You start to approach them before realising this is no regular sand... its SQUEAKY sand! The bees are startled by the sound and disperse.";
			CloseText = "Hmm...";
		//not today
		}else if(outcome==1){
			EventText = "On hearing you they disperse, and even after waiting for some time they show no signs of returning.";
			CloseText = "Hmm...";
		}
		return texture;
	}



	//BOOK IS FOR EXTENDED SCENARIOS
	public void ScenarioNine(){
		ScenarioId=9;
		HeaderText = "Studious Work";
		EventText = "You open your book and decide to consult a new passage, what do you choose to study?";
		OptionTwo = "The Lurker";
		OptionOne = "The Sky Holder";
	}
	public Texture ScenarioNineOutcome(int outcome){

		if(outcome==2){
			EventText = "Forgotten tales tell of a sunken God who lays in a magical kingdom beneath the tides in slumber. Apparently he will wake one day and reclaim his treasured island, wherever that may be.";
			CloseText = "The world grows a little darker...";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(-8);
		//not today
		}else if(outcome==1){
			EventText = "Once there stood a mighty giant who held the sky from the Earth. Long ago he perished and the sky fell, collapsing on the Earth and warping it into a sphere, the shape it has been ever since. His bones can still be found as roots in the deepest mountains.";
			CloseText = "Was that always there?...";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(-11);
		}
		happiness=true;
		return null;
	}

	public void ScenarioTen(){
		ScenarioId=10;
		HeaderText = "Studious Work";
		EventText = "Walking under some palms you find some nice shade and open up your trusted book, what do you choose to study?";
		OptionTwo = "Forest Dwellers";
		OptionOne = "Warrior Breeds";
	}
	public Texture ScenarioTenOutcome(int outcome){

		if(outcome==2){
			EventText = "There is a curious species of bee living deep within the most lush forests. These intelligent creatures spread life wherever they go and are known for their base understanding of the forces that connect all things. While rare, encounters with them have been brief and emphasized their love of beauty.";
			CloseText = "Interesting.";
		}else if(outcome==1){
			EventText = "This bee is not one to be trifled with. Most dangerous from its genetic roots being anchored in industrious breeds. Many a bee keeper has accidentally produced this species before and paid the price. Can be managed in small quantites but beware over populating your hives with them.";
			CloseText = "Interesting.";
		}
		happiness=true;
		return null;
	}


	public void ScenarioEleven(){
		ScenarioId=11;
		HeaderText = "Studious Work";
		EventText = "A gentle creek lays in front of you and you decide it is the perfect time for some reading. What do you choose to study?";
		OptionTwo = "Warrior Breeds";
		OptionOne = "Oceanic Bees";
	}
	public Texture ScenarioElevenOutcome(int outcome){

		if(outcome==2){
			EventText = "This bee is not one to be trifled with. Most dangerous from its genetic roots being anchored in industrious breeds. Many a bee keeper has accidentally produced this species before and paid the price. Can be managed in small quantites but beware over populating your hives with them.";
			CloseText = "Interesting.";
		}else if(outcome==1){
			EventText = "Amphibious bees with a preference for aquatic dwelling have been found on many occasions by unwary sailors. Just how they produce their honey is unknown, but they seem to have no trouble adapting from their coral hives to those of regular land bees. Strange mutations have been observed when such bees are confined to land for too long of a time.";
			CloseText = "Interesting.";
		}
		happiness=true;
		return null;
	}

	public void ScenarioTwelve(){
		ScenarioId=12;
		HeaderText = "Studious Work";
		EventText = "A dreamy canope of trees shields your current path from the morning sun. What better time to read? What do you choose to study?";
		OptionTwo = "Oceanic Bees";
		OptionOne = "Stone and Earth";
	}
	public Texture ScenarioTwelveOutcome(int outcome){

		if(outcome==2){
			EventText = "Amphibious bees with a preference for aquatic dwelling have been found on many occasions by unwary sailors. Just how they produce their honey is unknown, but they seem to have no trouble adapting from their coral hives to those of regular land bees. Strange mutations have been observed when such bees are confined to land for too long of a time.";
			CloseText = "Interesting";
		}else if(outcome==1){
			EventText = "Certain bees have been found residing inside of rocks and ore veins. Generally little is known of these bees save for their increibly slow speed and dense nature. Many a miner has been saved by this fact.";
			CloseText = "Interesting.";
		}
		happiness=true;
		return null;
	}


	public void ScenarioThirteen(){
		ScenarioId=13;
		HeaderText = "Studious Work";
		EventText = "The chilly morning breeze is too much to handle as you duck inside of a cave and whip out your trusty compendium. What do you choose to study?";
		OptionTwo = "Stone and Earth";
		OptionOne = "Beachs and Lakes";
	}
	public Texture ScenarioThirteenOutcome(int outcome){

		if(outcome==2){
			EventText = "Certain bees have been found residing inside of rocks and ore veins. Generally little is known of these bees save for their increibly slow speed and dense nature. Many a miner has been saved by this fact.";
			CloseText = "Interesting.";
		}else if(outcome==1){
			EventText = "A specific breed of bee has found its way to the lakes and beachs of the world. Common enough but easily startled, this bee is reknowned for the sheer solemnness of its hum. It is enough to bring any seasoned bee keeper to tears.";
			CloseText = "Interesting.";
		}
		happiness=true;
		return null;
	}

	public void ScenarioFourteen(){
		ScenarioId=14;
		HeaderText = "Studious Work";
		EventText = "As dawn cracks you decide there is no better time for learning than now. What do you choose to study?";
		OptionTwo = "Beaches and Lakes";
		OptionOne = "Frozen Tundras";
	}
	public Texture ScenarioFourteenOutcome(int outcome){

		if(outcome==2){
			EventText = "A specific breed of bee has found its way to the lakes and beachs of the world. Common enough but easily startled, this bee is reknowned for the sheer solemnness of its hum. It is enough to bring any seasoned bee keeper to tears.";
			CloseText = "Interesting.";
		}else if(outcome==1){
			EventText = "Rare and scattered encounters speak of remarkable bees which have the ability to survive in the coldest parts of the world. The arctic winds may have bleached the colour from their skin, but their temperament is legendary. If treated gently, these bees are bonified factory for honey.";
			CloseText = "Interesting.";
		}
		happiness=true;
		return null;
	}

	public void ScenarioFifteen(){
		ScenarioId=15;
		HeaderText = "Studious Work";
		EventText = "Your fire crackles in the silent night and you pull out your book before bed. What do you choose to study?";
		OptionTwo = "Frozen Tundras";
		OptionOne = "Forest Dwellers";
	}
	public Texture ScenarioFifteenOutcome(int outcome){

		if(outcome==2){
			EventText = "Rare and scattered encounters speak of remarkable bees which have the ability to survive in the coldest parts of the world. The arctic winds may have bleached the colour from their skin, but their temperament is legendary. If treated gently, these bees are bonified factory for honey.";
			CloseText = "Interesting.";
		}else if(outcome==1){
			EventText = "There is a curious species of bee living deep within the most lush forests. These intelligent creatures spread life wherever they go and are known for their base understanding of the forces that connect all things. While rare, encounters with them have been brief and emphasized their love of beauty.";
			CloseText = "Interesting.";
		}
		happiness=true;
		return null;
	}

	public void ScenarioSixteen(){
		ScenarioId=16;
		HeaderText = "Studious Work";
		EventText = "You decide to consult your book for information. What do you choose to study?";
		OptionTwo = "Mutations";
		OptionOne = "Protection";
	}
	public Texture ScenarioSixteenOutcome(int outcome){

		if(outcome==2){
			EventText = "Cross breeding is an interesting and crucial part of an bee keepers life. While it has the potential to create new and exciting variations of bees, it also has the potential for undesirable outcomes. Much thought should be put into cross breeding and which bees may have favourable genetic mutations.";
			CloseText = "Interesting.";
			Journal.addStringToRumoursPage("Mutations amongst bee species seem very interesting... I should consider the nature and types of bees I have before cross breeding to ensure the greatest mutation chance!", "book");
		}else if(outcome==1){
			EventText = "Ensuring the proper precations are taken before cross breeding is essential. Too many tales exist of farmers being melted, vaporized or even having their whole farm reduced to ruins by lack of preparation. Never underestimate nature, and the cost of messing with it.";
			CloseText = "Interesting.";
			Journal.addStringToRumoursPage("According to my studies cross-breeding can be quite dangers with more advanced species of bees... I should be careful!", "book");
		}
		happiness=true;
		return null;
	}


	public void ScenarioSeventeen(){
		ScenarioId=17;
		HeaderText = "Studious Work";
		EventText = "A proper bee-keeper values knowledge above all else. What do you choose to study?";
		OptionTwo = "Protection";
		OptionOne = "Intelligence";
	}
	public Texture ScenarioSeventeenOutcome(int outcome){

		if(outcome==2){
			EventText = "Ensuring the proper precations are taken before cross breeding is essential. Too many tales exist of farmers being melted, vaporized or even having their whole farm reduced to ruins by lack of preparation. Never underestimate nature, and the cost of messing with it.";
			CloseText = "Interesting.";
			Journal.addStringToRumoursPage("According to my studies cross-breeding can be quite dangers with more advanced species of bees... I should be careful!", "book");
		}else if(outcome==1){
			EventText = "Intelligence is an interesting trait for bees. On one hand it is incredibly desireable as it increases relations between the keeper and his hive, as well as increasing hive output. That being said, a surplus of intelligence can be very dangerous... beware the hive mind.";
			CloseText = "Interesting.";
		}
		happiness=true;
		return null;
	}

	public void ScenarioEighteen(){
		ScenarioId=18;
		HeaderText = "Studious Work";
		EventText = "The rain is pooring down outside and despite your efforts, you know nothing will get done today; and so you turn to your books. What do you choose to study?";
		OptionTwo = "Intelligence";
		OptionOne = "Family Trees";
	}
	public Texture ScenarioEighteenOutcome(int outcome){

		if(outcome==2){
			EventText = "Intelligence is an interesting trait for bees. On one hand it is incredibly desireable as it increases relations between the keeper and his hive, as well as increasing hive output. That being said, a surplus of intelligence can be very dangerous... beware the hive mind.";
			CloseText = "Interesting.";
		}else if(outcome==1){
			EventText = "Bee genetics can be loosely split into two categories: the Industrious/Natural tree and the Shy/Unnatural tree. Both have distinct genetic lines and safe mutations within them, though breeding between the trees is considered taboo and is an area lacking research.";
			CloseText = "Interesting.";
			Journal.addStringToRumoursPage("Thanks to my book I have learnt of the different family trees of bees! I should try to cross-breed within these trees for maximum mutation chance.", "book");
		}
		happiness=true;
		return null;
	}

	public void ScenarioNineteen(){
		ScenarioId=19;
		HeaderText = "Studious Work";
		EventText = "Book time! What do you choose to study?";
		OptionTwo = "Family Trees";
		OptionOne = "Mutations";
	}
	public Texture ScenarioNineteenOutcome(int outcome){

		if(outcome==2){
			EventText = "Bee genetics can be loosely split into two categories: the Industrious/Natural tree and the Shy/Unnatural tree. Both have distinct genetic lines and safe mutations within them, though breeding between the trees is considered taboo and is an area lacking research.";
			CloseText = "Interesting.";
			Journal.addStringToRumoursPage("Thanks to my book I have learnt of the different family trees of bees! I should try to cross-breed within these trees for maximum mutation chance.", "book");
		}else if(outcome==1){
			EventText = "Cross breeding is an interesting and crucial part of an bee keepers life. While it has the potential to create new and exciting variations of bees, it also has the potential for undesirable outcomes. Much thought should be put into cross breeding and which bees may have favourable genetic mutations.";
			CloseText = "Interesting.";
			Journal.addStringToRumoursPage("Mutations amongst bee species seem very interesting... I should consider the nature and types of bees I have before cross breeding to ensure the greatest mutation chance!", "book");
		}
		happiness=true;
		return null;
	}

	public void ScenarioThirty(){
		ScenarioId=30;
		HeaderText = "Studious Work";
		EventText = "Study time!";
		OptionTwo = "Bland Bees";
		OptionOne = "Common Bees";
	}
	public Texture ScenarioThirtyOutcome(int outcome){

		if(outcome==2){
			EventText = "Bland bees are highly unexciting and generally dull creatures. Best bred in submissive (second) the second slot as they are very timid and will not produce much in the way of mutation.";
			CloseText = "Interesting.";
		}else if(outcome==1){
			EventText = "Common bees are much like their bland counterparts, not very prone to mutation and not very dominant. They are known to have interesting results when bred with themselves...";
			CloseText = "Interesting.";
		}
		happiness=true;
		return null;
	}

	//<<<----- END OF BOOK/TUTORIAL EVENTS ----->>>

	//<<<-------Scenarios 20-30 are for events unique to having items--------->>>
	public void ScenarioTwenty(){ //PICKAXE IS FOR STONE BEES
		ScenarioId=20;
		HeaderText = "A Small Opening";
		EventText = "You have been following a small mountainside for some time and to your right is a large cliff face. You turn a corner and are confronted by an old mine entrance covered in rubble...";
		OptionTwo = "Try to remove the rocks";
		if(backpack.itemTruth["pickaxe"]){
			OptionOne = "Plow through with your pick";
		}else{
			OptionOne = "Carry on";
		}
	}
	public Texture ScenarioTwentyOutcome(int outcome){
		if(outcome==1&&backpack.itemTruth["pickaxe"]){
			EventText = "You chip at the rocks and have quickly cleared the rubble with ease. Inside the now open cave you find a small running spring amongst a few skeletons of miners who undoubtedly were trapped and perished in here. Near one of the skeletons you find a satchel of honey! A staple snack for hardworking miners. I'm sure he woudln't mind you taking it.";
			CloseText = "Did that skeleton move?";
			waterResult = Random.Range(30,55); //lots of water is stored in the cave! Underground creek
			honeyResult = Random.Range(22,40);
		}else if(outcome==1){
			EventText = "I don't have time to play around with rocks.";
			CloseText = "The sun is still high.";
		}else if(outcome==2){
			EventText = "You struggle with the rocks for some time but your attempts are fruitless. They won't budge!";
			CloseText = "I'll be back, rocks.";
		}
		return null;
	}

	public void ScenarioTwentyOne(){
		ScenarioId=21;
		HeaderText = "Danger!";
		EventText = "You are peacefully walking when you hear an aggressive buzz getting louder behind you. You turn around and realize you have been swamped by a rogue gang of red bees, out for loot...";
		if(apiary.GetComponentInChildren<ApiaryOrganiser>().beeTruth["warrior"]){
			OptionOne = "I CHOOSE YOU WARRIOR BEES!";
		}else{
			OptionOne = "Fight them";
		}
		OptionTwo = "Try to run!";
	}
	public Texture ScenarioTwentyOneOutcome(int outcome){
		if(outcome==1&&apiary.GetComponentInChildren<ApiaryOrganiser>().beeTruth["warrior"]){
			EventText = "You send your warrior bees out and with protective vigor they are able to overcome your assialiants, routing them to the hills!";
			CloseText = "Woohoo!";
		}else if(outcome==1){
			EventText = "They beat you up and took your stuff... Though they admired your courage";
			CloseText = "Nice!";
			waterResult=Random.Range(-5,-7);
			foodResult=Random.Range(-5,-7);
			honeyResult=Random.Range(-10,-15);
		}else if(outcome==2){
			EventText = "They beat you up and took your stuff...";
			CloseText = "Worth a try";
			waterResult=Random.Range(-10,-15);
			foodResult=Random.Range(-10,-15);
			honeyResult=Random.Range(-20,-30);
		}
		return null;
	}

	public void ScenarioTwentyTwo(){
		ScenarioId=22;
		HeaderText = "Today is a good day!";
		EventText = "You are walking merrily down the trail without a care in the world!";
		OptionOne = "Whistle a tune!";
		OptionTwo = "Enjoy the sights!";
	}
	public Texture ScenarioTwentyTwoOutcome(int outcome){
		if(outcome==1){
			EventText = "You whistle for some time and feel good as you hear the birds above following along.";
			CloseText = "Nice!";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(10,12));
		}else if(outcome==2){
			EventText = "The world sure is beautiful! You feel good.";
			CloseText = "Nice!";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(8,14));
		}
		happiness=true;
		return null;
	}

	public void ScenarioTwentyThree(){
		ScenarioId=23;
		HeaderText = "Stream";
		EventText = "You come to a stream to refill your water bottle, it is very tranquil around these parts.";
		OptionOne = "Draw some water";
		OptionTwo = "Have a swim";
	}
	public Texture ScenarioTwentyThreeOutcome(int outcome){
		if(outcome==1){
			waterResult=Random.Range(15,30);
			EventText="You drink deeply and fill up your reserves generously";
			CloseText="Thank you nature!";
		}else if(outcome==2){
			waterResult=Random.Range(5,10);
			foodResult=Random.Range(5,10);
			honeyResult=Random.Range(5,10);
			EventText="You lay on your back in the water and gently float away as the water carries you down stream. After some time it rests you on a bank, and next to you is an old camp site!";
			CloseText="What luck!";
		}
		return null;
	}

	public void ScenarioTwentyFour(){
		ScenarioId=24;
		HeaderText = "Decisions";
		EventText = "On your left hand side is a towering column of trees, darker than the edge of night, while on your right rocky terrain. Which do you explore?";
		OptionOne = "Rockies";
		OptionTwo = "The trees";
	}
	public Texture ScenarioTwentyFourOutcome(int outcome){
		int rand = Random.Range(0,10);
		if(outcome==1){
			if(rand<8){//BAD
				EventText="You find the terrain harsh and cruel and get lost within its crags... It will take you a while to get out.";
				CloseText="Damn!";
				foodResult=Random.Range(-10,-25);
				waterResult=Random.Range(-10,-25);
				honeyResult=Random.Range(-10,-25);
			}else{//BUT GOOD IS BETTER
				EventText="The crags are unkind to travellers, but today seem to be very accomodating - you have found an old hermits shack up within the mountain ranges!";
				CloseText="Such riches!";
				honeyResult=Random.Range(80,140);
			}
		}else if(outcome==2){
			if(rand<6){//BAD
				EventText="The forest is dark and bewildering and before long you are lost... it will take you some time to return, but at least the trees will shield you from the harsh sun.";
				CloseText="Back we go";
				foodResult=Random.Range(-5,-15);
				waterResult=Random.Range(-5,-15);
				honeyResult=Random.Range(-5,-15);
			}else{//BUT GOOD IS NOT AS GOOD
				EventText="You feel at home in the forest, and can track your way through its density with ease. You find a small cabin within a clearing which contains some well needed supplies!";
				CloseText="Yum!";
				foodResult=Random.Range(20,40);
				waterResult=Random.Range(20,40);
			}
		}
		return null;
	}

	public void ScenarioTwentyFive(){
		ScenarioId=25;
		HeaderText = "???";
		EventText = "Around you is only blackness... 'No man is above luck...' the void belows at you. 'And for every game there is a toll... What will you risk?' it asks...";
		OptionOne = "Everything!";
		OptionTwo = "Only what it takes";
	}
	public Texture ScenarioTwentyFiveOutcome(int outcome){
		int rand = Random.Range(0,10);
		if(outcome==1){
			EventText = "'An honourable choice... for without risk, how can one truly live?' the void whispers... Somewhere in the distance you can hear a coin flip... ";
			if(rand<=3){//hard but large reward
				EventText = EventText + "\n'Lady fortune is on your side today traveller...and she admires your courage. May you leave a wealthy man!' and like a feverish dream you reawaken, lying on the grass in the sun.";
				CloseText = "A dream?";
				honeyResult=Random.Range(112,145);
			}else{
				EventText = EventText + "\n'Today the mistress of fortune has forsaken you my friend... though she is impressed by your courage. She takes only because she must...' and like a feverish dream you reawaken, lying on the grass in the sun.";
				CloseText = "A dream?";
				honeyResult=Random.Range(-60,-85);
			}
		}else if(outcome==2){
			EventText = "'A safe life may be a long life, but it won't be without tedium...' the void whispers... Somewhere in the distance you can hear a coin flip... ";
			if(rand<=6){//good but low reward
				EventText = EventText + "\n'Today fortune is on your side traveller... but beware! One can only expect to gain what he is willing to risk...' and like a feverish dream you reawaken, lying on the grass in the sun.";
				CloseText = "A dream?";
				honeyResult=Random.Range(25,35);
			}else{
				EventText = EventText + "\n'Today the mistress of fortune has forsaken you my friend... perhaps it only gives to those willing to risk...' and like a feverish dream you reawaken, lying on the grass in the sun.";
				CloseText = "A dream?";
				honeyResult=Random.Range(-12,-22);
			}
		}
		return null;
	}

	//<<<<<<<<<<<------events past 30 are region specific------------->>>>>>>>>>>>>

	//MOUNTAINS:
	public void ScenarioThirtyOne(){ 
		ScenarioId=31;
		HeaderText = "Precarious Heights!";
		EventText = "Walking through the mountains you find a detour from the path, taking you to a steep crevas, crossed only by a small wooden bridge...";
		OptionTwo = "Probably not safe...";
		if(backpack.itemTruth["shoes"]){
			OptionOne = "You feel confident in the jump";
		}else{
			OptionOne = "Take the bridge";
		}
	}
	public Texture ScenarioThirtyOneOutcome(int outcome){
		if(outcome==1&&backpack.itemTruth["shoes"]){
			EventText = "Thanks to your Yeezys you clear the gap with ease and explore the area. An old miners colony was here and there are many supplies left behind!";
			CloseText = "Now to get back!";
			waterResult = Random.Range(30,55); //lots of water is stored in the cave! Underground creek
			foodResult = Random.Range(50,75);
			honeyResult = Random.Range(20,35);
		}else if(outcome==1){
			EventText = "One step onto the bridge and it immediatly gives way... You wake up next to a river a few hours later and in the distance can make out the area you fell from. This stream miust have broken your fall! Though you do notice some of your supplies have washed away...";
			CloseText = "Lucky!";
			waterResult = Random.Range(-25,-20); //lots of water is stored in the cave! Underground creek
			foodResult = Random.Range(-25,-20);
			honeyResult = Random.Range(-10,-1);
		}else if(outcome==2){
			EventText = "You think you made the right call.";
			CloseText = "Bye!";
		}
		return null;
	}

	public void ScenarioThirtyTwo(){ 
		ScenarioId=32;
		HeaderText = "Towering Peaks";
		EventText = "You come to a pass and see two towering peaks in the distance. Where are you headed?";
		OptionTwo = "Left!";
		OptionOne = "Right!";
	}
	public Texture ScenarioThirtyTwoOutcome(int outcome){
		if(outcome==1){
			EventText = "Onwards to victory! The left peak is being conquered today!";
			CloseText = "Here I come!";
		}else if(outcome==2){
			EventText = "Onwards to victory! The right peak is being conquered today!";
			CloseText = "Here I come!";
		}
		happiness=true;
		HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(7,10));
		return null;
	}

	public void ScenarioThirtyThree(){ 
		ScenarioId=33;
		HeaderText = "The Summit";
		EventText = "You raise your weary head and realise you have hit the top of a mountain! Looking around fills you with awe.";
		OptionTwo = "Enjoy the view";
		OptionOne = "Onwards!";
	}
	public Texture ScenarioThirtyThreeOutcome(int outcome){
		if(outcome==1){
			EventText = "The beauty of the world will have to wait another day.";
			CloseText = "Don't forget to smell the flowers!";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(-2,-4));
		}else if(outcome==2){
			EventText = "Taking in the sights inspires you. You feel like you have a lesson you can take back to your hive. [within species purity is better preserved for 10 turns]";
			CloseText = "Breathtaking!";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(10,20));
		}
		happiness=true;
		return null;
	}


	//SNOW
	public void ScenarioFourtyOne(){ //snow cave, unstable ground, play in the snow
		ScenarioId=41;
		HeaderText = "Blizzard!";
		EventText = "The weather is taking a turn for the worse! Soon this will be unbearable.";
		OptionOne = "Nothing will stop me.";
		OptionTwo = "Find some shelter!";
	}
	public Texture ScenarioFourtyOneOutcome(int outcome){
		if(outcome==1){
			EventText = "You trudge through the snow making less and less progress until you can barely take a step. While progress is slow, you feel your resolve has been hardened.";
			CloseText = "Take that nature!";
			waterResult = Random.Range(-10,-5); //lots of water is stored in the cave! Underground creek
			foodResult = Random.Range(-10,-5);
		}else if(outcome==2){
			EventText = "Around a corner is a small cave that shelters you from the harsh winds and snow. You decide to enjoy a snack while the storm passes over.";
			CloseText = "Yum!";
			foodResult = Random.Range(-4,-2);
		}
		return null;
	}

	public void ScenarioFourtyTwo(){ //snow cave, unstable ground, play in the snow
		ScenarioId=42;
		HeaderText = "Unstable Footing";
		EventText = "Ahead of you is a large frozen lake. You want to cross it but you are not sure how deep the ice is...";
		OptionTwo = "She'll be right";
		OptionOne = "I'll go around";
	}

	public Texture ScenarioFourtyTwoOutcome(int outcome){
		int rand = Random.Range(0,10);
		if(outcome==1){
			EventText = "The water looks mighty cold! Not risking it makes sense to you.";
			CloseText = "This could be worse";
		}else if(outcome==2&&rand>0.5){
			EventText = "The ice seems stable for a few steps until... it cracks! You fall in and get mighty chilly. You manage to crawl out fine but a large stash of your honey has frozen together, making it useless...";
			CloseText = "Blast!";
			foodResult = Random.Range(-40,-25);
		}else{
			EventText = "The ice seems stable for a few steps until... nothing! You manage to cross it safely and have a great time slipping around while you do it! [you feel inspired, bees are more likely to cross breed for 5 turns]";
			CloseText = "Wheee!";
		}
		return null;
	}

	public void ScenarioFourtyThree(){ 
		ScenarioId=43;
		HeaderText = "What are you doing here?";
		EventText = "Walking across a field you see a small group of bright blue bees wandering around. They look disoriented and lost...";
		OptionOne = "Walk over";
		OptionTwo = "Talk to them";
	}

	public Texture ScenarioFourtyThreeOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==1&&backpack.itemTruth["robe"]){
			EventText = "You manage to walk over without disturbing the bees. They seem confused and upon seeing you, decide you probably know where you are going. They settle into your backpack.";
			CloseText = "Hello friends";
			beeQuantity = Random.Range(3,6);
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				//saves me having to store it twice?
				if(text.GetComponentInChildren<RawImage>().texture.name=="shore"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
		}else if(outcome==1){
			EventText = "You walk over and the bees but as you get closer their humming starts to effect you and you break down in tears. When you regain your sense they are gone.";
			CloseText = "I hope they are okay";
		}else{
			EventText = "Your noise startles the bees and they leave with haste...";
			CloseText = "I hope they are okay";
		}
		return texture;
	}

	//FOREST
	public void ScenarioFiftyOne(){ //the allfather tree
		ScenarioId=51;
		HeaderText = "The Allfather";
		EventText = "You round a corner and are confronted by the most majestic tree you have ever seen; it's branches stretching to the skies and it's roots undoubtely deeper than the mountain. 'SPEAK. WHAT DOES YOUR HEART DESIRE?', it bellows at you in an ethereal voice";
		OptionOne = "Wealth";
		OptionTwo = "Happiness";
	}
	public Texture ScenarioFiftyOneOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==1){
			string[] beeOutcomes = {"shore","forest","ocean","warrior","icy","stone","toxic"};
			string bee = beeOutcomes[Random.Range(1,beeOutcomes.Length)];
			beeQuantity = Random.Range(3,6);
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				if(text.GetComponentInChildren<RawImage>().texture.name==bee){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
			EventText = "'IT IS YOURS'";
			CloseText = "Wow!";
		}else{
			EventText = "'THEN YOU ARE SEARCHING IN THE WRONG PLACE...'";
			CloseText = "Have I learnt something?";
		}
		return texture;
	}

	public void ScenarioFiftyTwo(){ 
		ScenarioId=52;
		HeaderText = "Mother Nature";
		EventText = "As you are walking you feel the ground beneath your feet start talking to you... if you can call it that. It vibrates in tones you seem to understand at a baser level. It wants you to pick a fortune...";
		OptionOne = "Good health";
		OptionTwo = "Good luck";
	}
	public Texture ScenarioFiftyTwoOutcome(int outcome){
		if(outcome==1){
			EventText = "You are overcome with a sense of zealous energy. You haven't felt this good since you were young.";
			CloseText = "I am going to run.";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(8,11));
		}else{
			EventText = "It suggests perhaps you already have this.";
			CloseText = "Have I learnt something?";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(9,12));
		}
		happiness=true;
		return null;
	}

	public void ScenarioFiftyThree(){ 
		ScenarioId=53;
		HeaderText = "???";
		EventText = "The world in front of you bends and distorts for a brief second, revealing vistas of strange architecture and light as if behind a curtain. Cries echo through the woods around you as it shuts...";
		OptionOne = "Find the source";
		OptionTwo = "Must've been the wind";
	}

	public Texture ScenarioFiftyThreeOutcome(int outcome){
		Texture texture=defaultTexture;
		if(outcome==1){
			EventText = "You track the source to a strange bee glowing with eldritch energy...";
			CloseText = "Not sure I like this.";
			beeQuantity = 1;
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				if(text.GetComponentInChildren<RawImage>().texture.name=="diligent"){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
		}else{
			EventText = "Some things are best not disturbed";
			CloseText = "I feel suffocated...";
		}
		return texture;
	}

	//GRASSLAND
	public void ScenarioSixetyOne(){
		ScenarioId=61;
		HeaderText = "War";
		EventText = "Over a field ahead of you lays two clans of bright red bees. To your attuned eye you can see they aren't quite the same, and a turf war seems to be in progress...";
		OptionOne = "Resolve the dispute";
		OptionTwo = "Leave them be";
	}
	public Texture ScenarioSixetyOneOutcome(int outcome){
		if(outcome==1&&backpack.itemTruth["sword"]){
			EventText = "You draw your sword and enter the field. At first the bees seem aggrivated but soon die down - it seems they respect your martial presence and disperse.";
			CloseText = "Not sure I like this.";
			honeyResult=Random.Range(20,60);
		}else if(outcome==1){
			EventText = "You shout reason at the bees for some time but they only look back at you perplexed. This soon turns to anger and they descend on you in droves. You barely make it to a nearby river before they catch you.";
			CloseText = "Oh no";
			foodResult=Random.Range(-20,-10);
			waterResult=Random.Range(-20,-10);
		}else{
			EventText = "Probably a good idea given the nature of these bees.";
			CloseText = "Onwards";
		}
		return null;
	}

	public void ScenarioSixetyTwo(){
		ScenarioId=62;
		HeaderText = "Exploration";
		EventText = "The delicate and desolate grasslands have given you time to think and reflect... you feel like you have learnt a lesson.";
		OptionOne = "Bees";
		OptionTwo = "The World";
	}
	public Texture ScenarioSixetyTwoOutcome(int outcome){
		if(outcome==1){
			EventText = "Bee keeping is about patience you decide. You owe as much to them as they owe to you...";
			CloseText = "You feel grateful";
		}else if(outcome==2){
			EventText = "Taming this world is not for men, but for bees. Without them the forests would dry up.";
			CloseText = "You feel grateful";
		}
		HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(5,9));
		happiness=true;
		return null;
	}
	public void ScenarioSixetyThree(){
		ScenarioId=63;
		HeaderText = "Decisions";
		EventText = "Your feet are draggin and the land keeps stretching to the horizion - your backpack is getting very heavy and you won't make it far unless you drop some weight... what is to go?";
		OptionOne = "Food";
		OptionTwo = "Water";
	}
	public Texture ScenarioSixetyThreeOutcome(int outcome){
		if(outcome==1){
			EventText = "You leave behind some food and already feel the weight lifting from your shoulders.";
			CloseText = "I can do without...";
			foodResult=Random.Range(-10,-20);
		}else if(outcome==2){
			EventText = "You leave behind some water and already feel the weight lifting from your shoulders.";
			CloseText = "I can do without...";
			waterResult=Random.Range(-10,-20);
		}
		return null;
	}

	//DESERT:
	public void ScenarioSeventyOne(){
		ScenarioId=71;
		HeaderText = "Another person?";
		EventText = "Across from you on the other side of the dune walks a man... Weren't you alone on this island? \n 'Hello my friend! I can read your surprise at meeting another human out here... I can see you have already met my brother the merchant - I can recognize his goods anywhere. I, my friend, am an Alchemist; searching these dunes to fulfill my legend. I wonder, what is yours?";
		OptionOne = "Bee Keeping";
		OptionTwo = "My Father";
	}
	public Texture ScenarioSeventyOneOutcome(int outcome){
		if(outcome==1){
			Texture texture=defaultTexture;
			EventText = "'A noble pursuit indeed! To spread life and push the boundaries of what we know is a dream only few can realise. To help you on your way:'";
			CloseText = "The man disappears";
			string[] beeOutcomes = {"shore","forest","ocean","warrior","icy","stone"};
			string bee = beeOutcomes[Random.Range(1,beeOutcomes.Length)]; //might cause out of bounds exception
			beeQuantity = Random.Range(3,6);
			foreach(GameObject text in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				if(text.GetComponentInChildren<RawImage>().texture.name==bee){
					texture=text.GetComponentInChildren<RawImage>().texture;	
					break;
				}
			}
			return texture;
		}else if(outcome==2){
			EventText = "'The past is an elusive mistress, and thus is it's allure, though one must be careful living their life through the memories of another... For your travels:'";
			CloseText = "The man dissapears";
			waterResult=Random.Range(35,50);
			foodResult=Random.Range(35,50);
		}
		return null;
	}
	public void ScenarioSeventyTwo(){
		ScenarioId=72;
		HeaderText = "Oasis";
		EventText = "In the hazy delusion of the heat you think you spot an oasis up ahead. Drawing closer reveals your mind has not tricked you - animals of all kinds have gatherd here for salvation.";
		OptionOne = "Drink from the Oasis";
		OptionTwo = "Leave it";
	}
	public Texture ScenarioSeventyTwoOutcome(int outcome){
		if(outcome==1){
			EventText = "You drink deeply and draw some water to take with you. All animals are equal at the oasis.";
			CloseText = "Slurp";
			waterResult=Random.Range(23,42);
		}else if(outcome==2){
			EventText = "Salvation can only be found through fasting... A man's resolve is all he needs.";
			CloseText = "You feel hardened";
		}
		return null;
	}
	public void ScenarioSeventyThree(){
		ScenarioId=73;
		HeaderText = "Mirage";
		EventText = "In front of you appears a large entity - clear as day! What do you see?!";
		OptionOne = "Secret Knowledge!";
		OptionTwo = "My Father!";
	}
	public Texture ScenarioSeventyThreeOutcome(int outcome){
		if(outcome==1){
			EventText = "As you lean in to learn the unlearnable the vision shimmers in front of you and dissapears... Must've been your imagination.";
			CloseText = "Was it really real?";
		}else if(outcome==2){
			EventText = "You approach the smile that beams down at you, and as you rise to greet the eyes you find yourself only staring into the sky... What happened?";
			CloseText = "Was it really real?";
			HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(8,10));
		}
		happiness=true;
		return null;
	}

	//OCEAN: can mostly be bland/boring/nothing happening (catch fish, wavey tide, boring day, etc)
	public void ScenarioEightyOne(){
		ScenarioId=81;
		HeaderText = "Jumping Fish";
		EventText = "To the side of your boat you occasionally spot gleaming fish jump from the water...";
		OptionOne = "Get the rod!";
		OptionTwo = "My hands will do!";
	}
	public Texture ScenarioEightyOneOutcome(int outcome){
		if(outcome==1){
			EventText = "You whip out your trusty rod and before long have caught a small number of fish. Dinner tonight is covered!";
			CloseText = "Yum!";
			foodResult=Random.Range(21,29);
		}else if(outcome==2){
			EventText = "The fish are less nimble than you expect and with your cat-like reflexes you are able to grab a number of them from the air.";
			CloseText = "Yum!";
			foodResult=Random.Range(20,30);
		}
		return null;
	}

	public void ScenarioEightyTwo(){
		ScenarioId=82;
		HeaderText = "WAVES!";
		EventText = "A storm has set in and the water around you is growing. A huge wave summits the horizon and you have no idea what to do...";
		OptionOne = "Turn this ship around!";
		OptionTwo = "Try to get over it!";
	}
	public Texture ScenarioEightyTwoOutcome(int outcome){ //add some chance in here maybe later?
		if(outcome==1){
			EventText = "You are not sure what happened. You wake up in your boat floating through sunny shores exhausted and wet... some of your supplies are missing, presumably washed into the dank depths of the sea.";
			CloseText = "Did I black out?";
			foodResult=Random.Range(-10,-20);
			waterResult=Random.Range(-10,-20);
			honeyResult=Random.Range(-10,-20);
		}else if(outcome==2){
			EventText = "With the might of a thousand men you are able to push the oars beyond their usual capacity and beeline for the wave. Your boat starts to gain height and is tipping in angle, almost to a full ninety degrees until... you get over it and find yourself being boosted down the other side!";
			CloseText = "Whee!";
		}
		return null;
	}

	public void ScenarioEightyThree(){
		ScenarioId=83;
		HeaderText = "Bored";
		EventText = "The sea can be a trying mistress... you've been sailing a whole day without a single happeneing, and the boat is barely large enough to lay down in...";
		OptionOne = "Look for some land.";
		OptionTwo = "Look down into the sea.";
	}
	public Texture ScenarioEightyThreeOutcome(int outcome){
		if(outcome==1){
			EventText = "You search the horizon for some time before coming up with nothing.";
			CloseText = "So bored.";
		}else if(outcome==2){
			EventText = "Beneath you is infinite depths of black, and your own unclean mug starting back into your eyes.";
			CloseText = "I should shave.";
		}
		return null;
	}


	//<<<----- UNIQUE STORY SCENARIOS-------->>>> All story scenarios should reward with a rare bee


	public void yuckyIf(int rand, Hex hex){
		if(identifyTile(hex,"mountain")){//we are on a mountain tile 
			if(rand==1){
				ScenarioSix(); //stone bees
			}else if(rand==2){
				ScenarioSix(); //stone bees
			}else if(rand==3){
				ScenarioFour(); //ice bees
			}else if(rand==4){
				ScenarioThirtyOne();
			}else if(rand==5){
				ScenarioThirtyTwo();
			}else if(rand==6){
				ScenarioThirtyThree();
			}else{
				restOfIf(rand);
			}
		}else if(identifyTile(hex,"snow")){//we are on a snow tile
			if(rand==1){
				ScenarioFour(); //ice bees
			}else if(rand==2){
				ScenarioFour(); //ice bees
			}else if(rand==3){
				ScenarioSix(); //stone bees
			}else if(rand==4){
				ScenarioFourtyOne();
			}else if(rand==5){
				ScenarioFourtyTwo();
			}else if(rand==6){
				ScenarioFourtyThree();
			}else{
				restOfIf(rand);
			}
		}else if(identifyTile(hex,"forest")){//we are on a forest tile
			if(rand==1){
				ScenarioFive(); //forest bees
			}else if(rand==2){
				ScenarioFive(); //forest bees
			}else if(rand==3){
				ScenarioThree(); //warrior bees
			}else if(rand==4){
				if(eventsThatHaveOccured.Contains(51)){
					restOfIf(rand);
				}else{
					ScenarioFiftyOne();
				}
			}else if(rand==5){
				ScenarioFiftyTwo();
			}else if(rand==6){ //only want players to be able to get one diligent bee
				if(eventsThatHaveOccured.Contains(53)){
					restOfIf(rand);
				}else{
					ScenarioFiftyThree();
				}
			}else{
				restOfIf(rand);
			}
		}else if(identifyTile(hex,"grassland")){//we are on a grassland tile
			if(rand==1){
				ScenarioThree(); //warrior bees
			}else if(rand==2){
				ScenarioThree(); //warrior bees
			}else if(rand==3){
				ScenarioFive(); //forest bees
			}else if(rand==4){
				ScenarioSixetyOne();
			}else if(rand==5){
				ScenarioSixetyTwo();
			}else if(rand==6){
				ScenarioSixetyThree();
			}else{
				restOfIf(rand);
			}
		}else if(identifyTile(hex, "desert")){//we are on a desert tile
			if(rand==1){
				ScenarioEight(); //shore bees
			}else if(rand==2){
				ScenarioEight(); //shore bees
			}else if(rand==3){
				ScenarioSeven(); //ocean bees
			}else if(rand==4){
				ScenarioSeventyOne();
			}else if(rand==5){
				ScenarioSeventyTwo();
			}else if(rand==6){
				ScenarioSeventyThree();
			}else{
				restOfIf(rand);
			}
		}else{ //we are on an ocean tile. If we are here we are assuming we have the boat as movement was allowed
			if(rand==1){
				ScenarioSeventy(); //ocean bees
			}else if(rand==2){
				ScenarioSeventy(); //ocean bees
			}else if(rand==3){
				ScenarioEighty(); //shore bees
			}else if(rand==4){
				ScenarioEightyOne();
			}else if(rand==5){
				ScenarioEightyTwo();
			}else{
				ScenarioEightyThree(); //OTHER situation is just boring times at sea
			}
		}
	}

	public void createBookEvent(){
		int rand = (int)Random.Range(9,20.99f);
		if(rand==9){
			ScenarioNine();
		}else if(rand==10){
			ScenarioTen();
		}else if(rand==11){
			ScenarioEleven();
		}else if(rand==12){
			ScenarioTwelve();
		}else if(rand==13){
			ScenarioThirteen();
		}else if(rand==14){
			ScenarioFourteen();
		}else if(rand==15){
			ScenarioFifteen();
		}else if(rand==16){
			ScenarioSixteen();
		}else if(rand==17){
			ScenarioSeventeen();
		}else if(rand==18){
			ScenarioEighteen();
		}else if(rand==19){
			ScenarioNineteen();		
		}else if(rand==20){
			ScenarioThirty();
		}
		if(backpack.itemTruth["book"]){
			CreateEvent();
		}
		
	}
	public void restOfIf(int rand){ //if none of the other events occur
		if(rand<999){
			rand = (int)Random.Range(7,14.35f);
		}//so that story events are not re-calculated

		if(rand==7){
			ScenarioOne();
		}else if(rand==8){
			ScenarioTwo();
		}else if(rand==9){
			ScenarioTwenty();
		}else if(rand==10){
			ScenarioTwentyOne();
		}else if(rand==11){
			ScenarioTwentyTwo();
		}else if(rand==12){
			ScenarioTwentyThree();
		}else if(rand==13){
			ScenarioTwentyFour();
		}else if(rand==14){
			ScenarioTwentyFive();
		}else if(rand==15){
			//ScenarioTwentySix();
		}else if(rand==27){
			//ScenarioTwentySeven();
		}else if(rand==28){
			//ScenarioTwentyEight();
		}else if(rand==29){
			//ScenarioTwentyNine();		
		}else if(rand==30){
			//ScenarioThirty();
		}else if(rand==1000){
			ScenarioStoryOneStart();
		}else if(rand==1002){
			ScenarioStoryOneAfter();
		}else if(rand==2000){
			ScenarioStoryTwoStart();
		}else if(rand==2001){
			ScenarioStoryTwoBefore();
		}else if(rand==2002){
			ScenarioStoryTwoAfter();
		}else if(rand==3000){
			ScenarioStoryThreeStart();
		}else if(rand==3001){
			ScenarioStoryThreeBefore();
		}else if(rand==3002){
			ScenarioStoryThreeAfter();
		}else if(rand==4000){
			ScenarioStoryFourStart();
		}else if(rand==4001){
			ScenarioStoryFourBefore();
		}else if(rand==4002){
			ScenarioStoryFourAfter();
		}else if(rand==5000){
			ScenarioStoryFiveStart();
		}else if(rand==5001){
			ScenarioStoryFiveBefore();
		}else if(rand==5002){
			ScenarioStoryFiveAfter();
		}else{
			rand = (int)Random.Range(7,9.99f);
			restOfIf(rand); //might be something to do with here
		}
	}

	public Texture yuckyOutcomeIf(int rand){
		eventsThatHaveOccured.Add(ScenarioId);
		if(ScenarioId==1){
			return ScenarioOneOutcome(rand);
		}else if(ScenarioId==2){
			return ScenarioTwoOutcome(rand);
		}else if(ScenarioId==3){
			return ScenarioThreeOutcome(rand);
		}else if(ScenarioId==4){
			return ScenarioFourOutcome(rand);
		}else if(ScenarioId==5){
			return ScenarioFiveOutcome(rand);
		}else if(ScenarioId==6){
			return ScenarioSixOutcome(rand);
		}else if(ScenarioId==7){
			return ScenarioSevenOutcome(rand);
		}else if(ScenarioId==70){
			return ScenarioSeventyOutcome(rand);
		}else if(ScenarioId==8){
			return ScenarioEightOutcome(rand);
		}else if(ScenarioId==80){
			return ScenarioEightyOutcome(rand);
		}else if(ScenarioId==9){
			return ScenarioNineOutcome(rand);
		}else if(ScenarioId==10){
			return ScenarioTenOutcome(rand);
		}else if(ScenarioId==11){
			return ScenarioElevenOutcome(rand);
		}else if(ScenarioId==12){
			return ScenarioTwelveOutcome(rand);
		}else if(ScenarioId==13){
			return ScenarioThirteenOutcome(rand);
		}else if(ScenarioId==14){
			return ScenarioFourteenOutcome(rand);
		}else if(ScenarioId==15){
			return ScenarioFifteenOutcome(rand);
		}else if(ScenarioId==16){
			return ScenarioSixteenOutcome(rand);
		}else if(ScenarioId==17){
			return ScenarioSeventeenOutcome(rand);
		}else if(ScenarioId==18){
			return ScenarioEighteenOutcome(rand);
		}else if(ScenarioId==19){
			return ScenarioNineteenOutcome(rand);		
		}else if(ScenarioId==20){
			return ScenarioTwentyOutcome(rand);
		}else if(ScenarioId==21){
			return ScenarioTwentyOneOutcome(rand);
		}else if(ScenarioId==22){
			return ScenarioTwentyTwoOutcome(rand);
		}else if(ScenarioId==23){
			return ScenarioTwentyThreeOutcome(rand);
		}else if(ScenarioId==24){
			return ScenarioTwentyFourOutcome(rand);
		}else if(ScenarioId==25){
			return ScenarioTwentyFiveOutcome(rand);
		}else if(ScenarioId==26){
			//ScenarioTwentySixOutcome(rand);
		}else if(ScenarioId==27){
			//ScenarioTwentySevenOutcome(rand);
		}else if(ScenarioId==28){
			//ScenarioTwentyEightOutcome(rand);
		}else if(ScenarioId==29){
			//ScenarioTwentyNineOutcome(rand);		
		}else if(ScenarioId==30){
			ScenarioThirtyOutcome(rand);
		}else if(ScenarioId==31){
			return ScenarioThirtyOneOutcome(rand);
		}else if(ScenarioId==32){
			return ScenarioThirtyTwoOutcome(rand);
		}else if(ScenarioId==33){
			return ScenarioThirtyThreeOutcome(rand);
		}else if(ScenarioId==41){
			return ScenarioFourtyOneOutcome(rand);
		}else if(ScenarioId==42){
			return ScenarioFourtyTwoOutcome(rand);
		}else if(ScenarioId==43){
			return ScenarioFourtyThreeOutcome(rand);
		}else if(ScenarioId==51){
			return ScenarioFiftyOneOutcome(rand);
		}else if(ScenarioId==52){
			return ScenarioFiftyTwoOutcome(rand);
		}else if(ScenarioId==53){
			return ScenarioFiftyThreeOutcome(rand);
		}else if(ScenarioId==61){
			return ScenarioSixetyOneOutcome(rand);
		}else if(ScenarioId==62){
			return ScenarioSixetyTwoOutcome(rand);
		}else if(ScenarioId==63){
			return ScenarioSixetyThreeOutcome(rand);
		}else if(ScenarioId==71){
			return ScenarioSeventyOneOutcome(rand);
		}else if(ScenarioId==72){
			return ScenarioSeventyTwoOutcome(rand);
		}else if(ScenarioId==73){
			return ScenarioSeventyThreeOutcome(rand);
		}else if(ScenarioId==81){
			return ScenarioEightyOneOutcome(rand);
		}else if(ScenarioId==82){
			return ScenarioEightyTwoOutcome(rand);
		}else if(ScenarioId==83){
			return ScenarioEightyThreeOutcome(rand);
		}
		return null;
	}

	//TRIBAL VILLAGE (1 bee needed - 1 natural)
	//requires a small offering of forest bees who will show the player where an old notebook is hidden. They hav an attunement to these kinds of things
	//CAN alter eventns functions a little so we can chain from opening screen to opening screen etc OR just change text on the same one until the end
	//IF it equals 3 then just reformat text etc.

	//SMALL HOLDING (2 bees needed - 2 natural)
	//has been taken over by aggressive bees who will attack the player when confronted
	//NEED: ocean bees to pacify the warriors, and then warriors to find what they were guarding
	//they were guarding ______ (journal entry and strange bee)

	//TOWN (3 bees needed - 1 natural 2 unnatural)
	//town is deserted
	//NEED: stone bees to show you where the hole into the cave is, worker bees to clear rocks from entrance, ice bees to show you safe passage across precarious ice
	//you find ______ (journal entry and strange bee)

	//CASTLE (4 bees needed - 1 natural 2 unnatural 1 high tier unnatural)

	//OCCULTISTS CASTLE (5 bees needed - 1 natural 1 high tier natural 1 unnatural 2 high tier unnatural)
	
	
	//TRIBAL ENCAMPMENT
	private bool scenarioOneCondition=false; private Texture scenarioOneTexture;
	public void ScenarioStoryOneStart(){
		ScenarioId = 1000;
		storyChain = 0;
		HeaderText = "On the trail...";
		EventText = "You approach a small, deserted encampment. A few tents lay scattered around amongst various debris and the ashes of many nights worth of fires. You are the first person to step foot here for some time.";
		OptionOne = "Look around";
		OptionTwo = "Not for me [Leave]";
		foreach(GameObject obj in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			Bee bee = obj.GetComponentInChildren<Bee>();
			if(bee.name=="worker"&&bee.quantity>0){
				scenarioOneCondition=true;
			}
			if(bee.name=="plains"){
				scenarioOneTexture=obj.GetComponentInChildren<RawImage>().texture;
				beeQuantity = Random.Range(4,6);
			}
		}
	}
	//MISC CHAINS FOR THE STORY EVENTS
	public void storyEventOne(int option){
		if(storyChain==0){ //STEP ONE: sending bees to explore
			if(option==1){
				//Debug.Log(scenarioOneCondition);
				HeaderText = "Investigating";
				EventText = "You decide to rummage through the remains and see what you can turn up...";
				OptionOne = "Search yourself";
				if(scenarioOneCondition){
					OptionTwo = "Send Worker drones";
				}else{
					OptionTwo = "Let your bees have a look";
				}
				SetStoryTexts();
				storyChain+=1;
			}else if(option==2){
				EventText = "You decide picking through these remains is not the best idea at the moment. Perhaps you will come back later.";
				CloseText = "...";
				EndStoryEvent(null); //PASSES IN A TEXTURE
			}

		}else if(storyChain==1){
			if(option==1){
				EventText = "You search through the remains but turn up little, even after an aggressive inspection. You walk away unsatisfied, sure that you missed something. It occurs to you that maybe certain kinds of bees could help in your endeavour...";
				CloseText = "I'll be back!";
				EndStoryEvent(null); 
			}else if(option==2){
				if(scenarioOneCondition){
					EventText = "You release your worker drones and they seem to stand still, communing with one another for a time before setting out. You follow them as they thoroughly rummage through the remains, revealing a hidden diary amongst a satchel of small, timid bees before long. The diary points you to the old mining town in the area, suggesting your father was headed that way, convinced it held knowledge he needed to further his studies...";
					CloseText = "A clue!";
					boi.storyProgress+=1;
					boi.highlightStory(boi.storyProgress,boi.storyProgress-1);
					HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(15,25));
					EndStoryEvent(scenarioOneTexture);
					AudioManager.audioManager.playSound("story");
				}else{
					EventText = "You release some bees and they stay still for a second, confused, before all spreading off in different directions... Their efforts are fruitless. Perhaps specific kinds of bees will be better for certain tasks...";
					CloseText = "Chaos.";
					EndStoryEvent(null);
				}
			}
		}
	}


	//SMALL HOLDING
	public void ScenarioStoryTwoStart(){
		ScenarioId = 2000;
		storyChain = 0;
		ScenarioTwoConditions = new List<string>();
		HeaderText = "On the trail...";
		EventText = "You lay in wait around the outskirts of a small abandoned town. This is the place the diary has pointed you to...";
		OptionOne = "Lets investigate!";
		OptionTwo = "Maybe later [Leave]";
		foreach(GameObject obj in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			Bee bee = obj.GetComponentInChildren<Bee>();
			if(bee.name=="warrior"&&bee.quantity>0){
				ScenarioTwoConditions.Add("warrior");
			}
			if(bee.name=="forest"&&bee.quantity>0){
				ScenarioTwoConditions.Add("forest");
			}
			if(bee.name=="nice"){
				scenarioTwoTexture=obj.GetComponentInChildren<RawImage>().texture;
				beeQuantity = Random.Range(4,6);
			}
		}
	}
	
	private List<string> ScenarioTwoConditions; private Texture scenarioTwoTexture;
	public void storyEventTwo(int option){
		if(storyChain==0){ //initial investigation, need warrior bees to get rid of other bees there
			if(option==1){
				EventText = "Approaching the village you notice a large gang of warrior bees have infested the place, making it their own. They seem to be guarding something...";
				if(ScenarioTwoConditions.Contains("warrior")){
					OptionOne="Send in your warriors!";
				}else if(backpack.itemTruth["sword"]){
					OptionOne="Draw your shiny sword!";
				}else{
					OptionOne="We have to go through them...";
				}
				
				if(ScenarioTwoConditions.Contains("forest")){
					OptionTwo="Send your forest bees to calm them";
				}else if(backpack.itemTruth["shoe"]){
					OptionTwo="Try to sneak past";
				}else{
					OptionTwo="Try to reason with them";
				}
				SetStoryTexts();
				storyChain+=1;
			}else if(option==2){
				EventText = "The town can wait another day, as can the myster surrounding your father... Perhaps you will come back later.";
				CloseText = "...";
				EndStoryEvent(null);
			}

		}else if(storyChain==1){//need warrior bees to find what they were guarding
			if(option==1){
				if(ScenarioTwoConditions.Contains("warrior")){
					EventText = "Your warriors triumphantly dive in with a stunning pincer movement to drive off the hovering bees. Sun Tzu would be proud! While the initial attack looks to be a crushing success eventually the tides turn and the pure numbers on the enemy side overwhelm your boys.";
					CloseText = "Retreat!";
					EndStoryEvent(null);
				}else if(backpack.itemTruth["sword"]){
					EventText = "You galliantly charge in with your sword drawn, face like steel. Initially some bees seem startled but you realise they are way too many for this tactic to be effective. They quikcly turn on you and route you out of the town...";
					CloseText = "Retreat!";
					EndStoryEvent(null);
				}else{
					EventText = "You walk up to the bees determined to get through, you're not sure how but by god you're determined! Unfortunately determination is no match for their stunning military prowess and you are soon routed from the town.";
					CloseText = "Retreat!";
					EndStoryEvent(null);
				}
			}else if(option==2){
				if(ScenarioTwoConditions.Contains("forest")){
					EventText = "Your bees seem to commune for some time before setting out, and you notice that a light breeze has picked up in this time. Like magic, your drones seem to pacify the masses and are able to make them disperse after a few moments of silence. The wind dies down and the town is yours...";
					if(ScenarioTwoConditions.Contains("warrior")){
						OptionOne="Let your Warriors loose!";
					}else{
						OptionOne="They must have been guarding something";
					}
					OptionTwo = "All you forest dwellers!";
					SetStoryTexts();
					storyChain+=1;
				}else if(backpack.itemTruth["shoe"]){
					EventText = "You sneak past the main, buzzing hub of the town with ease. However, the more of the town you circle the more you see how overrun the entire place is. Stealth will not help you here...";
					CloseText = "Hmm...";
					EndStoryEvent(null); //HAVE IT SO storyChain-=1 maybe so we can restart
				}else{
					EventText = "Your mouth is alight with pure fire as you lecture the bees on the triviality of anger and violence, and how a simple traveller like you should be permitted to explore their town without confrontation. \n\n They are not impressed by this.";
					CloseText = "Retreat!";
					EndStoryEvent(null); 
				}
			}
		}else if(storyChain==2){//success or failure
			if(option==1){
				if(ScenarioTwoConditions.Contains("warrior")){
					EventText = "Your warriors eagerly exit your satchel and culminate around the source that must've been attracting the others. It is a satchel much like yours containing a number of strange, pink bees, along with a diary excerpt in what is clearly your fathers handwriting. It points to the nearby industrial town!";
					CloseText = "Another clue!";
					boi.storyProgress+=1;
					boi.highlightStory(boi.storyProgress,boi.storyProgress-1);
					HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(14,20));
					EndStoryEvent(scenarioTwoTexture);
					AudioManager.audioManager.playSound("story");
				}else{
					OptionOne="You stand over what you believe to be the source of what the warriors were protecting but can see nothing, it looks exceptionally ordinary to you. You can hear a malevolent buzzing on the horizon and decide to moved on before it returns for vengeance!";
					CloseText = "Hmm...";
					EndStoryEvent(null);
				}
			}else if(option==2){
				EventText = "Your Forest drones look at you confused, unsure of what you want them to do. Even after some time they have not moved. You can hear a malevolent buzzing on the horizon and decide to moved on before it returns for vengeance!";
				CloseText = "Hmm...";
				EndStoryEvent(null);
			}
		}
	}


	//INDUSTRIOUS VILLAGE
	public void ScenarioStoryThreeStart(){ //USES ICY/OCEAN/STONE
		ScenarioId = 3000;
		storyChain = 0;
		ScenarioThreeConditions = new List<string>();
		HeaderText = "On the trail...";
		EventText = "You approach the old industrial village, hot on the heels of your father...";
		OptionOne = "Lets investigate!";
		OptionTwo = "Maybe later [Leave]";
		foreach(GameObject obj in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			Bee bee = obj.GetComponentInChildren<Bee>();
			if(bee.name=="icy"&&bee.quantity>0){
				ScenarioThreeConditions.Add("icy");
			}
			if(bee.name=="ocean"&&bee.quantity>0){
				ScenarioThreeConditions.Add("ocean");
			}
			if(bee.name=="stone"&&bee.quantity>0){
				ScenarioThreeConditions.Add("stone");
			}
			if(bee.name=="intelligent"){
				scenarioThreeTexture=obj.GetComponentInChildren<RawImage>().texture;
				beeQuantity = Random.Range(4,6);
			}
		}
	}
	private List<string> ScenarioThreeConditions; private Texture scenarioThreeTexture;
	public void storyEventThree(int option){
		if(storyChain==0){ 
			if(option==1){
				EventText="You enter the zone and all seems quiet. The cold has set in early over this area and much of it is snowy or iced over. A large patch of dubious looking snow seperates you from the main activity hub...";
				if(ScenarioThreeConditions.Contains("ocean")){
					OptionOne="Release the ocean bees";
				}else if(backpack.itemTruth["shoe"]){
					OptionOne="Adorn your shoes";
				}else{
					OptionOne="Attempt the crossing";
				}
				if(ScenarioThreeConditions.Contains("icy")){
					OptionTwo="Release the ice bees";
				}else{
					OptionTwo="Jump it";
				}
				SetStoryTexts();
				storyChain+=1;
			}else if(option==2){
				EventText = "The industrial area can wait another day, as can the myster surrounding your father... Perhaps you will come back later.";
				CloseText = "...";
				EndStoryEvent(null);
			}

		}else if(storyChain==1){
			if(option==1){
				if(ScenarioThreeConditions.Contains("ocean")){
					EventText = "Your bees look at you confused and soon return to their satchel. They don't like this kind of environment.";
					CloseText = "Hmm";
					EndStoryEvent(null);
				}else if(backpack.itemTruth["shoe"]){
					EventText = "Your shoes make quick work of the snow, effortlessly carrying you across it. As you silently drift you notice each step getting deeper and deeper until you are nearly up to your chest! Perhaps this is not the best idea...";
					CloseText = "Time to turn back";
					EndStoryEvent(null);
				}else{
					EventText = "After walking into the snow for a short time you realise it is either too deep or too treacherous to pass blindly...";
					CloseText = "Time to turn back";
					EndStoryEvent(null);
				}
			}else if(option==2){
				if(ScenarioThreeConditions.Contains("icy")){
					EventText="Your bees excitedly exit your satchel and wait expectantly for you to follow them. Tracing their movements you are able to easily find a safe path across. Now at the main building, a towering complex of stone, you decide what your next move is. Somewhere in the distance you can faintly hear running water...";
					if(ScenarioThreeConditions.Contains("stone")){
						OptionOne="Search with your Stone bees";//CORRECT
					}else{
						OptionOne="Search the stone for clues"; 
					}
					if(ScenarioThreeConditions.Contains("ocean")){
						OptionTwo="Use Ocean bees to find the noise source";
					}else{
						OptionTwo="Track the source of the sound";
					}
					SetStoryTexts();
					storyChain+=1;
				}else{
					EventText = "You jump a short distance into the snow and sink very far... there is still a fair few metres between you and the other side...";
					CloseText = "Time to turn back";
					EndStoryEvent(null);
				}
			}
		}else if(storyChain==2){
			if(option==1){ //CORRECT
				if(ScenarioThreeConditions.Contains("stone")){
					EventText="At a snails pace your bees lead you to the large back wall of the facility. Though slow, they seem calculated, and dissapearing into the wall for a second, trigger some machination as a small section of the floor beneath you drops out! You find yourself in a dimly lit cavern, with the noise source sounding much closer...";
					if(ScenarioThreeConditions.Contains("ocean")){
						OptionOne="Follow your ocean bees!";
					}else{
						OptionOne="Find the noise source";
					}
					OptionTwo="Follow the cave";
					SetStoryTexts();
					storyChain+=1;
				}else{
					EventText = "You scour the various rock faces of the building but find nothing to help you in your search...";
					CloseText = "Hmm";
					EndStoryEvent(null);
				}
			}else if(option==2){
				if(ScenarioThreeConditions.Contains("ocean")){
					EventText = "Your ocean bees lead you to a large stone wall along the back of the facility and buzz frustratingly around it for a time before giving up... Perhaps it is empty after all?";
					CloseText = "Hmm";
					EndStoryEvent(null);
				}else{
					EventText = "You attempt to tune into the sound but it is so far and quiet that you soon realise your efforts are pointless... I wonder what it could be?";
					CloseText = "Hmm";
					EndStoryEvent(null);
				}
			}

		}else if(storyChain==3){ //OCEAN BEES LEAD YOU DOWN RIVER TO OUTLET INTO THE OCEAN
			if(option==1){
				if(ScenarioThreeConditions.Contains("ocean")){
					EventText="Your bees lead you through the tunnel, following the noise until abruptly they take a right hand turn into a recess you never would have spotted! A small stream runs through it up to what appears to be someones chambers... Or what was. On the nightchamber you find a very perculiar bee amongst some of your fathers study notes. Apparently this 'Beemancer' had died in pursuit of this rare species, and now it is yours...";
					CloseText = "This is getting weird...";
					EndStoryEvent(scenarioThreeTexture);
					boi.storyProgress+=1;
					HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(-2,-5));
					boi.highlightStory(boi.storyProgress,boi.storyProgress-1);
					AudioManager.audioManager.playSound("story");
				}else{
					EventText = "You follow the noise through the lengths of the dingy cave. It shrinks and twists, being barely able to crawl through until... LIGHT! You emerge from the cave and find yourself outside again, peacefully resting on the beach...";
					CloseText = "Hmm";
					EndStoryEvent(null);
				}
			}else if(option==2){
				EventText = "You follow the twists and turns of the dingy cave. It shrinks and twists, being barely able to crawl through until... LIGHT! You emerge from the cave and find yourself outside again, peacefully resting on the beach...";
				CloseText = "Hmm";
				EndStoryEvent(null);
			}
		}
	}


	public void ScenarioStoryFourStart(){ //USES ICY/OCEAN/STONE
		ScenarioId = 4000;
		storyChain = 0;
		ScenarioFourConditions = new List<string>();
		HeaderText = "On the trail...";
		EventText = "You approach a towering castle of stone and iron... I wonder who built this, and why it was abandoned?";
		OptionOne = "Lets investigate!";
		OptionTwo = "Maybe later [Leave]";
		foreach(GameObject obj in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			Bee bee = obj.GetComponentInChildren<Bee>();
			if(bee.name=="shore"&&bee.quantity>0){
				ScenarioFourConditions.Add("shore");
			}
			if(bee.name=="bland"&&bee.quantity>0){
				ScenarioFourConditions.Add("bland");
			}
			if(bee.name=="intelligent"&&bee.quantity>0){
				ScenarioFourConditions.Add("intelligent");
			}
			if(bee.name=="magic"&&bee.quantity>0){
				ScenarioFourConditions.Add("magic");
			}
			if(bee.name=="exotic"){
				scenarioFourTexture=obj.GetComponentInChildren<RawImage>().texture;
				beeQuantity = Random.Range(4,6);
			}
		}
	}
	private List<string> ScenarioFourConditions; private Texture scenarioFourTexture;

	public void storyEventFour(int option){
		if(storyChain==0){ 
			if(option==1){
				EventText="A large moat stands between you and the castle. The bridge is raised and a servants door, the only way you can see into the castle, is filled with crocodiles.";
				if(ScenarioFourConditions.Contains("shore")){
					OptionOne="Send your shore bees!";
				}else if(ScenarioFourConditions.Contains("bland")){
					OptionOne="GO BLAND BEES!";
				}else{
					OptionOne="Stealthily swim around...";
				}
				if(backpack.itemTruth["sword"]){
					OptionTwo="Intimidate them with your sword";
				}else{
					OptionTwo="Rush for the door!";
				}
				SetStoryTexts();
				storyChain+=1;
			}else if(option==2){
				EventText = "The castle can wait another day, as can the myster surrounding your father... Perhaps you will come back later.";
				CloseText = "...";
				EndStoryEvent(null);
			}
		}else if(storyChain==1){
			if(option==1){
				if(ScenarioFourConditions.Contains("shore")){
					EventText = "Your bees drift slowly towards the water and settle down by it's edge. The hungry crocs charge but soon stop in their tracks, brought to tears by the hums of your lost companions... with a tear in your eye you slip into the castle. You now find yourself in an old passage with a single exit - a dubious looking corridor filled with various armours and trinkets that is obviously trapped! A bad aura eminates from within...";
					if(ScenarioFourConditions.Contains("magic")){
						OptionOne="Magic bees always find a way!";
					}else if(backpack.itemTruth["shoe"]){
						OptionOne="Use your shoes to avoid the traps";
					}else{
						OptionOne="Risk it!";
					}
					if(ScenarioFourConditions.Contains("bland")){
						OptionTwo="GO BLAND BEES!";
					}else{
						OptionTwo="Not worth the danger";
					}
					SetStoryTexts();
					storyChain+=1;
				}else if(backpack.itemTruth["bland"]){
					EventText = "Your bees stand by obliviously and do nothing... What did you expect?";
					CloseText = "...";
					EndStoryEvent(null);
				}else{
					EventText = "You go some distance towards the door, creeping through the water towards the back of the hungry animals, before one spots you and alerts the others...";
					CloseText = "RUN!";
					EndStoryEvent(null);
				}
			}else if(option==2){
				if(backpack.itemTruth["sword"]){
					EventText = "You whip out your trusted sword and point it towards the crocodiles... it seems they are less prone to intimidation than bees. After waiting a second they charge you!";
					CloseText = "RUN!";
					EndStoryEvent(null);
				}else{
					EventText = "You make a burst for the door but the crocs are quick to react. You never really stood a chance.";
					CloseText = "RUN!";
					EndStoryEvent(null);
				}
			}
		}else if(storyChain==2){
			if(option==1){
				if(ScenarioFourConditions.Contains("magic")){
					EventText = "You release your bees and watch as they approach the passage, confer, and then return to you without hesitation. Whatever is down there, they want no part in...";
					CloseText = "Hmm...";
					EndStoryEvent(null);
				}else if(backpack.itemTruth["shoe"]){
					EventText = "You adorn your shoes and make it a few steps before you hear something dart across your path - what was that?!... You decide crossing blind is not worth the risk.";
					CloseText = "I'll be back...";
					EndStoryEvent(null);
				}else{
					EventText = "You take a single step and hear something dart across your path - what was that?!... You decide crossing blind is not worth the risk.";
					CloseText = "I'll be back...";
					EndStoryEvent(null);
				}
			}else if(option==2){
				if(ScenarioFourConditions.Contains("bland")){
					EventText="Your bland bees drift through the room oblivious to the traps around them... and make it to the other side safely? Sometimes a room is purely decorative I suppose. At the end of the corridor lies a giant wall with a huge enscription on it in a language you dont understand. You can not see any other exits... It reads: " + generateRandomHeader(3) + " " + generateRandomHeader();
					OptionOne="Send your smartest bees";
					if(backpack.itemTruth["book"]){
						OptionTwo="Consult the Bee-Compendium";
					}else{
						OptionTwo="Try to decipher it";
					}
					SetStoryTexts();
					storyChain+=1;
				}else{
					EventText = "I dont deserve to die here...";
					CloseText = "...";
					EndStoryEvent(null);
				}
			}
		}else if(storyChain==3){
			if(option==1){
				if(ScenarioFourConditions.Contains("intelligent")){
					EventText = "Your bees stand around for some time before the ones emminating an orange glow step up to the wall. After a series of melodic hums the wall clicks and slides away, revealing a hidden room, and a dastardly scene... Incomprehensible books are strewn everywhere and in the middle of the room sits a warped worship site to an unknown entity. It emminates a strange energy...";
					if(backpack.itemTruth["book"]){
						OptionOne="Consult the Bee-Compendium";
					}else{
						OptionOne="Investigate the alter";
					}
					OptionTwo="Send your weirdest bees";
					SetStoryTexts();
					storyChain+=1;
				}else{
					EventText = "Your bees are smart little creatures but between them cant seem to understand what the symbols mean... perhaps these types don't quite have the mental fortitude for it?";
					CloseText = "I'll be back";
					EndStoryEvent(null);
				}
			}else if(option==2){
				if(backpack.itemTruth["book"]){
					EventText = "You flip through page after page and after some time realise there is nothing remotely related to the symbols. Whatever these runes are, they are not considered conventional knowledge...";
					CloseText = "Hmm";
					EndStoryEvent(null);
				}else{
					EventText = "Looking at the runes your brain feels like it is swelling in waves... you have to look away before you nearly black out.";
					CloseText = "That was weird...";
					EndStoryEvent(null);
				}
			}
		}else if(storyChain==4){
			if(option==1){
				if(backpack.itemTruth["book"]){
					EventText = "You look at your book trying to find answers but find yourself unable to read the text held within... the letters warp and shift before your eyes, as if they are about to melt off of the page!";
					CloseText = "Get me out of here.";
					EndStoryEvent(null);
				}else{
					EventText = "You within a few metres of the alter before you feel a slight trickle of blood run down your nose. Before you are aware what has happened you wake up outside the castle walls, gently resting on the grass...";
					CloseText = "I did not like that.";
					EndStoryEvent(null);
				}
			}else if(option==2){
				if(ScenarioFourConditions.Contains("magic")){
					EventText = "While your other bees look perplexed, your dark purple bees float through the field effortlessly and after a few seconds you notice it has died down... hesitantly, you step onto the alter and find the barely-legible scramblings of your father, amongst some unnaturally glowing bees who you did not notice until the removal of the field. They must've been trapped inside... I don't want to know what happened here.";
					CloseText = "I am close...";
					EndStoryEvent(scenarioFourTexture);
					boi.storyProgress+=1;
					HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(Random.Range(-35,-55));
					boi.highlightStory(boi.storyProgress,boi.storyProgress-1);
					AudioManager.audioManager.playSound("story");
				}else{
					EventText = "Your most naturally attuned bees float around the alter in confusion. After some time they return to you, perplexed and visibly upset...";
					CloseText = "Let us leave, friends.";
					EndStoryEvent(null);
				}
			}
		}
	}

	private string ScenarioFiveConditions="nope"; private Texture scenarioFiveTexture;
	//END GAME SCENARIO 
	public void ScenarioStoryFiveStart(){ //not going to be a very complicated scenario, very straightforward
		ScenarioId = 5000;
		storyChain = 0;
		HeaderText = "On the trail...";
		EventText = "In front of you stands the once mighty University... You have a feeling all paths end here.";
		OptionOne = "Here I come!";
		OptionTwo = "Maybe later [Leave]";
		foreach(GameObject obj in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			Bee bee = obj.GetComponentInChildren<Bee>();
			if(bee.name=="toxic"&&bee.quantity>0){
				ScenarioFiveConditions = "toxic";
			}
			if(bee.name=="intelligentNice"){
				scenarioFiveTexture=obj.GetComponentInChildren<RawImage>().texture;
				beeQuantity = 1;
			}
		}
	}

	public void storyEventFive(int option){
		if(storyChain==0){
			if(option==1){
				EventText="Entering the University sends a shiver down your spine... These once decadent halls seem distorted and malignant, as if the world itself is bending here. There are two wings to traverse, one on either side of you - Alchemy or Naturology?";
				OptionOne="Alchemy";
				OptionTwo="Naturology";
				SetStoryTexts();
				storyChain+=1;
			}else if(option==2){
				EventText = "Perhaps not knowing what happened to your father is for the best. Some secrets should remain shrouded in mystery...";
				CloseText = "Let us leave, friends.";
				EndStoryEvent(null);
			}
		}else if(storyChain==1){
			if(option==1){
				EventText="The Alchemical wing is filled with all manner of tormented machinations, violating and distilling evermore until all essence is transformed. The laboratories stretch on for some time before reaching an exit...";
			}else if(option==2){
				EventText="The Naturology wing is ripe with species and specimens from all the furthest and most exotic corners of the Earth. It is remarkable how science can reduce such majestic creatures to triviliality. The laboratories stretch on for some time before reaching an exit...";
			}
			OptionOne="Follow the corridor";
			OptionTwo="Leave";
			SetStoryTexts();
			storyChain+=1;
		}else if(storyChain==2){
			if(option==1){
				EventText="The corridor twists around and leads you to a single room... the apiary. There is a large lock on the door - the one thing in this place that is not ripe with decay...";
				if(apiary.GetComponentInChildren<ApiaryOrganiser>().beeTruth["magic"]){
					OptionOne="Use magic to cleanse it";
				}else{
					OptionOne="Pick the lock";
				}
				if(ScenarioFiveConditions=="toxic"){
					OptionTwo="Fight fire with fire";
				}else{
					OptionTwo="Bash the lock";
				}
				storyChain+=1;
				SetStoryTexts();
			}else if(option==2){
				EventText="You have learnt enough from this place. The pleasures of life are to be enjoyed, not studied...";
				CloseText = "Good riddance";
				EndStoryEvent(null);
			}
		}else if(storyChain==3){
			if(option==1){
				if(apiary.GetComponentInChildren<ApiaryOrganiser>().beeTruth["magic"]){
					EventText="The thickness of black magic is too strong is this place - your bees try their best but are unable to stray far from your satchel before being forced to return, visibly in pain...";
					CloseText = "Curse this place";
					EndStoryEvent(null);
				}else{
					EventText="Despite your best efforts the lock does not budge... Another approach might do the trick.";
					CloseText = "Curse this place";
					EndStoryEvent(null);
				}
			}else if(option==2){
				if(ScenarioFiveConditions=="toxic"){
					EventText="Your toxic bees are able to melt through the lock in seconds... I suppose, like everything, decay has its purpose. The door is unlocked and you can feel a vile aura eminating from within. This may be your last chance to turn back...";
					OptionOne="Curiosity besets me...";
					OptionTwo="I do not want to know...";
					SetStoryTexts();
					storyChain+=1;
				}else{
					EventText="Ouch.";
					CloseText = "Curse this place";
					EndStoryEvent(null);
				}
			}
		}else if(storyChain==4){
			if(option==1){
				EventText="You swing open the doors ready to face the worst! and are greeted by a turgid, twisting mass of metal... The purpose of the machine is unclear to you, but you can see a faint glow dawning from its main chamber. Opening the chamber you find a singularly, perculiar bee... your father. You are sure he is happy...";
				CloseText = "I am happy";
				EndStoryEvent(scenarioFiveTexture);
				boi.storyProgress+=1;
				HappinessMeter.meter.GetComponentInChildren<HappinessMeter>().fillMeter(100);
				boi.highlightStory(boi.storyProgress,boi.storyProgress-1);
				AudioManager.audioManager.playSound("story");
			}else if(option==2){
				EventText="As you turn away and leave that accursed place you can feel you psyche lifted... You are confident you made the right choice, and are happy you get to focus on your true passion, beekeeping. For one must remember not to live their life through the past of another...";
				CloseText = "I am happy";
				EndStoryEvent(null);
			}
		}
	}


	public static void CreateDisasterousEvent(){

		AudioManager.audioManager.playSound("weird");
		int rand = (int)Random.Range(0,3.99f);

		if(rand==0){ //food and water are erased
			HeaderText=instance.generateRandomHeader();
			EventText="You feel like something terrible has happened... God you're hungry...";
			instance.boi.UpdateResources(-10000,-10000,-10000);

		}else if(rand==1){ //your bees get sick and start dying
			HeaderText=instance.generateRandomHeader();
			EventText="You feel like something horrible has happened... A thousand voices all silenced at once...";
			foreach(GameObject obj in instance.apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				Bee bee = obj.GetComponentInChildren<Bee>();
				if(bee.quantity>0){
					int deletion = Random.Range(-3,-5);
					instance.apiary.GetComponentInChildren<ApiaryOrganiser>().addToBeeQuantity(obj.GetComponentInChildren<RawImage>().texture, deletion);
				}
			}

		}else if(rand==2){ //Have to create something different that is not shop keeper based, he dies way at end.
			HeaderText=instance.generateRandomHeader();
			EventText="You feel like something horrible has happened... An unending, horrific buzzing...";
			//TODO: think of an other event that can occur for this. AT THE MOMENT simply turns all bees in the hive into bland bees
			foreach(GameObject obj in instance.apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
				Bee bee = obj.GetComponentInChildren<Bee>();
				if(bee.type=="bland"){
					instance.apiary.GetComponentInChildren<ApiaryOrganiser>().addToBeeQuantity(obj.GetComponentInChildren<RawImage>().texture, 999);
				}else if(bee.quantity>0){
					instance.apiary.GetComponentInChildren<ApiaryOrganiser>().addToBeeQuantity(obj.GetComponentInChildren<RawImage>().texture, -100);
				}
			}
		}else if(rand==3){
			HeaderText=instance.generateRandomHeader();
			EventText="You feel like something horrible has happened... The physical deletion of a million atoms...";
			foreach(GameObject obj in instance.backpack.uniqueSlots){
				if(obj.GetComponentInChildren<UniqueItem>().name!="cloak"){ //dimensional rip stays
					obj.GetComponentInChildren<UniqueItem>().activated=false;
				}
			}
			instance.backpack.createDict();
		}

		GameObject outcome = (GameObject) Instantiate(instance.closePrefab);
		Text[] texts = outcome.GetComponentsInChildren<Text>();

		//sets header and event text
		if(texts[0].name=="HeaderText"){
			texts[0].text = HeaderText;
			texts[1].text = EventText;
		}else{
			texts[0].text = EventText;
			texts[1].text = HeaderText;
		}

		//sets resources gained text
		texts[2].text="?";
		texts[3].text="?";
		texts[4].text="?";
		
		//sets button text
		outcome.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Oh no...";
		outcome.GetComponentInChildren<Button>().onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});


		outcome.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		outcome.transform.localScale=Vector3.one;

		defaultPos.y = Screen.height/2;
		defaultPos.x = Screen.width/2;
		outcome.transform.position = defaultPos;
	}

	public string generateRandomHeader(int length=0){
		char[] chars = {'a','f','t','i','l','m','z','x','c','2','1','3','4','5','6','7','8','9','0','!','@','#','$','%','^','&','*','(',')','_','-','+','=','.',',','>','<','?'};
		if(length==0){
			length = Random.Range(9,23);
		}
		char[] output = new char[length];
		for(int i=0;i<length;i++){
			int index = Random.Range(0,chars.Length);
			output[i] = chars[index];
		}
		string s = new string(output);
		return s;
	}

	public static void CreateEndGameEvent(){
		//TODO: trigger for game ending event

		foreach(GameObject obj in instance.backpack.uniqueSlots){ //adds dimensional rip to backpack
			UniqueItem item = obj.GetComponentInChildren<UniqueItem>();
			if(item.uniqueName=="hole"){
				item.activated=true; 
				obj.GetComponentInChildren<Button>().onClick.AddListener(delegate{instance.dimensionalRipClick();}); //adds onclick
				obj.GetComponentInChildren<Button>().onClick.AddListener(delegate{AudioManager.audioManager.playSound("weird");});
				instance.backpack.updateTextures(); 
				instance.backpack.createDict();
			}
		}

		foreach(GameObject bee in ApiaryOrganiser.apiaryOrg.bees){ //sets all bee quantites to "?"
			bee.GetComponentInChildren<RawImage>().texture = Journal.j.defaultBee;
			ApiaryOrganiser.apiaryOrg.DisplayBee(bee,bee.GetComponentInChildren<Bee>(),false);
		}
		
		//do all strange stuff in here

		TurnOrganiser.turnOrg.turnCount.GetComponentInChildren<Text>().text = "?";
		TurnOrganiser.turnOrg.endGame=true;

		ChangeResourceText.updateEndGameUI();

	}

	public void dimensionalRipClick(){
		//creates the screen that will end the game
		//need a special death prefab
		GameObject gameOver  = (GameObject) Instantiate(instance.endGame);

		Button button = gameOver.GetComponentInChildren<Button>();
		button.onClick.AddListener(delegate{AudioManager.audioManager.playSound("button");});
		button.onClick.AddListener(delegate{Destroy(gameOver);});
		button.onClick.AddListener(delegate{TurnOrganiser.turnOrg.startFresh();});

		TurnOrganiser.turnOrg.fadeIn(gameOver);

		gameOver.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		gameOver.transform.localScale=Vector3.one;
		//Setting position to be correct
		defaultPos.y = Screen.height/2;
		defaultPos.x = Screen.width/2;
		gameOver.transform.position = defaultPos;
	}


	public void ScenarioStoryOneAfter(){
		EventText = "The camp is alive with the buzzing of bees who have moved in. The area feels alive.";
		CloseText = "Nice!";
		foreach(GameObject bee in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			if(bee.GetComponentInChildren<RawImage>().name=="common"){
				EndStoryEvent(bee.GetComponentInChildren<RawImage>().texture, false);
			}
		}
	}

	public void ScenarioStoryTwoBefore(){
		EventText = "You can hear an aggressive buzz emminating from the town centre. You are not sure you should investigate... yet.";
		CloseText = "Another time.";
		EndStoryEvent(null);
	}
	public void ScenarioStoryTwoAfter(){
		EventText = "With nothing to guard the resident bees have abandoned the town, leaving it a refuge displaced denizens of the forest. It is very peaceful here.";
		CloseText = "Nice!";
		foreach(GameObject bee in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			if(bee.GetComponentInChildren<RawImage>().name=="forest"){
				EndStoryEvent(bee.GetComponentInChildren<RawImage>().texture, false);
			}
		}
	}

	public void ScenarioStoryThreeBefore(){
		EventText = "The industrial zone is unusually quiet and covered in a large blanket of snow... is it always this deep around these parts? Perhaps you will explore this in the future.";
		CloseText = "Another time";
		EndStoryEvent(null);
	}
	public void ScenarioStoryThreeAfter(){
		EventText = "The snow has largely melted from this area and life is returning... it is becoming a hub of activity for bees of all kinds!";
		CloseText = "Nice!";
		foreach(GameObject bee in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			if(bee.GetComponentInChildren<RawImage>().name=="nice"){
				EndStoryEvent(bee.GetComponentInChildren<RawImage>().texture, false);
			}
		}
	}

	public void ScenarioStoryFourBefore(){
		EventText = "There are many rumours about this castle - its heritage and history are largely shrouded in secret... Perhaps one day you will venture forth in search of these secrets...";
		CloseText = "Another time";
		EndStoryEvent(null);
	}
	public void ScenarioStoryFourAfter(){
		EventText = "Since dismantling the sacreligious aparatus a vibe of tranquility has returned to the castle, and an ambitious group of particularly skilled bees has made it their home.";
		CloseText = "Nice!";
		foreach(GameObject bee in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			if(bee.GetComponentInChildren<RawImage>().name=="magic"){
				EndStoryEvent(bee.GetComponentInChildren<RawImage>().texture, false);
			}
		}
	}

	public void ScenarioStoryFiveBefore(){
		EventText = "The old University... decrepit and delapidated. What secrets it must hold... One day you might choose to explore its mysterious halls...";
		CloseText = "Another time";
		EndStoryEvent(null);
	}
	public void ScenarioStoryFiveAfter(){
		EventText = "You can hear bees hornily buzzing... you go Dad!";
		CloseText = "Nice!";
		foreach(GameObject bee in apiary.GetComponentInChildren<ApiaryOrganiser>().bees){
			if(bee.GetComponentInChildren<RawImage>().name=="intelligentNice"){
				EndStoryEvent(bee.GetComponentInChildren<RawImage>().texture, false);
			}
		}
	}
}

[System.Serializable]
class ScenarioData{
	public List<int> occuredEvents;
}
