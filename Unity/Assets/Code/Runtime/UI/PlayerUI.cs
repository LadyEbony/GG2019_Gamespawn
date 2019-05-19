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
  private PlayerController mainPC;

  [Header("UI")]
  private Vector3 positionMainBase;
  private Vector3 positionOtherBase;
  private Vector3 positionOtherOffset;
  public float imageScale = 0.6f;
  public float transitionTime = 0.25f;

  private class SelectBundle{
    public PlayerController pc;
    public Transform transform;
    public Image image;
    public Coroutine coroutine;

    public SelectBundle(PlayerController pc, Transform transform, Image image){
      this.pc = pc;
      this.transform = transform;
      this.image = image;
      this.coroutine = null;
    }
  }
  private List<SelectBundle> selections;

  private void Start() {
    var selected = PlayerSwitch.instance.Selected;
    var additional = GlobalList<PlayerController>.GetList.Where(s => s != selected);

    mainPC = selected;
    selections = new List<SelectBundle>();

    var selected_ts = selectionGameObject.transform;
    var selected_im = selectionGameObject.GetComponent<Image>();
    selected_im.sprite = selected.manager.Icon;
    selections.Add(new SelectBundle(selected, selected_ts, selected_im));

    mainText.text = selected.manager.CharacterName;
    mainBackdrop.color = selected.manager.IconColor;

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

  private void UpdateIcons(){
    foreach(var s in selections){
      if (s.coroutine != null) StopCoroutine(s.coroutine);
    }

    var selected = PlayerSwitch.instance.Selected;
    var count = selections.Count;

    SelectBundle first = selections[0];
    int firstIndex = 0;
    for(var i = 0; i < count; i++){
      if (selections[i].pc == selected){
        first = selections[i];
        firstIndex = i;
        break;
      }
    }

    first.coroutine = StartCoroutine(UpdateIconCoroutine(first, positionMainBase, Vector3.one));
    mainText.text = first.pc.manager.CharacterName;
    mainBackdrop.color = first.pc.manager.IconColor;

    var j = 0;
    for(var i = (firstIndex + 1) % count; i != firstIndex; i = (i + 1) % count){
      first = selections[i];
      first.coroutine = StartCoroutine(UpdateIconCoroutine(first, positionOtherBase + j * positionOtherOffset, Vector3.one * imageScale));
      j++;
    }

  }

  private IEnumerator UpdateIconCoroutine(SelectBundle bundle, Vector3 destination, Vector3 scale){
    var icon = bundle.transform;
    icon.SetAsFirstSibling();

    var ogpos = icon.position;
    var ogsc = icon.localScale;
    var time = 0.0f;
    while (time < transitionTime) {
      var ratio = time / transitionTime;
      icon.position = Vector3.Lerp(ogpos, destination, ratio);
      icon.localScale = Vector3.MoveTowards(ogsc, scale, ratio);
      time += Time.deltaTime;
      yield return null;
    }
    icon.position = destination;
    icon.localScale = scale;
  }

  private void LateUpdate() {
    var selected = PlayerSwitch.instance.Selected;

    if (mainPC != selected){
      mainPC = selected;
      UpdateIcons();
    }
  }
}
