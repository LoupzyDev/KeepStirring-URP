using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DumbWays : MonoBehaviour
{
    public Animator bigSmoke;
    public Animator bigSmoke2;
    public AudioSource explodeAudio;
    public AudioSource alarm;
    public Animator charFlying;
    
    // Start is called before the first frame update
    void Start()
    {
        charFlying.SetBool("Dancing", true);
        bigSmoke.SetBool("NotExploding", true);
        bigSmoke2.SetBool("NotExploding", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeathByExplosion()
    {
        StartCoroutine(WaitForExplosion());
        explodeAudio.Play();
        if (bigSmoke && bigSmoke2 != null)
        {
            //Debug.Log("no soy nulo");
            bigSmoke.SetBool("NotExploding", false);
            bigSmoke2.SetBool("NotExploding", false);
        }
        if (charFlying != null)
        {
            charFlying.SetBool("Dancing", false);
        }
        
    }

    public void DeathByCountdown()
    {
        StartCoroutine(WaitForCountdown());
        alarm.Play();
    }

    public IEnumerator WaitForExplosion()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("YouExplode");
    }

    public IEnumerator WaitForCountdown()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("TimesUp");
    }

 
}
