using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using GameSpawn;

using TMPro;

public class PlayerUI : MonoBehaviour {

  [Header("References")]
  public TextMeshProUGUI mainText;
  public Image mainBackdrop;
  public GameObject selectionGameObject;

  [Header("UI")]
  public Vector3 positionMainBase;
  public Vector3 positionOtherBase;
  public Vector3 positionOtherOffset;
  public float imageScale = 0.6f;

  private struct SelectBundle{
    public PlayerController pc;
    public Transform transform;
    public Image image;

    public SelectBundle(PlayerController pc, Transform transform, Image image){
      this.pc = pc;
      this.transform = transform;
      this.image = image;
    }
  }
  private List<SelectBundle> selections; 

  private void Start() {
    var selected = PlayerSwitch.instance.Selected;
    var additional = GlobalList<PlayerController>.GetList.Where(s => s != selected);

    selections = new List<SelectBundle>();

    var selected_ts = selectionGameObject.transform;
    var selected_im = selectionGameObject.GetComponent<Image>();
    selected_im.sprite = selected.manager.Icon;
    selections.Add(new SelectBundle(selected, selected_ts, selected_im));

    positionMainBase = selected_ts.position;

    var width = Vector3.right * selectionGameObject.GetComponent<RectTransform>().rect.width;
    positionOtherBase = positionMainBase +  width * (3 / 4f);
    positionOtherOffset = width * imageScale;
    var i = 0;
    foreach(var a in additional){
      var temp = Instantiate(selectionGameObject, positionOtherBase + i * positionOtherOffset, Quaternion.identity, transform);
      var temp_ts = temp.transform;
      var temp_im = temp.GetComponent<Image>();
      temp_ts.SetAsFirstSibling();
      temp_ts.localScale = Vector3.one * imageScale;
      temp_im.sprite = a.manager.Icon;
      selections.Add(new SelectBundle(a, temp.transform, temp_im));
      i++;
    }

  }

  private void LateUpdate() {
    var selected = PlayerSwitch.instance.Selected;

    var first = selections.First(s => s.pc == selected);
    var firstIndex = selections.IndexOf(first);
    first.transform.position = positionMainBase;
    first.transform.localScale = Vector3.one;

    mainText.text = first.pc.manager.CharacterName;
    mainBackdrop.color = first.pc.manager.IconColor;

    var j = 0;
    for(var i = (firstIndex + 1) % selections.Count; i != firstIndex; i = (i + 1) % selections.Count){
      selections[i].transform.position = positionOtherBase + j * positionOtherOffset;
      selections[i].transform.localScale = Vector3.one * imageScale;
      j++;
    }

  }
}
