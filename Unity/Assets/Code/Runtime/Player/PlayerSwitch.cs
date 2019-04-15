using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using GameSpawn;

public class PlayerSwitch : MonoBehaviour
{
  public static PlayerSwitch instance { get; private set; }

  public PlayerController Selected { get; private set; }
  public List<PlayerController> characters { get { return GlobalList<PlayerController>.GetListUnsafe; } }

  private void Awake() {
    if (instance){
      Destroy(this);
      return;
    }
    instance = this;
  }

  private void Start() {
    var characters = this.characters;

    if (characters.Count == 0){
      Debug.Log("No player controllers in scene");
    } else {
      Selected = characters[0];
    }
  }

  private void FixedUpdate() {
    var player = PlayerInput.instance;
    var characters = this.characters;
    var characterCount = characters.Count;

    if (player.DisableInput == 0 && player.DisableSwitch == 0){
      if (player.oneInput.IsDown() && characterCount >= 1){
        Selected = characters[0];
      } else if (player.twoInput.IsDown() && characterCount >= 2) {
        Selected = characters[1];
      } else if (player.threeInput.IsDown() && characterCount >= 3) {
        Selected = characters[2];
      } else if (player.tabInput.IsDown() && characterCount >= 2) {
        var index = characters.IndexOf(Selected);
        if (index == -1) {
          Selected = characters[0];
        } else {
          Selected = characters[(index + 1) % characters.Count];
        }
      }

    }

  }

}
