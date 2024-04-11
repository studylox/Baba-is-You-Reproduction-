using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public List<Sentence> info;

    public List<Sentence> correctSentences;//存储当前帧所有成立的语句

    Vector2[] modir = {Vector2.left,Vector2.right,Vector2.up,Vector2.down };//存储基础方向

    public Transform player;//指定当前玩家

    public string[] verbTags;//动词Tag

    public string[] nounTags;//名词Tag

    bool isWrong = false;

    List<Transform> disposers;

    List<Transform> actions;

    List<Transform> objects;

    public GameObject FailPanel;

    public GameObject SucceedPanel;

   
    void Start()
    {
        info=new List<Sentence> ();
        correctSentences = new List<Sentence>();
        disposers = new List<Transform>();
        actions = new List<Transform>();
        objects = new List<Transform>();

    }

    
    void Update()
    {
        
        FindAllIs();
        GetAllCorrectSentence();
        SentenceAction();
        CheckDeath();

    }

    //找到场景中语句成立中的目标作用对象对应的object
  List<Transform> FindAllObjects()
    {

        for(int i=0;i<disposers.Count;i++)
        {
            string tag = disposers[i].tag + "Object";
            GameObject foundObject = GameObject.FindWithTag(tag);
            if(foundObject != null) objects.Add(foundObject.transform);
          

        }

        return objects;
    }

    //设置物体的碰撞器大小为0
    void SetObjectColliderSize()
    {
        List<Transform> objects = FindAllObjects();
        
       for(int i= 0; i <objects.Count; i++)
        {
            //得先找到object才能下一步
            //ObjectList[i].GetComponent<Collider>().enabled = false;
            
            TilemapCollider2D tilemapCollider = objects[i].GetComponent<TilemapCollider2D>();
            tilemapCollider.enabled = false;
            //ObjectList[i].GetComponent<Tilemap>() .GetComponent<TilemapCollider2D>().
        }
    }


    void FindAllIs()
    {
        info.Clear ();
        //存好当前时刻内所有的is块
        int j = 0;
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Is");//找到所有包含is`
        foreach (GameObject gameObject in gameObjects)
        {
            Sentence s=new Sentence();
            s.Is = gameObject.transform;
            info.Add(s);

            j++;
        }
    }

    //获取特定物体指定方向上指定距离内离它最近的物体
    Transform GetObjectByDir(Transform transform,Vector2 modir, float distance)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, modir);

        // 根据距离或其他条件选择最合适的物体(优先返回最近的物体)
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.transform != transform && Vector2.Distance(transform.position, hit.transform.position) <= distance&&hit.transform.GetComponent<Box>()) // 排除自身
            {
                return hit.transform;
            }
        }

        return null; 
    }


    //判断给定的这句话是否成立，若语句成立，则存储它的周围的物体
    bool JudgeSentence(Sentence s)
    {
       
        int j = 0, i = 0;
        while (j < 2 && i < modir.Length)
        {
           
            Transform item1, item2;

            item1 = GetObjectByDir(s.Is, modir[i],1.5f);
            item2=GetObjectByDir(s.Is, modir[i+1],1.5f);

            //Debug.Log(item2);
            //Debug.Log(item2);
            if ((item1 == null || item2 == null) )
            {
                isWrong = false;
                s.isCorrect[j] = false;

            }
            else if((verbTags.Contains(item1.tag)) || (item1.tag == "You"))
            {
                isWrong = true;
                s.isCorrect[j] = false;
            }
            else
            {
                s.isCorrect[j] = true;
                if (i==0)
                {
                    s.Left = item1;
                    s.Right = item2;

                    
                    
                }
                else if(i==2)
                {
                    s.Up = item1;
                    s.Down = item2;
                }

                disposers.Add(item1);
                actions.Add(item2);
            }

            j++;
            i = i + 2;
        }



        if (s.isCorrect[0] || s.isCorrect[1])
        {
            s.Is.GetComponent<Renderer>().material.color = Color.white;
            return true;
        }
        else
        {
            if(isWrong) s.Is.GetComponent<Renderer>().material.color = Color.red;
            else s.Is.GetComponent<Renderer>().material.color = Color.gray;
            Debug.Log(isWrong);
            return false;

        }
    }



    //获取当前帧，当前场景中所有成立的语句,存到correctSentences中
    void GetAllCorrectSentence()
    {
        correctSentences.Clear();
        disposers.Clear();
        actions.Clear();

        for (int i=0;i<info.Count;i++)
        {

            if(JudgeSentence(info[i]))
            {
                correctSentences.Add(info[i]);
                if (correctSentences[i]!=null)
                {
                    //Debug.Log(correctSentences[i].Up);
                    //Debug.Log(correctSentences[i].Down);

                } 
            }
            else
            {
                correctSentences.Add(null);
            }
        }
        SetObjectColliderSize();

    }


    //根据成立的语句对物体实施对应操作
    void SentenceAction()
    {
        int j = 0;
        
        for(int i=0;i<actions.Count;i++)
        {
            if (actions[i].tag=="You")//玩家切换（先不做这个了）
            {
                Debug.Log("玩家切换");
            }
            else if(j<objects.Count)
            {
                switch (actions[i].tag)
                {
                    case "Stop":
                        ObjectController.Stop(objects[j]);
                        break;
                    case "Win":
                        ObjectController.Win(PlayerController.collisionName, objects[j],SucceedPanel);
                        break;

                }
                j++;
            }
            
        }
    }


    //检测玩家死亡,按R键可重开(感觉有点问题这里)
    void CheckDeath()
    {
        int i;
       
        for( i=0;i<correctSentences.Count;i++)
        {
            if(correctSentences[i]!=null)
            {
                if (correctSentences[i].Right || correctSentences[i].Down) break;

            }
           
        }
        if (i >= correctSentences.Count)
        {
            FailPanel.SetActive(true);
            Debug.Log("弹失败面板");
        }


    }



}
