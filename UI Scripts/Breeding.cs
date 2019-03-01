using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Breeding : MonoBehaviour {

	//TODO: turns
	private List<Recipe> recipes;
	private Bee bees;
	private Dictionary<string, Recipe> beeRecipes;
	private UniquesBackpack backpack;
	public string[] beeResults;
	public int honeyNumberResult;
	private List<string> illegalBees;

	void Start(){
		beeRecipes = new Dictionary<string, Recipe>();
		//add all recipes
		addAllRecipesGross();
		addToMantleList();

		backpack = UniquesBackpack.backpack;
	}


	//have this called from apiaryOrganiser
	public int startBreeding(GameObject beeOne, GameObject beeTwo){
		//get the types of the bees from the slots
		//then based on the rules breed them
		string beeO = beeOne.GetComponentInChildren<Bee>().type;
		string beeT = beeTwo.GetComponentInChildren<Bee>().type;

		string key = beeO + "_" + beeT;
		List<string> keys = new List<string>(beeRecipes.Keys);
		//it stopped initialising here due to the duplicate key: FIXED
		Recipe temp;

		if(beeRecipes.TryGetValue(key, out temp)){
			temp=beeRecipes[key];
			beeResults = breed(temp);
			honeyNumberResult = honeyResult(temp);

			//have to put condition in here: IF it is a valid recipe then check for game ending event, then check for null magic mantle
			if(key=="diligentWorker_exoticWorker"||key=="exoticWorker_diligentWorker"){
				ScenarioManager.CreateEndGameEvent();

			}else if(illegalBees.Contains(beeO)||illegalBees.Contains(beeT)){ //one or both of these is an illegal bee
				if(!backpack.itemTruth["cloak"]){
					ScenarioManager.CreateDisasterousEvent();
				}
			}

			return temp.turns;
		}else{
			return -100;
		}
	}

	public string[] breed(Recipe recipe){

		List<string> results = new List<string>();

		results.Add(recipe.beeOne); //first bee is always the first one that went in

		//if the player has honeycomb it doubles mutation chance
		if(backpack.itemTruth["honeycomb"]){
			recipe.chance = recipe.chance*2;
		}

		//second bee depends on the chance
		if(Random.Range(0,100)<=recipe.chance){
			results.Add(recipe.beeResultOne);
		}else{
			results.Add(recipe.beeOne);
		}

		//third bee depends on half the chance. Much rarer!
		if(Random.Range(0,100)<=(recipe.chance/2)){
			results.Add(recipe.beeResultTwo);
		}else{
			results.Add(recipe.beeOne);
		}

		//honey result is a base. Have a function that multiplies around/adds to this within a range
		return results.ToArray();
	}

	public int honeyResult(Recipe recipe){
		int result = 0;
		result = Random.Range(recipe.honeyResult-5,recipe.honeyResult+5) + 3;
		return result;
	}

	public void addToMantleList(){ //adds all bee names to a list that require null magic mantle so a disasterous event doesn't occur
		illegalBees = new List<string>();

		illegalBees.Add("exotic"); //exotic
		illegalBees.Add("diligent"); //diligent
		illegalBees.Add("intelligent"); //intelligent
		illegalBees.Add("mutant"); //toxic?
		illegalBees.Add("exoticShore"); //void
		illegalBees.Add("diligentWarrior"); //killer
		illegalBees.Add("intelligentCommon"); //genius
		illegalBees.Add("mutantToxic"); //stank
		illegalBees.Add("intelligentNice"); //horny
		illegalBees.Add("mutantMagic"); //eldritch
		illegalBees.Add("diligentWorker"); //black hole
		illegalBees.Add("exoticWorker"); //infinibee
	}

	public void addAllRecipesGross(){
		Recipe recipe;
		
		//BASE HONEY RESULT IS 15
		//TODO: fill out higher level bee recipes (go through one breed at a time)
		//---->>>> suss out names for the breeds so they can be treated properly on the back end
		//add recipes breeding into exotic and mutant bees


		//<---BLAND---> 
		recipe = new Recipe("bland", "common", 70, "common", "plains", 14,2); //one in four chance for the unnatural tree
		beeRecipes.Add("bland_common", recipe);

		recipe = new Recipe("bland", "bland", 70, "common", "common", 13,2);
		beeRecipes.Add("bland_bland", recipe);

		recipe = new Recipe("bland", "plains", 65, "common", "plains", 14,3);
		beeRecipes.Add("bland_plains", recipe);

		recipe = new Recipe("bland", "forest", 55, "forest", "common", 14,3);
		beeRecipes.Add("bland_forest", recipe);

		recipe = new Recipe("bland", "icy", 50, "icy", "common", 13,3);
		beeRecipes.Add("bland_icy", recipe);

		recipe = new Recipe("bland", "stone", 60, "common", "stone", 11,4); 
		beeRecipes.Add("bland_stone", recipe);

		recipe = new Recipe("bland", "worker", 50, "common", "common", 12,3);
		beeRecipes.Add("bland_worker", recipe);

		recipe = new Recipe("bland", "warrior", 60, "common", "worker", 13,3);
		beeRecipes.Add("bland_warrior", recipe);

		recipe = new Recipe("bland", "shore", 60, "common", "common", 14,3);
		beeRecipes.Add("bland_shore", recipe);

		recipe = new Recipe("bland", "nice", 50, "common", "plains", 13,3);
		beeRecipes.Add("bland_nice", recipe);

		recipe = new Recipe("bland", "ocean", 70, "shore", "shore", 15,3);
		beeRecipes.Add("bland_ocean", recipe);

		recipe = new Recipe("bland", "magic", 40, "forest", "shore", 18,3);
		beeRecipes.Add("bland_magic", recipe);

		recipe = new Recipe("bland", "toxic", 50, "toxic", "toxic", 13,3);
		beeRecipes.Add("bland_toxic", recipe);

		//<---COMMON---> breeds into worker
		recipe = new Recipe("common", "common", 72, "common", "worker", 15,2); //one in three for the natural tree
		beeRecipes.Add("common_common", recipe);

		recipe = new Recipe("common", "bland", 70, "common", "plains", 16,2);
		beeRecipes.Add("common_bland", recipe);

		recipe = new Recipe("common", "plains", 60, "plains", "plains", 16,4);
		beeRecipes.Add("common_plains", recipe);

		recipe = new Recipe("common", "forest", 60, "forest", "forest", 18,3);
		beeRecipes.Add("common_forest", recipe);

		recipe = new Recipe("common", "icy", 50, "icy", "plains", 15,4);
		beeRecipes.Add("common_icy", recipe);

		recipe = new Recipe("common", "stone", 30, "stone", "stone", 18,4); 
		beeRecipes.Add("common_stone", recipe);

		recipe = new Recipe("common", "worker", 60, "worker", "worker", 16,3);
		beeRecipes.Add("common_worker", recipe);

		recipe = new Recipe("common", "warrior", 70, "worker", "warrior", 16,3);
		beeRecipes.Add("common_warrior", recipe);

		recipe = new Recipe("common", "shore", 80, "common", "shore", 16,3);
		beeRecipes.Add("common_shore", recipe);

		recipe = new Recipe("common", "nice", 75, "plains", "nice", 17,3);
		beeRecipes.Add("common_nice", recipe);

		recipe = new Recipe("common", "ocean", 75, "ocean", "shore", 15,3);
		beeRecipes.Add("common_ocean", recipe);

		recipe = new Recipe("common", "magic", 50, "forest", "shore", 19,3);
		beeRecipes.Add("common_magic", recipe);

		recipe = new Recipe("common", "toxic", 50, "toxic", "toxic", 18,3);
		beeRecipes.Add("common_toxic", recipe);

		//<---WORKER---> breeds into warrior and intelligent
		recipe = new Recipe("worker", "forest", 65, "forest", "warrior", 23,3);
		beeRecipes.Add("worker_forest", recipe);

		recipe = new Recipe("worker", "ocean", 40, "ocean", "intelligent", 48,5);
		beeRecipes.Add("worker_ocean", recipe);

		recipe = new Recipe("worker", "warrior", 40, "warrior", "diligent", 41,5);
		beeRecipes.Add("worker_warrior", recipe);

		recipe = new Recipe("worker", "bland", 70, "common", "bland", 19,3); 
		beeRecipes.Add("worker_bland", recipe);

		recipe = new Recipe("worker", "common", 70, "common", "common", 21,3);
		beeRecipes.Add("worker_common", recipe);

		recipe = new Recipe("worker", "plains", 80, "common", "bland", 8,4);
		beeRecipes.Add("worker_plains", recipe);

		recipe = new Recipe("worker", "icy", 70, "common", "common", 9,4);
		beeRecipes.Add("worker_icy", recipe);

		recipe = new Recipe("worker", "stone", 60, "common", "stone", 20,5); 
		beeRecipes.Add("worker_stone", recipe);

		recipe = new Recipe("worker", "worker", 15, "worker", "common", 26,4);
		beeRecipes.Add("worker_worker", recipe);

		recipe = new Recipe("worker", "shore", 90, "common", "shore", 22,3);
		beeRecipes.Add("worker_shore", recipe);

		recipe = new Recipe("worker", "nice", 40, "common", "common", 15,3);
		beeRecipes.Add("worker_nice", recipe);

		recipe = new Recipe("worker", "magic", 50, "magic", "warrior", 28,4);
		beeRecipes.Add("worker_magic", recipe);

		recipe = new Recipe("worker", "toxic", 60, "toxic", "toxic", 21,4);
		beeRecipes.Add("worker_toxic", recipe);

		//<---FOREST---> breeds into ocean
		recipe = new Recipe("forest", "shore", 55, "shore", "ocean", 34,4);
		beeRecipes.Add("forest_shore", recipe);

		recipe = new Recipe("forest", "worker", 68, "worker", "warrior", 24,3);
		beeRecipes.Add("forest_worker", recipe);

		recipe = new Recipe("forest", "bland", 60, "common", "bland", 15,3); 
		beeRecipes.Add("forest_bland", recipe);

		recipe = new Recipe("forest", "common", 65, "common", "common", 20,3);
		beeRecipes.Add("forest_common", recipe);

		recipe = new Recipe("forest", "plains", 80, "common", "bland", 11,5);
		beeRecipes.Add("forest_plains", recipe);

		recipe = new Recipe("forest", "forest", 40, "forest", "common", 25,3);
		beeRecipes.Add("forest_forest", recipe);

		recipe = new Recipe("forest", "icy", 50, "icy", "common", 18,4);
		beeRecipes.Add("forest_icy", recipe);

		recipe = new Recipe("forest", "stone", 75, "stone", "stone", 22,4); 
		beeRecipes.Add("forest_stone", recipe);

		recipe = new Recipe("forest", "warrior", 60, "worker", "warrior", 20,4);
		beeRecipes.Add("forest_warrior", recipe);

		recipe = new Recipe("forest", "nice", 10, "nice", "intelligent", 26,3);
		beeRecipes.Add("forest_nice", recipe);

		recipe = new Recipe("forest", "ocean", 80, "forest", "shore", 25,3);
		beeRecipes.Add("forest_ocean", recipe);

		recipe = new Recipe("forest", "magic", 60, "magic", "magic", 35,4);
		beeRecipes.Add("forest_magic", recipe);

		recipe = new Recipe("forest", "toxic", 70, "toxic", "toxic", 23,4);
		beeRecipes.Add("forest_toxic", recipe);
		
		//<---SHORE---> breeds into ocean 
		recipe = new Recipe("shore", "forest", 58, "shore", "ocean", 33,3);
		beeRecipes.Add("shore_forest", recipe);

		recipe = new Recipe("shore", "bland", 50, "common", "bland", 17,3);
		beeRecipes.Add("shore_bland", recipe);

		recipe = new Recipe("shore", "common", 65, "common", "common", 19,3);
		beeRecipes.Add("shore_common", recipe);

		recipe = new Recipe("shore", "plains", 80, "common", "bland", 11,4);
		beeRecipes.Add("shore_plains", recipe);

		recipe = new Recipe("shore", "icy", 50, "icy", "common", 17,4);
		beeRecipes.Add("shore_icy", recipe);

		recipe = new Recipe("shore", "stone", 60, "stone", "stone", 22,5); 
		beeRecipes.Add("shore_stone", recipe);

		recipe = new Recipe("shore", "worker", 60, "worker", "common", 25,4);
		beeRecipes.Add("shore_worker", recipe);

		recipe = new Recipe("shore", "warrior", 60, "worker", "forest", 22,4);
		beeRecipes.Add("shore_warrior", recipe);

		recipe = new Recipe("shore", "shore", 10, "shore", "ocean", 26,3);
		beeRecipes.Add("shore_shore", recipe);

		recipe = new Recipe("shore", "nice", 20, "nice", "ocean", 24,3);
		beeRecipes.Add("shore_nice", recipe);

		recipe = new Recipe("shore", "ocean", 90, "ocean", "ocean", 25,3);
		beeRecipes.Add("shore_ocean", recipe);

		recipe = new Recipe("shore", "magic", 60, "magic", "ocean", 34,4);
		beeRecipes.Add("shore_magic", recipe);

		recipe = new Recipe("shore", "toxic", 80, "toxic", "toxic", 24,4);
		beeRecipes.Add("shore_toxic", recipe);

		//<---OCEAN---> breeds into intelligent 
		recipe = new Recipe("ocean", "worker", 40, "ocean", "intelligent", 45,5);
		beeRecipes.Add("ocean_worker", recipe);

		recipe = new Recipe("ocean", "bland", 75, "bland", "shore", 19,3); 
		beeRecipes.Add("ocean_bland", recipe);

		recipe = new Recipe("ocean", "common", 65, "forest", "shore", 28,3);
		beeRecipes.Add("ocean_common", recipe);

		recipe = new Recipe("ocean", "plains", 80, "common", "bland", 15,5);
		beeRecipes.Add("ocean_plains", recipe);

		recipe = new Recipe("ocean", "forest", 65, "forest", "forest", 24,4);
		beeRecipes.Add("ocean_forest", recipe);

		recipe = new Recipe("ocean", "icy", 80, "bland", "bland", 13,5);
		beeRecipes.Add("ocean_icy", recipe);

		recipe = new Recipe("ocean", "stone", 45, "icy", "bland", 22,6); 
		beeRecipes.Add("ocean_stone", recipe);

		recipe = new Recipe("ocean", "warrior", 50, "warrior", "warrior", 23,4);
		beeRecipes.Add("ocean_warrior", recipe);

		recipe = new Recipe("ocean", "shore", 55, "shore", "forest", 26,4);
		beeRecipes.Add("ocean_shore", recipe);

		recipe = new Recipe("ocean", "nice", 30, "nice", "nice", 29,4);
		beeRecipes.Add("ocean_nice", recipe);

		recipe = new Recipe("ocean", "ocean", 2, "shore", "forest", 30,3);
		beeRecipes.Add("ocean_ocean", recipe);

		recipe = new Recipe("ocean", "magic", 30, "magic", "mutant", 34,4);
		beeRecipes.Add("ocean_magic", recipe);

		recipe = new Recipe("ocean", "toxic", 80, "toxic", "toxic", 28,4);
		beeRecipes.Add("ocean_toxic", recipe);

		//<---WARRIOR---> breeds into diligent
		recipe = new Recipe("warrior", "worker", 40, "worker", "diligent", 47,5);
		beeRecipes.Add("warrior_worker", recipe);

		recipe = new Recipe("warrior", "bland", 50, "worker", "bland", 14,3); 
		beeRecipes.Add("warrior_bland", recipe);

		recipe = new Recipe("warrior", "common", 50, "forest", "worker", 28,3);
		beeRecipes.Add("warrior_common", recipe);

		recipe = new Recipe("warrior", "plains", 90, "common", "bland", 4,5);
		beeRecipes.Add("warrior_plains", recipe);

		recipe = new Recipe("warrior", "forest", 50, "forest", "worker", 26,4);
		beeRecipes.Add("warrior_forest", recipe);

		recipe = new Recipe("warrior", "icy", 98, "bland", "bland", 4,6);
		beeRecipes.Add("warrior_icy", recipe);

		recipe = new Recipe("warrior", "stone", 80, "worker", "bland", 19,6); 
		beeRecipes.Add("warrior_stone", recipe);

		recipe = new Recipe("warrior", "warrior", 50, "warrior", "warrior",32,4);
		beeRecipes.Add("warrior_warrior", recipe);

		recipe = new Recipe("warrior", "shore", 20, "shore", "shore", 25,4);
		beeRecipes.Add("warrior_shore", recipe);

		recipe = new Recipe("warrior", "nice", 100, "bland", "bland", 4,8);
		beeRecipes.Add("warrior_nice", recipe);

		recipe = new Recipe("warrior", "ocean", 74, "worker", "common", 25,4);
		beeRecipes.Add("warrior_ocean", recipe);

		recipe = new Recipe("warrior", "magic", 100, "nice", "nice", 40,4);
		beeRecipes.Add("warrior_magic", recipe);

		recipe = new Recipe("warrior", "toxic", 80, "toxic", "toxic", 27,4);
		beeRecipes.Add("warrior_toxic", recipe);

		//<<<<----------UNNATURAL TREE----------->>>>

		//<---PLAINS---> breeds into nice
		recipe = new Recipe("plains", "icy", 70, "icy", "nice", 25,4);
		beeRecipes.Add("plains_icy", recipe);

		recipe = new Recipe("plains", "bland", 90, "common", "bland", 11,2);
		beeRecipes.Add("plains_bland", recipe);

		recipe = new Recipe("plains", "common", 50, "common", "bland", 19,3);
		beeRecipes.Add("plains_common", recipe);

		recipe = new Recipe("plains", "plains", 20, "common", "common", 18,2);
		beeRecipes.Add("plains_plains", recipe);

		recipe = new Recipe("plains", "forest", 70, "common", "forest", 18,3);
		beeRecipes.Add("plains_forest", recipe);

		recipe = new Recipe("plains", "stone", 60, "common", "stone", 22,4); 
		beeRecipes.Add("plains_stone", recipe);

		recipe = new Recipe("plains", "worker", 90, "common", "common", 20,3);
		beeRecipes.Add("plains_worker", recipe);

		recipe = new Recipe("plains", "warrior", 90, "common", "common", 12,3);
		beeRecipes.Add("plains_warrior", recipe);

		recipe = new Recipe("plains", "shore", 70, "shore", "common", 19,3);
		beeRecipes.Add("plains_shore", recipe);

		recipe = new Recipe("plains", "nice", 80, "nice", "worker", 24,3);
		beeRecipes.Add("plains_nice", recipe);

		recipe = new Recipe("plains", "ocean", 60, "shore", "shore", 18,3);
		beeRecipes.Add("plains_ocean", recipe);

		recipe = new Recipe("plains", "magic", 50, "icy", "metallic", 26,4);
		beeRecipes.Add("plains_magic", recipe);

		recipe = new Recipe("plains", "toxic", 80, "toxic", "toxic", 22,4);
		beeRecipes.Add("plains_toxic", recipe);

		//<<---ICY---> breeds into nice
		recipe = new Recipe("icy", "plains", 70, "plains", "nice", 24,4);
		beeRecipes.Add("icy_plains", recipe);

		recipe = new Recipe("icy", "bland", 80, "common", "bland", 12,3);
		beeRecipes.Add("icy_bland", recipe);

		recipe = new Recipe("icy", "common", 55, "plain", "common", 21,3);
		beeRecipes.Add("icy_common", recipe);

		recipe = new Recipe("icy", "forest", 70, "forest", "forest", 17,4);
		beeRecipes.Add("icy_forest", recipe);

		recipe = new Recipe("icy", "icy", 50, "icy", "nice", 39,4);
		beeRecipes.Add("icy_icy", recipe);

		recipe = new Recipe("icy", "stone", 60, "stone", "nice", 22,5); 
		beeRecipes.Add("icy_stone", recipe);

		recipe = new Recipe("icy", "worker", 80, "common", "plains", 21,4);
		beeRecipes.Add("icy_worker", recipe);

		recipe = new Recipe("icy", "warrior", 90, "bland", "bland", 8,5);
		beeRecipes.Add("icy_warrior", recipe);

		recipe = new Recipe("icy", "shore", 10, "shore", "ocean", 20,4);
		beeRecipes.Add("icy_shore", recipe);

		recipe = new Recipe("icy", "nice", 70, "nice", "nice", 24,3);
		beeRecipes.Add("icy_nice", recipe);

		recipe = new Recipe("icy", "ocean", 90, "shore", "shore", 20,4);
		beeRecipes.Add("icy_ocean", recipe);

		recipe = new Recipe("icy", "toxic", 80, "toxic", "toxic", 32,4);
		beeRecipes.Add("icy_toxic", recipe);

		recipe = new Recipe("icy", "magic", 75, "worker", "magic", 40,4);
		beeRecipes.Add("icy_magic", recipe);

		//<<---NICE---> breeds into mutant(toxic) and magic
		recipe = new Recipe("nice", "stone", 50, "stone", "toxic", 28,5);
		beeRecipes.Add("nice_stone", recipe);

		recipe = new Recipe("nice", "toxic", 30, "magic", "magic", 28,5);
		beeRecipes.Add("nice_toxic", recipe);

		recipe = new Recipe("nice", "bland", 90, "icy", "plain", 14,3);
		beeRecipes.Add("nice_bland", recipe);

		recipe = new Recipe("nice", "common", 55, "icy", "common", 24,3);
		beeRecipes.Add("nice_common", recipe);

		recipe = new Recipe("nice", "plains", 60, "icy", "plains", 22,4);
		beeRecipes.Add("nice_plains", recipe);

		recipe = new Recipe("nice", "forest", 60, "forest", "icy", 20,4);
		beeRecipes.Add("nice_forest", recipe);

		recipe = new Recipe("nice", "icy", 40, "icy", "icy", 27,3);
		beeRecipes.Add("nice_icy", recipe);

		recipe = new Recipe("nice", "worker", 70, "worker", "worker", 23,4);
		beeRecipes.Add("nice_worker", recipe);

		recipe = new Recipe("nice", "warrior", 90, "bland", "bland", 11,5);
		beeRecipes.Add("nice_warrior", recipe);

		recipe = new Recipe("nice", "shore", 70, "shore", "plains", 21,4);
		beeRecipes.Add("nice_shore", recipe);

		recipe = new Recipe("nice", "nice", 30, "forest", "forest", 25,4);
		beeRecipes.Add("nice_nice", recipe);

		recipe = new Recipe("nice", "ocean", 82, "ocean", "ocean", 21,4);
		beeRecipes.Add("nice_ocean", recipe);

		recipe = new Recipe("nice", "magic", 100, "warrior", "warrior", 40,4);
		beeRecipes.Add("nice_magic", recipe);

		//<<---STONE---> breeds into mutant(toxic) and toxic(mutant)
		recipe = new Recipe("stone", "nice", 50, "nice", "toxic", 28,5);
		beeRecipes.Add("stone_nice", recipe);

		recipe = new Recipe("stone", "toxic", 30, "toxic", "mutant", 31,6); //MIGHT BE WRONG TOXIC/MUTANT
		beeRecipes.Add("stone_toxic", recipe);

		recipe = new Recipe("stone", "bland", 80, "common", "bland", 12,3);
		beeRecipes.Add("stone_bland", recipe);

		recipe = new Recipe("stone", "common", 50, "common", "common", 28,3);
		beeRecipes.Add("stone_common", recipe);

		recipe = new Recipe("stone", "plains", 75, "common", "common", 22,4);
		beeRecipes.Add("stone_plains", recipe);

		recipe = new Recipe("stone", "forest", 60, "plain", "forest", 21,4);
		beeRecipes.Add("stone_forest", recipe);

		recipe = new Recipe("stone", "icy", 70, "icy", "nice", 26,4);
		beeRecipes.Add("stone_icy", recipe);

		recipe = new Recipe("stone", "stone", 100, "stone", "stone", 65,7); 
		beeRecipes.Add("stone_stone", recipe);

		recipe = new Recipe("stone", "worker", 95, "common", "common", 54,6);
		beeRecipes.Add("stone_worker", recipe);

		recipe = new Recipe("stone", "warrior", 60, "common", "common", 13,3);
		beeRecipes.Add("stone_warrior", recipe);

		recipe = new Recipe("stone", "shore", 70, "bland", "common", 24,4);
		beeRecipes.Add("stone_shore", recipe);

		recipe = new Recipe("stone", "ocean", 80, "icy", "ocean", 26,5);
		beeRecipes.Add("stone_ocean", recipe);

		recipe = new Recipe("stone", "magic", 75, "magic", "ocean", 38,5);
		beeRecipes.Add("stone_magic", recipe);

		//<<---MUTANT---> breeds into magic, strange (exotic) and mutant(toxic)
		recipe = new Recipe("toxic", "nice", 30, "magic", "magic", 28,5);
		beeRecipes.Add("toxic_nice", recipe);

		recipe = new Recipe("toxic", "magic", 30, "magic", "exotic", 38,6);
		beeRecipes.Add("toxic_magic", recipe);

		recipe = new Recipe("toxic", "stone", 36, "stone", "mutant", 30,6); //MIGHT BE WRONG TOXIC/MUTANT
		beeRecipes.Add("toxic_stone", recipe);

		recipe = new Recipe("toxic", "bland", 40, "stone", "mutant", 20,4);
		beeRecipes.Add("toxic_bland", recipe);

		recipe = new Recipe("toxic", "common", 55, "nice", "common", 29,3);
		beeRecipes.Add("toxic_common", recipe);

		recipe = new Recipe("toxic", "plains", 65, "nice", "nice", 24,5);
		beeRecipes.Add("toxic_plains", recipe);

		recipe = new Recipe("toxic", "forest", 80, "bland", "bland", 16,5);
		beeRecipes.Add("toxic_forest", recipe);

		recipe = new Recipe("toxic", "icy", 80, "bland", "plains", 33,4);
		beeRecipes.Add("toxic_icy", recipe);

		recipe = new Recipe("toxic", "worker", 80, "common", "bland", 29,5);
		beeRecipes.Add("toxic_worker", recipe);

		recipe = new Recipe("toxic", "warrior", 70, "worker", "worker", 26,4);
		beeRecipes.Add("toxic_warrior", recipe);

		recipe = new Recipe("toxic", "shore", 40, "shore", "forest", 28,4);
		beeRecipes.Add("toxic_shore", recipe);

		recipe = new Recipe("toxic", "ocean", 80, "bland", "bland", 23,5);
		beeRecipes.Add("toxic_ocean", recipe);

		recipe = new Recipe("toxic", "toxic", 44, "toxic", "mutant", 28,5);
		beeRecipes.Add("toxic_toxic", recipe);

		//<<---MAGIC---> breeds into mutant(toxic) and toxic(mutant)
		recipe = new Recipe("magic", "toxic", 30, "toxic", "exotic", 42,6);
		beeRecipes.Add("magic_toxic", recipe);

		recipe = new Recipe("magic", "magic", 20, "exotic", "exotic", 50,6);
		beeRecipes.Add("magic_magic", recipe);

		recipe = new Recipe("magic", "bland", 32, "toxic", "exotic", 22,5); 
		beeRecipes.Add("magic_bland", recipe);

		recipe = new Recipe("magic", "common", 55, "nice", "common", 45,4);
		beeRecipes.Add("magic_common", recipe);

		recipe = new Recipe("magic", "plains", 60, "nice", "nice", 44,5);
		beeRecipes.Add("magic_plains", recipe);

		recipe = new Recipe("magic", "forest", 70, "worker", "ocean", 34,5);
		beeRecipes.Add("magic_forest", recipe);

		recipe = new Recipe("magic", "icy", 70, "nice", "icy", 48,5);
		beeRecipes.Add("magic_icy", recipe);

		recipe = new Recipe("magic", "stone", 65, "stone", "mutant", 49,6); 
		beeRecipes.Add("magic_stone", recipe);

		recipe = new Recipe("magic", "worker", 50, "worker", "warrior", 48,7);
		beeRecipes.Add("magic_worker", recipe);

		recipe = new Recipe("magic", "warrior", 70, "forest", "warrior", 48,5);
		beeRecipes.Add("magic_warrior", recipe);

		recipe = new Recipe("magic", "shore", 55, "forest", "ocean", 47,5);
		beeRecipes.Add("magic_shore", recipe);

		recipe = new Recipe("magic", "nice", 75, "forest", "shore", 43,4);
		beeRecipes.Add("magic_nice", recipe);

		recipe = new Recipe("magic", "ocean", 80, "forest", "ocean", 46,5);
		beeRecipes.Add("magic_ocean", recipe);


		//<<<<<<<<--------------------------- HIGH LEVEL BEES --------------------------->>>>>>>>//
		//intelligent, diligent, exotic (Strange), mutant //base level of honey is 50
		//diligentWarrior (Killer Bee), diligentWorker (Black Hole)
		//intelligentCommon (Genius) , intelligentNice (horny bee - create new texture)
		//exoticShore, exoticWorker (The Infinibee)
		//mutantMagic, mutantToxic
		//Tonbee HawK?
		//BREEDING: infinibee with black hole will end the game -> give dimensional rip

		//4 devestating events can occur if bees are bred without protection: other bees start dying - shop keeper is killed - food and water become 0
		//SHIT GETS WHACK: VERY HIGH MUTATION CHANCE (keep in mind players probably have honeycomb)

		//horny bee starts fucking the other bees in the hive to death


		//<<---INTELLIGENT--->
		recipe = new Recipe("intelligent", "intelligent", 100, "intelligent", "intelligent", 55,3);
		beeRecipes.Add("intelligent_intelligent", recipe);

		recipe = new Recipe("intelligent", "diligent", 35, "intelligentCommon", "diligentWarrior", 65,3); //genius, killer
		beeRecipes.Add("intelligent_diligent", recipe);

		recipe = new Recipe("intelligent", "exotic", 35, "diligentWarrior", "exoticShore", 78,3); //killer, void
		beeRecipes.Add("intelligent_exotic", recipe);

		recipe = new Recipe("intelligent", "mutant", 35, "intelligentCommon", "mutantToxic", 40,4); //genius, stank
		beeRecipes.Add("intelligent_mutant", recipe);

		recipe = new Recipe("intelligent", "intelligentCommon", 100, "intelligentCommon", "intelligentCommon", 71,3);
		beeRecipes.Add("intelligent_intelligentCommon", recipe);

		recipe = new Recipe("intelligent", "mutantToxic", 70, "intelligentNice", "mutant", 46,4);
		beeRecipes.Add("intelligent_mutantToxic", recipe);

		recipe = new Recipe("intelligent", "diligentWarrior", 70, "intelligentNice", "diligent", 62,4);
		beeRecipes.Add("intelligent_diligentWarrior", recipe);

		recipe = new Recipe("intelligent", "exoticShore", 70, "intelligentNice", "exotic", 75,4); 
		beeRecipes.Add("intelligent_exoticShore", recipe);

		recipe = new Recipe("intelligent", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligent_intelligentNice", recipe);

		recipe = new Recipe("intelligent", "mutantMagic", 40, "intelligentNice", "intelligentCommon", 53, 5);
		beeRecipes.Add("intelligent_mutantMagic", recipe);

		//<<---DILIGENT--->
		recipe = new Recipe("diligent", "diligent", 100, "diligent", "diligent", 60,3);
		beeRecipes.Add("diligent_diligent", recipe);

		recipe = new Recipe("diligent", "intelligent", 35, "intelligentCommon", "intelligentCommon", 68,3); //genius, genius
		beeRecipes.Add("diligent_intelligent", recipe);

		recipe = new Recipe("diligent", "exotic", 35, "exoticShore", "diligentWarrior", 80,3); //void, killer
		beeRecipes.Add("diligent_exotic", recipe);

		recipe = new Recipe("diligent", "mutant", 35, "diligentWarrior", "mutantToxic", 42,4); //killer, stank
		beeRecipes.Add("diligent_mutant", recipe);

		recipe = new Recipe("diligent", "diligentWarrior", 100, "diligentWarrior", "diligentWarrior", 53,3);
		beeRecipes.Add("diligent_diligentWarrior", recipe);

		recipe = new Recipe("diligent", "intelligentCommon", 70, "intelligentNice", "intelligent", 62,4);
		beeRecipes.Add("diligent_intelligentCommon", recipe);

		recipe = new Recipe("diligent", "mutantToxic", 70, "intelligentNice", "mutant", 42,4);
		beeRecipes.Add("diligent_mutantToxic", recipe);

		recipe = new Recipe("diligent", "exoticShore", 70, "intelligentNice", "exotic", 73,4);
		beeRecipes.Add("diligent_exoticShore", recipe);

		recipe = new Recipe("diligent", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("diligent_intelligentNice", recipe);

		recipe = new Recipe("diligent", "mutantMagic", 40, "mutantMagic", "exoticShore", 51, 5);
		beeRecipes.Add("diligent_mutantMagic", recipe);

		//<<---EXOTIC--->
		recipe = new Recipe("exotic", "exotic", 100, "exotic", "exotic", 50,3);
		beeRecipes.Add("exotic_exotic", recipe);

		recipe = new Recipe("exotic", "intelligent", 35, "exoticShore", "intelligentCommon", 77,3); //void, genius
		beeRecipes.Add("exotic_intelligent", recipe);

		recipe = new Recipe("exotic", "diligent", 35, "exoticShore", "diligentWarrior", 78,3); //void, warrior
		beeRecipes.Add("exotic_diligent", recipe);

		recipe = new Recipe("exotic", "mutant", 35, "mutantToxic", "exoticShore", 69,4); //stank, void
		beeRecipes.Add("exotic_mutant", recipe);

		recipe = new Recipe("exotic", "exoticShore", 100, "exoticShore", "exoticShore", 72,3);
		beeRecipes.Add("exotic_exoticShore", recipe);

		recipe = new Recipe("exotic", "intelligentCommon", 70, "intelligentNice", "intelligent", 78,4); //eldritch, horny
		beeRecipes.Add("exotic_intelligentCommon", recipe);

		recipe = new Recipe("exotic", "mutantToxic", 70, "intelligentNice", "mutant", 78,4); //eldritch, horny
		beeRecipes.Add("exotic_mutantToxic", recipe);

		recipe = new Recipe("exotic", "diligentWarrior", 70, "intelligentNice", "diligent", 78,4);//horny, eldritch
		beeRecipes.Add("exotic_diligentWarrior", recipe);

		recipe = new Recipe("exotic", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("exotic_intelligentNice", recipe);

		recipe = new Recipe("exotic", "mutantMagic", 70, "exoticShore", "mutantToxic", 48, 5);
		beeRecipes.Add("exotic_mutantMagic", recipe);

		//<<---MUTANT--->
		recipe = new Recipe("mutant", "mutant", 100, "mutant", "mutant", 50,3);
		beeRecipes.Add("mutant_mutant", recipe);

		recipe = new Recipe("mutant", "intelligent", 35, "intelligentCommon", "diligentWarrior", 41,3); //genius, killer
		beeRecipes.Add("mutant_intelligent", recipe);

		recipe = new Recipe("mutant", "diligent", 35, "diligentWarrior", "intelligentCommon", 42,3); //warrior, genius
		beeRecipes.Add("mutant_diligent", recipe);

		recipe = new Recipe("mutant", "exotic", 35, "mutantToxic", "exoticShore", 68,3); //stank, void
		beeRecipes.Add("mutant_exotic", recipe);
		//t2
		recipe = new Recipe("mutant", "mutantToxic", 100, "mutantToxic", "mutantToxic", 18,3);
		beeRecipes.Add("mutant_mutantToxic", recipe);

		recipe = new Recipe("mutant", "intelligentCommon", 70, "intelligentNice", "intelligent", 41,4);
		beeRecipes.Add("mutant_intelligentCommon", recipe);

		recipe = new Recipe("mutant", "diligentWarrior", 70, "intelligentNice", "diligent", 42,4);
		beeRecipes.Add("mutant_diligentWarrior", recipe);

		recipe = new Recipe("mutant", "exoticShore", 70, "intelligentNice", "exotic", 52,4);
		beeRecipes.Add("mutant_exoticShore", recipe);
		//t3
		recipe = new Recipe("mutant", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("mutant_intelligentNice", recipe);

		recipe = new Recipe("mutant", "mutantMagic", 70, "mutantMagic", "mutantMagic", 48, 4);
		beeRecipes.Add("mutant_mutantMagic", recipe);

		//<<---INTELLIGENT COMMON---> (Genius)
		recipe = new Recipe("intelligentCommon", "intelligentCommon", 100, "intelligentCommon", "intelligentCommon", 75,3);
		beeRecipes.Add("intelligentCommon_intelligentCommon", recipe);

		recipe = new Recipe("intelligentCommon", "mutantToxic", 200, "intelligentNice", "intelligentNice", 48,4);
		beeRecipes.Add("intelligentCommon_mutantToxic", recipe);

		recipe = new Recipe("intelligentCommon", "diligentWarrior", 200, "intelligentNice", "intelligentNice", 64,4);
		beeRecipes.Add("intelligentCommon_diligentWarrior", recipe);

		recipe = new Recipe("intelligentCommon", "exoticShore", 20, "intelligentNice", "mutantMagic", 75,4);
		beeRecipes.Add("intelligentCommon_exoticShore", recipe);

		recipe = new Recipe("intelligentCommon", "exotic", 35, "exotic", "exoticShore", 74,4);
		beeRecipes.Add("intelligentCommon_exotic", recipe);

		recipe = new Recipe("intelligentCommon", "mutant", 35, "mutant", "mutantToxic", 42,4);
		beeRecipes.Add("intelligentCommon_mutant", recipe);

		recipe = new Recipe("intelligentCommon", "diligent", 35, "diligent", "diligentWarrior", 69,4);
		beeRecipes.Add("intelligentCommon_diligent", recipe);

		recipe = new Recipe("intelligentCommon", "intelligent", 200, "intelligentNice", "intelligentNice", 72,4);
		beeRecipes.Add("intelligentCommon_intelligent", recipe);

		recipe = new Recipe("intelligentCommon", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentCommon_intelligentNice", recipe);

		recipe = new Recipe("intelligentCommon", "mutantMagic", 20, "exoticWorker", "mutantMagic", 48, 5);
		beeRecipes.Add("intelligentCommon_mutantMagic", recipe);

		//<<---DILIGENT WARRIOR---> (Killer)
		recipe = new Recipe("diligentWarrior", "diligentWarrior", 100, "diligentWarrior", "diligentWarrior", 55,3);
		beeRecipes.Add("diligentWarrior_diligentWarrior", recipe);

		recipe = new Recipe("diligentWarrior", "intelligentCommon", 200, "intelligentNice", "intelligentNice", 64,3);
		beeRecipes.Add("diligentWarrior_intelligentCommon", recipe);

		recipe = new Recipe("diligentWarrior", "mutantToxic", 200, "intelligentNice", "intelligentNice", 44,4);
		beeRecipes.Add("diligentWarrior_mutantToxic", recipe);

		recipe = new Recipe("diligentWarrior", "exoticShore", 20, "intelligentNice", "mutantMagic", 75,4);
		beeRecipes.Add("diligentWarrior_exoticShore", recipe);

		recipe = new Recipe("diligentWarrior", "exotic", 35, "exotic", "exoticShore", 73,4);
		beeRecipes.Add("diligentWarrior_exotic", recipe);

		recipe = new Recipe("diligentWarrior", "mutant", 35, "mutant", "mutantToxic", 40,4);
		beeRecipes.Add("diligentWarrior_mutant", recipe);

		recipe = new Recipe("diligentWarrior", "diligent", 12, "diligent", "diligent", 64,4);
		beeRecipes.Add("diligentWarrior_diligent", recipe);

		recipe = new Recipe("diligentWarrior", "intelligent", 35, "intelligent", "intelligentCommon", 68,4);
		beeRecipes.Add("diligentWarrior_intelligent", recipe);

		recipe = new Recipe("diligentWarrior", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("diligentWarrior_intelligentNice", recipe);

		recipe = new Recipe("diligentWarrior", "mutantMagic", 15, "diligentWorker", "mutantMagic", 45, 5);
		beeRecipes.Add("diligentWarrior_mutantMagic", recipe);

		//<<---EXOTIC SHORE---> (Void)
		recipe = new Recipe("exoticShore", "exoticShore", 20, "exoticShore", "exoticShore", 80,3);
		beeRecipes.Add("exoticShore_exoticShore", recipe);

		recipe = new Recipe("exoticShore", "intelligentCommon", 20, "mutantMagic", "intelligentNice", 80,4); //eldritch, horny
		beeRecipes.Add("exoticShore_intelligentCommon", recipe);

		recipe = new Recipe("exoticShore", "mutantToxic", 20, "mutantMagic", "intelligentNice", 80,4); //eldritch, horny
		beeRecipes.Add("exoticShore_mutantToxic", recipe);

		recipe = new Recipe("exoticShore", "diligentWarrior", 20, "intelligentNice", "mutantMagic", 80,4);//horny, eldritch
		beeRecipes.Add("exoticShore_diligentWarrior", recipe);

		recipe = new Recipe("exoticShore", "mutant", 35, "mutant", "mutantToxic", 64,4); //breeding backwards gives tier 0 and tier 1 of that bee
		beeRecipes.Add("exoticShore_mutant", recipe);

		recipe = new Recipe("exoticShore", "exotic", 12, "exotic", "mutantMagic", 85,3);
		beeRecipes.Add("exoticShore_exotic", recipe);

		recipe = new Recipe("exoticShore", "intelligent", 35, "intelligent", "intelligentCommon", 79,4);
		beeRecipes.Add("exoticShore_intelligent", recipe);

		recipe = new Recipe("exoticShore", "diligent", 35, "diligent", "diligentWarrior", 78,4);
		beeRecipes.Add("exoticShore_diligent", recipe);

		recipe = new Recipe("exoticShore", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("exoticShore_intelligentNice", recipe);

		recipe = new Recipe("exoticShore", "mutantMagic", 15, "diligentWorker", "mutantMagic", 43, 5);
		beeRecipes.Add("exoticShore_mutantMagic", recipe);

		//<<---DILIGENT WARRIOR---> (Stank)
		recipe = new Recipe("mutantToxic", "mutantToxic", 100, "mutantToxic", "mutantToxic", 20,3);
		beeRecipes.Add("mutantToxic_mutantToxic", recipe);

		recipe = new Recipe("mutantToxic", "intelligentCommon", 200, "intelligentNice", "intelligentNice", 43,4);
		beeRecipes.Add("mutantToxic_intelligentCommon", recipe);

		recipe = new Recipe("mutantToxic", "diligentWarrior", 200, "intelligentNice", "intelligentNice", 44,4);
		beeRecipes.Add("mutantToxic_diligentWarrior", recipe);

		recipe = new Recipe("mutantToxic", "exoticShore", 20, "intelligentNice", "mutantMagic", 55,4);
		beeRecipes.Add("mutantToxic_exoticShore", recipe);

		recipe = new Recipe("mutantToxic", "exotic", 35, "exotic", "exoticShore", 50,4);
		beeRecipes.Add("mutantToxic_exotic", recipe);

		recipe = new Recipe("mutantToxic", "mutant", 12, "mutant", "mutant", 40,4);
		beeRecipes.Add("mutantToxic_mutant", recipe);

		recipe = new Recipe("mutantToxic", "diligent", 35, "diligent", "diligentWarrior", 42,4);
		beeRecipes.Add("mutantToxic_diligent", recipe);

		recipe = new Recipe("mutantToxic", "intelligent", 35, "intelligent", "intelligentCommon", 41,4);
		beeRecipes.Add("mutantToxic_intelligent", recipe);

		recipe = new Recipe("mutantToxic", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("mutantToxic_intelligentNice", recipe);

		recipe = new Recipe("mutantToxic", "mutantMagic", 70, "mutantMagic", "exoticShore", 45, 5);
		beeRecipes.Add("mutantToxic_mutantMagic", recipe);

		//<<---INTELLIGENT NICE---> (Horny Bee)
		recipe = new Recipe("intelligentNice", "intelligentNice", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_intelligentNice", recipe);

		recipe = new Recipe("intelligentNice", "diligent", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_diligent", recipe);

		recipe = new Recipe("intelligentNice", "intelligent", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_intelligent", recipe);

		recipe = new Recipe("intelligentNice", "exotic", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_exotic", recipe);

		recipe = new Recipe("intelligentNice", "mutant", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_mutant", recipe);

		recipe = new Recipe("intelligentNice", "intelligentCommon", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_intelligentCommon", recipe);

		recipe = new Recipe("intelligentNice", "diligentWarrior", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_diligentWarrior", recipe);

		recipe = new Recipe("intelligentNice", "exoticShore", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_exoticShore", recipe);

		recipe = new Recipe("intelligentNice", "mutantToxic", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_mutantToxic", recipe);

		recipe = new Recipe("intelligentNice", "mutantMagic", 200, "intelligentNice", "intelligentNice", 40, 3);
		beeRecipes.Add("intelligentNice_mutantMagic", recipe);

		//<<---MUTANT MAGIC---> (Eldritch)
		recipe = new Recipe("mutantMagic", "diligent", 65, "diligent", "mutantToxic", 41, 4); //MAYBE: redo these next 4, might not be the best recipe bindings
		beeRecipes.Add("mutantMagic_diligent", recipe);

		recipe = new Recipe("mutantMagic", "intelligent", 65, "intelligent", "mutantToxic", 40, 4);
		beeRecipes.Add("mutantMagic_intelligent", recipe);

		recipe = new Recipe("mutantMagic", "exotic", 65, "exotic", "mutantToxic", 42, 4);
		beeRecipes.Add("mutantMagic_exotic", recipe);

		recipe = new Recipe("mutantMagic", "mutant", 65, "mutantToxic", "mutantToxic", 38, 4);
		beeRecipes.Add("mutantMagic_mutant", recipe);

		recipe = new Recipe("mutantMagic", "diligentWarrior", 15, "diligentWorker", "diligentWorker", 53, 5); //black hole (void/killer)
		beeRecipes.Add("mutantMagic_diligentWarrior", recipe);

		recipe = new Recipe("mutantMagic", "intelligentCommon", 20, "exoticWorker", "exoticWorker", 54, 5); //infinibee (genius/eldritch)
		beeRecipes.Add("mutantMagic_intelligentCommon", recipe);

		recipe = new Recipe("mutantMagic", "exoticShore", 15, "diligentWorker", "diligentWorker", 60, 5); //black hole (void/eldritch)
		beeRecipes.Add("mutantMagic_exoticShore", recipe);

		recipe = new Recipe("mutantMagic", "mutantToxic", 40, "mutantToxic", "exoticShore", 48, 5);
		beeRecipes.Add("mutantMagic_mutantToxic", recipe);

		recipe = new Recipe("mutantMagic", "intelligentNice", 200, "intelligentNice", "intelligentNice", 45, 4);
		beeRecipes.Add("mutantMagic_intelligentNice", recipe);

		recipe = new Recipe("mutantMagic", "mutantMagic", 100, "mutantMagic", "mutantToxic", 50, 5);
		beeRecipes.Add("mutantMagic_mutantMagic", recipe);

		//DISCLAIMER: At the moment these two only breed with each other, and nothing else (at the moment)

		//<<---DILIGENT WORKER---> (Black Hole)
		recipe = new Recipe("diligentWorker", "exoticWorker", 1000, "bland", "bland", 10000, 999999999);
		beeRecipes.Add("diligentWorker_exoticWorker", recipe);

		//<<---EXOTIC WORKER---> (Infinibee)
		recipe = new Recipe("exoticWorker", "diligentWorker", 1000, "bland", "bland", 10000, 999999999);
		beeRecipes.Add("exoticWorker_diligentWorker", recipe); 
		
	}	
}
