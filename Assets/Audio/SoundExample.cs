using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundExample : MonoBehaviour
{
    public AudioClip BattleMusic;

    void Start()
    {
        SoundManager.Instance.PlayMusic(BattleMusic);
    }
}
