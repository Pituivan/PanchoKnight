using System.Collections;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace PanchoKnight;

[BepInPlugin("com.pituivan.panchoknight", "Pancho Knight", "0.0.1")]
public class Mod : BaseUnityPlugin
{
    private readonly List<AudioClip> moans = [];
    
    public static Mod Instance { get; private set; } = null!;

    public IReadOnlyList<AudioClip> Moans => moans;
    
    void Awake()
    {
        Instance = this;
        
        LoadResources();
        
        var harmony = new Harmony("com.pituivan.panchoknight");
        harmony.PatchAll();
    }

    private void LoadResources()
    {
        StartCoroutine(ResourceLoader.LoadAudioClipsFromWavsCoroutine("Moan*", moans));
    }
}

file static class ResourceLoader
{
    private static readonly string path = Path.Combine(Paths.PluginPath, "Resources");
    
    public static IEnumerator LoadAudioClipsFromWavsCoroutine(string searchPatternWithoutExtension, IList<AudioClip> output)
    {
        string path = Path.Combine(ResourceLoader.path, "Audio");
        string[] files = Directory.GetFiles(path, searchPatternWithoutExtension + ".wav");

        for (int i = 0; i < files.Length; i++)
        {
            var filePath = files[i];
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);
            yield return www.SendWebRequest();

            output[i] = www.result == UnityWebRequest.Result.Success
                ? DownloadHandlerAudioClip.GetContent(www)
                : throw new InvalidOperationException(www.error);
        }
    }
}