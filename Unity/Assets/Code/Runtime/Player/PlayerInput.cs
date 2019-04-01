using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
  public struct AxisInput {
    public string axisName;
    public float value;

    public AxisInput(string axis) {
      axisName = axis;
      value = 0.0f;
    }
  }

  public struct ButtonInput{
    public string buttonName;
    public float duration;
    public bool state;

    public ButtonInput(string button){
      buttonName = button;
      duration = 0.0f;
      state = false;
    }

    public bool IsDown(){
      return state && duration == 0.0f;
    }

    public bool IsDown(float duration){
      return state && duration >= this.duration;
    }
  }

  public static PlayerInput instance{ get; private set; }

  public AxisInput horizontalInput;
  public AxisInput verticalInput;

  public ButtonInput eInput;
  public ButtonInput lbInput;
  public ButtonInput rbInput;

  public int DisableInput = 0;
  public int DisableMovement = 0;
  public int DisableRotation = 0;

  private void Awake() {
    // Set singleton
    if (instance){
      Destroy(this);
      return;
    }
    instance = this;

    horizontalInput = new AxisInput("Horizontal");
    verticalInput = new AxisInput("Vertical");

    eInput = new ButtonInput("Fire3");
    lbInput = new ButtonInput("Fire1");
    rbInput = new ButtonInput("Fire2");
  }

  private void FixedUpdate() {
    UpdateAxisInput(ref horizontalInput);
    UpdateAxisInput(ref verticalInput);

    UpdateButtonInput(ref eInput);
    UpdateButtonInput(ref lbInput);
    UpdateButtonInput(ref rbInput);
  }

  private void UpdateAxisInput(ref AxisInput input){
    input.value = DisableInput == 0 ? Input.GetAxisRaw(input.axisName) : 0.0f;
  }

  private void UpdateButtonInput(ref ButtonInput input){
    bool state = Input.GetButton(input.buttonName);

    // Held down
    if (input.state == state){
      input.duration += Time.fixedUnscaledDeltaTime;
    } else {
      input.duration = 0.0f;
    }

    input.state = state;
  }

  public Vector3 GetMovementInput {
    get {
      return DisableMovement == 0 ? GetDirectionInput : Vector3.zero;
    }
  }

  public Vector3 GetRotationInput {
    get {
      return DisableRotation == 0 ? GetDirectionInput : Vector3.zero;
    }
  }

  public Vector3 GetDirectionInput{
    get {
      return new Vector3(horizontalInput.value, 0.0f, verticalInput.value).normalized;
    }
  }

}
