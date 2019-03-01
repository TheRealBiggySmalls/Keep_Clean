using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Continent : Map {

	override public void GenerateMap(){

		//first generate a map of base water hexes
		base.GenerateMap();
		//base.GenerateStaticMap(); //Will generate the same hard coded map every time

		//then generate some kind of raised area
		GenerateContinents();

		//Add lumpiness Perlin Noise?
		GenerateNoise();

		//Set mesh to mountain/hill/flat/water based on height

		// Simulate rainfall/moisture and PROBABLY JUST perlin for now) set plains/grasslands/forest
		GenerateMoisture();

		//Now make sure all hex visuals are update to match elevation
		UpdateHexVisuals();
	}

	//CAN hardcode a map here for each tile knowing exactly what tile type it should be

	void ElevateArea(int q, int r, int range, float centreHeight = 0.5f){
		
		Hex centreHex = GetHexAt(q,r);
		Hex[] areaHexes = GetHexesWithinRangeOf(centreHex, range);

		//have a descending height from the centre down to the coast
		foreach(Hex h in areaHexes){
			/*if(h.Elevation < 0){
				h.Elevation = 0;
			}*/
			h.Elevation = centreHeight * Mathf.Lerp(1f,0.25f,Mathf.Pow(Hex.Distance(centreHex, h)/range,2f));
		}
	}

	private void GenerateContinents(){

		/*int numContinents = 2;
		int continentSpacing = numCols/numContinents;

		//for sharing seeds
		//Random.InitState(2);

		for(int c=0;c<numContinents;c++){
			//COPY CODE BELOW INTO HERE FOR MULTIPLE RANDOM GEN IF DESIRED
		}*/

		Random.InitState(33);
		//can easily randomise these if required (requires MAX parameters)
		int numSplats = Random.Range(5,10);
		for(int i=0;i<numSplats;i++){
			int range = Random.Range(3,8);
			int y = Random.Range(range, numRows-range);
			int x = Random.Range(0,12) - y/2 + 24;

			ElevateArea(x,y,range);
		}

		ElevateArea(19, 11, 7);
		//SECRET ISLAND: 	- hard code an elevate after randomness
		//					- OR just randomise twice????
		ElevateArea(7, 6, 3);
	}

	void GenerateNoise(){

		float noiseResolution = 0.1f;
		Vector2 noiseOffest = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
		float noiseScale = 1.5f; //larger values makes more islands/lakes

		for (int col = 0; col < numCols; col++) {
			for (int row = 0; row < numRows; row++) {
				Hex h = GetHexAt(col, row);
				float n = Mathf.PerlinNoise(((float)col/Mathf.Max(numCols,numRows)/noiseResolution)+noiseOffest.x,
					((float)row/Mathf.Max(numCols,numRows)/noiseResolution)+noiseOffest.y )
					-0.5f;

				h.Elevation += n*noiseScale;
			}
		}
	}

	void GenerateMoisture(){
		float noiseResolution = 0.5f;
		Vector2 noiseOffest = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
		float noiseScale = 0.8f;

		for (int col = 0; col < numCols; col++) {
			for (int row = 0; row < numRows; row++) {
				Hex h = GetHexAt(col, row);
				float n = Mathf.PerlinNoise(((float)col/Mathf.Max(numCols,numRows)/noiseResolution)+noiseOffest.x,
					((float)row/Mathf.Max(numCols,numRows)/noiseResolution)+noiseOffest.y )
					-0.5f;

				h.Moisture = n*noiseScale;
			}
		}
	}
}
