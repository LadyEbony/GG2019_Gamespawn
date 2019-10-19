using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using GameSpawn;

[RequireComponent(typeof(SphereTriggerBounds))]
public class TutorialZone : MonoBehaviour
{
  public SpriteRenderer UIIcon { get; private set; }
  public TextMeshPro UIText { get; private set; }
  public SphereTriggerBounds Bounds { get; private set; }

  private bool once;
  [SerializeField] private float fadeTime = 0.25f;
  private float fadeDuration = 0.0f;

  private void Awake() {
    UIIcon = GetComponentInChildren<SpriteRenderer>();
    UIText = GetComponentInChildren<TextMeshPro>();
    Bounds = GetComponent<SphereTriggerBounds>();
  }

  private void Update() {
    var selected = false;
    float sqrdist;
    foreach(var player in GlobalList<PlayerController>.GetList){
      if (Bounds.Intersect(player.interactiveBounds, out sqrdist)){
        selected = true;
        once = true;
        break;
      }
    }

    if (UIIcon)
      UIIcon.color = Color.white * (fadeDuration / fadeTime) * 0.75f;
    if (UIText)
      UIText.color = Color.white * (fadeDuration / fadeTime) * 0.75f;

    fadeDuration = Mathf.Clamp(fadeDuration + (selected ? Time.deltaTime : -Time.deltaTime), 0.0f, fadeTime);

    if (once && fadeDuration == 0.0f){
      Destroy(gameObject);
    }

  }
}
