using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Bee : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public string type;

	//DESCRIPTIVE VARIABLES FOR TOOLTIP
	private string nature, description,displayName;
	private bool dominant;

	private bool activated=false;
	public int quantity;
	public GameObject beeObject;
	private GameObject toolTip;
	
	public void setToolTip(GameObject obj){
		toolTip = obj;
		initText();
		//assignHover();
	}

	public bool updateVisuals(){
		if(quantity>=0){
			activated=true;
		}
		return activated;
	}

	public void showToolTip(){
		toolTip.SetActive(true);
		Text[] fields = toolTip.GetComponentsInChildren<Text>();
		fields[0].text = displayName;
		fields[1].text = "Nature:  " + nature;
		if(dominant){
			fields[2].text = "Genes: " + "Dominant";
		}else{
			fields[2].text = "Genes: " + "Recessive";
		}
		fields[3].text = "'" + description + ".'";
	}

	public void hideToolTip(){
		toolTip.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData){
		showToolTip();
	}

	public void OnPointerExit(PointerEventData eventData){
		hideToolTip();
	}

	//in here have it pass its texture


	public void initText(){
		//HARD CODE THESE
		if(this.type=="bland"){
			displayName = "Bland";
			nature = "Oblivious";
			dominant = false;
			description = "The most plain bee I ever did see";
		}else if(this.type=="common"){
			displayName = "Common";
			nature = "Mild";
			dominant = false;
			description = "A staple of apiaries around the globe";
		}else if(this.type=="diligent"){
			displayName = "Exotic";
			nature = "???";
			dominant = true;
			description = "Pulsating with Occult energies";
		}else if(this.type=="diligentWorker"){
			displayName = "Black Hole";
			nature = "???";
			dominant = true;
			description = "A strange power draws this to unending energy";
		//will kill off other bees in your hive if above a certain percentage
		}else if(this.type=="diligentWarrior"){
			displayName = "Killer";
			nature = "Hostile";
			dominant = true;
			description = "The death of the Universe";
		//exotic should have a chance to produce any bee
		}else if(this.type=="exotic"){
			displayName = "Strange";
			nature = "???";
			dominant = false;
			description = "One would think the laws of physics are wrong after observing this little critter";
		}else if(this.type=="exoticShore"){
			displayName = "Void";
			nature = "???";
			dominant = false;
			description = "The world is starting to break, and warps in weird shapes around this creature";
		}else if(this.type=="exoticWorker"){
			displayName = "Infinibee";
			nature = "???";
			dominant = false;
			description = "A strange power draws this to twisted blackness";
		}else if(this.type=="forest"){
			displayName = "Forest";
			nature = "Pure";
			dominant = true;
			description = "A child of the Earth";
		}else if(this.type=="icy"){
			displayName = "Ice";
			nature = "Distant";
			dominant = false;
			description = "Cold to the touch. Prefers to live in isolation outside of the hive";
		}else if(this.type=="intelligent"){
			displayName = "Intelligent";
			nature = "???";
			dominant = true;
			description = "The mental capacity of these bees is noteworthy";
		//will form a hive mind and create a npc that tries to steal your resources
		}else if(this.type=="intelligentCommon"){
			displayName = "Genius";
			nature = "???";
			dominant = false;
			description = "Knows things that would drive the sturdiest man mad";
		}else if(this.type=="intelligentNice"){
			displayName = "Horny Bee";
			nature = "Horny";
			dominant = true;
			description = "So potent it will eventually out-fuck all other animals until it is the last remaining life form in the Universe";
		}else if(this.type=="magic"){
			displayName = "Magical";
			nature = "Miraculous";
			dominant = true;
			description = "This peculiar bee seems to exist half in our reality and half in another, phasing in and out at will";
		}else if(this.type=="mutant"){ //MUTANT AND TOXIC ARE SWITCHED
			displayName = "Mutant";
			nature = "???";
			dominant = true;
			description = "Hideous and deformed";
		}else if(this.type=="mutantMagic"){
			displayName = "Eldritch";
			nature = "Wyrd";
			dominant = true;
			description = "Some things are best left forgotten";
		}else if(this.type=="mutantToxic"){
			displayName = "Radioactive";
			nature = "Sickly";
			dominant = true;
			description = "The physical manifestation of decay";
		}else if(this.type=="nice"){
			displayName = "Nicey";
			nature = "Kind";
			dominant = false;
			description = "This strange species has no ability to sting";
		}else if(this.type=="ocean"){
			displayName = "Oceanic";
			nature = "Soothing";
			dominant = true;
			description = "Hums in waves";
		}else if(this.type=="plains"){
			displayName = "Plain";
			nature = "Simple";
			dominant = false;
			description = "Simple and hardy. Found everywhere despite its timid nature";
		}else if(this.type=="shore"){
			displayName = "Shore";
			nature = "Agitated";
			dominant = true;
			description = "At night it sings out to its brothers, as if stranded or lost";
		}else if(this.type=="stone"){
			displayName = "Stone";
			nature = "Stubborn";
			dominant = true;
			description = "Heavier than a dog, and slower than a snail";
		}else if(this.type=="toxic"){ //TOXIC AND MUTANT ARE SWITCHED NAMEWISE
			displayName = "Toxic";
			nature = "???";
			dominant = true;
			description = "Its flesh drips relentlessly... it is a wonder it is able to hold together at all";
		}else if(this.type=="warrior"){
			displayName = "Warrior";
			nature = "Aggressive";
			dominant = true;
			description = "A quick way to a sore thumb";
		}else if(this.type=="worker"){
			displayName = "Worker";
			nature = "Busy";
			dominant = true;
			description = "A true friend to the flowers";
		}
	}
}
