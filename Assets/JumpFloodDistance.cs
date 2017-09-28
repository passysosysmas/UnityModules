using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

[ExecuteInEditMode]
public class JumpFloodDistance : MonoBehaviour {

  public Material jumpMat;
  public int startStep = 32;

  private void OnRenderImage(RenderTexture source, RenderTexture destination) {
    RenderTexture tex0 = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
    RenderTexture tex1 = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
    tex0.wrapMode = TextureWrapMode.Clamp;
    tex1.wrapMode = TextureWrapMode.Clamp;

    Graphics.Blit(source, tex0, jumpMat, 0);

    int step = startStep;
    while (step != 0) {
      jumpMat.SetFloat("_Step", step);
      Graphics.Blit(tex0, tex1, jumpMat, 1);

      Utils.Swap(ref tex0, ref tex1);

      step /= 2;
    }

    Graphics.Blit(source, destination);
    Graphics.Blit(tex0, destination, jumpMat, 2);

    RenderTexture.ReleaseTemporary(tex0);
    RenderTexture.ReleaseTemporary(tex1);
  }
}
