using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeParent : MonoBehaviour {
    protected List<ChainHover> chains;

    protected virtual void Awake()
    {
        chains = new List<ChainHover>();
        GetComponentsInChildren(chains);
    }

    public virtual void RopeBroken()
    {
        for (int i = 0; i < chains.Count; i++)
        {
            if (chains[i] == null)
                continue;

            chains[i].RopeBroke();
        }
    }
}
