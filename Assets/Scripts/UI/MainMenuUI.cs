using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject landscapeUI;
    [SerializeField] private GameObject portraitUI;
    
   private ScreenOrientation CurrentScreenOrientation
   {
      get{
#if UNITY_EDITOR
                if(Screen.height > Screen.width){
                    return ScreenOrientation.Portrait;
                } else {
                    return ScreenOrientation.LandscapeLeft;
                }
#else
         return Screen.orientation;
#endif
      }
   }

   private void Update()
   {
       if (CurrentScreenOrientation == ScreenOrientation.Portrait)
       {
           portraitUI.SetActive(true);
           landscapeUI.SetActive(false);
       }
       else
       {
           portraitUI.SetActive(false);
           landscapeUI.SetActive(true);
       }
   }

   public void StartGame()
   {
       SceneManager.LoadScene("Game");
   }
}
