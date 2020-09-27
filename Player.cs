using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Player : MonoBehaviour
{
    public float chargePower = 100;
    public float maxSpeed;
    public Transform groundCheck;
    public float jumpForce;
    public float FireRate;
    public float maxHealth;
    public float maxMana = 100f;

    public GameObject manaPopup;
    public GameObject chargeManaPopup;
    public Transform muzzle;
    public GameObject bullet;
    public GameObject chargedBullet;
    public GameObject DAB;
    private float healthGain = 20f;
    private float manaGain;
    public bool canMove;
    public bool canActiveLight = false;
    public Animator anim;
    public float mana;
    public float health;
    public Rigidbody2D rb;
    public AudioSource chargingShot;
    public Vector3 respawnPoint;

    public bool onGround;

    private DashMove dashMove;
    private bool canDash;

    private bool canChargeShot;

    public GameObject dashEffect;
    public GameObject chargeEffect;
    public float dashCooldown;
    public float chargedShotCooldown;
    public Transform effectPosition;

    public GameObject deathUI;

    private float timer = 0f;
    private float startTime = 0f;
    public float holdTime = 2f;
    private bool held = false;
    private float chargedShotCDRate = 0;
    private float dashCDRate = 0;
    private Camera mainCamera;
    private CameraFollow cameraFollow;
    public Vector2 maxXandY_direita;
    public Vector2 minXandY_esquerda;
    private GameObject item;
    private float speed;
    private bool facingRight = true;
    private bool jump = false;
    private bool doubleJump;
    private Attack SHOOT;
    private float nextAttack;
    private bool canDamage = true;
    private SpriteRenderer sprite;
    private Player player;
    private Inventory inventory;
    private ManaInventory manaInventory;

    private bool buttonHeldDown;
    private int maxPower = 60;
    private float chargeSpeed = 3;
    private bool isCharging;
    private GameManager gm;

    // controle de poções de HP no inventário
    public int contaPocoesHP = 0; // conta poções de HP no inentário
    public int contaPocoesMana = 0; // conta poções de MANA no inventário

    // define os objetos que limitam as paredes do cenário
    private GameObject paredeEsquerda;
    private GameObject paredeDireita;
    private CheckpointController checkpointController;
    public bool checkPointIsSaved = false;

    // Boss Fight
    public AudioClip newtrack;
    public GameObject boss;
    public GameObject bossTrigger;
    public GameObject paredeBoss;

    public GameObject[] bossReset;

    private BossHealth bhealth;


    // Start is called before the first frame update
    void Start()
    {
        deathUI.GetComponent<GameObject>();
        dashMove = GetComponent<DashMove>();
        mainCamera = GetComponent<Camera>();
        inventory = GetComponent<Inventory>();
        manaInventory = GetComponent<ManaInventory>();
        rb = GetComponent<Rigidbody2D>();
        speed = maxSpeed;
        anim = GetComponent<Animator>();
        SHOOT = GetComponentInChildren<Attack>();
        mana = maxMana;
        health = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        canMove = true;
        StartCoroutine(addMana());
        deathUI.SetActive(false);
        chargingShot.GetComponent<AudioSource>();
        DAB.GetComponent<GameObject>();
        respawnPoint = transform.position;
        cameraFollow = FindObjectOfType<CameraFollow>();
        checkpointController = FindObjectOfType<CheckpointController>();
        gm = FindObjectOfType<GameManager>();
        bhealth = FindObjectOfType<BossHealth>();

        isCharging = false;
        chargeEffect.SetActive(false);


        manaGain = 20f;

        // footStep = GetComponent<FootStep>();

        // InvokeRepeating("PlaySound", 0.0f, 0.3f);
        paredeEsquerda = GameObject.FindGameObjectWithTag("ParedeEsquerda");
        paredeDireita = GameObject.FindGameObjectWithTag("ParedeDireita");

    }

    // Update is called once per frame
    void Update()
    {

        if (checkpointController.checkpointReached == true)
        {
            checkPointIsSaved = true;
        }
        else
        {
            checkPointIsSaved = false;
        }
                                                                                                                                                                                                                                            
        if (Time.time > dashCDRate)
        {
            if (Input.GetButtonDown("Fire2") && mana >= 20f && rb.velocity != Vector2.zero)
            {
                canDash = true;

                if (mana >= 20f)
                {

                    dashCDRate = Time.time + dashCooldown;
                    ConsumirMana();
                    AudioManager.PlaySound("playerdash");

                    GameObject dashing = Instantiate(dashEffect, effectPosition.position, Quaternion.identity) as GameObject;
                    Destroy(dashing.gameObject, 2f);

                    if (Input.GetAxis("Horizontal") < 0)
                    {
                        canDash = true;
                        StartCoroutine("DashMove");
                        anim.SetBool("isDashing", true);

                    }

                    else if (Input.GetAxis("Horizontal") > 0)
                    {
                        canDash = true;
                        StartCoroutine("DashMove");
                        anim.SetBool("isDashing", true);

                    }


                }

            }
        }

        else if (mana <= 19f || rb.velocity == Vector2.zero)
        {
            anim.SetBool("isDashing", false);
            canDash = false;
        }

        if (buttonHeldDown && chargePower <= maxPower)
        {
            isCharging = true;
            chargePower += Time.deltaTime * chargeSpeed;
        }

        if (mana <= 39f)
        {
            canChargeShot = false;
            isCharging = false;

            if (mana >= 40f)
            {
                canChargeShot = true;
            }
        }


        if (canMove == false)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (Input.GetButtonDown("Fire3"))
        {
            if (canActiveLight == true)
            {
                Debug.Log("QUE HAJA LUZ!");
                GameObject.FindWithTag("MainCamera").GetComponent<VignetteAndChromaticAberration>().enabled = false;
            }
        }


        onGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        if (onGround)
            doubleJump = false;
        //anim.SetTrigger("stopJump");


        if (Input.GetButtonDown("Jump") && (onGround || !doubleJump))
        {
            anim.SetTrigger("takeOf");
            AudioManager.PlaySound("playerjump");
            jump = true;
            if (!doubleJump && !onGround)
                doubleJump = true;
            anim.SetBool("isJumping", true);

        }
        //else
        //{
        //    anim.SetBool("isJumping", true);
        //}

        if (rb.velocity.y <= 0)
        {
            //anim.SetBool("isJumping", false);
            anim.SetTrigger("stopJump");
        }

        if (rb.velocity.y >= 0.01)
        {
            anim.SetBool("isJumping", true);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            AudioManager.PlaySound("playerfire");
            Shoot();
        }

        if (Time.time > chargedShotCDRate)
        {
            if (Input.GetButtonDown("Fire4") && mana >= 40f)
            {
                startTime = Time.time;
                timer = startTime;

            }

            if (Input.GetButton("Fire4") && buttonHeldDown == false && mana >= 40f)
            {
                chargeEffect.SetActive(true);
                PlayChargingSound();
                timer += Time.deltaTime;
                buttonHeldDown = true;

                if (timer > (startTime + holdTime))
                {
                    chargedShotCDRate = Time.time * chargedShotCooldown;

                    isCharging = true;
                    HoldButton();

                }
                else
                {
                    chargeEffect.SetActive(false);
                    canChargeShot = false;
                    buttonHeldDown = false;
                    isCharging = false;
                    StopChargingSound();
                }
            }
        }

        if (Time.time > chargedShotCDRate)
        {
            if (Input.GetButtonUp("Fire4"))
            {
                buttonHeldDown = false;

                if (isCharging)
                {
                    chargedShotCDRate = Time.time * chargedShotCooldown;
                    AudioManager.PlaySound("releasingshot");
                    ReleaseButton();
                    chargeEffect.SetActive(false);
                    ConsumirManaChargeShot();
                }
                else
                {
                    chargeEffect.SetActive(false);
                }

            }
        }



        if (Input.GetButtonDown("RB"))
        {
            // tem poções de HP disponíveis?
            if (contaPocoesHP > 0 && health != maxHealth)
            {
                AudioManager.PlaySound("playerteleport");
                if (healthGain > maxHealth - health)
                {
                    healthGain = maxHealth - health;
                    health += healthGain;
                }
                else
                {
                    health += healthGain;
                }

                health += healthGain;
                contaPocoesHP--;

                for (int i = inventory.slots.Length - 1; i >= 0; i--)
                {
                    if (inventory.isFull[i] == true)
                    {
                        inventory.isFull[i] = false;
                        inventory.slots[i].SetActive(false);
                        break;
                    }
                }
            }
        }

        if (Input.GetButtonDown("LB") && mana != 100.09969f)
        {

            if (contaPocoesMana > 0 && mana != maxMana)
            {
                AudioManager.PlaySound("playerteleport");
                if (manaGain > maxMana - mana)
                {
                    manaGain = maxMana - mana;
                    mana += manaGain;
                }
                else
                {
                    mana += manaGain;
                }

                contaPocoesMana--;

                for (int i = manaInventory.slots.Length - 1; i >= 0; i--)
                {
                    if (manaInventory.isFull[i] == true)
                    {
                        manaInventory.isFull[i] = false;
                        manaInventory.slots[i].SetActive(false);
                        break;
                    }
                }
            }
        }

    }

    IEnumerator DashMove()
    {
        speed += 4;
        yield return new WaitForSeconds(.3f);
        speed -= 4;
        anim.SetBool("isDashing", false);
    }


    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (canDamage)
            rb.velocity = new Vector2(h * speed, rb.velocity.y);

        anim.SetFloat("Speed", Mathf.Abs(h));


        if (h > 0 && !facingRight)
        {
            Flip();
        }
        else if (h < 0 && facingRight)
        {
            Flip();
        }

        if (jump)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce);
            jump = false;


        }

        // if (jump == true)
        // {
        // StopSound();
        // }



    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Parede"))
        {
            anim.SetTrigger("stopJump");
        }

        if (col.CompareTag("Checkpoint"))
        {
            respawnPoint = col.transform.position;
        }

        if (col.CompareTag("FallDetector"))
        {
            transform.position = respawnPoint;
            
            if (checkPointIsSaved == true)
            {
                StartCoroutine(WaitandReload());
            }
            else
            {
                AudioManager.PlaySound("playerdeath");
                DAB.SetActive(false);
                deathUI.SetActive(true);
                FindObjectOfType<KillPlayer>().Invoke("ReloadScene", 3f);
            }

        }
    }

    IEnumerator WaitandReload()
    {
        AudioManager.PlaySound("playerdeath");
        DAB.SetActive(false);
        deathUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        deathUI.SetActive(false);
        Respawn();
        cameraFollow.maxXAndY = maxXandY_direita;
        cameraFollow.minXAndY = minXandY_esquerda;
        anim.SetTrigger("GetAlive");
        FindObjectOfType<AudioManager>().ChangeBGM(newtrack);
        boss.SetActive(false);
        bossTrigger.SetActive(true);
        paredeBoss.SetActive(false);
        FindObjectOfType<GameManager>().currentTime = 0;

        { foreach (GameObject obj in boss.GetComponent<Boss>().laminas_1) { obj.GetComponent<Animator>().SetBool("isON", false); } }

        foreach (GameObject obj in gm.perigoUI)
            obj.SetActive(false);

        foreach (GameObject obj in bossReset)
            obj.SetActive(false);


    }

    void Flip()
    {
        facingRight = !facingRight;

        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
    public void Heal(int amount)
    {
        health += amount;

        if (health > maxHealth)
        {
            maxHealth = health;
        }
    }

    public float GetHealth()
    {
        return health;
    }


    public void TakeDamage(int damage)
    {
        if (canDamage)
        {
            canDamage = false;
            health -= (damage);
            if (health <= 0)
            {
                anim.SetTrigger("Dead");
                AudioManager.PlaySound("playerdeath");
                deathUI.SetActive(true);

                if (checkPointIsSaved == true)
                {
                    StartCoroutine(WaitandReload());
                }
                else
                {
                    Invoke("ReloadScene", 5f);
                }
            }

            else
            {
                StartCoroutine(DamageCoroutine());

            }
        }
    }



    IEnumerator DamageCoroutine()
    {
        for (float i = 0; i < 0.6f; i += 0.2f)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        canDamage = true;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        deathUI.SetActive(false);
    }

    public void ConsumirMana()
    {
        mana = mana - 20;
        GameObject points = Instantiate(manaPopup, transform.position, Quaternion.identity) as GameObject;
        points.transform.GetComponent<TextMesh>().text = "20";

    }

    public void ConsumirManaChargeShot()
    {
        mana = mana - 40;
        GameObject points = Instantiate(chargeManaPopup, transform.position, Quaternion.identity) as GameObject;
        points.transform.GetComponent<TextMesh>().text = "40";
    }

    public void Shoot()
    {
        GameObject mBullet = Instantiate(bullet, muzzle.position, muzzle.rotation);
        Destroy(mBullet.gameObject, 0.3f);
    }

    public void SetNewMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }

    public void SetNewMaxMana(int newMaxMana)
    {
        maxMana = newMaxMana;
    }
    
    IEnumerator addMana()
    {
        while (true)
        { // loops forever...
            if (mana < 100f)
            { // if mana < 80...
                mana += 0.1f; // increase mana and wait the specified time
                yield return new WaitForSeconds(0.01f);
            }
            else
            { // if mana >= 80, just yield 
                yield return null;
            }
        }

    }

    public void HoldButton()
    {

        isCharging = true;

    }

    public void ReleaseButton()
    {
        chargePower = 70;
        GameObject newBullet = Instantiate(chargedBullet, muzzle.position, muzzle.rotation) as GameObject;
        newBullet.GetComponent<Bullet>().SetNewDamage((int)chargePower);
        buttonHeldDown = false;
        isCharging = false;
        StopChargingSound();
        

    }

    public void StopChargingSound()
    {
        chargingShot.Stop();
    }

    public void PlayChargingSound()
    {
        chargingShot.Play();
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        canMove = true;
        canDamage = true;
        health += 100;
        DAB.SetActive(true);
    }



}



