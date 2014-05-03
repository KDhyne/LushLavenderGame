using System.Collections.Generic;
using UnityEngine;

public class CollectableGroup : MonoBehaviour
{
    private readonly List<Collectable> collectables = new List<Collectable>();

    public void Start()
    {
        //Get all child gameobjects and add them to the collectables list
        for (var i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i).gameObject.GetComponent<Collectable>();
            this.collectables.Add(c);
        }

        //Set all collectables' group setting to true
        foreach (var collectable in this.collectables)
        {
            collectable.PartOfGroup = true;
        }
    }

    public void DestroyGroup()
    {
        foreach (var collectable in this.collectables)
        {
            collectable.DestroyCollectable();
        }
    }

}
