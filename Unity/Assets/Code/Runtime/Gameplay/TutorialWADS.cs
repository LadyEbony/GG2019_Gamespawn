using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class TutorialWADS : MonoBehaviour
{
  public SpriteRenderer UIIcon { get; private set; }

  private bool selected;
  private bool input;
  [SerializeField] private float fadeTime = 0.25f;
  private float fadeDuration = 0.0f;

  private void Awake() {
    UIIcon = GetComponent<SpriteRenderer>();
  }

  private void Update() {
    if (PlayerInput.instance.DisableInput != 0) return;

    if (PlayerInput.instance.GetDirectionInput != Vector3.zero){
      input = true;
    }

    selected = !input;

    if (UIIcon)
      UIIcon.color = Color.white * (fadeDuration / fadeTime) * 0.75f;

    fadeDuration = Mathf.Clamp(fadeDuration + (selected ? Time.deltaTime : -Time.deltaTime), 0.0f, fadeTime);

    if (fadeDuration == 0.0f) Destroy(gameObject);
  }
}
