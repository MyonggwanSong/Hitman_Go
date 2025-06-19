using UnityEngine;
using UnityEngine.Events;

public abstract class GameEvent<T> : ScriptableObject where T : GameEvent<T>
{
    abstract public T Item { get; }

    // Action 에서 파생된 UnityAction은 unity전용 delegate. 이벤트를 사용하기 위한 핵심 오브젝트
    public UnityAction<T> OnEventRaise;     // 인스팩터상에 노출 안함
                                            // public UnityEvent OnEventRaise2;     // 유니티 클래스 인스팩터상에 노출


    public void Register(UnityAction<T> listener)      // 이벤트 등록
    {
        OnEventRaise += listener;                       // delegate 는 리스트의 성격도 갖고있어서 listener들을 추가, 해제 할 수 있다. 
    }
    public void Unregister(UnityAction<T> listener)    // 이벤트 해제
    {
        OnEventRaise -= listener;
    }
    public void Clear()                                // 이벤트 전체 삭제
    {
        OnEventRaise = null;
    }

    public void Raise()
    {
        OnEventRaise?.Invoke(Item);  // 이벤트 발생
    }

}
