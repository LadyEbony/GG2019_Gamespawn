using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

  // so the question is, why?
  // cause i wanted an easy place to store hold durations

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

    public bool IsUp(){
      return !state && duration == 0.0f;
    }

  }

  public static PlayerInput instance{ get; private set; }

  // Axis
  public AxisInput horizontalInput;
  public AxisInput verticalInput;

  // Abilites
  public ButtonInput eInput;
  public ButtonInput lbInput;
  public ButtonInput rbInput;

  // Switch
  public ButtonInput oneInput;
  public ButtonInput twoInput;
  public ButtonInput threeInput;
  public ButtonInput tabInput;

  // Sprint
  public ButtonInput shiftInput;

  public int disableInput = 0;
  public int disableSwitch = 0;

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

    oneInput = new ButtonInput("One");
    twoInput = new ButtonInput("Two");
    threeInput = new ButtonInput("Three");
    tabInput = new ButtonInput("Tab");

    shiftInput = new ButtonInput("Shift");
  }

  private void FixedUpdate() {
    UpdateAxisInput(ref horizontalInput);
    UpdateAxisInput(ref verticalInput);

    UpdateButtonInput(ref eInput);
    UpdateButtonInput(ref lbInput);
    UpdateButtonInput(ref rbInput);

    UpdateButtonInput(ref oneInput);
    UpdateButtonInput(ref twoInput);
    UpdateButtonInput(ref threeInput);
    UpdateButtonInput(ref tabInput);

    UpdateButtonInput(ref shiftInput);
  }

  private void UpdateAxisInput(ref AxisInput input){
    input.value = disableInput == 0 ? Input.GetAxisRaw(input.axisName) : 0.0f;
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

  public Vector3 GetDirectionInput{
    get {
      var ct = Camera.main.transform;

      // Get the forward and right for the camera, flatten them on the XZ plane, then renormalize
      var fwd = ct.forward;
      var right = ct.right;

      fwd.y = 0;
      right.y = 0;

      fwd = fwd.normalized;
      right = right.normalized;

      var hor = horizontalInput.value;
      var ver = verticalInput.value;

      var delta = hor * right;
      delta += ver * fwd;

      // Clamp magnitude, so going diagonally isn't faster.
      if (delta != Vector3.zero) delta = delta.normalized;

      return delta;
    }
  }

}
