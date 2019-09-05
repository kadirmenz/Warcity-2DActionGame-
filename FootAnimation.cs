using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootAnimation : MonoBehaviour {

    private SpriteRenderer spriterenderer;
    public Sprite başlangıçsprite;

    public Sprite[] ilerigerianimasyonlar;
    public Sprite[] sagasolaanimasyonlar;
    void Start ()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        başlangıçsprite = spriterenderer.sprite;
	}
    public SpriteRenderer GetSpriteRenderer()
    {
        return spriterenderer;

    }
	
	
	void Update ()
    {
	
	}
}
