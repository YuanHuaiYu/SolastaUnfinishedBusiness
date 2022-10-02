﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Patches;

public static class ReactionModalPatcher
{
    //TODO: Create a FeatureBuilder with Validators to create a generic check here
    [HarmonyPatch(typeof(ReactionModal), "ReactionTriggered")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ReactionTriggered_Patch
    {
        public static bool Prefix(ReactionRequest request)
        {
            // wildshape heroes should not be able to cast spells
            var rulesetCharacter = request.Character.RulesetCharacter;

            if (rulesetCharacter.IsSubstitute
                && request is ReactionRequestCastSpell or ReactionRequestCastFallPreventionSpell
                    or ReactionRequestCastImmunityToSpell)
            {
                ServiceRepository.GetService<ICommandService>().ProcessReactionRequest(request, false);
                return false;
            }

            // Tacticians heroes should only CounterStrike with melee weapons
            if (request is ReactionRequestCounterAttackWithPower &&
                request.SuboptionTag == MartialTactician.CounterStrikeTag &&
                request.Character.RulesetCharacter is RulesetCharacterHero hero && hero.IsWieldingRangedWeapon())
            {
                ServiceRepository.GetService<ICommandService>().ProcessReactionRequest(request, false);
                return false;
            }

            return true;
        }
    }
}
