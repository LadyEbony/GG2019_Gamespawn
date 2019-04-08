using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

  public PlayerAbility MouseOveride;

  private NavMeshAgent nva;
  private PlayerAbility[] abilities;

  private Vector3 velocity;

  public int DisableInput = 0;
  public int DisableMovement = 0;
  public int DisableRotation = 0;
  public int DisableSwitch = 0;

  private void Awake() {
    nva = GetComponent<NavMeshAgent>();
    if (!nva) Debug.LogFormat("Missing Nav Mesh Agent component");

    abilities = transform.Find("Abilities").GetComponentsInChildren<PlayerAbility>();
  }

  private void OnEnable() {
    PlayerSwitch.instance.AddCharacter(this);
  }

  private void OnDisable() {
    PlayerSwitch.instance.RemoveCharacter(this);
  }

  private void Update() {
    var selected = PlayerSwitch.instance.Selected == this;

    // Get input
    Vector3 input;
    if (selected){
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

    if (directionalInput != Vector3.zero){
      transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionalInput, Vector3.up),
        nva.angularSpeed * Time.deltaTime
      );
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

}
