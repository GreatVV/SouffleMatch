

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ParseFNTfile : ScriptableObject
{


    [MenuItem("Assets/Parse fnt")]
    static void ParseFnt()
    {
        Object[] fonts = GetSelectedTextAssest();
        Selection.objects = new Object[0];
        List<Object> created = new List<Object>();
        foreach (TextAsset font in fonts)
        {
            string path = AssetDatabase.GetAssetPath(font);
            var customFont = new Font(font.name);

            var index = 0;
            var textureWidth = 0f;
            var textureHeight = 0f;
            var lines = font.text.Split('\n');
            var charInfo = new List<CharacterInfo>();
            foreach (var line in lines)
            {
                var currentLineWords = line.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                if (line.StartsWith("char "))
                {
                    var currentCharacterInfo = new CharacterInfo();

                    foreach (var currentLineWord in currentLineWords)
                    {
                        var split = currentLineWord.Split(new []{'='},StringSplitOptions.RemoveEmptyEntries);
                     //   Debug.Log("split:"+split[0]);
                        switch (split[0])
                        {
                            case "id":
                                Debug.Log("id: "+split[1]);
                                currentCharacterInfo.index = Convert.ToInt32(split[1]);
                                break;
                            case "x":
                                currentCharacterInfo.vert.x = Convert.ToInt32(split[1]);
                                currentCharacterInfo.uv.x = currentCharacterInfo.vert.x/textureWidth;
                                break;
                            case "y":
                                currentCharacterInfo.vert.y = Convert.ToInt32(split[1]);
                                currentCharacterInfo.uv.y = currentCharacterInfo.vert.y / textureHeight;
                                break;
                            case "width":
                                currentCharacterInfo.vert.width = Convert.ToInt32(split[1]);
                                currentCharacterInfo.uv.width = currentCharacterInfo.vert.width / textureWidth;
                                break;
                            case "height":
                                currentCharacterInfo.vert.height = Convert.ToInt32(split[1]);
                                currentCharacterInfo.uv.height = currentCharacterInfo.vert.y / textureHeight;
                                break;
                            case "xadvance":
                                currentCharacterInfo.width = Convert.ToInt32(split[1]);
                                break;
                        }
                    }
                    charInfo.Add(currentCharacterInfo);
                }
                else
                {
                    foreach (var currentLineWord in currentLineWords)
                    {
                      //  Debug.Log("current word: "+currentLineWord);
                        if (currentLineWord.StartsWith("face"))
                        {
                          //  var fontFamily = currentLineWord.Split('=');
                            //customFont.fontNames = new[] {fontFamily[1]};
                        }

                        if (currentLineWord.StartsWith("count") && line.StartsWith("chars"))
                        {
                            var count = Convert.ToInt32(currentLineWord.Split('=')[1]);
                            Debug.Log("count: "+count);
                            customFont.characterInfo = new CharacterInfo[count];
                        }

                        if (currentLineWord.StartsWith("scaleW"))
                        {
                            textureWidth = Convert.ToInt32(currentLineWord.Split('=')[1]);
                        }

                        if (currentLineWord.StartsWith("scaleW"))
                        {
                            textureHeight = Convert.ToInt32(currentLineWord.Split('=')[1]);
                        }
                    }
                }
            }
            customFont.characterInfo = charInfo.ToArray();
            foreach (var characterInfo in customFont.characterInfo)
            {
                Debug.Log("info: "+characterInfo.index + " : "+characterInfo);
            }
            
            string mpath = path;
            mpath = mpath.Substring(0, mpath.LastIndexOf('.')) + ".fontsettings";
            created.Add(customFont);
            AssetDatabase.CreateAsset(customFont, mpath);
        }
        Selection.objects = created.ToArray();
    }

    static Object[] GetSelectedTextAssest()
    {
        return Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);
    }

}
