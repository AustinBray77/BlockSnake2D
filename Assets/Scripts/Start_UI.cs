using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_UI : MonoBehaviour
{
    public void Click_Start()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void Click_Quit()
    {
        Application.Quit();
    }
}
