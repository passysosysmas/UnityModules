using UnityEngine;

public class ChaoticRotator : MonoBehaviour {
  public float speed = 5f;

  private Vector3 _v1;

  private Vector3 _v2;

  public void Awake() {
    this._v1 = UnityEngine.Random.onUnitSphere;
    this._v2 = Vector3.Cross(this._v1, UnityEngine.Random.onUnitSphere);
  }

  private void Update() {
    this._v1 = Quaternion.AngleAxis(-1f, this._v2) * this._v1;
    this._v2 = Quaternion.AngleAxis(1.3f, this._v1) * this._v2;
    base.transform.rotation = Quaternion.AngleAxis(this.speed * Vector3.Dot(this._v1, Vector3.up), this._v2) * base.transform.rotation;
    base.transform.rotation = Quaternion.AngleAxis(this.speed * Vector3.Dot(this._v2, Vector3.up), this._v1) * base.transform.rotation;
  }

  private void OnDrawGizmosSelected() {
    Gizmos.DrawLine(base.transform.position, base.transform.position + this._v1);
    Gizmos.DrawLine(base.transform.position, base.transform.position + this._v2);
  }
}
