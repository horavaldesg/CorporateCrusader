using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string equipmentName;
    [TextArea(5,10)]public string equipmentDescription;
    public Sprite equipmentSprite;

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
    
    public virtual void Level5()
    {
    }
}
