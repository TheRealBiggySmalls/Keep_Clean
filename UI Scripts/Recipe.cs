using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe {
		public string beeOne, beeTwo;

		public int chance; //out of 100 always

		public string beeResultOne;
		public string beeResultTwo;
		public int honeyResult;
		public string honeyCombResult;

		public int turns;

		public Recipe(string one, string two, int chance, string beeResultOne, string beeResultTwo, int honeyResult, int turns, string honeyCombResult="none"){
			this.beeOne = one; 
			this.beeTwo = two;
			this.chance = chance;
			this.beeResultOne = beeResultOne;
			this.beeResultTwo = beeResultTwo;
			this.honeyResult = honeyResult;
			this.honeyCombResult = honeyCombResult;
			this.turns = turns;
		}

	}
