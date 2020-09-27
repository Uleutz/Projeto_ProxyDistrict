using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speedRight;
    public float speedLeft;

    public GameObject damagePopup;
    public GameObject superDamagePopup;

    public Rigidbody2D rb;
    public int damage = 15;
    public GameObject impactEffect;


    private GameObject player;
    private float chargeDamage;

    void Start()
    {

        player = GameObject.FindWithTag("Player");

        if (player.transform.localScale.x < 0)
        {
            rb.velocity = new Vector2(speedLeft, 0);
            Destroy(this.gameObject, 1f);
        }
        else if (player.transform.localScale.x > 0)
        {
            rb.velocity = new Vector2(speedRight, 0);
            Destroy(this.gameObject, 1f);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if(enemy != null)
        {
            
            enemy.TakeDamage(damage);

            if (this.gameObject.tag == "chargedshot")
            {
            GameObject superPoint = Instantiate(superDamagePopup, transform.position, Quaternion.identity) as GameObject;
            superPoint.transform.GetComponent<TextMesh>().text = "70";
            }
            else
            {
            GameObject points = Instantiate(damagePopup, transform.position, Quaternion.identity) as GameObject;
            points.transform.GetComponent<TextMesh>().text = "15";
            }

            impactEffect = Instantiate(impactEffect, transform.position, transform.rotation) as GameObject;
            Destroy(gameObject);
            Destroy(impactEffect.gameObject, 0.2f);
        }

        Enemy_Parasita enemy2 = other.GetComponent<Enemy_Parasita>();

        if (enemy2 != null)
        {
            enemy2.TakeDamage(damage);
            if (this.gameObject.tag == "chargedshot")
            {
            GameObject superPoint = Instantiate(superDamagePopup, transform.position, Quaternion.identity) as GameObject;
            superPoint.transform.GetComponent<TextMesh>().text = "70";
            }
            else
            {
            GameObject points = Instantiate(damagePopup, transform.position, Quaternion.identity) as GameObject;
            points.transform.GetComponent<TextMesh>().text = "15";
            }

            impactEffect = Instantiate(impactEffect, transform.position, transform.rotation) as GameObject;
            Destroy(gameObject);
            Destroy(impactEffect.gameObject, 0.2f);
        }

        Boss bossEnemy = other.GetComponent<Boss>();

        if (bossEnemy != null)
        {
            bossEnemy.TakeDamage(damage);
            if (this.gameObject.tag == "chargedshot")
            {
            GameObject superPoint = Instantiate(superDamagePopup, transform.position, Quaternion.identity) as GameObject;
            superPoint.transform.GetComponent<TextMesh>().text = "70";
            }
            else
            {
            GameObject points = Instantiate(damagePopup, transform.position, Quaternion.identity) as GameObject;
            points.transform.GetComponent<TextMesh>().text = "15";
            }

            impactEffect = Instantiate(impactEffect, transform.position, transform.rotation) as GameObject;
            Destroy(gameObject);
            Destroy(impactEffect.gameObject, 0.2f);
        }

        Monster turrets = other.GetComponent<Monster>();
        if (turrets != null)
        {
            turrets.TakeDamage(damage);
            if (this.gameObject.tag == "chargedshot")
            {
            GameObject superPoint = Instantiate(superDamagePopup, transform.position, Quaternion.identity) as GameObject;
            superPoint.transform.GetComponent<TextMesh>().text = "70";
            }
            else
            {
            GameObject points = Instantiate(damagePopup, transform.position, Quaternion.identity) as GameObject;
            points.transform.GetComponent<TextMesh>().text = "15";
            }

            impactEffect = Instantiate(impactEffect, transform.position, transform.rotation) as GameObject;
            Destroy(gameObject);
            Destroy(impactEffect.gameObject, 0.2f);
        }

        if (other.gameObject.tag == "Parede")
        {
            impactEffect = Instantiate(impactEffect, transform.position, transform.rotation) as GameObject;
            Destroy(this.gameObject);
            Destroy(impactEffect.gameObject, 0.2f);
        }      
    }

    public void SetNewDamage(int newDamage)
    {
        damage = newDamage;
    }

}
