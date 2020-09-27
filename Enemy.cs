using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int maxHealth = 100;
    public int damage;
    public int playerAggro;

    private int _curHealth;
    private Transform player;
    private Rigidbody2D rb;
    public Animator anim;
    private Vector3 playerDistance;
    private bool facingRight = true;
    private bool isDead = false;
    private SpriteRenderer sprite;

    [SerializeField]
    GameObject bullet;
    public Transform muzzle;
    public float fireRate;
    private float nextFire;
    public int curHealth
    {
        get { return _curHealth; }
        set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public void Init()
    {
        curHealth = maxHealth;
    }

    [SerializeField]
    private StatusIndicator statusIndicator;


    // Start is called before the first frame update
    void Start()
    {
        Init();

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(curHealth, maxHealth);
        }

        nextFire = Time.time;


        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            playerDistance = player.transform.position - transform.position;
            if (Mathf.Abs(playerDistance.x) < playerAggro && Mathf.Abs(playerDistance.y) < 3)
            {
                rb.velocity = new Vector2(speed * (playerDistance.x / Mathf.Abs(playerDistance.x)), rb.velocity.y);
                StartCoroutine(CheckIfTimeToFire());
            }

            if (Mathf.Abs(playerDistance.x) < 3 && Mathf.Abs(playerDistance.y) > 3)
            {
                playerAggro = 0;
            }


            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

            if (rb.velocity.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (rb.velocity.x < 0 && facingRight)
            {
                Flip();
            }
        }

    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        AudioManager.PlaySound("enemyhit");
        curHealth -= damage;
        StartCoroutine(DamageCoroutine());

        if (curHealth <= 0)
        {
            isDead = true;
            anim.SetTrigger("Dead");
            AudioManager.PlaySound("enemydeath");
            this.damage = 0;
            rb.velocity = Vector2.zero;
            StartCoroutine(DestroyEnemy());
        }

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(curHealth, maxHealth);
        }
    }



    IEnumerator DamageCoroutine()
    {
        for (float i = 0; i < 0.2f; i += 0.2f)
        {
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator DestroyEnemy()
    {

        if (isDead == true)
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(this.gameObject);
        }



    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            player.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 3 * (playerDistance.x / Mathf.Abs(playerDistance.x)), ForceMode2D.Impulse);
            AudioManager.PlaySound("playerdamage");

            if (player.health <= 0)
            {
                StartCoroutine(LoseAggro());
            }
        }
    }

    public void SetNewDamage(int newDamage)
    {
        damage = newDamage;
    }

    IEnumerator CheckIfTimeToFire()
    {
        yield return new WaitForSeconds(2f);

        if (Time.time > nextFire)
        {
            Instantiate(bullet, muzzle.position, Quaternion.identity);
            AudioManager.PlaySound("enemyfire");
            yield return new WaitForSeconds(0.1f);
            nextFire = Time.time + fireRate;

        }
    }
    IEnumerator LoseAggro()
    {
        playerAggro = 0;
        yield return new WaitForSeconds(3f);
        playerAggro = 3;
    }
}
