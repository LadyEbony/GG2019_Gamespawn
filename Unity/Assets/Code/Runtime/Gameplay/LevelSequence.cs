using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.SceneManagement;

public class LevelSequence : MonoBehaviour {
  public static LevelSequence Instance {get; private set;}

  [SerializeField] private int[] levelBuildIndexes;
  public int levelCurrentBuildIndex;
  public int levelIndex;

  private void Awake() {
    Instance = this;
    levelCurrentBuildIndex = SceneManager.GetActiveScene().buildIndex;
    levelIndex = System.Array.IndexOf(levelBuildIndexes, levelCurrentBuildIndex);
  }

  public void ProceedLevel(){
    StartCoroutine(ProceedLevelAsync());
  }

  public void ResetGame() {
    levelIndex = -1;
    StartCoroutine(ProceedLevelAsync());
  }

  private IEnumerator ProceedLevelAsync(){
    AsyncOperation op;  

    op = SceneManager.UnloadSceneAsync(levelCurrentBuildIndex);
    while (!op.isDone) yield return null;

    levelCurrentBuildIndex = levelBuildIndexes[++levelIndex];

    op = SceneManager.LoadSceneAsync(levelCurrentBuildIndex, LoadSceneMode.Additive);
    while (!op.isDone) yield return null;
  }

}
