using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{

    AudioSource source;
    [SerializeField] private AudioClip audioClip;
    [Range(0.0f, 2.0f)] [SerializeField] private float volume = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.loop = true;
        source.volume = volume;
        source.Play();
    }

    private void OnValidate()
    {
        if (source !=null)
        {
            source.volume = volume;
        }
    }
}
