using System;
using System.IO;
using System.Linq;
using UnityEngine;
using MrRabbit;

public static class GuideSaveSafety
{
    public sealed class Entry {public string key,value;public bool exists,integer;}
    public static string Capture()
    {
        if(Application.isPlaying)throw new Exception("Capture original disk saves before Play");
        var keys=new[]{GardenController.SaveKey,CityProgress.SaveKey,SchoolController.SaveKey,LearningProgress.SaveKey,GameGuideProgress.SaveKey,"MrRabbit.TV.Muted",ItalianVoice.MuteKey};
        var entries=keys.Select(k=>new Entry{key=k,exists=PlayerPrefs.HasKey(k),integer=k==ItalianVoice.MuteKey||k=="MrRabbit.TV.Muted",value=k==ItalianVoice.MuteKey||k=="MrRabbit.TV.Muted"?PlayerPrefs.GetInt(k).ToString():PlayerPrefs.GetString(k)}).ToArray();
        string path=Path.Combine(Directory.GetCurrentDirectory(),"Library/MrRabbitQA/guide-original-"+Guid.NewGuid().ToString("N")+".json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,Newtonsoft.Json.JsonConvert.SerializeObject(entries,Newtonsoft.Json.Formatting.Indented));return path;
    }
    public static string Restore(string path)
    {
        if(Application.isPlaying)throw new Exception("Stop Play before restoring disk saves");
        var entries=Newtonsoft.Json.JsonConvert.DeserializeObject<Entry[]>(File.ReadAllText(path));
        foreach(var e in entries){if(!e.exists)PlayerPrefs.DeleteKey(e.key);else if(e.integer)PlayerPrefs.SetInt(e.key,int.Parse(e.value));else PlayerPrefs.SetString(e.key,e.value);}PlayerPrefs.Save();return "Original PC saves restored: "+entries.Length+" keys";
    }
}
