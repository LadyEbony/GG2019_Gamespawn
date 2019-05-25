using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{

  public bool state;
  public GameObject child;

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)){
      state = !state;
      if (state){
        Time.timeScale = 0.0f;
        child.SetActive(true);
      } else {
        Time.timeScale = 1.0f;
        child.SetActive(false);
      }
    } else if (Input.GetKeyDown(KeyCode.R) && state){
      state = false;
      Time.timeScale = 1.0f;
      child.SetActive(false);
      LevelSequence.Instance.ResetGame();
    }

  }
}
