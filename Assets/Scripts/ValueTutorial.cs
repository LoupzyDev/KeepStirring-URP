using UnityEngine;

public class ValueTutorial : MonoBehaviour
{
    Countdown time;
    [SerializeField] GameObject bottle3;

    [SerializeField] Transform spawn;
    public static int value = 0;

    public GameObject potionSpotlight;

    public AudioSource audio;

    public static bool ingrediente1 = false;//manzana
    public static bool ingrediente2 = false;//tentaculo
    public static bool ingrediente3 = false;//pata
    public static bool ingrediente4 = false;//corazon
    public static bool ingrediente5 = false;//pluma
    public static bool ingrediente6 = false;//cola
    public static bool ingrediente7 = false;//dedo
    public static bool ingrediente8 = false;//calabaza
    public static bool ingrediente9 = false;//girasol
    public static bool ingrediente10 = false;//violeta
    public static bool ingrediente11 = false;//pez


    private void Start()
    {
        time = FindObjectOfType<Countdown>();
        potionSpotlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (ingrediente3 == true && ingrediente9 == true)
        {
            audio.Play();
            Instantiate(bottle3, spawn.position, Quaternion.identity);
            potionSpotlight.SetActive(true);


            ingrediente3 = false;
            ingrediente9 = false;

        }
    }

    public void TurnOffSpotlight()
    {
        potionSpotlight.SetActive(false);
    }


}
