using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    [SerializeField] Sound[] effectSounds;
    [SerializeField] AudioSource[] effectPlayer;

    [SerializeField] Sound[] bgmSounds;
    [SerializeField] AudioSource bgmPlayer;

    [SerializeField] AudioSource voicePlayer;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //�� �̵��ɶ� ���� ���� �Ŵ����� �����Ǿ� ���ݾ�.
        }
        else
        {
            Destroy(gameObject); // instance�� null�� �ƴѰ�� ��, �̹� �� �۾��� ����ǰ� �� ������ ��� �ı��ؾ���
                                 // �ٽ� ������ ���������� SoundManager�� 2�� ������ �ȵǴ� �ı��Ѵٴ� �Ҹ�
        }
    }

    void PlayBGM(string p_name)
    {
        for(int i = 0; i < bgmSounds.Length; i++) 
        {
            if(p_name == bgmSounds[i].name) 
            {
                bgmPlayer.clip = bgmSounds[i].clip;
                bgmPlayer.Play();
                return;
            }
        }
       Debug.LogError(p_name + "�� �ش�Ǵ� BGM�� �����ϴ�.");
    }

    void StopBGM()
    {
        bgmPlayer.Stop();
    }

    void PauseBGM()
    {
        bgmPlayer.Pause();
    }

    void UnPauseBGM()
    {
        bgmPlayer.UnPause();
    }

    void PlayEffectSound(string p_name)
    {
        for (int i = 0;i < effectSounds.Length;i++) 
        {
            if (p_name == effectSounds[i].name)
            {
                for(int j = 0; j < effectPlayer.Length; j++)
                {
                    if (!effectPlayer[j].isPlaying) 
                    {
                        effectPlayer[j].clip = effectSounds[i].clip;
                        effectPlayer[j].Play();
                        return;
                    }
                }
                Debug.LogError("��� ȿ���� �÷��̾ ������Դϴ�.");
                return;
            }
        }
        Debug.LogError(p_name + "�� �ش��ϴ� ȿ���� ���尡 �����ϴ�.");
    }

    void StopAllEffectSound()
    {
        for(int i = 0;i < effectPlayer.Length; i++)
        {
            effectPlayer[i].Stop();
        }
    }

    void PlayVoiceSound(string p_name)
    {
        AudioClip _Clip = Resources.Load<AudioClip>("Sounds/Voices/" + p_name);
        if(_Clip != null )
        {
            voicePlayer.clip = _Clip;
            voicePlayer.Play();
        }
        else
        {
            Debug.LogError(p_name + "�� �ش��ϴ� ���̽� ���尡 �����ϴ�.");
        }
        
    }

    /// 
    /// p_Type : 0 -> ��� ���
    /// p_Type : 1 -> ȿ���� ���
    /// p_Type : 2 -> ���̽� ���� ���
    /// 

    public void PlaySound(string p_name, int p_Type)
    {
        if (p_Type == 0) PlayBGM(p_name);
        else if (p_Type == 1) PlayEffectSound(p_name);
        else PlayVoiceSound(p_name);
    }
}
