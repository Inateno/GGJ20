using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatch : MonoBehaviour
{
    public AudioSource openHatch;
    public BoxCollider fakeCollider;
    void Start()
    {
        openHatch = GetComponent<AudioSource>();
    }

    void UnlockHatch()
    {
        this.GetComponent<Rigidbody>().isKinematic = false;
        fakeCollider.enabled = false;
    }

    void PlayOpenHatchSound()
    {
        openHatch.Play();
    }

    public void Open()
    {
        UnlockHatch();
        PlayOpenHatchSound();
    }
}
