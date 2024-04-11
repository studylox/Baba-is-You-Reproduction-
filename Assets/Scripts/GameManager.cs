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

    public List<Sentence> correctSentences;//�洢��ǰ֡���г��������

    Vector2[] modir = {Vector2.left,Vector2.right,Vector2.up,Vector2.down };//�洢��������

    public Transform player;//ָ����ǰ���

    public string[] verbTags;//����Tag

    public string[] nounTags;//����Tag

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

    //�ҵ��������������е�Ŀ�����ö����Ӧ��object
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

    //�����������ײ����СΪ0
    void SetObjectColliderSize()
    {
        List<Transform> objects = FindAllObjects();
        
       for(int i= 0; i <objects.Count; i++)
        {
            //�����ҵ�object������һ��
            //ObjectList[i].GetComponent<Collider>().enabled = false;
            
            TilemapCollider2D tilemapCollider = objects[i].GetComponent<TilemapCollider2D>();
            tilemapCollider.enabled = false;
            //ObjectList[i].GetComponent<Tilemap>() .GetComponent<TilemapCollider2D>().
        }
    }


    void FindAllIs()
    {
        info.Clear ();
        //��õ�ǰʱ�������е�is��
        int j = 0;
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Is");//�ҵ����а���is`
        foreach (GameObject gameObject in gameObjects)
        {
            Sentence s=new Sentence();
            s.Is = gameObject.transform;
            info.Add(s);

            j++;
        }
    }

    //��ȡ�ض�����ָ��������ָ���������������������
    Transform GetObjectByDir(Transform transform,Vector2 modir, float distance)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, modir);

        // ���ݾ������������ѡ������ʵ�����(���ȷ������������)
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.transform != transform && Vector2.Distance(transform.position, hit.transform.position) <= distance&&hit.transform.GetComponent<Box>()) // �ų�����
            {
                return hit.transform;
            }
        }

        return null; 
    }


    //�жϸ�������仰�Ƿ������������������洢������Χ������
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



    //��ȡ��ǰ֡����ǰ���������г��������,�浽correctSentences��
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


    //���ݳ�������������ʵʩ��Ӧ����
    void SentenceAction()
    {
        int j = 0;
        
        for(int i=0;i<actions.Count;i++)
        {
            if (actions[i].tag=="You")//����л����Ȳ�������ˣ�
            {
                Debug.Log("����л�");
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


    //����������,��R�����ؿ�(�о��е���������)
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
            Debug.Log("��ʧ�����");
        }


    }



}
