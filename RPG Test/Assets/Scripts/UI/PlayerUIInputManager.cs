using System;
using UnityEngine;

public class PlayerUIInputManager : MonoBehaviour {
   public static PlayerUIInputManager Instance;
   

   void Awake() {
      if (Instance == null) {
         Instance = this;
      } else {
         Destroy(gameObject);
      }
   }
   
   

}
