using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

namespace AppKit
{

    [RequireComponent(typeof(Button))]
    public class JumpToButton : MonoBehaviour
    {
        public float seconds;

        TimelineSync sync;

        void Start()
        {
            sync = GameObject.FindObjectOfType<TimelineSync>();
            if (sync == null)
            {
                Debug.LogWarning("Require TimelineSync in scene");
                enabled = false;
                return;
            }

            var button = GetComponent<Button>();
            button.onClick.AddListener(OnClickButton);
            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = string.Format("{0:0.0} sec", seconds);
            }
        }

        void OnDestroy()
        {
            var button = GetComponent<Button>();
            button.onClick.RemoveListener(OnClickButton);
        }

        void OnClickButton()
        {
            Debug.LogFormat("Set time to {0} sec", seconds);
            sync.Time = seconds;
        }


    }
}