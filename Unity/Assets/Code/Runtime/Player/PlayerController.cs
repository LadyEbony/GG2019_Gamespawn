using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;
using GameSpawn;

public class PlayerController : MonoBehaviour, IWeight, IItem {

  /// <summary>
  /// The ability that overrides control of the mouse.
  /// </summary>
  public PlayerAbility mouseOveride;
  public float sprintModifier = 1.5f;

  public NavMeshAgent nva { get; private set; }
  public PlayerManager manager { get; private set; }
  public Animator animator { get; private set; }
  public PlayerAbility[] abilities { get; private set; }

  private Vector3 velocity; // we store our own velocity bbi

  /// <summary>
  /// Disables all local movement input if > 0.
  /// </summary>
  [Header("Inputs")]
  public int disableMovement = 0;
  /// <summary>
  /// Disables all local rotation input if > 0.
  /// </summary>
  public int disableRotation = 0;

  /// <summary>
  /// Disables the nav mesh agent if > 0.
  /// </summary>
  [SerializeField] private int _disableNavMesh = 0;
  public int disableNavMesh{
    get {
      return _disableNavMesh;
    } set {
      _disableNavMesh = value;
      nva.enabled = _disableNavMesh == 0;
    }
  }

  /// <summary>
  /// Disables the renderer if > 0.
  /// </summary>
  [SerializeField] private int _disableRenderer = 0;
  public int disableRenderer {
    get {
      return _disableRenderer;
    }
    set {
      _disableRenderer = value;
      animator.transform.localScale = _disableRenderer == 0 ? Vector3.one : Vector3.zero;
    }
  }

  /// <summary>
  /// The player bounds for custom scripts.
  /// </summary>
  public CylinderTriggerBounds interactiveBounds { get; private set; }
  /// <summary>
  /// The player head.
  /// </summary>
  public Transform headTransform { get; private set; }

  private void Awake() {
    nva = GetComponent<NavMeshAgent>();
    manager = GetComponent<PlayerManager>();
    if (!nva) Debug.LogFormat("Missing Nav Mesh Agent component");

    abilities = transform.Find("Abilities").GetComponentsInChildren<PlayerAbility>();

    animator = transform.GetFastComponentInChildren<Animator>();
    if (animator == null) Debug.LogErrorFormat("{0} does not an Animator", gameObject.name);

    interactiveBounds = GetComponent<CylinderTriggerBounds>();
    if (interactiveBounds == null) Debug.LogErrorFormat("{0} does not contain a Bounds.", gameObject.name);

    headTransform = animator.transform.Find("head");
    if (headTransform == null) Debug.LogErrorFormat("{0} does not a head Transform in Renderer", gameObject.name);
  }

  private void OnEnable() {
    GlobalList<PlayerController>.Add(this);
  }

  private void OnDisable() {
    GlobalList<PlayerController>.Remove(this);
  }

  private void Update() {
    var selected = PlayerSwitch.instance.selected == this;

    if (disableNavMesh == 0) {
      // Get input
      Vector3 input;
      if (selected) {
        input = PlayerInput.instance.GetDirectionInput;
      } else {
        input = Vector3.zero;
      }

      // Acceleration and Movement
      var movementInput = disableMovement == 0 ? input : Vector3.zero;
      velocity = Vector3.MoveTowards(velocity,
        movementInput * nva.speed * (PlayerInput.instance.shiftInput.state == true ? sprintModifier : 1f),
        Time.deltaTime * nva.acceleration
      );

      nva.Move(velocity * Time.deltaTime);

      // Rotation
      var directionalInput = disableRotation == 0 ? input : Vector3.zero;

      if (directionalInput != Vector3.zero) {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionalInput, Vector3.up),
          nva.angularSpeed * Time.deltaTime
        );
      }
    }

    // Simulate abilities.
    foreach (var ab in abilities) {
      ab.UpdateSimulate(selected);
    }
    
  }

  private void FixedUpdate() {
    var selected = PlayerSwitch.instance.selected == this;

    // Simulate abilites.
    foreach (var ab in abilities){
      ab.FixedSimulate(selected);
    }
  }

  public void Drop(PlayerController pc) {
    throw new System.NotImplementedException();
  }

  public void Pickup(PlayerController pc) {
    throw new System.NotImplementedException();
  }

  public void Enter(Weight weight) {
    return;
  }

  public void Exit(Weight weight) {
    return;
  }


}
