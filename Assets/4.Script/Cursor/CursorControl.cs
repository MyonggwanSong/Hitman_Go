using UnityEngine;
// using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum CursorType { MOVE = 0, INTERACT, ATTACT, DIALOGUE }

[System.Serializable]
public class CursorData
{
    public CursorType cursortype;
    public Texture2D texture;
    public Vector2 offset;
}
public class CursorControl : MonoBehaviour

{
    [Space(20)]
    public bool IsShowDebugCursor = false;

    //eyePoint : 플레이어 눈 위치
    //cursorPoint : 마우스와 레벨 충돌 위치
    //cursorFixedPoint : 마우스와 레벨 충돌 위치를 플레이어 눈 높이 맞게 수정.

    [Space(10)]



    [SerializeField] Transform cursorPoint;     // 커서의 위치
    [SerializeField] Transform cursorFixedPoint;   // 커서의 위치를 눈높이로 수정 (카메라 흔들림 방지)
    public Transform CursorFixedPoint => cursorFixedPoint;
    [SerializeField] private LineRenderer line;
    [SerializeField] LayerMask targetLayer;     // 커서가 충돌할 레이어
    //2D 커서
    public CursorType cursorType = CursorType.MOVE;
    [SerializeField] List<CursorData> cursors = new List<CursorData>();

     #region EVENT
    [Header("Events")]
    [SerializeField]  EventCursorHover eventCursorHover;

    #endregion

    private Camera cam;
    private CursorSelectable curHover;        // 현재 커서위치의 게임 오브젝트
    private CursorSelectable preHover;



    void Start()
    {
        cam = Camera.main;

        line.enabled = IsShowDebugCursor;
        cursorPoint.GetComponentInChildren<Renderer>().enabled = IsShowDebugCursor;
        cursorFixedPoint.GetComponentInChildren<Renderer>().enabled = IsShowDebugCursor;


    }
    void Update()
    {
        if (cam == null ) return;

        preHover = curHover;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            // 현재 커서 아래의 오브젝트 
            curHover = hit.collider.GetComponentInParent<CursorSelectable>();

            // 현재 커서와 이전 커서 오브젝트가 다를때만 갱신
            if (curHover != preHover)
                OnHoverEnter();

            cursorPoint.position = hit.point;
            
        }
        else
        {
            OnHoverExit();

            curHover = null;
        }
    }


    


    private void OnHoverEnter()
    {
        if (preHover != null)
        {
            preHover.Select(false);

            
        }

        if (curHover == null) return;

        var sel = curHover.GetComponentInParent<CursorSelectable>();    // CursorSelectable 만 커서와 반응 하도록
        if (sel == null) return;

        curHover.Select(true);
        //curHover.layer = LayerMask.NameToLayer("Outline");
       

        if (sel.player != null)
        {
            eventCursorHover.player = sel.player;
            eventCursorHover.Raise();
        }
        

    }

    // 커서를 벗어났을때 이벤트 처리
    private void OnHoverExit()
    {
        if (preHover != null)
         preHover.Select(false);
         //   preHover.layer = LayerMask.NameToLayer("Default");
    }
}