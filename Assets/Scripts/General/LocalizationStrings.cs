using System;
using System.Xml.Schema;

#region

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

#endregion

//Phrase should be always static, or it can be problems with localization and loading
public class Phrase
{
    public string Id;
    public string Str;

    public Phrase(string s, string id)
    {
        Str = s;
        Id = id;
        //start id with location. Example: WinPopup_Score
        if (LocalizationStrings.LoadedStings.Any(x => x.Id == id) )
        {
            Debug.Log("This locid id used!");
            return;
        }

        LocalizationStrings.LoadedStings.Add(this);
    }

    public override string ToString()
    {
        return Str;
    }
}

public class LocalizationStrings : MonoBehaviour
{
    public static List<Phrase> LoadedStings = new List<Phrase>();

    public static string CurrentLocalization;

    public static string GetString(Phrase phrase, params object[] paramsObjects)
    {
        return string.Format(phrase.ToString(), paramsObjects);
    }

    public static void LoadLanguage(string language, Dictionary<string, string> languageDictionary)
    {
        foreach (var phrase in languageDictionary)
        {
            var first = LoadedStings.FirstOrDefault(x => x.Id == phrase.Key);
            if (first != null) first.Str = phrase.Value;
        }
    }

    public static void PrintStrings()
    {
        var answer = String.Empty;
        foreach (Phrase sting in LoadedStings)
        {   
            var replaced = sting.Str.Replace(Environment.NewLine, "&n");
            answer = answer + (sting.Id + "|" + replaced + Environment.NewLine);
        }
        Debug.Log(answer);
    }

    public static LocalizationStrings Instance;
    public void Awake()
    {
        Instance = this;
        PrintStrings();
        LoadLocalization(DefaultLocalization);
    }

    public TextAsset English;
    public TextAsset Russian;

    public TextAsset DefaultLocalization;

    public void LoadLocalization(TextAsset language)
    {
        var lines = language.text.Split(new[] {Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        var dict = new Dictionary<string, string>();
        foreach (var line in lines)
        {
            var split = line.Split('|');
            split[1] = split[1].Replace("&n", Environment.NewLine);
            dict[split[0]] = split[1];
        }
        LoadLanguage("En", dict);
    }
}