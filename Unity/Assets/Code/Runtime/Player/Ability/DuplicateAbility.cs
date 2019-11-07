using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAbility : PlayerAbility {

  public GameObject dup;
  private ItemAbility itemAbility;

  private void OnEnable() {
    itemAbility = interactive.GetComponentInChildren<ItemAbility>();
    if (itemAbility == null) Debug.LogErrorFormat("{0} does not have an item ability.", player);
  }

  private void OnDisable() {
    
  }

  public override void UpdateSimulate(bool selected) {
    
  }

  public override void FixedSimulate(bool selected) {
    var pc = player;

    var e_input = PlayerInput.instance.eInput;
    if (e_input.IsDown()){
      var item = ItemHolder.Has(itemAbility);
      if (item){
        if (dup) Destroy(dup);
        dup = Instantiate(item.gameObject);

        var comp = dup.GetComponent<Item>();
        comp.Drop(pc);
        comp.DisableInteraction();
      }
    }
  }

}
