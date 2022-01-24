using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameStat
{
    beforeStartButton = 0,
    inGame = 1,
    gameOver = 2,
}


public class Game: MonoBehaviour
{
    //使用的组件实例
    public Button startButton;
    public Button reStartButton;
    public Transform bornTransform;
    public Text scoreBoard;
   

    public static List<GameObject> fruitList;//这里只声明一下，在Inspector中填入时会实例化
    
    public GameObject standByFruit;


    public GameStat gs;
    public static Dictionary<int, bool> dic = new Dictionary<int, bool>();
    private List<Fruit> fruits = new List<Fruit>();//场景内的水果

    public static Game gameInstance;

    private int score;
    private float timer;
//成员访问接口
    public void SetStat(GameStat x)
    {
        gs = x;
    }

    
//初始化逻辑
    private void Awake()//在start前执行，和constructor作用类似
    {
        timer = 1f;
        score = 0;
        fruitList = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs"));
        gameInstance = new Game();
        standByFruit = null;
    //    counter = 0;
    }

    public void Start()//游戏前初始化
    {
        reStartButton.gameObject.SetActive(false);
        startButton.onClick.AddListener(StartGame);
        reStartButton.onClick.AddListener(ReStartGame);
    }

    void StartGame()
    {
        startButton.gameObject.SetActive(false);
        standByFruit = StandbyGen();
        gs = GameStat.inGame;
    }

    void ReStartGame()
    {
        Debug.Log("Restart button");
        //startButton.gameObject.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartGame();
    }

//游戏主运行逻辑
    private void Update()
    {
        if (gs == GameStat.beforeStartButton)
        {
            return;
        }
        if (gs == GameStat.inGame)
        {
            if (standByFruit != null)
            {
                gameLoop();
            }
        }
        if(gs == GameStat.gameOver)
        {
            reStartButton.gameObject.SetActive(true);
           // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    private void gameLoop()//单个水果的生命周期
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && standByFruit.GetComponent<Fruit>().stat == FruitStat.StandBy)
        {
            //Debug.Log("mouse down");
            if (timer < 0.1f)//每次鼠标左键按下间隔
            {
                return;
            }
            timer = 0;
            standByFruit.GetComponent<Fruit>().stat = FruitStat.InMove;
        }
        if (standByFruit.GetComponent<Fruit>().stat == FruitStat.InMove)
        {
            var x = Input.mousePosition.x;
            var y = bornTransform.position.y;

            
            standByFruit.GetComponent<Fruit>().MoveTo(new Vector2(x, y));
        }
        if (Input.GetMouseButtonUp(0) && standByFruit.GetComponent<Fruit>().stat == FruitStat.InMove)
        {
            //Debug.Log("mouse up");
            standByFruit.GetComponent<Fruit>().stat = FruitStat.Dropping;
            standByFruit.GetComponent<Fruit>().Drop();
            standByFruit = StandbyGen();
        }
    }
    
 
//prefab生成逻辑
    public GameObject StandbyGen()
    {
        int rand = Random.Range(0, 2);
        //rand = 3;

        var pos = bornTransform.position;
        return GenFruit(rand, bornTransform.position);
    }


    //这里有分数更新逻辑
    public GameObject CombineGen(Vector3 pos, int prefabId)
    {
        //var pos1 = a.gameObject.transform.position;
        //var pos2 = b.gameObject.transform.position;
        //var pos = (pos1 + pos2) * 0.5f;
        //var nextPrefabId = a.GetComponent<Fruit>().prefabId+1;

        var obj = GenFruit(prefabId, pos);
        obj.GetComponent<Rigidbody2D>().simulated = true;

        score += prefabId; //合成水果的编号作为分数
        scoreBoard.text = score.ToString();
        return obj;
    }


    //统一生成接口
    private GameObject GenFruit( int idx, Vector3 pos)
    {
        
        var clone = Instantiate(fruitList[idx], pos, Quaternion.identity) as GameObject;
        Debug.Log("gen " + idx.ToString() + " " + pos.ToString() + " " + clone.GetInstanceID());
        clone.GetComponent<Rigidbody2D>().simulated = false;//初始不模拟刚体
        // clone.GetComponent<Fruit>().id = counter++;
        clone.GetComponent<Fruit>().SetPrefabId(idx);
        dic.Add(clone.GetInstanceID(), true);
        return clone;
    }
}
