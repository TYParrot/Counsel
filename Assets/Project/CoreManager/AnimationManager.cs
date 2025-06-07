using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator avatarAnimator;

    // Start is called before the first frame update
    void Start()
    {
        if(avatarAnimator == null){
            avatarAnimator = GameObject.Find("Counseler").GetComponent<Animator>();
        }

    }

    public void Nodding(){
        avatarAnimator.SetTrigger("Nod");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
