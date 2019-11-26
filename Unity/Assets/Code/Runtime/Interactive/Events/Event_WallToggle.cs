using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Event_WallToggle : InteractiveEvent {

  // Animations
  private new GameObject collider;
  private Transform wall;
  private ParticleSystem gem;

  private Coroutine wallCoroutine;

  public float speed = 12f;
  public float height = -3f;

  private void Update() {
    if (Input.GetKeyDown(KeyCode.R)){
      Interact(null, null);
    }
  }

  private void Start() {
    collider = transform.Find("Collider").gameObject;
    wall = transform.Find("Wall");
    gem = transform.Find("Gem").GetComponentInChildren<ParticleSystem>();
  }

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    var state = collider.activeSelf;

    collider.SetActive(!state);
    foreach (var surface in NavMeshSurface.activeSurfaces)
      surface.BuildNavMesh();

    if (wallCoroutine != null) StopCoroutine(wallCoroutine);
    // wall was up, now going down
    if (state){
      wallCoroutine = StartCoroutine(WallAnimation(height));
      gem.Play();
    } 
    // wall was down, now going up
    else {
      wallCoroutine = StartCoroutine(WallAnimation(0f));
      gem.Stop();
    }

    /*
    foreach(var wall in Walls){
      var collider = wall.GetComponent<BoxCollider>();
      var boxes = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, 1 << LayerMask.NameToLayer("Item"));

      var navmeshsize = Mathf.Max(collider.size.x, collider.size.z);

      foreach (var box in boxes) {
        box.GetComponent<Item>().Bounce(navmeshsize);
      }
    }
    */

  }

  IEnumerator WallAnimation(float y){
    var gotopos = new Vector3(0f, y, 0f);
    while(Vector3.SqrMagnitude(wall.localPosition - gotopos) > 0f){
      yield return null;
      wall.localPosition = Vector3.MoveTowards(wall.localPosition, gotopos, speed * Time.deltaTime);
    }
    wallCoroutine = null;
  }

}
