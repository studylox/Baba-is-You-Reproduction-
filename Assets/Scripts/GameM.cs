using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameM : MonoBehaviour
{
    public int totalBoxes;
    public int finishedBoxes;
    public GameObject FailPanel;
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

   public void CheckFinish()
    {
        if(finishedBoxes==totalBoxes)
        {
            Debug.Log("You win the Game!");
            StartCoroutine(LoadNextScene());
        }

    }
    private void Update()
    {
        

        if(Input.GetKeyDown(KeyCode.R))
        {
            FailPanel.SetActive(false);
            RestartGame();
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    
}
