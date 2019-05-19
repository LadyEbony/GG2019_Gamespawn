using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Button : WallInteractive {

  private InteractiveEvent bevent;

  public Material OrbMaterial { get; private set; }
  public Light OrbLight { get; private set; }
  public SpriteRenderer UIIcon { get; private set; }
  public TextMeshPro UIText { get; private set; }

  private bool selected;
  [Header("Selection ")]
  [SerializeField] private Color SelectColor = new Color(1f, 0f, .76f);
  [SerializeField] private Color DeselectColor = new Color(.28f, 0f, .21f);
  [SerializeField] private float fadeTime = 0.25f;
  private float fadeDuration = 0.0f;

  public override void Awake() {
    base.Awake();

    bevent = GetComponent<InteractiveEvent>();

    var etransform = GameObjectExtender.FindChildWithTag(transform, "Main Material");
    if (etransform){
      var ecomp = etransform.GetComponent<MeshRenderer>();
      if (ecomp){
        OrbMaterial = ecomp.material;   
      }
      var lcomp = etransform.GetComponent<Light>();
      if (lcomp){
        OrbLight = lcomp;
      }
      Deselect(null);
    }

    var uitransform = GameObjectExtender.FindChildWithTag(transform, "Main UI");
    if (uitransform){
      UIIcon = uitransform.GetComponentInChildren<SpriteRenderer>();
      UIText = uitransform.GetComponentInChildren<TextMeshPro>();
    }
  }

  private void Update() {
    var color = Color.Lerp(DeselectColor, SelectColor, fadeDuration / fadeTime);

    if (OrbMaterial)
      OrbMaterial.SetColor("_Color", color);
    if (OrbLight)
      OrbLight.color = color;
    if (UIIcon)
      UIIcon.color = Color.white * (fadeDuration / fadeTime) * 0.75f;
    if (UIText)
      UIText.color = Color.white * (fadeDuration / fadeTime) * 0.75f;

    fadeDuration = Mathf.Clamp(fadeDuration + (selected ? Time.deltaTime : -Time.deltaTime), 0.0f, fadeTime);
  }

  public override void Interact(PlayerController pc) {
    if (bevent) bevent.Interact(pc, this);
  }

  public override void Select(PlayerController pc) {
    selected = true;
    if (pc == null) fadeDuration = fadeTime;
  }

  public override void Deselect(PlayerController pc) {
    selected = false;
    if (pc == null) fadeDuration = 0.0f;
  }
}
