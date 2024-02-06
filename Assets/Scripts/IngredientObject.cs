using UnityEngine;


public class Ingredient
{
    public IngredientPrimitive Primitive;
    public int Quantity;
    public ChopState Chop;
    public CookState Cook;

    public Ingredient(IngredientPrimitive primitive, ChopState chop, CookState cook, int quantity = 1)
    {
        Primitive = primitive;
        Quantity = quantity;
        Chop = chop;
        Cook = cook;
    }

    public Ingredient(IngredientObject ingredient)
    {
        Primitive = ingredient.Primitive;
        Quantity = 1;
        Chop = ingredient.Chop;
        Cook = ingredient.Cook;
    }

    public override string ToString()
    {
        return Quantity.ToString() + "x " + Primitive.Name + " (" + Primitive.Color.ToString() + ", " +
               Chop.ToString() + ", " + Cook.ToString() + ")";
    }
}

public class IngredientObject : MonoBehaviour
{
    public IngredientPrimitive Primitive;
    public int Quantity;
    public ChopState Chop;
    public CookState Cook;


    public override string ToString()
    {
        return Quantity.ToString() + "x " + Primitive.Name + " (" + Primitive.Color.ToString() + ", " +
               Chop.ToString() + ", " + Cook.ToString() + ")";
    }
}