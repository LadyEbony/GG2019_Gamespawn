using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;
using UnityEngine.AI;

public class Item : Interactive, IItem, IWeight {
  
  public new Rigidbody rigidbody { get; private set; }
  public new BoxCollider collider { get; private set; }

  public Material materialInner { get; private set; }
  public Material materialShell { get; private set; }

  public SpriteRenderer spriteRendererMain { get; private set; }
  public SpriteRenderer spriteRendererInner { get; private set; }

  // Special events that occur when the items are picked up
  private InteractiveEvent pickupEvent;
  private InteractiveEvent dropEvent;

  private PlayerController selectedpc;
  private Color selectedcolor;

  private Color standardInnerColor;
  private Color standardShellColor;
  [Header("Selection")]
  public Color alternateInnerColor = new Color(1, 1, 1);
  public Color alternateShellColor = new Color(1, 1, 1);
  public float innerRatio = 0.5f;
  public float fadeTime = 0.25f;
  private float fadeDuration = 0.0f;

  [Header("Bounce")]
  public float bounceTime = 0.25f;
  public float bounceHeight = 0.5f;

  [Header("Additional")]
  public bool weighted;
  public bool duplicate;

  public override void Awake() {
    base.Awake();

    rigidbody = GetComponent<Rigidbody>();
    collider = GetComponent<BoxCollider>();

    var mesht = transform.Find("Inner");
    if (mesht) { 
      materialInner = mesht.GetComponent<MeshRenderer>().material;
      standardInnerColor = materialInner.color;
    }

    var meshs = transform.Find("Shell");
    if (meshs) { 
      materialShell = meshs.GetComponent<MeshRenderer>().material;
      standardShellColor = materialShell.color;
    }

    var spriter = transform.Find("Aura");
    if (spriter){
      spriteRendererMain = spriter.GetComponent<SpriteRenderer>();
      spriteRendererInner = spriter.GetChild(0).GetComponent<SpriteRenderer>();
    }

    var events = GetComponents<InteractiveEvent>();
    if (events.Length >= 1) pickupEvent = events[0];
    if (events.Length >= 2) dropEvent = events[1];
  }

  private void Update() {
    // Box color
    materialInner.color = weighted ? alternateInnerColor : standardInnerColor;
    materialShell.color = duplicate ? alternateShellColor : standardShellColor;

    if (duplicate){
      
    }

    // Selection particles
    var pcolor = Color.Lerp(Color.clear, selectedcolor, fadeDuration / fadeTime);
    spriteRendererMain.color = pcolor;

    pcolor = Color.Lerp(pcolor, new Color(1f, 1f, 1f, pcolor.a), innerRatio);
    spriteRendererInner.color = pcolor;
    

    // Out of bounds
    if (transform.position.y <= -1f){
      Bounce(5.0f);
    }

    fadeDuration = Mathf.Clamp(fadeDuration + (selectedpc != null ? Time.deltaTime : -Time.deltaTime), 0.0f, fadeTime);
  }

  /// <summary>
  /// The <paramref name="pc"/> just picked up this item.
  /// </summary>
  /// <param name="pc"></param>
  public void Pickup(PlayerController pc){
    rigidbody.velocity = Vector3.zero;
    rigidbody.angularVelocity = Vector3.zero;
    rigidbody.useGravity = false;

    gameObject.layer = LayerMask.NameToLayer("ItemPickup");

    if (pickupEvent) pickupEvent.Interact(pc, this);

    if (bounce != null) StopCoroutine(bounce);
  }

  /// <summary>
  /// The <paramref name="pc"/> just dropped this item.
  /// </summary>
  /// <param name="pc"></param>
  public void Drop(PlayerController pc) {
    rigidbody.velocity = Vector3.zero;
    rigidbody.angularVelocity = Vector3.zero;
    rigidbody.useGravity = true;

    gameObject.layer = LayerMask.NameToLayer("Item");

    if (pickupEvent) dropEvent.Interact(pc, this);
  }

  public override void Select(PlayerController pc) {
    selectedpc = pc;
    selectedcolor = pc.manager.IconColor;
  }

  public override void Deselect(PlayerController pc) {
    if (pc == selectedpc) {
      selectedpc = null;
    }
  }

  public void Enter(Weight weight) {
    weighted = true;
  }

  public void Exit(Weight weight) {
    weighted = false;
  }

  public void EnableInteraction(){
    if (disablePlayer == 0){
      gameObject.layer = LayerMask.NameToLayer("ItemPickup");
    }
    disablePlayer++;
  }

  public void DisableInteraction(){
    disablePlayer--;
    if (disablePlayer == 0){
      gameObject.layer = LayerMask.NameToLayer("Item");
    }
  }

  private Coroutine bounce;
  public void Bounce(float range){
    NavMeshHit hit;
    if (NavMesh.SamplePosition(transform.position, out hit, range, NavMesh.AllAreas)) {
      bounce = StartCoroutine(BounceCoroutine(hit.position));
    } 
  }

  private IEnumerator BounceCoroutine(Vector3 destination) {
    rigidbody.isKinematic = true;
    rigidbody.detectCollisions = false;
    rigidbody.velocity = Vector3.zero;
    rigidbody.angularVelocity = Vector3.zero;
    rigidbody.useGravity = false;

    gameObject.layer = LayerMask.NameToLayer("ItemPickup");

    var startposition = transform.position;
    var startrotation = transform.rotation;
    var finalrotation = GetClosestVector(transform.rotation, Vector3.up);
    var time = 0.0f;
    while(time < 1.0f){
      transform.position = Vector3.Lerp(startposition, destination, time);
      transform.position += new Vector3(0, Mathf.Sin(Mathf.PI * time) * bounceHeight, 0);
      transform.rotation = Quaternion.Slerp(startrotation, finalrotation, time);
      yield return null;

      time += Time.deltaTime / bounceTime;
    }
    transform.position = destination;
    transform.rotation = finalrotation;

    rigidbody.isKinematic = false;
    rigidbody.detectCollisions = true;
    rigidbody.useGravity = true;
    gameObject.layer = LayerMask.NameToLayer("Item");

    bounce = null;
  }

  private Quaternion GetClosestVector(Quaternion direction, Vector3 upVector){
    Vector3 final = Vector3.forward;
    float dot = 0.0f;

    Vector3 tempv;
    float tempd;

    tempv = direction * Vector3.forward;
    tempd = Vector3.Dot(tempv, upVector);
    if (tempd > dot){
      final = tempv;
      dot = tempd;
    }

    tempv = direction * Vector3.back;
    tempd = Vector3.Dot(tempv, upVector);
    if (tempd > dot) {
      final = tempv;
      dot = tempd;
    }

    tempv = direction * Vector3.right;
    tempd = Vector3.Dot(tempv, upVector);
    if (tempd > dot) {
      final = tempv;
      dot = tempd;
    }

    tempv = direction * Vector3.left;
    tempd = Vector3.Dot(tempv, upVector);
    if (tempd > dot) {
      final = tempv;
      dot = tempd;
    }

    tempv = direction * Vector3.up;
    tempd = Vector3.Dot(tempv, upVector);
    if (tempd > dot) {
      final = tempv;
      dot = tempd;
    }

    tempv = direction * Vector3.down;
    tempd = Vector3.Dot(tempv, upVector);
    if (tempd > dot) {
      final = tempv;
      dot = tempd;
    }

    return Quaternion.LookRotation(direction * Vector3.forward, final);
  }

  public override Vector3 CenterPosition => collider.bounds.center;

}
