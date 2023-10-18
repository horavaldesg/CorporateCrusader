using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Hat.ChosenHat hat;
    public string equipmentName;
    [TextArea(5,10)]public string equipmentDescription;
    public Sprite equipmentSprite;
    public Sprite evoSprite;

    public int level;
    
    private void Start()
    {
        Destroy(gameObject, 2);
    }

    public void AffectPlayer(int level)
    {
        this.level = level;
        if(this.level >= 5) return;
        switch (level)
        {
            case 1:
                Level1();
                break;
            case 2:
                Level2();
                break;
            case 3:
                Level3();
                break;
            case 4:
                Level4();
                break;
            case 5:
                Level5();
                break;
            case 6:
                Evolve();
                break;
        }
    }

    public virtual void Level1()
    {
    }
    
    public virtual void Level2()
    {
    }
    
    public virtual void Level3()
    {
    }
    
    public virtual void Level4()
    {
    }

    protected virtual void Level5()
    {
        if (HatMatches())
        {
            Evolve();
        }
    }
    
    protected virtual void Evolve()
    {
        level = 5;
        if (!HatMatches()) return;
        // Equipment Evolution
    }

    private bool HatMatches()
    {
        var matches = false;
        foreach (var unused in PlayerController.Instance.ChosenHat().Where(currentHat => currentHat == hat))
        {
            matches = true;
        }
        
        return matches;
    }
}
