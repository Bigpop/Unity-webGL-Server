using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitStat
{
    StandBy = 0,// 默认状态
    InMove=1, //被鼠标移动中
    Dropping = 2,// 只要变成该状态就能继续点击
    InPool = 3,//第一次发生撞击时
}

public class Fruit : MonoBehaviour
{

    public FruitStat stat = FruitStat.StandBy;
    public int prefabId; //包含下一变量信息
    
    private bool isTouchRedline;
    private float timer;
    
    private void Awake()
    {
        int postion = this.name.IndexOf("(");
        if (postion != -1)
        {
            prefabId = int.Parse(this.name.Substring(0, postion));
        }
        isTouchRedline = false;
        timer = 0;
        
    }

    // 碰撞逻辑，因为两个对象碰撞都会调用碰撞方法，因此要比较id，选择id小的
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        //   stat = FruitStat.InPool;
        //Debug.Log(collision.gameObject);
        GameObject a = this.gameObject;
        GameObject b = collision.gameObject;

        if(a.tag=="Fruit" && b.tag=="Fruit" && a.name==b.name)
        {
            
            var id_1 = a.GetInstanceID();
            var id_2 = b.GetInstanceID();

            var prefabId = a.GetComponent<Fruit>().prefabId;
            if (id_1 < id_2 && prefabId < Game.fruitList.Capacity-1)
            {
                if(Game.dic.ContainsKey(id_1)==false || Game.dic.ContainsKey(id_2)==false)
                {
                    Debug.Log(id_1);
                    Debug.Log(id_2);
                    return;
                }
                var pos1 = a.gameObject.transform.position;
                var pos2 = b.gameObject.transform.position;
                var pos = (pos1 + pos2) * 0.5f;
                var nextPrefabId = a.GetComponent<Fruit>().prefabId + 1;
                Destroy(this.gameObject);
                Destroy(collision.gameObject);
                Game.dic.Remove(id_1);
                Game.dic.Remove(id_2);
                Debug.Log(GameObject.Find("Game").GetComponent<Game>());

                GameObject.Find("Game").GetComponent<Game>().CombineGen(pos, nextPrefabId);
            
            }
        }
    }

    //private IEnumerator Timer()
    //{
    //    // process pre-yield
    //    yield return new WaitForSeconds(1f);
    //    // process post-yield
    //}


    public void MoveTo(Vector2 p)
    {
        var y = p.y;//y坐标不变
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(p);
        var x = worldPos.x;
        if (x < -3)
        {
            x = -3;
        }
        if (x > 3)
        {
            x = 3;
        }
        this.gameObject.transform.position = new Vector2(x, y);
    }

    public void Drop()
    {
        this.gameObject.GetComponent<Rigidbody2D>().simulated = true;
    }

    public void SetPrefabId(int x)
    {
        this.prefabId = x;
    }


//Game Over逻辑
    private void Update()
    {
        if (isTouchRedline == false)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer > 3)
        {
            //Game.gameInstance.gs = GameStat.gameOver;
            GameObject.Find("Game").GetComponent<Game>().SetStat(GameStat.gameOver);
            //Debug.Log(Game.gameInstance.gs);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter2D");
        var obj = collision.gameObject;
        if (obj.name=="Top")
        {
            isTouchRedline = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerExit2D");
        var obj = collision.gameObject;
        if (obj.name == "Top")
        {
            isTouchRedline = false;
            timer = 0;
        }
    }

}



// 实例间的通信方法
//      1.Fruit调用Game的情况：
//          1.1 GameObject.Find("Game").GetComponent<Game>()得到实例的脚本对象通过其访问函数进行访问
//          1.2 或者在Fruit脚本中增加 public Game；并将Game实例拖拽过去
//      2.Game中调用Fruit的情况：
//          因为Fruit在Game中instancelize 因此可以直接对该实例操作