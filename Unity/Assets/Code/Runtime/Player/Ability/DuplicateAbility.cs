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
  private Transform linkTransform;
  public float linkTime = 1f;
  public float linkCurrentTime = 0f;
  public bool linkIncrementing = true;

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
      var start = originalItem.CenterPosition;
      var end = duplicateItem.CenterPosition;

      linkTransform.position = linkIncrementing ? start : end;
      linkIncrementing = !linkIncrementing;

      return;
      linkTransform.position = Vector3.Lerp(start, end, linkCurrentTime / linkTime);

      if (linkIncrementing){
        linkCurrentTime += Time.deltaTime;
        if (linkCurrentTime >= linkTime){
          linkCurrentTime = linkTime;
          linkIncrementing = false;
        }
      } else {
        linkCurrentTime -= Time.deltaTime;
        if (linkCurrentTime <= 0){
          linkCurrentTime = 0f;
          linkIncrementing = true;
        }
      }
    }
  }

  public override void FixedSimulate(bool selected) {
    if (!selected) return;

    var pc = player;
    var heldItem = ItemHolder.Has(itemAbility);

    var e_input = PlayerInput.instance.eInput;

    // dup exists, we deleting
    if (duplicateItem) {
      if (e_input.IsDown()){
        // forcefully drop it from the holder
        var h = ItemHolder.Has(duplicateItem);
        if (h) h.Drop(h.player);

        Destroy(duplicateItem.gameObject);
        Destroy(linkTransform.gameObject, 1f);

        originalItem = null;
        duplicateItem = null;
      }

      ControlUI.Instance.eInput.SetSprite(destroySprite);
    }
    // creating dup 
    else if (heldItem) {
      if (e_input.IsDown()) {
        originalItem = heldItem;
        duplicateItem = Instantiate(heldItem.gameObject).GetComponent<Item>();
        duplicateItem.Drop(pc);

        //originalItem.duplicate = false;
        duplicateItem.duplicate = true;

        linkTransform = Instantiate(linkPrefab, originalItem.transform.position, Quaternion.identity).transform;
        linkIncrementing = true;
      }
      ControlUI.Instance.eInput.SetSprite(createSprite);
    } else {
      ControlUI.Instance.eInput.SetSprite(null);
    }

  }

}
