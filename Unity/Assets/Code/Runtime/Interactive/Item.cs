using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

public class Item : Interactive, IItem, IWeight {
  
  public Rigidbody Rigidbody { get; private set; }

  public Material MaterialInner { get; private set; }
  public ParticleSystem Particle { get; private set; }

  private InteractiveEvent pickupEvent;
  private InteractiveEvent dropEvent;

  private bool weighted;
  private PlayerController selectedpc;
  private Color selectedcolor;

  [Header("Selection")]
  [SerializeField] private Color StandardColor = new Color(1, 1, 1);
  [SerializeField] private Color WeightedColor = new Color(1, 1, 1);
  [SerializeField] private float fadeTime = 0.25f;
  private float fadeDuration = 0.0f;

  [Header("Bounce")]
  [SerializeField] private float bounceTime = 0.25f;
  [SerializeField] private float bounceHeight = 0.5f;

  public override void Awake() {
    base.Awake();

    Rigidbody = GetComponent<Rigidbody>();

    var mesht = transform.Find("Inner");
    if (mesht) {
      MaterialInner = mesht.GetComponent<MeshRenderer>().material;
    }
    Particle = GetComponentInChildren<ParticleSystem>(true);

    var events = GetComponents<InteractiveEvent>();
    if (events.Length >= 1) pickupEvent = events[0];
    if (events.Length >= 2) dropEvent = events[1];
  }

  private void Update() {
    if (weighted){
      MaterialInner.color = WeightedColor;
    } else {
      MaterialInner.color = StandardColor;
    }

    if (Particle) {
      var pcolor = Color.Lerp(Color.clear, selectedcolor, fadeDuration / fadeTime);

      var particles = new ParticleSystem.Particle[12];
      var particlesize = Particle.GetParticles(particles);
      
      for(var i = 0; i < particlesize; i++){
        particles[i].startColor = pcolor;
      }

      Particle.SetParticles(particles, particlesize);

      var pmain = Particle.main;
      pmain.startColor = pcolor;

    }

    fadeDuration = Mathf.Clamp(fadeDuration + (selectedpc != null ? Time.deltaTime : -Time.deltaTime), 0.0f, fadeTime);
  }

  public void Pickup(PlayerController pc){
    Rigidbody.velocity = Vector3.zero;
    Rigidbody.angularVelocity = Vector3.zero;
    Rigidbody.useGravity = false;

    gameObject.layer = LayerMask.NameToLayer("ItemPickup");

    if (pickupEvent) pickupEvent.Interact(pc, this);
  }

  public void Drop(PlayerController pc) {
    Rigidbody.velocity = Vector3.zero;
    Rigidbody.angularVelocity = Vector3.zero;
    Rigidbody.useGravity = true;

    gameObject.layer = LayerMask.NameToLayer("Item");

    if (pickupEvent) dropEvent.Interact(pc, this);
  }

  public override void Select(PlayerController pc) {
    selectedpc = pc;
    selectedcolor = pc.manager.IconColor;
  }

  public override void Deselect(PlayerController pc) {
    if (pc == selectedpc) {
      selectedpc = null;
    }
  }

  public void Enter(Weight weight) {
    weighted = true;
  }

  public void Exit(Weight weight) {
    weighted = false;
  }

  public void Bounce(Vector3 destination){
    StartCoroutine(BounceCoroutine(destination));
  }

  private IEnumerator BounceCoroutine(Vector3 destination) {
    Rigidbody.isKinematic = true;
    Rigidbody.detectCollisions = false;
    Rigidbody.velocity = Vector3.zero;
    Rigidbody.angularVelocity = Vector3.zero;
    Rigidbody.useGravity = false;

    gameObject.layer = LayerMask.NameToLayer("ItemPickup");

    var startposition = transform.position;
    var time = 0.0f;
    while(time < 1.0f){
      transform.position = Vector3.Lerp(startposition, destination, time);
      transform.position += new Vector3(0, Mathf.Sin(Mathf.PI * time) * bounceHeight, 0);
      yield return null;

      time += Time.deltaTime / bounceTime;
    }
    transform.position = destination;

    Rigidbody.isKinematic = false;
    Rigidbody.detectCollisions = true;
    Rigidbody.useGravity = true;
    gameObject.layer = LayerMask.NameToLayer("Item");
  }
}
