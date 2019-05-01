using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : WallInteractive {

  private InteractiveEvent bevent;

  public Material OrbMaterial { get; private set; }
  public Light OrbLight { get; private set; }
  public readonly Color SelectColor = new Color(1f, 0f, .76f);
  public readonly Color DeselectColor = new Color(.28f, 0f, .21f);

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
  }

  public override void Interact(PlayerController pc) {
    if (bevent) bevent.Interact(pc, this);
  }

  public override void Select(PlayerController pc) {
    OrbMaterial.SetColor("_Color", SelectColor);
    OrbLight.color = SelectColor;
  }

  public override void Deselect(PlayerController pc) {
    OrbMaterial.SetColor("_Color", DeselectColor);
    OrbLight.color = DeselectColor;
  }
}
