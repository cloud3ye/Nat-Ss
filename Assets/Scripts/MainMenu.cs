using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField cNum;
    public TMP_InputField tDes;
    public TMP_InputField bTF;
    public TMP_InputField bTW;
    public TMP_InputField mTF;
    public TMP_InputField mTW;
    public TMP_InputField sTF;
    public TMP_InputField sTW;
    public TMP_InputField hDR;
    public TMP_InputField huDR;
    public TMP_InputField tDR;
    public Toggle sTT;
    public Toggle mTT;
    public Toggle lTT;
    


    [SerializeField] GameObject OptionsPanel;
    [SerializeField] GameObject Mainmenu;

    public void PlayGame()
    {
        //Gets the Scene with Index 1 And Loads it
        SceneManager.LoadSceneAsync(1);
    }
    public void Settings()
    {
        //opens the settings panel
        OptionsPanel.SetActive(true);
        Mainmenu.SetActive(false);

    }
    public void QuitGame()
    {
        //Quits the Game
        Application.Quit();
    }
    public void Return()
    {
        //returns to main menu
        SceneManager.LoadSceneAsync(0);
    }
    public void Pause()
    {
        //stops time
        Time.timeScale = 0;
    }
    public void Resume()
    {
        Time.timeScale = 1;
    }
    public void FastForward()
    {
        Time.timeScale +=1;
    }

    public void Save()
    {
        if (cNum.text != "")
        {
            PlayerPrefs.SetInt("CreatureNum", Int32.Parse(cNum.text));

        }
        if (tDes.text != "")
        {
            PlayerPrefs.SetFloat("TreeDensity", float.Parse(tDes.text));

        }
        else
        {
            PlayerPrefs.SetFloat("TreeDensity", 0.25f);
        }
        if (bTF.text != "")
        {
            PlayerPrefs.SetInt("BigTreeFoodValue", Int32.Parse(bTF.text));

        }
        if (bTW.text != "")
        {
            PlayerPrefs.SetInt("BigTreeWaterValue", Int32.Parse(bTW.text));

        }
        if (mTF.text != "")
        {
            PlayerPrefs.SetInt("MedTreeFoodValue", Int32.Parse(mTF.text));

        }
        if (mTW.text != "")
        {
            PlayerPrefs.SetInt("MedTreeWaterValue", Int32.Parse(mTW.text));

        }
        if (sTF.text != "")
        {
            PlayerPrefs.SetInt("SmallTreeFoodValue", Int32.Parse(sTF.text));
        }
        if (sTW.text != "")
        {
            PlayerPrefs.SetInt("SmallTreeWaterValue", Int32.Parse(sTW.text));
        }
        if (hDR.text != "")
        {
            PlayerPrefs.SetFloat("HealthDepletionRate", float.Parse(hDR.text));
        }
        if (huDR.text != "")
        {
            PlayerPrefs.SetFloat("HungerDepletionRate", float.Parse(huDR.text));
        }
        if (tDR.text != "")
        {
            PlayerPrefs.SetFloat("ThirstDepletionRate", float.Parse(tDR.text));
        }
        if(sTT.isOn)
        {
            PlayerPrefs.SetInt("ToggleSmall", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ToggleSmall", 0);
        }
        if(mTT.isOn)
        {
           PlayerPrefs.SetInt("ToggleMed",1); 
        }
        else
        {
            PlayerPrefs.SetInt("ToggleMed",0);
        }
        if(lTT.isOn)
        {
            PlayerPrefs.SetInt("ToggleLarge",1);
        }
        else
        {
            PlayerPrefs.SetInt("ToggleLarge",0);
        }
    }
}
