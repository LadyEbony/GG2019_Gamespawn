using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
  public static PlayerSwitch instance { get; private set; }

  public PlayerController Selected { get; private set; }
  private List<PlayerController> characters;

  private void Awake() {
    if (instance){
      Destroy(this);
      return;
    }
    instance = this;
  }

  private void Start() {
    if (characters.Count == 0){
      Debug.Log("No player controllers in scene");
    } else {
      Selected = characters[0];
    }
  }

  private void FixedUpdate() {
    var player = PlayerInput.instance;

    if (player.DisableInput == 0 && player.DisableSwitch == 0){
      if (player.oneInput.IsDown() && characters.Count >= 1){
        Selected = characters[0];
      } else if (player.twoInput.IsDown() && characters.Count >= 2) {
        Selected = characters[1];
      } else if (player.threeInput.IsDown() && characters.Count >= 3) {
        Selected = characters[2];
      } else if (player.tabInput.IsDown() && characters.Count >= 2) {
        var index = characters.IndexOf(Selected);
        if (index == -1) {
          Selected = characters[0];
        } else {
          Selected = characters[(index + 1) % characters.Count];
        }
      }

    }

  }

  public void AddCharacter(PlayerController pc) {
    if (characters == null) characters = new List<PlayerController>();
    if (!characters.Contains(pc)) characters.Add(pc);
  }

  public void RemoveCharacter(PlayerController pc) {
    if (characters == null) characters = new List<PlayerController>();
    characters.Remove(pc);
  }


}
