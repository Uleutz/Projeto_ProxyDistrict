using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global instance;

    public Bullet bulletScript;
    public Player playerScript;
    public Enemy enemyScript;
    public Mana manaScript;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if(instance == null)
        {
            instance = this;
        }

        else if (instance != null)
        {
            Destroy(this);
        }
    }

}
