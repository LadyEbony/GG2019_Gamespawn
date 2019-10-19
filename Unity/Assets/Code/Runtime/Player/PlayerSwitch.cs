using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using GameSpawn;

public class PlayerSwitch : MonoBehaviour
{
  public static PlayerSwitch instance { get; private set; }

  public PlayerController selected { get; private set; }
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
      selected = characters[0];
    }
  }

  private void FixedUpdate() {
    var player = PlayerInput.instance;
    var characters = this.characters;
    var characterCount = characters.Count;

    // do the switching
    if (player.disableInput == 0 && player.disableSwitch == 0){
      if (player.oneInput.IsDown() && characterCount >= 1){
        selected = characters[0];
      } else if (player.twoInput.IsDown() && characterCount >= 2) {
        selected = characters[1];
      } else if (player.threeInput.IsDown() && characterCount >= 3) {
        selected = characters[2];
      } else if (player.tabInput.IsDown() && characterCount >= 2) {
        var index = characters.IndexOf(selected);
        if (index == -1) {
          selected = characters[0];
        } else {
          selected = characters[(index + 1) % characters.Count];
        }
      }

    }

  }

  /// <summary>
  /// Instantly switch to <paramref name="pc"/>.
  /// </summary>
  /// <param name="pc"></param>
  public void ForceSelection(PlayerController pc){
    if (pc != null)
      selected = pc;
  }

}
