using UnityEngine;
using System.Collections;
using QPath;

//defines the grid position, world space position, neighbours and size etc of hex tile
public class Hex {

	//Q + R + S = 0
	// S = -(Q + R)
	public readonly int Q; //column
	public readonly int R; //row
	public readonly int S;

	//TODO: some kind of property to track hex type and maybe hex detail? (top and bottom meshes)
	//Data for map generation and maybe in-game effects
	//TODO: generate hex move cost function for each tile
	//TODO: update resources and create events for tile.
	//-----> TextMesh component which will display the values (FOR NOW)
	//TODO: SHOW resource cost of each move to user before move
	public float Elevation;
	public float Moisture;
	public int foodCost, waterCost;
	static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
	public readonly float radius = 0.575f;

	public readonly Map hexMap;

	private Character boy;
	//SHOULD we have this as a variable here to ensure we are always on the right tile?

	public Hex(Map map, int q, int r){
		this.hexMap=map;
		this.Q=q;
		this.R=r;
		this.S= -(q + r);
		calculateFoodAndWaterCost();
	}

	//returns the world space position of this hex
	public Vector3 Position(){

		return new Vector3(
			HexHorizontalSpacing()*(this.Q + this.R/2f),
			0,
			HexVerticalSpacing() * this.R
		);
	}

	public float HexHeight(){
		return radius*2;
	}

	public float HexWidth(){
		return WIDTH_MULTIPLIER * HexHeight();
	}

	public float HexVerticalSpacing(){
		return HexHeight() * 0.75f;
	}

	public float HexHorizontalSpacing(){
		return HexWidth();
	}

	public Vector3 PositionFromCamera(){
		return hexMap.GetHexPosition(this);
	}
	public Vector3 PositionFromCamera(Vector3 camPosition, float numRows, float numCols){
		float mapHeight = numRows * HexVerticalSpacing();
		float mapWidth = numCols * HexHorizontalSpacing();

		//we want how many widths to be between -0.5 and 0.5 (just specified by us)
		Vector3 position = Position();

		//can toggle these on and off
		//ALLOWS CAMERA TO LOOP EAST TO WEST (we dont really want/need this)
		if(hexMap.allowWrapEastWest){
			float howManyWidthsFromCamera = (position.x - camPosition.x) / mapWidth;

			//CAN bail in here early to get a max/min number of hexes that can be seen by the cam

			if(howManyWidthsFromCamera > 0){
				howManyWidthsFromCamera += 0.5f;
			}else{
				howManyWidthsFromCamera -= 0.5f;
			}

			int howManyWidthToFix = (int)howManyWidthsFromCamera;

			position.x -= howManyWidthToFix * mapWidth;
		}
		
		if(hexMap.allowWrapNorthSouth){
			float howManyHeightsFromCamera = (position.z - camPosition.z) / mapHeight;

			if(howManyHeightsFromCamera > 0){
				howManyHeightsFromCamera += 0.5f;
			}else{
				howManyHeightsFromCamera -= 0.5f;
			}

			int howManyHeightToFix = (int)howManyHeightsFromCamera;

			position.z -= howManyHeightToFix * mapHeight;
		}

		return position;
	}

	public static float Distance(Hex a, Hex b){
		
		//possibly wrong for wrapping

		int dQ = Mathf.Abs(a.Q-b.Q);
		if(a.hexMap.allowWrapEastWest){
			if(dQ>a.hexMap.numCols/2){
				dQ = a.hexMap.numCols - dQ;
			}
		}
		
		int dR = Mathf.Abs(a.R-b.R);
		if(a.hexMap.allowWrapNorthSouth){
			if(dR>a.hexMap.numRows/2){
				dR = a.hexMap.numRows - dR;
			}
		}
		
		return
			Mathf.Max(
				dQ,
				dR,
				Mathf.Abs(a.S - b.S));
	}

	//not really useful for our purposes
	public void AddBoy(Character boi){
		boy = boi;
	}

	public int BaseMovementCost(){
		//FOR US can return 2 ints, one for food one for water
		//TODO: Factor in terrain type and features
		return 1;
	}

	public void calculateFoodAndWaterCost(){
		this.foodCost = Random.Range(1,13);
		this.waterCost = Random.Range(3,9);
	}

}

