﻿using System;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using WOTR_FxTweaks.Config;

namespace WOTR_FxTweaks
{
    static class Spells
    {
		[HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;
                if (!Main.Enabled) { return; }
                Main.Log("Initialize blueprint patching process...");
                if (ModSettings.BuffsToTweak.Count > 0)
                {
                    Main.Log($"Tweaking {ModSettings.BuffsToTweak.Count} buffs.");
                    try
                    {
                        foreach (Buff buffToTweak in ModSettings.BuffsToTweak)
                        {
                            Main.DebugLog("Tweaking: " + buffToTweak.Name);
                            if (buffToTweak.IsMythicClassFx)
                            {
                                var BlueprintClassToTweak = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(buffToTweak.Id);
                                var ClassWithNullVisualSettingsFx = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>("0937bec61c0dabc468428f496580c721"); // Using Alchemist which is empty
                                if (buffToTweak.DisableFx)
                                {
                                    BlueprintClassToTweak.m_AdditionalVisualSettings = ClassWithNullVisualSettingsFx.m_AdditionalVisualSettings;
                                }
                                else
                                {
                                    var OverrideBlueprintClass = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(buffToTweak.OverrideFxId);
                                    BlueprintClassToTweak.m_AdditionalVisualSettings = OverrideBlueprintClass.m_AdditionalVisualSettings;
                                }
                            }
                            else
                            {
                                var BlueprintBuffToTweak = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(buffToTweak.Id);
                                if (buffToTweak.DisableFx)
                                {
                                    BlueprintBuffToTweak.FxOnStart = BlueprintBuffToTweak.FxOnRemove;
                                }
                                else
                                {
                                    var OverrideBlueprintBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(buffToTweak.OverrideFxId);
                                    BlueprintBuffToTweak.FxOnStart = OverrideBlueprintBuff.FxOnStart;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Main.Error("Failed Blueprint patching.");
                        Main.Error(e.Message);
                    }
                }
                Main.Log("Patching process complete.");
            }
        }
    }
}
