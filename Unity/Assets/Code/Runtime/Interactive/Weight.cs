using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

[RequireComponent(typeof(CylinderTriggerBounds))]
public class Weight : InteractiveBase {

  private InteractiveEvent bevent;
  [SerializeField] private IWeight focus;

  public Material ShellMaterial { get; private set; }
  public Material FlowerMaterial { get; private set; }
  public Light[] CandleLights { get; private set; }

  private bool selected, disabled;
  private Vector3 disabledscale;
  [Header("Selection ")]
  [SerializeField] private Color DeselectColor = new Color(.28f, 0f, .21f);
  [SerializeField] private Color SelectShellColor = new Color(1f, 0f, .76f);
  [SerializeField] private Color SelectFlowerColor = new Color(1f, 0f, .76f);
  [SerializeField] private float SelectLightRange = 0.25f;
  [SerializeField] private float DeselectLightRange = 0.5f;
  [SerializeField] private float fadeTime = 0.25f;
  [SerializeField] private float disableTime = 1.0f;
  private float fadeDuration = 0.0f;

  public override void Awake() {
    base.Awake();

    if (!CylinderBounds) Debug.LogErrorFormat("{0} does not have a cylinder trigger bounds!", gameObject.name);

    bevent = GetComponent<InteractiveEvent>();

    Transform temptrans;
    temptrans = transform.Find("Shell");
    if (temptrans){
      ShellMaterial = temptrans.GetComponent<MeshRenderer>().material;
    }
    temptrans = transform.Find("Inner");
    if (temptrans) {
      FlowerMaterial = temptrans.GetComponent<MeshRenderer>().material;
    }

    CandleLights = GetComponentsInChildren<Light>();
  }

  private void FixedUpdate() {
    if (disabled) return;

    IWeight act = null;
    float actDist = float.MaxValue;
    float temp;

    // Check collisions
    foreach (Item item in GlobalTypeList<Interactive>.GetTypeList(typeof(Item))) {
      if (item.Bounds.Intersect(CylinderBounds, out temp)) {
        if (temp < actDist) {
          act = item;
          actDist = temp;
        }
      }
    }

    // Items have priority
    if (act == null) {
      foreach (var player in GlobalList<PlayerController>.GetList) {
        if (player.interactiveBounds.Intersect(CylinderBounds, out temp)) {
          if (temp < actDist) {
            act = player;
            actDist = temp;
          }
        }
      }
    }

    if (focus != null && focus != act) {
      focus.Exit(this);
    }

    focus = act;

    if (focus != null) {
      focus.Enter(this);
    }

    var statusEvent = focus != null;
    if (statusEvent ^ selected) {
      selected = statusEvent;
      if (bevent) bevent.Interact(null, this);
    }

  }

  private void Update() {
    if (disabled){
      transform.localScale = Vector3.Lerp(Vector3.zero, disabledscale, fadeDuration / disableTime);
    } else {
      var ratio = fadeDuration / fadeTime;
      var range = Mathf.Lerp(DeselectLightRange, SelectLightRange, ratio);
      if (ShellMaterial)
        ShellMaterial.color = Color.Lerp(DeselectColor, SelectShellColor, ratio);
      if (FlowerMaterial)
        FlowerMaterial.color = Color.Lerp(DeselectColor, SelectFlowerColor, ratio);
      foreach (var l in CandleLights)
        l.range = range;
    }

    fadeDuration = Mathf.Clamp(fadeDuration + (selected && !disabled ? Time.deltaTime : -Time.deltaTime), 0.0f, disabled ? disableTime : fadeTime);

    if (disabled && fadeDuration == 0f) {
      Destroy(gameObject);
      transform.localScale = Vector3.zero;
    }
  }

  public override void DisableEvents() {
    Destroy(bevent);
    bevent = null;

    disabled = true;
    disabledscale = transform.localScale;
    fadeDuration = disableTime;
  }

}
