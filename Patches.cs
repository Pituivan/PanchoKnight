using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PanchoKnight;

[HarmonyPatch(typeof(HeroController), nameof(HeroController.TakeDamage))]
internal class HeroController_TakeDamage_Patch
{
    static void Postfix(HeroController __instance)
    {
        var moans = Mod.Instance.Moans;
        if (moans is not { Count: > 0 }) return;
        
        AudioClip randomMoan = moans[Random.Range(0, moans.Count)];
        AudioSource.PlayClipAtPoint(randomMoan, __instance.transform.position);   
    }
}