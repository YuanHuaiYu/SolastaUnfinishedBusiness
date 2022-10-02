﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class FeatureDescriptionItemPatcher
{
    //PATCH: Disables choices dropdown for features already taken on previous levels (MULTICLASS)
    [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix([NotNull] FeatureDescriptionItem __instance)
        {
            var hero = Global.ActiveLevelUpHero;

            if (hero == null)
            {
                return;
            }

            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

            if (!isClassSelectionStage)
            {
                return;
            }

            __instance.choiceDropdown.gameObject.SetActive(false);
        }
    }
}
