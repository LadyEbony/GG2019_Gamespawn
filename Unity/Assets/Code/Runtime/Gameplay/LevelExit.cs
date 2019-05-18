using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

public class LevelExit : MonoBehaviour
{
  public SphereTriggerBounds Bounds { get; private set; }
  public bool once = false;

  private void Awake() {
    Bounds = GetComponent<SphereTriggerBounds>();
  }

  public void Exit() {
    StartCoroutine(EnterCoroutine());
  }

  public void ExitLevel(){
    LevelSequence.Instance.ProceedLevel();
  }

  private IEnumerator EnterCoroutine() {
    foreach(var player in GlobalList<PlayerController>.GetList){
      player.DisableSwitch++;
    }

    yield return null;
  }

  private void FixedUpdate() {
    if (once) return;

    float sqr;
    foreach(var player in GlobalList<PlayerController>.GetList){
      if(player.InteractiveBounds.Intersect(Bounds, out sqr)){
        once = true;
        GetComponent<Animator>().Play("Exit");
      }
    }
  }
}
