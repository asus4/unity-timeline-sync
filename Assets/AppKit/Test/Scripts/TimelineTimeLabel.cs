using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

[RequireComponent(typeof(Text))]
public class TimelineTimeLabel : MonoBehaviour
{
    [Multiline()]
    public string format = "Current : {0}";
    PlayableDirector director;
    Text label;

    void Start()
    {
        director = GameObject.FindObjectOfType<PlayableDirector>();
        label = GetComponent<Text>();
    }

    void Update()
    {
        label.text = string.Format(format, director.time);
    }
}
