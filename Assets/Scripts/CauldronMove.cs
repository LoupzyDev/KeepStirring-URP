using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CauldronMove : MonoBehaviour
{
    public Animator stir;
    public Animator barsie;
    public Animator soupSpin;
    public bool stirring = false;
    public GameObject bar;
    public AudioSource elStir;
    public float ingredientIncrease = 15;
    public float ingredientDecrease = 10;
    public float meterIncrease = 5f;
    public float meterDecrease = 1f;
    public float stirringMeter = 100f;
    public float stirringMax = 80f;
    public float tutorialDecrease;
    public bool isNear = false;
    public bool isPlaying = false;
    private float animCD = .5f;
    private float timer = 0;
    private bool isHeld = false;
    public DumbWays dumbWays;

    private void OnEnable()
    {
        RecipeManager.OnIngredientAdded += IncreaseBar;
        RecipeManager.OnIngredientStrike += LowerBar;
    }

    private void OnDisable()
    {
        RecipeManager.OnIngredientAdded -= IncreaseBar;
        RecipeManager.OnIngredientStrike -= LowerBar;
    }


    private void Start()
    {
        var currentScene = SceneManager.GetActiveScene();

        var sceneName = currentScene.name;

        if (sceneName == "Tutorial") meterDecrease = tutorialDecrease;
    }


    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > animCD && isPlaying) SetIdle();

        if (Input.GetKeyDown(KeyCode.E) && isNear)
        {
            //Debug.Log("presione E");
            elStir.Play();
            stirring = true;
            timer = 0;
        }

        else if (Input.GetKeyUp(KeyCode.E) || !isNear)
        {
            stirring = false;
            isHeld = false;
            elStir.Stop();
        }

        if (Input.GetKey(KeyCode.E) && isNear) isHeld = true;

        if (!isPlaying && isNear && isHeld)
        {
            //Debug.Log("animadote");
            stir.SetBool("Stirring", true);
            stir.speed = 1;
            soupSpin.SetBool("Spinning", true);
            soupSpin.speed = 1;
            isPlaying = true;
        }

        if (stirringMeter <= 0) dumbWays.DeathByExplosion();

        AnimateBarFill();

        if (stirringMeter <= 15f)
        {
            if (barsie != null)
                //Debug.Log("no soy nulo");
                barsie.SetBool("Trembling", true);
        }

        else
        {
            barsie.SetBool("Trembling", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isNear = false;
    }

    public void AnimateBarFill()
    {
        float newMeter;

        if (stirring)
            newMeter = stirringMeter + meterIncrease * Time.deltaTime;

        else
            newMeter = stirringMeter - meterDecrease * Time.deltaTime;
        stirringMeter = Mathf.Clamp(newMeter, 0, stirringMax);

        //Debug.Log(newMeter);

        bar.transform.localScale = new Vector3(stirringMeter / stirringMax, 1f, 1f);
        //LeanTween.scaleX(bar, 1, stirringChange);
    }

    public void SetIdle()
    {
        //stir.SetBool("Stirring", false);
        isPlaying = false;
        stir.speed = 0;
        soupSpin.speed = 0;
    }

    public void LowerBar()
    {
        stirringMeter -= ingredientDecrease;
    }

    public void IncreaseBar(Ingredient ingredient, int amount)
    {
        stirringMeter += ingredientIncrease;
    }
}