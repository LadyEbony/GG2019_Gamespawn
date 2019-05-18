using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;
using GameSpawn;

public class PlayerController : MonoBehaviour, IWeight, IItem {

  public PlayerAbility MouseOveride;

  public NavMeshAgent nva { get; private set; }
  public PlayerManager manager { get; private set; }
  private PlayerAbility[] abilities;

  private Vector3 velocity;

  [Header("Inputs")]
  public int DisableInput = 0;
  public int DisableMovement = 0;
  public int DisableRotation = 0;
  public int DisableSwitch = 0;

  [SerializeField] private int disableNavMesh = 0;
  public int DisableNavMesh{
    get {
      return disableNavMesh;
    } set {
      disableNavMesh = value;
      nva.enabled = disableNavMesh == 0;
    }
  }

  public CylinderTriggerBounds InteractiveBounds { get; private set; }

  private void Awake() {
    nva = GetComponent<NavMeshAgent>();
    manager = GetComponent<PlayerManager>();
    if (!nva) Debug.LogFormat("Missing Nav Mesh Agent component");

    abilities = transform.Find("Abilities").GetComponentsInChildren<PlayerAbility>();

    InteractiveBounds = GetComponent<CylinderTriggerBounds>();
    if (InteractiveBounds == null) Debug.LogErrorFormat("{0} does not contain a Bounds.", gameObject.name);
  }

  private void OnEnable() {
    GlobalList<PlayerController>.Add(this);
  }

  private void OnDisable() {
    GlobalList<PlayerController>.Remove(this);
  }

  private void Update() {
    var selected = PlayerSwitch.instance.Selected == this;

    if (DisableNavMesh == 0) {
      // Get input
      Vector3 input;
      if (selected) {
        input = PlayerInput.instance.GetDirectionInput;
      } else {
        input = Vector3.zero;
      }

      // Acceleration and Movement
      var movementInput = DisableMovement == 0 ? input : Vector3.zero;
      velocity = Vector3.MoveTowards(velocity,
        movementInput * nva.speed,
        Time.deltaTime * nva.acceleration
      );

      nva.Move(velocity * Time.deltaTime);

      var directionalInput = DisableRotation == 0 ? input : Vector3.zero;

      if (directionalInput != Vector3.zero) {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionalInput, Vector3.up),
          nva.angularSpeed * Time.deltaTime
        );
      }
    }

    foreach (var ab in abilities) {
      ab.UpdateSimulate(selected);
    }
    
  }

  private void FixedUpdate() {
    var selected = PlayerSwitch.instance.Selected == this;

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
