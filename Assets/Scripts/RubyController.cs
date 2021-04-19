using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RubyController : MonoBehaviour
{
    public ParticleSystem HealthIncrease;

    public ParticleSystem HealthDecrease;

    public float speed = 3.0f;

    public int maxHealth = 5;

    public int cog = 4;
    public Text CogText;

    public GameObject projectilePrefab;

    public Text countText;
    public int count;

    public int hardestenemycounter = 0;

    //check
    public GameObject cam;
    public GameObject secretCam;

    public Text WinText;
    public Text LoseText;

    public Scene scene;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip cogpickup;
    public AudioClip frogchat;
    public float speeduptimer=5;

    public AudioClip WinSound;
    public AudioClip LossSound;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    public HardestEnemy he;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        CogText.text = "Cogs:" + cog.ToString();

        countText.text = "Fixed Robots:" + count.ToString();

        //Because these animations do not need to be played at start of game
        HealthIncrease.Stop();

        HealthDecrease.Stop();

        currentHealth = maxHealth;

        WinText.text = "";

       audioSource = GetComponent<AudioSource>();

        scene = SceneManager.GetActiveScene();
        if(scene.name == "Scene2")
        {
            count = 5;
            countText.text = "Fixed Robots:" + count.ToString();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (count == 10)
        {
            speed = 0.0f;
            WinText.text = "You Win! Game created by Jacob Stuart. Press R to Restart";
            PlaySound(WinSound);
            //Destroy(gameObject);
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("MainScene 1");
            }
            //Destroy(gameObject);
        }

        if(speed == 6.0f && speeduptimer > 0.0 )
        {
            speeduptimer -= Time.deltaTime;
        }
        else if(speed == 6.0f && speeduptimer <= 0.0)
        {
            speed = 3.0f;
        }

        scene = SceneManager.GetActiveScene();
        if (count == 5 && scene.name == "MainScene 1")
        {
            cam = GameObject.Find("CM vcam1");
            secretCam = GameObject.Find("CM vcam2");

            if (cam == true)
            {
                WinText.text = "Talk to Jambi to visit stage two!";
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
                if (hit.collider != null)
                {
                    secretCam.SetActive(true);
                    cam.SetActive(false);

                    //confiner.InvalidatePathCache();
                    SceneManager.LoadScene("Scene2");
                    WinText.text = "";
                    PlaySound(frogchat);
                    //transform.position = new Vector3(53.0f, 4.0f, 0f);
                }
            }
        }

        if (currentHealth == 0)
        {
            speed = 0.0f;
            PlaySound(LossSound);
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C) && cog > 0)
        {
            Launch();
            cog = cog - 1;
            CogText.text = "Cogs:" + cog.ToString();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    PlaySound(frogchat);
                }
            }
        }
    }

    public void controlhardestenemy()
    {
        hardestenemycounter++;

        if(hardestenemycounter == 2)
        {
            hardestenemycounter = 0;
            he = FindObjectOfType(typeof(HardestEnemy)) as HardestEnemy;
            if (he != null)
            {
                he.Fix();
                IncreaseCount();
            }
        }
    }

    public void addspeed() 
    {
        speed = 6.0f;
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            ParticleSystem projectileObject = Instantiate(HealthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            PlaySound(hitSound);
        }
        else
        {
            ParticleSystem projectileObject = Instantiate(HealthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (currentHealth == 0)
        {
            LoseText.text = "You lose! Press R to Restart";
            //Destroy(gameObject);
        }

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void IncreaseCount()
    {
        count = count + 1;
        countText.text = "Fixed Robots:" + count.ToString();
    }

    public void addcog()
    {
        cog = cog + 4;
        CogText.text = "Cogs:" + cog.ToString();
        PlaySound(cogpickup);
    }
}