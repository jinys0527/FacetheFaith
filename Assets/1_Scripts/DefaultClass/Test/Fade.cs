using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image image;
    public GameObject canvas;
    public GameObject blur;
    Material material;
    SpriteRenderer spriteBlur;
    public bool fade;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        spriteBlur = blur.GetComponent<SpriteRenderer>();
        material = spriteBlur.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fade = !fade;
        }
        
        if (fade)
        {
            if (image.color.a >= 1)
            {
                material.SetFloat("_BlurAmount", 0.005f);
                return;
            }
            image.color = new Color(0, 0, 0, image.color.a + 0.5f * Time.deltaTime);
            material.SetFloat("_BlurAmount", material.GetFloat("_BlurAmount") + 0.0025f*Time.deltaTime);
        }
        else
        {
            if (image.color.a <= 0)
            {
                material.SetFloat("_BlurAmount", 0);
                return;
            }
            image.color = new Color(0, 0, 0, image.color.a - 0.5f * Time.deltaTime);
            material.SetFloat("_BlurAmount", material.GetFloat("_BlurAmount") - 0.0025f*Time.deltaTime);
        }    
    }
}
