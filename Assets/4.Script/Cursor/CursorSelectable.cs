using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursorSelectable : MonoBehaviour
{
  // 커서 타겟의 CaharacterControl
  [HideInInspector] public PlayerControl player;
  public CursorType cursorType;
  public List<Renderer> meshRenders = new List<Renderer>();


  public void SetupRenderer()
  {
    // CharacterControl이 있으면 사용.
    player = GetComponentInParent<PlayerControl>();

    
    meshRenders.Clear();

    var skinnedmeshes = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();

    // if (meshRenders.Length <= 0)
    var meshes = GetComponentsInChildren<MeshRenderer>().ToList();
    meshRenders.AddRange(skinnedmeshes);
    meshRenders.AddRange(meshes);
  }
  public void Select(bool on)
  {
    if (meshRenders.Count <= 0) return;

    foreach (var r in meshRenders)
    {
      string layerName = on ? "Outline" : "Default";

      r.gameObject.layer = LayerMask.NameToLayer(layerName);
    }
  }
}
