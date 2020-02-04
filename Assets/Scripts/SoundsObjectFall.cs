using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NamedSound {
    public string tagName;
    public AudioSource sound;
}

public class SoundsObjectFall : MonoBehaviour
{
    public Dictionary<string, AudioSource> sounds = new Dictionary<string, AudioSource>() {};
    
    public NamedSound[] editorSounds;

    void Start()
    {
        foreach (NamedSound item in editorSounds)
        {
            sounds.Add(item.tagName, item.sound);
        };
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (sounds.ContainsKey(collisionInfo.gameObject.tag)) {
            sounds[collisionInfo.gameObject.tag].Play();
        }
    }
}
