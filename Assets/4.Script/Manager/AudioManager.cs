using UnityEngine;



[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
}
public class AudioManager :  BehaviourSingleton<AudioManager>
{
    protected override bool IsDontdestroy() => true;
    
    [Header("Object")]
    public GameObject BGM_g;
    public GameObject SFX_g;

    //SerializeField] private AudioSource audio_BGM;

    [Header("Sound")]
    [SerializeField] private Sound[] BGM;
    [SerializeField] private Sound[] SFX;

    private AudioSource audio_BGM;
    private AudioSource[] audio_SFX;




    void Start()
    {
        audio_BGM = BGM_g.GetComponent<AudioSource>();
        audio_SFX = SFX_g.GetComponents<AudioSource>();

    }

    public void PlayBGM(string name)
    {
        if (audio_BGM.isPlaying) return;
        foreach (Sound s in BGM)
        {
            if (s.Name.Equals(name))
            {
                audio_BGM.clip = s.Clip;
                audio_BGM.Play();
                break;
            }
        }
        //if (!audio_BGM.isPlaying) audio_BGM.Play();
    }
    public void StopBGM()
    {
        audio_BGM.Stop();
    }
    public void PlaySFX(string name)
    {
        foreach (Sound s in SFX)
        {
            if (s.Name.Equals(name))
            {
                // 오디오 클립 찾기
                for (int i = 0; i < audio_SFX.Length; i++)
                {
                    if (!audio_SFX[i].isPlaying)
                    {
                        audio_SFX[i].clip = s.Clip;
                        audio_SFX[i].Play();
                        return;
                    }
                    Debug.Log(" all Audio is laying");

                    return;
                }
            }
        }
    }
}
