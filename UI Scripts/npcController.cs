using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class npcController : MonoBehaviour {

	public GameObject map; 
	public GameObject apiary;
	public Texture[] allItemTextures; //initialised in the editor
	private Item[] allItems;
	private GameObject instance;
	public GameObject npcStartPrefab, npcMainPrefab, npcShopPrefab, endPrefab;
	private GameObject canvas;
	private UniquesBackpack backpack;
	private Item[] shopInventory;
	private Vector3 defaultPos = Vector3.zero;

	//for initial interaction
	private Button startButton; private Text startText;

	//for main screen
	private Button shopButton, rumourButton; private Text dialogueText;

	//for shop screen
	private Button exit;private Image slotOne, slotTwo, slotThree, slotFour, slotFive, slotSix;

	public static npcController npc;

	void Awake(){
		if(npc==null){
			npc=this;
		}else if(npc!=this){
			Destroy(gameObject);
		}
	}
	void Start(){
		if(allItemTextures!=null){
			initAllItems();
		}
		
		if(player==null||apiaryOrg==null||canvas==null){
			player = Map.map.player;
			apiaryOrg = apiary.GetComponentInChildren<ApiaryOrganiser>();
			backpack = UniquesBackpack.backpack;
		}
	}

	public void initNpc(bool first){

		//dont need this if, even though it is there for safety
		if(first){
			instance = Instantiate(npcStartPrefab);
			count=0;
		}else{
			instance = Instantiate(npcMainPrefab);
		}
		//finds button and sets onclick
		startButton = instance.GetComponentInChildren<Button>();
		startButton.onClick.AddListener(delegate{introRoutine(1);});
		//finds text
		startText = instance.GetComponentsInChildren<Text>()[1];

		positionCorrect(instance);
		instance.SetActive(true);
		//have to get button when it's created and assign an onclick value etc
	}

	public void openNpcScreen(bool comeFromStart){
		numberOfRumours=0;
		if(instance){
			Destroy(instance);
		}
		instance = Instantiate(npcMainPrefab);

		//get buttons and set OnClick
		//TODO: setOnClicks
		shopButton = instance.GetComponentsInChildren<Button>()[0];
		rumourButton = instance.GetComponentsInChildren<Button>()[1];
		shopButton.onClick.AddListener(delegate{openShopScreen();});
		rumourButton.onClick.AddListener(delegate{pickRumour(1);});

		//get text object
		dialogueText = instance.GetComponentsInChildren<Text>()[2];

		if(comeFromStart){
			dialogueText.text = ". . .";
		}

		positionCorrect(instance);
		instance.SetActive(true);
	}

	//onClick for shop button
	public void openShopScreen(){
		//generate four random items
		//Index 0 is always food and 1 is always water
		shopInventory = generateItems(6); //SHOULD work

		if(instance){
			Destroy(instance);
		}
		instance = Instantiate(npcShopPrefab);
		
		//gets exit button and assigns onclick
		exit = instance.GetComponentInChildren<Button>();
		exit.onClick.AddListener(delegate{CloseNpc();});

		//gets shop slots as images and inits their buttons
		Image[] images = instance.GetComponentsInChildren<Image>();
		
		slotOne = images[2];
		slotOne.GetComponentInChildren<Button>().onClick.AddListener(delegate{BuyItem(slotOne, shopInventory[0]);});
		setImage(slotOne, shopInventory[0]);
		
		slotTwo = images[5];
		slotTwo.GetComponentInChildren<Button>().onClick.AddListener(delegate{BuyItem(slotTwo, shopInventory[1]);});
		setImage(slotTwo, shopInventory[1]);

		slotThree = images[8];
		slotThree.GetComponentInChildren<Button>().onClick.AddListener(delegate{BuyItem(slotThree, shopInventory[2]);});
		setImage(slotThree, shopInventory[2]);

		slotFour = images[11];
		slotFour.GetComponentInChildren<Button>().onClick.AddListener(delegate{BuyItem(slotFour, shopInventory[3]);});
		setImage(slotFour, shopInventory[3]);

		slotFive = images[14];
		slotFive.GetComponentInChildren<Button>().onClick.AddListener(delegate{BuyItem(slotFive, shopInventory[4]);});
		setImage(slotFive, shopInventory[4]);

		slotSix = images[17];
		slotSix.GetComponentInChildren<Button>().onClick.AddListener(delegate{BuyItem(slotSix, shopInventory[5]);});
		setImage(slotSix, shopInventory[5]);


		positionCorrect(instance);
		instance.SetActive(true);
		checkHoneyBalance(); //visual aid for if player has insufficient honey
	}

	public void endShopKeeper(){
		instance = Instantiate(endPrefab);
		instance.GetComponentInChildren<Button>().onClick.AddListener(delegate{Destroy(instance);});
		positionCorrect(instance);
	}	

	public void positionCorrect(GameObject obj){
		obj.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		obj.transform.localScale=Vector3.one;

		//Setting position to be correct
		defaultPos.y = Screen.height/2 -30;
		defaultPos.x = Screen.width/2;
		obj.transform.position = defaultPos;
	}

	//initialises the slot with an image
	public void setImage(Image slot, Item item){

		slot.GetComponentInChildren<RawImage>().texture = item.texture;

		//sets text variables
		Text[] texts = slot.GetComponentsInChildren<Text>();
		texts[0].text = item.generateHoneyCost().ToString();
		texts[1].text = item.generateQuantity().ToString();
	}

	//generates x unique items to be added to the shop window
	public Item[] generateItems(int toGenerate){
		List<int> indexIn = new List<int>();
		//index 0 is always food and 1 is always water
		List<Item> itemsToReturn = new List<Item>();
		
		itemsToReturn.Add(allItems[0]);
		indexIn.Add(0);
		itemsToReturn.Add(allItems[1]);
		indexIn.Add(1);

		//check if player has book. If not hard spawn it : BOOK HARDSPAWNING WORKS!
		if(!backpack.itemTruth["book"]){
			itemsToReturn.Add(allItems[9]); //BOOK
			indexIn.Add(9);
			toGenerate-=3;
		}else{
			toGenerate-=2;
		}

		for(int i=0;i<toGenerate;i++){
			
			int item = generateItem();
			Debug.Log(allItems[item].type);
			//regenerates duplicates 
			int count=0;
			while(true){ 
				if(allItems[item].type=="unique"){ //regenerate if item is unique
					if(backpack.itemTruth[allItems[item].identifier]){
						item = generateItem();
					}else if(indexIn.Contains(item)){ //regenerate if item is unique and already in indexIn
						item=generateItem();
					}else{
						break;
					}
				}else{
					if(indexIn.Contains(item)){ //regenerate if item is not unique
						item=generateItem();
					}else{
						break;
					}
				}
				if(count>20){
					break;
				}
				count++;
			}

			itemsToReturn.Add(allItems[item]);
			indexIn.Add(item);
		}
		return itemsToReturn.ToArray();
	}

	//generates a single item by randomly selecting one from the list
	public int generateItem(){
		int rand = Random.Range(2,allItems.Length);
		return rand;
	}

	public void checkHoneyBalance(){ //changes colour of honeyCost text if the user has insufficient funds. Need this function to check ALL costs
		int index=0;
		foreach(Item item in shopInventory){ //could've really had all the shop slots in an array and then index could be used inplicitly
			if(item.tempCost>player.Honey){
				if(index==0){
					slotOne.GetComponentInChildren<Text>().color=Color.red;
				}else if(index==1){
					slotTwo.GetComponentInChildren<Text>().color=Color.red;
				}else if(index==2){
					slotThree.GetComponentInChildren<Text>().color=Color.red;
				}else if(index==3){
					slotFour.GetComponentInChildren<Text>().color=Color.red;
				}else if(index==4){
					slotFive.GetComponentInChildren<Text>().color=Color.red;
				}else if(index==5){
					slotSix.GetComponentInChildren<Text>().color=Color.red;
				}
			}
			index+=1;
		}
	}

	public Character player;
	public ApiaryOrganiser apiaryOrg; 
	//onClick for the button on each slot
	public void BuyItem(Image slot,Item item){
	
		//check honeyCost
		if(item.tempCost>player.Honey){
			Debug.Log("insufficient honey to purchase!");
			return;
		}

		//remove cost from honey
		player.UpdateResources(0,0,-item.tempCost);
		
		//add item to inventory
		if(item.type=="resource"){ //FOR FOOD AND WATER
			if(item.identifier=="MelonWater"){
				player.UpdateResources(item.tempQuantity,0,0);
				Debug.Log("BOUGHT" + item.identifier);
			}else if(item.identifier=="Stein"){
				player.UpdateResources(0,item.tempQuantity,0);
				Debug.Log("BOUGHT" + item.identifier);
			}
			regenerateCosts(item, slot);
		}else if(item.type=="bee"){ //FOR BEE
			foreach(GameObject bee in apiaryOrg.bees){
				if(item.identifier==bee.GetComponentInChildren<Bee>().type){
					if(bee.GetComponentInChildren<Bee>().quantity<0){
						bee.GetComponentInChildren<Bee>().quantity=0;
					}
					apiary.GetComponentInChildren<ApiaryOrganiser>().addToBeeQuantity(item.texture, item.tempQuantity);
					break;
				}
				Debug.Log("BOUGHT" + item.identifier);
			}
			regenerateCosts(item, slot);
		}else if(item.type=="unique"){//FOR UNIQUES
			buyUniqueItem(item.identifier);
			Debug.Log("BOUGHT" + item.identifier);
			updateSlotTextures(slot);
		}
		
		checkHoneyBalance();
	}

	public void regenerateCosts(Item item, Image slot){
		Text[] texts = slot.GetComponentsInChildren<Text>();
		texts[0].text = item.generateHoneyCost().ToString();
		texts[1].text = item.generateQuantity().ToString();
	}

	public void updateSlotTextures(Image slot){
		slot.GetComponentInChildren<RawImage>().enabled=false; //can re-set the graphic here to something really nice and friendly if we want :^). Just turn it off atm
		Text[] texts = slot.GetComponentsInChildren<Text>();
		texts[0].text=""; //CAN do a foreach text in texts here but not really necessary. Is a lot cleaner I guess
		texts[1].text=":^)";
		slot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
		slot.GetComponentsInChildren<Image>()[1].enabled = false; //disables honey graphic
	}

	public void buyUniqueItem(string nam){
		//set bought variable to true for the item with name nam
		foreach(GameObject obj in backpack.uniqueSlots){
			UniqueItem item = obj.GetComponentInChildren<UniqueItem>();
			if(item.uniqueName==nam){
				item.activated=true; 
				backpack.updateTextures(); 
				backpack.createDict();
				return;
			}
		}
	}

	int count=0;
	public void introRoutine(int number){
		count += number;
		if(count==1){
			startText.text = "Greetings traveller! I have heard strange tales of your unique honey and travelled long and far to reach you! May I humbly request a sample?";
			startButton.GetComponentInChildren<Text>().text = "Where did you come from?";
		}else if(count==2){
			startText.text = "Wouldn't you like to know. Will you give me a sample or not! Hmm?";
			startButton.GetComponentInChildren<Text>().text = "Uhh... Okay";
		}else if(count==3){
			startText.text = "My oh my! This is... FANTASTIC!";
			startButton.GetComponentInChildren<Text>().text = ". . .";
		}else if(count==4){
			startText.text = "I have many wares! Yes many wares indeed! Please if you will allow me to buy your honey I will return every week! I can offer you many things...";
			startButton.GetComponentInChildren<Text>().text = "Sounds good.";
		}else if(count==5){
			startText.text = "Brilliant! Let's get started!";
			startButton.GetComponentInChildren<Text>().text = "Sure old man";
		}else if(count==6){
			//delete the old instance and create an instance of the new prefab with the shop
			openNpcScreen(true);
		}
	}

	public void CloseNpc(){
		Destroy(instance);
		shopInventory=null;
		player.currentHex.hexMap.highlightSelectableTiles(player.currentHex);//roundabout/super messy but should fix bug where tiles aren't highlighted after buying food
	}

	//onClick for rumour button. Should use enums but meh
	int numberOfRumours;
	public void pickRumour(int number){
		//TODO: implement some feature so rumours that have been displayed can be tracked
		//for now 1 is passed. Find a way to count or similar later
		numberOfRumours+=number;
		if(numberOfRumours>2){
			dialogueText.text = "That's enough folklore for today my friend.";	
			return;
		} //catch block so only a few rumours can be accessed at a time

		int rand = (int)Random.Range(0,22.99f); //CAN MA
		if(rand==1){
			dialogueText.text = "Even at sea I've heard strange buzzing at night.";
		}else if(rand==2){
			dialogueText.text = "Exploring is fun but it takes resources to do so! Make sure to keep an eye on your stockpiles...";
			Journal.addStringToRumoursPage("Exploring is fun but it takes resources to do so! Make sure to keep an eye on your stockpiles...", "shop");
		}else if(rand==3){
			dialogueText.text = "Always think before cross-breeding higher tier bees... dangerous traits can arise.";
		}else if(rand==4){
			dialogueText.text = "Are you sure you're alone on this island? Funny, I thought I saw... nevermind.";
		}else if(rand==5){
			dialogueText.text = "Have you tried exploring the island? I'm sure there's all kinds of secrets hiding away.";
		}else if(rand==6){
			dialogueText.text = "If you're getting stuck with cross breeding, try exploring different biomes to find different species!";
			Journal.addStringToRumoursPage("If you're getting stuck with cross breeding, try exploring different biomes to find different species!", "shop");
		}else if(rand==7){
			dialogueText.text = "There was another young man I used to visit on this island. I wonder what happened to him...";
		}else if(rand==8){
			dialogueText.text = "Have you seen those red bees around? Nasty little creatures! Stung me right on the eye!";
		}else if(rand==9){
			dialogueText.text = "I used to visit another island close to here but recently I haven't been able to find it. It's as if the whole place has sunk!";
		}else if(rand==10){
			dialogueText.text = "Be sure to get the Bee-Compendium if you can! It is a must have for all budding bee keepers!";
			if(!backpack.itemTruth["shop"]){
				Journal.addStringToRumoursPage("Be sure to get the Bee-Compendium if you can! It is a must have for all budding bee keepers!", "shop");
			}
		}else if(rand==11){
			dialogueText.text = "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn... Oh sorry! Just clearing my throat.";
		}else if(rand==12){
			dialogueText.text = "I've heard there are certain rare bees that won't interact with lesser developed species.";
			Journal.addStringToRumoursPage("I've heard there are certain rare bees that won't interact with lesser developed species.","shop");
		}else if(rand==13){
			dialogueText.text = "If you start finding rarer and stranger bees pleasure make sure to take the proper precautions before putting them in the apiary!";
			Journal.addStringToRumoursPage("If you start finding rarer and stranger bees pleasure make sure to take the proper precautions before putting them in the apiary!", "shop");
		}else if(rand==14){
			dialogueText.text = "I swear I have seen a human made entierely of bees! Or at least it looked like a human...";
		}else if(rand==15){
			dialogueText.text = "If you're cold and emotionless like me just find some of those bees near the water. Their siren song will make you weep in seconds!";
		}else if(rand==16){
			dialogueText.text = "When cross-breeding, at least one of the bee in your dominant slot should stick around!";
			Journal.addStringToRumoursPage("When cross-breeding, at least one of the bee in your dominant slot should stick around!", "shop");
		}else if(rand==17){
			dialogueText.text = "Sticky Comb will double your chances of getting mutations when cross-breeding! As well as generally increasing the strength of the species in your dominant slot!";
		}else if(rand==18){
			dialogueText.text = "Have you tried exploring the various abandoned outlets around the island? This place was once teeming with activity - who knows what it's hiding...";
		}else if(rand==19){
			dialogueText.text = "If you're struggling to find a bee, try exploring different biomes! Each one has it's own native species!";
			Journal.addStringToRumoursPage("If you're struggling to find a bee, try exploring different biomes! Each one has it's own native species!", "shop");
		}else if(rand==20){
			dialogueText.text = "Make sure to buy miscellaneous items! You may need them to aquire certain kinds of bees!";
			Journal.addStringToRumoursPage("Make sure to buy miscellaneous items! You may need them to aquire certain kinds of bees!", "shop");
		}else if(rand==21){
			dialogueText.text = "Bland bees are quite boring! They very rarely result in mutations occuring in my experience.";
			Journal.addStringToRumoursPage("Bland bees are quite boring! They very rarely result in mutations occuring in my experience.", "shop");
		}else if(rand==22){
			dialogueText.text = "Be sure to spend your honey wisely! Common bees won't generate very much in the apiary.";
		}

	}


	//used in start to init all items
	public void initAllItems(){
		List<Item> items = new List<Item>();

		foreach(Texture texture in allItemTextures){
			Item item = new Item(texture.name,texture);
			items.Add(item);
		}

		allItems = items.ToArray();

		assignAllValues();
	}

	public void assignAllValues(){
		foreach(Item item in allItems){
			if(item.identifier=="MelonWater"){
				item.assignValues("resource",new int[2] {5,10},new int[2] {8,14});
			}else if(item.identifier=="Stein"){
				item.assignValues("resource",new int[2] {4,9},new int[2] {11,13});
			}else if(item.identifier=="bland"){
				item.assignValues("bee",new int[2] {8,13},new int[2] {3,7});
			}else if(item.identifier=="common"){
				item.assignValues("bee",new int[2] {14,18},new int[2] {2,6});
			}else if(item.identifier=="worker"){
				item.assignValues("bee",new int[2] {41,53},new int[2] {2,4});
			}else if(item.identifier=="plains"){
				item.assignValues("bee",new int[2] {40,52},new int[2] {2,4});
			}else if(item.identifier=="magic"){
				item.assignValues("bee",new int[2] {754,845},new int[2] {1,3});
			}else if(item.identifier=="boat"){
				item.assignValues("unique",new int[2] {74,105},new int[2] {1,1});
			}else if(item.identifier=="honeycomb"){
				item.assignValues("unique",new int[2] {678,702},new int[2] {1,1});
			}else if(item.identifier=="book"){
				item.assignValues("unique",new int[2] {5,10},new int[2] {1,1});
			}else if(item.identifier=="pickaxe"){
				item.assignValues("unique",new int[2] {74,105},new int[2] {1,1});
			}else if(item.identifier=="robot"){
				item.assignValues("unique",new int[2] {74,105},new int[2] {1,1});
			}else if(item.identifier=="rose"){
				item.assignValues("unique",new int[2] {74,105},new int[2] {1,1});
			}else if(item.identifier=="sword"){
				item.assignValues("unique",new int[2] {74,105},new int[2] {1,1});
			}else if(item.identifier=="shoe"){
				item.assignValues("unique",new int[2] {74,105},new int[2] {1,1});
			}else if(item.identifier=="cloak"){
				item.assignValues("unique",new int[2] {179,341},new int[2] {1,1});
			}
		}
	}

}