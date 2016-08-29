using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {


  public ProceduralColor color = Color.green;

  void Update() {
    GetComponent<Renderer>().material.color = color.value;//
  }

}
