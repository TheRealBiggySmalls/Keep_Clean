using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioClip deathSound, buySound, buttonClickSound, footsteps, invOpen, invClose, storyFinish;
	private bool walking;
	public AudioClip[] weirdEvents, npcArriveSound;
	private AudioSource source; public static AudioManager audioManager;
	void Start () {
		source = GetComponent<AudioSource>();
		audioManager = this;
		walking=false;
	}

	//play footsteps when an event comes up from moving tiles (repeat it four times)
	//have a sound for picking the right option in a scenario
	//can have heaps of scenario sounds if we like
	int count=0;
	void Update(){

		if(walking&&count<3){
			if(!source.isPlaying){
				source.PlayOneShot(footsteps,2.5f);
				count+=1;
			}
		}else{
			walking=false;
			count=0;
			source.volume=1.0f;
		}
	}

	
	public void playSound(string key){
		if(key=="death"){ //TODO
			source.PlayOneShot(deathSound);
		}else if(key=="npc"){ //TODO
			int rand = (int)Random.Range(0,npcArriveSound.Length-0.01f);
			source.PlayOneShot(npcArriveSound[rand]);
		}else if(key=="buy"){
			source.PlayOneShot(buySound);
		}else if(key=="button"){
			source.PlayOneShot(buttonClickSound);
		}else if(key=="invopen"){
			source.PlayOneShot(invOpen);
		}else if(key=="invclose"){
			source.PlayOneShot(invClose);
		}else if(key=="footsteps"){
			walking=true; count=0;
		}else if(key=="story"){ //DONE
			source.PlayOneShot(storyFinish);
		}else if(key=="weird"){
			int rand = (int)Random.Range(0,weirdEvents.Length-0.01f);
			source.PlayOneShot(weirdEvents[rand]);
		}
	}
	
}
