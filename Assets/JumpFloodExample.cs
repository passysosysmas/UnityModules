using UnityEngine;
using Leap.Unity;

[ExecuteInEditMode]
public class JumpFloodExample : MonoBehaviour {
  public const int PASS_INIT = 0;
  public const int PASS_JUMP = 1;
  public const int PASS_COMPOSITE = 2;

  public Shader jumpFloodShader;

  [Range(1, 12)]
  public int steps = 512;

  private Material _jumpFloodMat;

  private void tryInitMaterial() {
    if (_jumpFloodMat != null) {
      return;
    }

    if (jumpFloodShader == null) {
      return;
    }

    _jumpFloodMat = new Material(jumpFloodShader);
    _jumpFloodMat.hideFlags = HideFlags.HideAndDontSave;
    _jumpFloodMat.name = "Jump Flood Material";
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination) {
    tryInitMaterial();

    if (_jumpFloodMat == null) {
      Graphics.Blit(source, destination);
      return;
    }

    RenderTexture tex0 = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
    RenderTexture tex1 = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
    tex0.wrapMode = TextureWrapMode.Clamp;
    tex1.wrapMode = TextureWrapMode.Clamp;

    Graphics.Blit(source, tex0, _jumpFloodMat, PASS_INIT);

    int step = Mathf.RoundToInt(Mathf.Pow(2, steps - 1));
    while (step != 0) {
      _jumpFloodMat.SetFloat("_Step", step);
      Graphics.Blit(tex0, tex1, _jumpFloodMat, PASS_JUMP);

      Utils.Swap(ref tex0, ref tex1);

      step /= 2;
    }

    Graphics.Blit(source, destination);
    Graphics.Blit(tex0, destination, _jumpFloodMat, PASS_COMPOSITE);

    RenderTexture.ReleaseTemporary(tex0);
    RenderTexture.ReleaseTemporary(tex1);
  }
}
