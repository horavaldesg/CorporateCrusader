using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FishingNet : Equipment
{
   public float range;

   public override void Level1()
   {
      base.Level1();
      PlayerController.Instance.IncreaseRange(range);
   }

   public override void Level2()
   {
      base.Level2();
      PlayerController.Instance.IncreaseRange(range);
   }

   public override void Level3()
   {
      base.Level3();
      PlayerController.Instance.IncreaseRange(range);
   }

   public override void Level4()
   {
      base.Level4();
      PlayerController.Instance.IncreaseRange(range);
   }

   public override void Level5()
   {
      base.Level5();
      PlayerController.Instance.IncreaseRange(range);
   }
}
