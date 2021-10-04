using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Structure : MonoBehaviour
{
    public int hp;
    public int cost;    //Cost is only relevant for Structures that are placable or removable
    public bool removable = false;

    public void Damage(int dmg) {
        hp -= dmg;
        if (hp <= 0) {
            Remove();
        }
    }

    public int GetHp() {
        return hp;
    }

    public void Remove() {
        //Inform the Node
        var node = GetComponentInParent<WorldNode>();
        Assert.IsNotNull(node);
        node.RemoveStructure();
        //Destroy the Structure
        Destroy(gameObject);
    }
}
