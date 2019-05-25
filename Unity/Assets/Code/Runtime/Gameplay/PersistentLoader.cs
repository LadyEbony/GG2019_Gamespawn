using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PersistentLoader : MonoBehaviour
{
  private void Awake() {
    for(var i = 0; i < SceneManager.sceneCount; i++){
      if (SceneManager.GetSceneAt(i).name == "_PersistanceScene")
        return;
    }

    SceneManager.LoadScene("_PersistanceScene", LoadSceneMode.Additive);

  }
}
