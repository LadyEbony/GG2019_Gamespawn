using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

  private Animator animator;
  private int blendId;

  private Transform parent;
  private Vector3 lastPosition;

  public float blendMultiplier;

  private void Awake() {
    animator = GetComponent<Animator>();
    blendId = Animator.StringToHash("Blend");
    parent = transform.parent;

    lastPosition = parent.position;
  }

  // Update is called once per frame
  void Update() {
    if (animator) {
      var pos = parent.position;

      var isMoving = pos != lastPosition;

      var blend = animator.GetFloat(blendId);
      blend = Mathf.MoveTowards(blend, isMoving ? 0 : 1, blendMultiplier * Time.deltaTime);
      animator.SetFloat(blendId, blend);

      lastPosition = pos;
    }
  }
}
