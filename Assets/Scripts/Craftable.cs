using UnityEngine;

[System.Serializable]
public enum IngredientColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Orange
}

[CreateAssetMenu(fileName = "Craftable Data", menuName = "Crafting/Craftable")]
public class Craftable : ScriptableObject
{
    public IngredientPrimitive Primitive;

    // Each ingredient must have the ingredient script attached
    public GameObject RawIngredient;

    // Single state prefabs
    public GameObject ChoppedIngredient;
    public GameObject CookedIngredient;

    public GameObject BurntIngredient;

    // Multi state prefabs
    public GameObject ChoppedCookedIngredient;
    public GameObject ChoppedBurntIngredient;


    public override string ToString()
    {
        return Primitive.ToString();
    }
}