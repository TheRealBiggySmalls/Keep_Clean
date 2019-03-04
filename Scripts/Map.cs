using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QPath;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


//QPATH is just a set of interfaces to make pathfinding generic for later projects
public class Map : MonoBehaviour {

	
	//make map stay around after being rendered once
	public GameObject hexPrefab;
	public GameObject selectionPrefab;
	public GameObject hexTop;
	public GameObject Boy;
	public Character player;

	public static Map map;


	//can here have arrays of mesh filters for different biomes
	//NEED BASE COLOR MESH FOR GROUND THEN TO DRAW MESH ON TOP USING COMPONENTS

	//ONLY need top meshes. Change colour of base hex but it's always the same. Change mesh
	//of topHex
	public Mesh MeshWater;//DEFAULT hex mesh (hex11)
	public Mesh TopMountain; public Mesh[] storyFlowers; public Mesh selectableMesh; public Mesh houseMesh; public Mesh[] TopDesert; public Mesh[] TopForest; public Mesh[] TopPlains; public Mesh[] TopSnow;
	public Material MatOcean;public Material MatPlains;public Material MatDesert;public Material MatMountains;public Material MatForest;public Material MatGrasslands;public Material MatSnow;

	//tiles with height above ____ are considered ____
	public float HeightMountain=0.5f;
	public float HeightSnow=0.46f;
	public float HeightForest=0.5f;
	public float HeightGrass = 0.1f;
	public float HeightFlat = 0.0f;


	// Size of the map in terms of number of hex tiles
	// This is NOT representative of the amount of 
	// world space that we're going to take up.
	// (i.e. our tiles might be more or less than 1 Unity World Unit)
	public int numRows = 20;
	public int numCols = 40;

	private Hex[,] hexes;
	public Dictionary<Hex,GameObject> hexToGameObjectMap;
	private Dictionary<Character, GameObject> charToGameObject;
	public bool allowWrapNorthSouth=false, allowWrapEastWest=false;


	void Awake(){
		if(map==null){
			map=this;
		}else if(map!=this){
			Destroy(gameObject);
		}
		if(map.player == null){
			map.player = new Character();
		}
	}

	// Use this for initialization
	void Start () {
		GenerateMap();
		//after generate map can generate more ocean tiles around the whole world that are in
		//a new array and are never interacted with
		
		//TODO: make place boy spawn on a random tile of specific elevation
		//easy to do: just loop through all tiles after map has been spawned and choose one close
		//to land
		PlaceBoy(Boy, 12, 11);
		FocusCameraOnBoy();
		highlightSelectableTiles(player.currentHex);
		ChangeResourceText.UpdateUIResources(player.Food,player.Water, player.Honey);
		Random.InitState(1);
		placeFlowers(); //CAN: place this after reinit of random for more fun generation if we want

		//rerandomises the seed so debugging isnt no fun
		Random.InitState(System.Environment.TickCount);
		if(player!=null){
			player.highlightCurrentObjective(0);
		}
	}

	public void doTurn(){
		//Debug.Log("do turn");
	}

	public Hex GetHexAt(int x, int y){
		if(hexes == null){
			return null;
		}
		if(allowWrapEastWest){
			x = x % numRows;
			if(x<0){
				x+=numRows;
			}
		}
		if(allowWrapNorthSouth){
			y = y % numCols;
			if(y<0){
				y+=numCols;
			}
		}	

		//args must be passed in as col:row
		try{
			return hexes[x,y];
		}catch{
			return null;
		}
	}

	public Vector3 GetHexPosition(int q, int r){
		Hex h = GetHexAt(q,r);
		return GetHexPosition(h);
	}
	public Vector3 GetHexPosition(Hex hex){
		return hex.PositionFromCamera(Camera.main.transform.position, numRows, numCols);
	}

	//CURRENTLY generates map in a rhombus shape... not bad for landing on corner of map/boat?
	virtual public void GenerateMap(){
		
		//stores all hex tiles with columns first them rows as index
		//Dictionary maps hex to game object at that location
		hexes = new Hex[numCols,numRows];
		hexToGameObjectMap = new Dictionary<Hex, GameObject>();

		//Generate map filled with ocean
		for (int col = 0; col < numCols; col++) {
			for (int row = 0; row < numRows; row++) {

				Hex h = new Hex(this,col,row);

				h.Elevation = -0.5f; //-1 is default elevation for water
				hexes[col,row] = h;

				Vector3 pos = h.PositionFromCamera(Camera.main.transform.position,numRows,numCols);

				GameObject hexGo = (GameObject) Instantiate(hexPrefab,pos,
					Quaternion.identity,this.transform);
				//currently spawns candy everywhere
				//GameObject hexT = (GameObject) Instantiate(hexTop, pos,
					//Quaternion.identity,hexGo.transform);

				//hexToGameObjectMap.Add(h, hexGo);
				hexToGameObjectMap[h] = hexGo;

				hexGo.name = string.Format("HEX: {0}, {1}", col, row);
				hexGo.GetComponent<HexComponent>().hex = h;
				hexGo.GetComponent<HexComponent>().hexMap = this;

				//basic food and water assignment

				hexGo.GetComponentInChildren<TextMesh>().text = "";//string.Format("{0},{1}", col, row);
				
				//lowPolyHex has a combined material. Could consider using this???
				MeshRenderer mr = hexGo.GetComponentInChildren<MeshRenderer>();
				mr.material = MatOcean; //hexMaterials[Random.Range(0, hexMaterials.Length)];

				MeshFilter mf = hexGo.GetComponentInChildren<MeshFilter>();
				mf.mesh = MeshWater;

			}
		}
	}

	public List<Hex> storyLocations; //reordered to be chronological
	public void placeFlowers(){
		storyLocations = new List<Hex>();

		//Hard codes father's memories into castles
		//castles need some entry condition
		//THIS WAY: we can link bee progression to story progression

		int numPlaced=0,numDesired=5;

		while(numPlaced<numDesired){
			int row = (int)Random.Range(0,numRows);
			int column = (int)Random.Range(0,numCols);

			Hex candidate = hexes[column,row]; //could use GetHexAt

			if(candidate.Elevation>0){ //checks hex is not a water tile

				string objString = string.Format("HEX: " + column + ", " + row);
				GameObject obj = GameObject.Find(objString);
				Transform t = obj.GetComponentInChildren<Transform>().Find("Hex11").Find("HexTop");
				MeshFilter mf = t.GetComponentInChildren<MeshFilter>();
				
				mf.mesh = storyFlowers[numPlaced];//hard spawning only castles at the moment
				numPlaced+=1;
				storyLocations.Add(candidate);
			}
		}
	}

	public void UpdateHexVisuals(){

		for (int col = 0; col < numCols; col++) {
			for (int row = 0; row < numRows; row++) {
				Hex h = hexes[col, row];
				GameObject hexGo = hexToGameObjectMap[h];
	
				MeshRenderer mr = hexGo.GetComponentInChildren<MeshRenderer>();
				//MeshFilter mf = hexGo.GetComponentInChildren<MeshFilter>();
				
				string objString = string.Format("HEX: " + col + ", " + row);
				GameObject obj = GameObject.Find(objString);

				Transform t = obj.GetComponentInChildren<Transform>().Find("Hex11").Find("HexTop");
				MeshFilter mf = t.GetComponentInChildren<MeshFilter>();
				//GameObject temp = hexGo.GetComponent<GameObject>();
				//MeshFilter mf = temp.GetComponentInChildren<MeshFilter>();

				//Gives a 1 in 2 chance of items being placed
				//WOOHOO! Can change random values and dilute arrays to make chance of getting
				//certain items to spawn to be smaller
				int placeEntity = Random.Range(0,100);

				//TODO: Check neighbours of water tiles to put in topMeshes when it's a lake
				//TODO: Add in magical tiles for plot progression/flower specifics
				//TODO: Add in town
				//TODO: Add in actual terrain height generation

				/*Make castle and magic tiles spawn over on special island


					for(int i in range [0,5]){
						if(ELEVATION ABOVE A CERTAIN HEIGHT){
							mr.material = magic_purple_material;
						}
					} */

				//CAN include moisture in this as a component to seperate elevation from tile colour
				if(h.Elevation >= HeightMountain){
					mr.material = MatMountains;
					if(placeEntity>20){
						mf.mesh = TopMountain;
					}else{
						mf.mesh=null;
					}
				}else if(h.Elevation>=HeightSnow){
					mr.material=MatSnow;
					if(placeEntity>35){
						mf.mesh = TopSnow[Random.Range(0,TopSnow.Length-1)];
					}else{
						mf.mesh=null;
					}
				}else if(h.Elevation>=HeightForest){
					mr.material = MatForest;
					if(placeEntity>25){
						mf.mesh = TopForest[Random.Range(0,TopForest.Length-1)];
					}else{
						mf.mesh=null;
					}
				}else if(h.Elevation>= HeightGrass){
					mr.material = MatGrasslands;
					if(placeEntity>55){
						mf.mesh = TopPlains[Random.Range(0,TopPlains.Length-1)];
					}else{
						mf.mesh=null;
					}
				}else if(h.Elevation>= HeightFlat){
					mr.material = MatDesert;
					if(placeEntity>45){
						mf.mesh = TopDesert[Random.Range(0,TopDesert.Length-1)];
					}else{
						mf.mesh=null;
					}
				}else{
					mr.material = MatOcean;
					mf.mesh = MeshWater;
				}
				//mf.mesh=MeshWater;

				if(h.foodCost==0||h.waterCost==0){
					//calculateResourceCost()
				}
			}

			//this function is NOT called every frame. Only at the start when map is generated
		}
	}

	public Hex[] GetHexesWithinRangeOf(Hex centreHex, int range){
		List<Hex> results = new List<Hex>();
		
		for(int dx=-range;dx<range-1;dx++){
			for(int dy=Mathf.Max(-range+1, -dx-range);dy<Mathf.Min(range,-dx+range-1);dy++){
				results.Add(GetHexAt(centreHex.Q +dx, centreHex.R +dy));
			}
		}
		return results.ToArray();
	}

	public Hex[] getNeighbours(Hex centreHex){
		List<Hex> neighbours = new List<Hex>();
		int q = centreHex.Q; int r = centreHex.R;

		neighbours.Add(GetHexAt(q-1,r+1)); //to the top left
		neighbours.Add(GetHexAt(q,r+1)); //to the top right
		neighbours.Add(GetHexAt(q-1,r)); //to the left
		neighbours.Add(GetHexAt(q+1,r)); //to the right
		neighbours.Add(GetHexAt(q,r-1)); //to the bottom left
		neighbours.Add(GetHexAt(q+1,r-1)); //to the bottom right

		return neighbours.ToArray();
	}

	public void PlaceBoy(GameObject preFab, int q, int r){
		if(charToGameObject==null){
			charToGameObject = new Dictionary<Character, GameObject>();
		}

		Hex a = GetHexAt(q,r);
		GameObject hex = hexToGameObjectMap[a];
		//Boy should have a class like Hex which contains its position and current tile etc info
		GameObject boy = (GameObject) Instantiate(Boy,hex.transform.position,
					Quaternion.identity,this.transform);

		player.SetHex(a);
		player.OnCharacterMoved += boy.GetComponent<CharacterView>().OnCharacterMoved;
		
		charToGameObject[player] = boy;

		PlaceHouse(q, r);
	}

	//IN MAP.UPDATE call PLAYER.UPDATE which is just if(moving) then MOVE or sumthn?
	public void FocusCameraOnBoy(){
		Vector3 pos = charToGameObject[player].transform.position;
		pos.y = 4; pos.z -=3;
		Camera.main.transform.position=pos;
	}

	//kinda works. Can fix later
	public void PlaceHouse(int col, int row){
		string objString = string.Format("HEX: " + col + ", " + row);
		GameObject obj = GameObject.Find(objString);

		Transform t = obj.GetComponentInChildren<Transform>().Find("Hex11").Find("HexTop");
		MeshFilter mf = t.GetComponentInChildren<MeshFilter>();

		mf.mesh = houseMesh;
	}


	List<GameObject> selectables;

	//HERE: instantiate a new game object where they are using the prefab, so highlight should
	//show up in same place. THEN delete the new object after move
	public void highlightSelectableTiles(Hex hex){
		if(selectables==null){
			selectables = new List<GameObject>();
		}
		Hex[] highlight = getNeighbours(hex);

		//TODO: dont highlight all neighbours, only legal moves
		foreach(Hex hexa in highlight){			
			if(player.checkIllegalTiles(hexa)&&player.checkResourcesSufficient(hexa)){
				GameObject inst = (GameObject)Instantiate(selectionPrefab,hexa.Position(),Quaternion.identity);
				MeshFilter mf = inst.GetComponentInChildren<MeshFilter>();
				mf.mesh = selectableMesh;
				selectables.Add(inst);
			}
		}
	}

	//TODO: have a variable that tracks old player position so if they move then dont execute 
	//either of these code blocks
	public void deleteOldSelectables(){
		//WHY is selectables null all the time
		if(selectables==null || selectables.Count<1){
			return;
		}
		//might be something wrong with indexing?
		foreach(GameObject obj in selectables){
			GameObject.Destroy(obj);
		}
	}
}
