using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//<summary>
//Game object, that creates maze and instantiates it in scene
//同时随机生成5个金币作为线索物体，供指向任务使用。
//</summary>
public class MazeSpawner : MonoBehaviour {

    // 线索物体信息类
    public class ClueObjectInfo {
        public string name;
        public Vector3 position;
    }

    public List<ClueObjectInfo> clueObjects = new List<ClueObjectInfo>(); // 线索物体列表
    public int clueCount = 5; // 线索物体数量

    public enum MazeGenerationAlgorithm {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }

    public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    public bool FullRandom = false;
    public int RandomSeed = 12345;
    public GameObject Floor = null;
    public GameObject Wall = null;
    public GameObject Pillar = null;
    public int Rows = 5;
    public int Columns = 5;
    public float CellWidth = 5;
    public float CellHeight = 5;
    public bool AddGaps = true;
    public GameObject GoalPrefab = null;  // 这里GoalPrefab当成“金币”使用

    private BasicMazeGenerator mMazeGenerator = null;

    void Start () {
        if (!FullRandom) {
            Random.seed = RandomSeed;
        }
        // 生成迷宫
        switch (Algorithm) {
            case MazeGenerationAlgorithm.PureRecursive:
                mMazeGenerator = new RecursiveMazeGenerator (Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                mMazeGenerator = new RecursiveTreeMazeGenerator (Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RandomTree:
                mMazeGenerator = new RandomTreeMazeGenerator (Rows, Columns);
                break;
            case MazeGenerationAlgorithm.OldestTree:
                mMazeGenerator = new OldestTreeMazeGenerator (Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                mMazeGenerator = new DivisionMazeGenerator (Rows, Columns);
                break;
        }
        mMazeGenerator.GenerateMaze ();

        // 生成迷宫地板和墙体
        for (int row = 0; row < Rows; row++) {
            for(int column = 0; column < Columns; column++){
                float x = column*(CellWidth+(AddGaps ? 0.2f : 0));
                float z = row*(CellHeight+(AddGaps ? 0.2f : 0));
                MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
                GameObject tmp;
                tmp = Instantiate(Floor, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0)) as GameObject;
				tmp.transform.parent = transform;

				// 如果是起点（左下角），将地板颜色改为红色
				if (row == 0 && column == 0) {
					Renderer rend = tmp.GetComponent<Renderer>();
					if (rend != null) {
						rend.material.color = Color.red;
					}
				}


                if(cell.WallRight){
                    tmp = Instantiate(Wall,new Vector3(x+CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,90,0)) as GameObject;
                    tmp.transform.parent = transform;
                }
                if(cell.WallFront){
                    tmp = Instantiate(Wall,new Vector3(x,0,z+CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,0,0)) as GameObject;
                    tmp.transform.parent = transform;
                }
                if(cell.WallLeft){
                    tmp = Instantiate(Wall,new Vector3(x-CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,270,0)) as GameObject;
                    tmp.transform.parent = transform;
                }
                if(cell.WallBack){
                    tmp = Instantiate(Wall,new Vector3(x,0,z-CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,180,0)) as GameObject;
                    tmp.transform.parent = transform;
                }
                // 不再通过IsGoal生成金币，这里注释掉或删除
                /*
                if(cell.IsGoal && GoalPrefab != null){
                    tmp = Instantiate(GoalPrefab,new Vector3(x,1,z), Quaternion.Euler(0,0,0)) as GameObject;
                    tmp.transform.parent = transform;
                }
                */
            }
        }

        // 生成柱子
        if(Pillar != null){
            for (int row = 0; row < Rows+1; row++) {
                for (int column = 0; column < Columns+1; column++) {
                    float x = column*(CellWidth+(AddGaps ? 0.2f : 0));
                    float z = row*(CellHeight+(AddGaps ? 0.2f : 0));
                    GameObject tmp = Instantiate(Pillar,new Vector3(x-CellWidth/2,0,z-CellHeight/2),Quaternion.identity) as GameObject;
                    tmp.transform.parent = transform;
                }
            }
        }

        // 随机生成5个金币作为线索物体
        GenerateClueObjects();
    }

    void GenerateClueObjects() {
        if (GoalPrefab == null) {
            Debug.LogWarning("GoalPrefab未设置，无法生成线索物体！");
            return;
        }

        HashSet<(int, int)> usedCells = new HashSet<(int,int)>();
        int placed = 0;

        while (placed < clueCount) {
            int r = Random.Range(0, Rows);
            int c = Random.Range(0, Columns);

            if (usedCells.Contains((r, c))) continue;  // 避免重复

            usedCells.Add((r, c));

            float x = c * (CellWidth + (AddGaps ? 0.2f : 0));
            float z = r * (CellHeight + (AddGaps ? 0.2f : 0));
            Vector3 pos = new Vector3(x, 1, z);

            GameObject clue = Instantiate(GoalPrefab, pos, Quaternion.identity);
            clue.name = $"Coin_{placed + 1}";
            clue.tag = "Coin";  // 方便识别


			if (clue.GetComponent<Collider>() == null)
			{
				SphereCollider collider = clue.AddComponent<SphereCollider>();
				collider.isTrigger = true;
			}

			if (clue.GetComponent<Rigidbody>() == null)
			{
				Rigidbody rb = clue.AddComponent<Rigidbody>();
				rb.useGravity = false;
				rb.isKinematic = true;
			}

            clue.transform.parent = transform;

            ClueObjectInfo info = new ClueObjectInfo();
            info.name = clue.name;
            info.position = pos;

            clueObjects.Add(info);

            placed++;
        }
    }
}
