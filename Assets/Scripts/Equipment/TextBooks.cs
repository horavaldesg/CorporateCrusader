using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBooks : Equipment
{
   public float xpMultiplier;

   public override void Level1()
   {
      base.Level1();
      PlayerController.Instance.IncreaseXpGain(xpMultiplier);
   }
   
   public override void Level2()
   {
      base.Level2();
      PlayerController.Instance.IncreaseXpGain(xpMultiplier);
   }
   
   public override void Level3()
   {
      base.Level3();
      PlayerController.Instance.IncreaseXpGain(xpMultiplier);
   }
   
   public override void Level4()
   {
      base.Level4();
      PlayerController.Instance.IncreaseXpGain(xpMultiplier);
   }

   protected override void Level5()
   {
      base.Level5();
      PlayerController.Instance.IncreaseXpGain(xpMultiplier);
   }
}
