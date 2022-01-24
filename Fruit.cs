using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitStat
{
    StandBy = 0,// Ĭ��״̬
    InMove=1, //������ƶ���
    Dropping = 2,// ֻҪ��ɸ�״̬���ܼ������
    InPool = 3,//��һ�η���ײ��ʱ
}

public class Fruit : MonoBehaviour
{

    public FruitStat stat = FruitStat.StandBy;
    public int prefabId; //������һ������Ϣ
    
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

    // ��ײ�߼�����Ϊ����������ײ���������ײ���������Ҫ�Ƚ�id��ѡ��idС��
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
        var y = p.y;//y���겻��
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


//Game Over�߼�
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



// ʵ�����ͨ�ŷ���
//      1.Fruit����Game�������
//          1.1 GameObject.Find("Game").GetComponent<Game>()�õ�ʵ���Ľű�����ͨ������ʺ������з���
//          1.2 ������Fruit�ű������� public Game������Gameʵ����ק��ȥ
//      2.Game�е���Fruit�������
//          ��ΪFruit��Game��instancelize ��˿���ֱ�ӶԸ�ʵ������