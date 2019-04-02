using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

  public PlayerAbility MouseOveride;

  private NavMeshAgent nva;
  private PlayerAbility[] abilities;

  private Vector3 velocity;

  private void Awake() {
    nva = GetComponent<NavMeshAgent>();
    if (!nva) Debug.LogFormat("Missing Nav Mesh Agent component");

    abilities = transform.Find("Abilities").GetComponentsInChildren<PlayerAbility>();
  }

  private void Update() {
    var player = PlayerInput.instance;
    Vector3 input;

    // Acceleration and Movement
    input = player.GetMovementInput;
    velocity = Vector3.MoveTowards(velocity,
        input * nva.speed,
        Time.deltaTime * nva.acceleration
      );

    nva.Move(velocity * Time.deltaTime);

    // Turnrate
    input = player.GetRotationInput;
    if (input != Vector3.zero) {
      transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(input, Vector3.up),
        nva.angularSpeed * Time.deltaTime
      );
    }

    foreach (var ab in abilities) {
      ab.UpdateSimulate(this);
    }
    
  }

  private void FixedUpdate() {
    foreach(var ab in abilities){
      ab.FixedSimulate(this);
    }
  }

}
