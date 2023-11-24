using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerAnimationBehaviour : MonoBehaviour
{

    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        float bs = Random.Range(.5f, 1.7f);
        animator.SetFloat("BlinkingSpeed", bs);
        
    }
}
