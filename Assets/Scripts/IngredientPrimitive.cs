using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient Primitive", menuName = "Crafting/Ingredient Primitive")]
public class IngredientPrimitive : ScriptableObject
{
    public string Name;
    public Sprite SourceImage;
    public IngredientColor Color;

    public IngredientPrimitive(string name, Sprite image, IngredientColor color)
    {
        this.Name = name;
        this.SourceImage = image;
        this.Color = color;
    }

    public override string ToString()
    {
        return Name;
    }
}