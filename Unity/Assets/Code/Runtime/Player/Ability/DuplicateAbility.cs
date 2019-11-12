using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAbility : PlayerAbility {

  [Header("Debug")]
  public Item originalItem;
  public Item duplicateItem;
  private ItemAbility itemAbility;

  [Header("Particles")]
  public GameObject linkPrefab;
  public float linkSpeed = 10f;
  private Transform linkTransform;
  private Transform linkDestination;

  [Header("Sprites")]
  public Sprite createSprite;
  public Sprite createFailSprite;
  public Sprite destroySprite;

  public override void Awake() {
    base.Awake();

    itemAbility = interactive.GetComponentInChildren<ItemAbility>();
    if (itemAbility == null) Debug.LogErrorFormat("{0} does not have an item ability.", player);
  }

  public override void UpdateSimulate(bool selected) {
    if (originalItem){
      linkTransform.position = Vector3.MoveTowards(linkTransform.position, linkDestination.position, linkSpeed * Time.deltaTime);

      if (Vector3.SqrMagnitude(linkTransform.position - linkDestination.position) < 0.25f){
        linkDestination = originalItem.transform == linkDestination ? duplicateItem.transform : originalItem.transform;
      }
    }
  }

  public override void FixedSimulate(bool selected) {
    if (!selected) return;

    var pc = player;
    var heldItem = ItemHolder.Has(itemAbility);

    if (heldItem){
      var e_input = PlayerInput.instance.eInput;
      
      // dup exists, so we are deleting it.
      if (duplicateItem){

        // we must be holding it
        Item del;
        if (heldItem == duplicateItem) del = originalItem;
        else if (heldItem == originalItem) del = duplicateItem;
        else {
          ControlUI.Instance.eInput.SetSprite(createFailSprite);
          return;
        }

        if (e_input.IsDown()){
          // forcefully drop it from the holder
          var h = ItemHolder.Has(del);
          if (h) h.Drop(h.player);
          
          originalItem.duplicate = false;
          duplicateItem.duplicate = false;

          Destroy(del.gameObject);
          Destroy(linkTransform.gameObject, 1f);

          originalItem = null;
          duplicateItem = null;
        }
        ControlUI.Instance.eInput.SetSprite(destroySprite);
      } else {
        if (e_input.IsDown()){
          originalItem = heldItem;
          duplicateItem = Instantiate(heldItem.gameObject).GetComponent<Item>();
          duplicateItem.Drop(pc);

          originalItem.duplicate = true;
          duplicateItem.duplicate = true;

          linkTransform = Instantiate(linkPrefab, originalItem.transform.position, Quaternion.identity).transform;
          linkDestination = duplicateItem.transform;
        }
        ControlUI.Instance.eInput.SetSprite(createSprite);
      }
    } else {
      ControlUI.Instance.eInput.SetSprite(null);
    }
  }

}
