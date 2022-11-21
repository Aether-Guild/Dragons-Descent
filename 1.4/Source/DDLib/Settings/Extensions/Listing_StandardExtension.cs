using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DD
{
    public static class Listing_StandardExtension
    {

        public static float SliderLabeled(this Listing_Standard listing, string label, float val, float min, float max)
        {
            var rect = listing.GetRect(22f);
            Widgets.LabelFit(rect.LeftHalf(), label);
            float num = Widgets.HorizontalSlider(rect.RightHalf(), val, min, max, true);
            if (num != val)
            {
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
            }

            listing.Gap(listing.verticalSpacing);
            return num;
        }
    }
}
