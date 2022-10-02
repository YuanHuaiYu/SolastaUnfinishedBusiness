﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class NarrativeStateNpcSpeechPatcher
{
    [HarmonyPatch(typeof(NarrativeStateNpcSpeech), "RecordSpeechLine")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class RecordSpeechLine_Patch
    {
        public static void Postfix(string speakerName, string textLine)
        {
            //PATCH: EnableLogDialoguesToConsole
            if (!Main.Settings.EnableLogDialoguesToConsole)
            {
                return;
            }

            Gui.Game.GameConsole.LogSimpleLine($"<b><color=white>{speakerName}:</color></b> {textLine}");
        }
    }
}
