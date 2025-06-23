using System.Linq;
using UnityEngine;

public static class NewBaseType
{
    // 슬롯을 이름으로 찾기.(대소문자 상관 없음)
    public static Transform FindSlot(this Transform root, params string[] slotNames)
    {
        Transform[] children = root.GetComponentsInChildren<Transform>();

        foreach (var slot in slotNames)
        {
            foreach (Transform t in children)
            {

                if (t.name.ToLower().Contains(slot.ToLower()))
                    return t;
            }
        }
        Debug.Log($"못 찾음 : {slotNames.ToList()}");
        return null;
    }
}
