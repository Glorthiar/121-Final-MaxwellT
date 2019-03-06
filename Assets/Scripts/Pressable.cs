using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Pressable : MonoBehaviour
{
        public bool Pressed;

        public Pressable()
        {
            Pressed = false;
        }

        public virtual void Toggle()
        {
            Pressed = !Pressed;
        }
    }
