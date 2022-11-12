﻿using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class TwoWeaponCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featDualFlurry = BuildDualFlurry();
        var featDualWeaponDefense = BuildDualWeaponDefense();

        feats.Add(featDualFlurry);
        feats.Add(featDualWeaponDefense);

        GroupFeats.MakeGroup("FeatGroupTwoWeaponCombat", null,
            FeatDefinitions.Ambidextrous,
            FeatDefinitions.TwinBlade,
            featDualFlurry,
            featDualWeaponDefense);
    }

    private static FeatDefinition BuildDualFlurry()
    {
        var conditionDualFlurryApply = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryApply")
            .SetGuiPresentation(Category.Condition)
            .SetDuration(DurationType.Round, 0, false)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Beneficial)
            .AddToDB();

        var conditionDualFlurryGrant = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryGrant")
            .SetGuiPresentation(Category.Condition)
            .SetDuration(DurationType.Round, 0, false)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create(AdditionalActionSurgedMain, "AdditionalActionDualFlurry")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionDefinitions.ActionType.Bonus)
                    .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                    .AddToDB())
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatDualFlurry")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("OnAttackDamageEffectFeatDualFlurry")
                    .SetGuiPresentation("FeatDualFlurry", Category.Feat)
                    .SetCustomSubFeatures(
                        new OnAttackHitEffectFeatDualFlurry(conditionDualFlurryGrant, conditionDualFlurryApply))
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildDualWeaponDefense()
    {
        return FeatDefinitionBuilder
            .Create("FeatDualWeaponDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierSwiftBladeBladeDance)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private sealed class OnAttackHitEffectFeatDualFlurry : IOnAttackHitEffect
    {
        private readonly ConditionDefinition _conditionDualFlurryApply;
        private readonly ConditionDefinition _conditionDualFlurryGrant;

        internal OnAttackHitEffectFeatDualFlurry(
            ConditionDefinition conditionDualFlurryGrant,
            ConditionDefinition conditionDualFlurryApply)
        {
            _conditionDualFlurryGrant = conditionDualFlurryGrant;
            _conditionDualFlurryApply = conditionDualFlurryApply;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode == null)
            {
                return;
            }

            var condition = attacker.RulesetCharacter.HasConditionOfType(_conditionDualFlurryApply.Name)
                ? _conditionDualFlurryGrant
                : _conditionDualFlurryApply;

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                attacker.RulesetCharacter.Guid,
                condition,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                attacker.RulesetCharacter.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }
}
