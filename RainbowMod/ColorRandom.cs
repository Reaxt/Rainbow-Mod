using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace RainbowMod
{
    class ColorRandom : SimpleColorSO
    {
        public override void SetColor(Color c)
        {
            this._color = c;
        }

        public override Color color
        {
           

            get { return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)); }
            
        }
    }
}
