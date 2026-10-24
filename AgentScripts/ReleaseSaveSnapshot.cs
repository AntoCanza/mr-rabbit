using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using MrRabbit;
public static class ReleaseSaveSnapshot
{
    [Serializable] public sealed class Entry {public string key,value;public bool exists,integer;}
    [Serializable] public sealed class Snapshot {public Entry[] entries;public string garden,city,school,learning;}
    static string PathName=>Path.Combine(Directory.GetCurrentDirectory(),"Library","MrRabbitQA","saves-0.9.0.json");
    static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
    static Entry[] Entries(){var keys=new[]{GardenController.SaveKey,CityProgress.SaveKey,SchoolController.SaveKey,LearningProgress.SaveKey,"MrRabbit.TV.Muted",ItalianVoice.MuteKey};var entries=new Entry[keys.Length];for(int i=0;i<keys.Length;i++)entries[i]=new Entry{key=keys[i],integer=i>=4,exists=PlayerPrefs.HasKey(keys[i]),value=i>=4?PlayerPrefs.GetInt(keys[i]).ToString():PlayerPrefs.GetString(keys[i])};return entries;}
    static Snapshot Read(){var s=Newtonsoft.Json.JsonConvert.DeserializeObject<Snapshot>(File.ReadAllText(PathName));if(s.entries==null){s.entries=Entries();File.WriteAllText(PathName,Newtonsoft.Json.JsonConvert.SerializeObject(s,Newtonsoft.Json.Formatting.Indented));}return s;}
    public static string Begin()
    {
        if(File.Exists(PathName))throw new Exception("A snapshot already exists. Restore before replacing it.");
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();var keys=new[]{GardenController.SaveKey,CityProgress.SaveKey,SchoolController.SaveKey,LearningProgress.SaveKey,"MrRabbit.TV.Muted",ItalianVoice.MuteKey};var snap=new Snapshot{entries=new Entry[keys.Length]};
        for(int i=0;i<keys.Length;i++)snap.entries[i]=new Entry{key=keys[i],integer=i>=4,exists=PlayerPrefs.HasKey(keys[i]),value=i>=4?PlayerPrefs.GetInt(keys[i]).ToString():PlayerPrefs.GetString(keys[i])};
        snap.garden=JsonUtility.ToJson(a.GetComponent<GardenController>().Progress);snap.city=JsonUtility.ToJson(a.GetComponent<CityController>().Progress);snap.learning=JsonUtility.ToJson(a.GetComponent<LearningJourney>().Progress);snap.school=JsonUtility.ToJson(typeof(SchoolController).GetField("progress",F).GetValue(a.GetComponent<SchoolController>()));
        Directory.CreateDirectory(Path.GetDirectoryName(PathName));File.WriteAllText(PathName,Newtonsoft.Json.JsonConvert.SerializeObject(snap,Newtonsoft.Json.Formatting.Indented));return "Player saves backed up outside Assets";
    }
    public static string Restore()
    {
        var s=Read();if(Application.isPlaying)
        {
            var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();a.GetComponent<LearningJourney>().Close();a.GetComponent<CharacterSelection>().ShowSelection();
            JsonUtility.FromJsonOverwrite(s.garden,a.GetComponent<GardenController>().Progress);typeof(GardenController).GetField("dirty",F).SetValue(a.GetComponent<GardenController>(),false);
            JsonUtility.FromJsonOverwrite(s.city,a.GetComponent<CityController>().Progress);JsonUtility.FromJsonOverwrite(s.learning,a.GetComponent<LearningJourney>().Progress);
            JsonUtility.FromJsonOverwrite(s.school,typeof(SchoolController).GetField("progress",F).GetValue(a.GetComponent<SchoolController>()));a.GetComponent<LearningJourney>().AutoOpen=true;
        }
        foreach(var e in s.entries){if(!e.exists)PlayerPrefs.DeleteKey(e.key);else if(e.integer)PlayerPrefs.SetInt(e.key,int.Parse(e.value));else PlayerPrefs.SetString(e.key,e.value);}PlayerPrefs.Save();return "Original player saves restored";
    }
}
