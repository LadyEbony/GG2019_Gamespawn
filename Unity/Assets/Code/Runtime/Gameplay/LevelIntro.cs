﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelIntro : MonoBehaviour {
  [System.Serializable]
  private class Character{
    public PlayerController refe;
    public Vector3[] path;
    public int index;
    public bool done;
  }

  [SerializeField] private Character[] characters;

  private void Start() {
    Character c;
    PlayerController refe;
    Vector3 selfPosition = transform.position;

    for(var i = 0; i < characters.Length; i++){
      c = characters[i];
      if (c.path.Length > 0){
        refe = c.refe;
        refe.DisableNavMesh++;
        refe.transform.position = c.path[0] + selfPosition;
        c.index = 1;
        if (c.path.Length == 1) c.done = true;
      }
    }
  }

  public void AnimateWalk(){
    StartCoroutine(AnimateCoroutine());
  }

  private IEnumerator AnimateCoroutine(){
    Character c;
    Transform t;
    PlayerController pc;

    Vector3 selfPosition = transform.position;
    Vector3 destinationPosition;

    while (true) {
      for (var i = 0; i < characters.Length; i++) {
        c = characters[i];
        if (c.done) continue;

        pc = c.refe;
        t = pc.transform;
        destinationPosition = selfPosition + c.path[c.index];
        t.position = Vector3.MoveTowards(t.position, destinationPosition, pc.nva.speed * Time.deltaTime);
        if (Vector3.SqrMagnitude(t.position - selfPosition - c.path[c.index]) < 0.25f) {
          c.index++;
          if (c.index == c.path.Length) c.done = true;
        }

      }

      if (characters.All(d => d.done)) break;

      yield return null;
    }

    foreach(var temp in characters){
      temp.refe.DisableNavMesh--;
    }
  }

  private static readonly Color[] gizmoColors = new Color[] { Color.red, Color.gray, Color.blue };

  private void OnDrawGizmosSelected() {
    if (characters != null) {
      Character c;
      Vector3[] path;
      int pathlength;
      Vector3 position = transform.position;
      for(var i = 0; i < characters.Length; i++){
        c = characters[i];
        path = c.path;
        pathlength = path.Length;

        Gizmos.color = gizmoColors[i];
        if (pathlength > 0) {
          GizmoExtender.DrawWireCircle(position + path[0], 0.25f);
          GizmoExtender.DrawWireCircle(position + path[pathlength - 1], 0.25f);
          for(var j = 0; j < pathlength - 1; j++){
            Gizmos.DrawLine(position + path[j], position + path[j + 1]);
          }
        }
      }
    }
  }
}