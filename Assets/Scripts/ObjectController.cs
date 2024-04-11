using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectController : MonoBehaviour
{
    
   public static void Stop(Transform transform)
    {
        
        TilemapCollider2D tilemapCollider = transform.GetComponent<TilemapCollider2D>();
        tilemapCollider.enabled =true;

    }

    public static void Win(string collisionName,Transform transform,GameObject succeedPanel)
    {
       Stop(transform);
        if (collisionName=="FlagObject")
        {
            //���ɹ����
            Debug.Log("���ɹ����");
            succeedPanel.gameObject.SetActive(true);

        }
    }

    
}
