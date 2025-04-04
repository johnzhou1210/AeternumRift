using System;
using UnityEngine;
using UnityEngine.SceneManagement;


/*
 * GAME SCENES
 * DungeonScene
 * GameManagerScene
 * BattleScene
 * 
 */


public class SceneUtility : MonoBehaviour {
   public static SceneUtility Instance;

  
   private void Awake() {
      if (Instance == null) {
         Instance = this;
      } else {
         Destroy(gameObject);
      }
   }

   public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive) {
      SceneManager.LoadSceneAsync(sceneName, mode);
   }

   public void UnloadScene(string sceneName) {
      SceneManager.UnloadSceneAsync(sceneName);
   }

   
   

   
   
}
