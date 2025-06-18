using UnityEngine;
// Template :  틀, 형  사용법: <T,V,...>
// 싱글톤 : 관리자, 전역, 유일
public abstract class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour     // 모노 비해비어 형태의 T를 정의
{
   private static T _instance;         // instance(reference) : 동일한 객체 복제 / copy : 다른객체 복사

    public static T I                   //템플릿을 외부 사용으로 열어둠
    
    {
        get 
        {
             if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();      // 인스턴스가 없으면 한번 찾아봐
                if(_instance == null)                      
                {
                   GameObject o = new GameObject(typeof(T).Name); // 그래도 없으면 게임 오브젝트의 컴퍼런트를 만들어서 인스턴트를 만들어라
                   _instance = o.AddComponent<T>();
                }      
            }
            return _instance;
        }

        
        // set { I = value;}
    }
    protected abstract bool IsDontdestroy();

    protected virtual void Awake()
    {
        if(I != null && I != this)
        {
            Destroy(gameObject);                        // 다음 씬에 갔는데 이미 있으면 파괴해라
            return;
        }

        if(IsDontdestroy())                                 // 다음 씬에 가서도 파괴되지 않게
        DontDestroyOnLoad(gameObject);


    }
}
