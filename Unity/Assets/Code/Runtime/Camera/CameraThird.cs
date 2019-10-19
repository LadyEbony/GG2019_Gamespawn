using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThird : MonoBehaviour {

  [Header("Main Stuff")]
  public float cameraDistance = 3f; // distance from player
  public float smoothTime = 0.3f;
  private Vector3 cameraVelocity = Vector3.zero;

  [Header("Wall Checking")]
  public float hitBuffer = 0.0625f; 
  public LayerMask cameraLayerMask;

  [Header("Rotation")]
  public float clampAngle = 80f;
  public float yawSensitivity = 100f;
  public float pitchSensitivity = 100f;

  private float rotX = 0f;
  private float rotY = 0f;

  private Transform t;

  private void Start() {
    Cursor.lockState = CursorLockMode.Locked;

    t = transform;

    var rot = transform.localRotation.eulerAngles;
    rotX = rot.x;
    rotY = rot.y;
  }

  // Update is called once per frame
  void LateUpdate() {
    var player = PlayerSwitch.instance.selected;

    var horizontal = -Input.GetAxis("Mouse Y");
    var vertical = Input.GetAxis("Mouse X");

    rotX += horizontal * yawSensitivity * Time.deltaTime;
    rotY += vertical * pitchSensitivity * Time.deltaTime;

    rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
    var rot = Quaternion.Euler(rotX, rotY, 0f);
    t.rotation = rot;

    if (player){
      var headPos = player.headTransform.position;
      var dir = rot * Vector3.back;

      // send a ray to walls
      Vector3 cameraPos;
      var ray = new Ray(headPos, dir);
      if (Physics.Raycast(ray, out var hit, cameraDistance, cameraLayerMask)){
        // stop right before a wall (or else, the wall doesn't render)
        cameraPos = hit.point - dir * hitBuffer;
      } else {
        cameraPos = headPos + dir * cameraDistance;
      }

      t.position = Vector3.SmoothDamp(transform.position, cameraPos, ref cameraVelocity, smoothTime);
    }

  }
}
