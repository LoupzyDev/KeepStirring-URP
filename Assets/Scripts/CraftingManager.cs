using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Data", menuName = "Crafting/Manager")]
public class CraftingManager : ScriptableObject
{
    public List<Craftable> craftables;

    public void AddCraftable()
    {
        craftables.Add(new Craftable());
        Debug.Log("Added craftable");
        Debug.Log(craftables);
    }

    public void RemoveCraftable(int index)
    {
        craftables.RemoveAt(index);
    }

}
