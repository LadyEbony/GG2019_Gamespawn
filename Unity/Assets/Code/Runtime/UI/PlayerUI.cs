using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using GameSpawn;

using TMPro;

public class PlayerUI : MonoBehaviour {

  [Header("References")]
  private PlayerController mainPC;

  [Header("UI")]
  private Vector3 positionMain;
  public float spacing = 200f;
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
    var selected = PlayerSwitch.instance.selected;

    selections = new List<SelectBundle>();

    var item = transform.GetChild(0).gameObject;
    positionMain = item.transform.position;

    var i = 0;
    foreach(var p in GlobalList<PlayerController>.GetList){
      var man = p.manager;

      var temp = Instantiate(item, positionMain + new Vector3(i * spacing, 0f, 0f), Quaternion.identity, transform);
      var t = temp.transform;
      t.localScale = selected == p ? Vector3.one : Vector3.one * imageScale;

      var image = t.Find("Logic").GetComponent<Image>();
      image.sprite = man.Icon;

      t.Find("Backdrop").GetComponent<Image>().color = man.IconColor;
      t.Find("Text").GetComponent<TextMeshProUGUI>().text = man.CharacterName;

      selections.Add(new SelectBundle(p, t, image));

      i++;
    }

    item.SetActive(false);

  }

  private void UpdateIcons(){
    foreach(var s in selections){
      if (s.coroutine != null) StopCoroutine(s.coroutine);
    }

    var selected = PlayerSwitch.instance.selected;
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

    first.coroutine = StartCoroutine(UpdateIconCoroutine(first, positionMain, Vector3.one));

    var j = 0;
    for(var i = (firstIndex + 1) % count; i != firstIndex; i = (i + 1) % count){
      first = selections[i];
      first.coroutine = StartCoroutine(UpdateIconCoroutine(first, positionMain + new Vector3(spacing * (j + 1), 0, 0), Vector3.one * imageScale));
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
    var selected = PlayerSwitch.instance.selected;

    if (mainPC != selected){
      mainPC = selected;
      UpdateIcons();
    }
  }
}
