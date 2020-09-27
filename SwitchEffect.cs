using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchEffect : MonoBehaviour
{

    public GameObject[] lights;
    public Animator[] plataforma1Animation;
    public GameObject[] lasers;

    public GameObject sinalVerde;
    public GameObject sinalVermelho;

    public Animator elevadorAnimation;
    public Outline elevadorCollisionTrigger;
    public ElevadorTrigger elevadorTriggerScript;
    public GameObject cidadeCameraTrigger;

    // Start is called before the first frame update
    void Start()
    {

        FindObjectOfType<ObjectTrigger>().playerLight.SetActive(false);
        DesativarLuzes();

        foreach (Animator anim in plataforma1Animation)
        {
            anim.enabled = false;
        }

        foreach (GameObject obj in lasers)
        {
            obj.GetComponent<Laser>().enabled = false;
        }

        elevadorAnimation.GetComponent<Animator>().enabled = false;
        elevadorTriggerScript.GetComponent<ElevadorTrigger>().enabled = false;

        sinalVermelho.SetActive(true);
        sinalVerde.SetActive(false);
        cidadeCameraTrigger.SetActive(false);

    }

    public void AcenderLuzes()
    {
        foreach (GameObject obj in lights)
        {
            obj.gameObject.SetActive(true);
        }
    }

    public void DesativarLuzes()
    {
        foreach (GameObject obj in lights)
        {
            obj.gameObject.SetActive(false);
        }
    }

}
