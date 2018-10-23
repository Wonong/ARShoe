using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour {
    public GameObject shoe;
    public AudioSource footStepOnLeave;
    public Image shoePrintImage;
    public ParticleSystem shoeParticle;

	// Use this for initialization
	void Start () {
        Screen.orientation = ScreenOrientation.Landscape;
        shoe.SetActive(false);
        shoePrintImage.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetMouseButtonDown(0))
        {
            shoePrintImage.enabled = shoe.activeSelf;
            shoe.SetActive(!shoe.activeSelf);
            if (shoe.activeSelf)
            {
                footStepOnLeave.Play();
                shoeParticle.Play();
            }
        }
	}
}
