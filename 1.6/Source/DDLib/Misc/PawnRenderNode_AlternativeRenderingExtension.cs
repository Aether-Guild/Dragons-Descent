
using Verse;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LudeonTK;
using UnityEngine;
using System.Text.RegularExpressions;

namespace DD
{
    public class PawnRenderNode_AlternativeRenderingExtension(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : PawnRenderNode_AnimalPart_Body(pawn, props, tree)
    {

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.def.GetModExtension<AlternativeRenderingExtension>() is AlternativeRenderingExtension ext && ext.ageRange is IntRange range && pawn.ageTracker.AgeBiologicalYears >= range.min && pawn.ageTracker.AgeBiologicalYears <= range.max && ext.pathReplacement is AlternativeRenderingExtension.Data replacement)
            {
                Graphic graphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
                return GraphicDatabase.Get<Graphic_Multi>(Regex.Replace(graphic.path, replacement.from, replacement.to, RegexOptions.IgnoreCase), graphic.Shader, graphic.drawSize, graphic.color != null ? graphic.color : Color.white, graphic.colorTwo != null ? graphic.colorTwo : Color.white, graphic.data);
            }
            else
            {
                return base.GraphicFor(pawn);
            }
        }
    }
}