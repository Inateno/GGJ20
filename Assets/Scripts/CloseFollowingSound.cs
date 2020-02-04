using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseFollowingSound : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            audioSource.Play(0);
            // TODO move to the player
        }
    }

}
