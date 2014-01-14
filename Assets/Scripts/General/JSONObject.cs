using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/*
 * http://www.opensource.org/licenses/lgpl-2.1.php
 * JSON class
 * for use with Unity
 * Copyright Matt Schoen 2010
 * Updated by GreatVV in 2013
 */

public class JSONObject : Nullable
{
    private const int MaxDepth = 1000;

    public enum Type
    {
        NULL,
        STRING,
        NUMBER,
        OBJECT,
        ARRAY,
        BOOL
    }

    public JSONObject parent;
    public Type type = Type.NULL;
    public List<JSONObject> list = new List<JSONObject>();
    public List<string> keys = new List<string>();
    public string str;
    public float n; // double is not supported in Flash :(
    public bool b;

    public int integer
    {
        get
        {
            return (int)n;
        }
    }

    public static JSONObject NullJo
    {
        get { return new JSONObject(Type.NULL); }
    }

    public static JSONObject Obj
    {
        get { return new JSONObject(Type.OBJECT); }
    }

    public static JSONObject Arr
    {
        get { return new JSONObject(Type.ARRAY); }
    }

    public JSONObject(Type t)
    {
        type = t;
        switch (t)
        {
            case Type.ARRAY:
                list = new List<JSONObject>();
                break;
            case Type.OBJECT:
                list = new List<JSONObject>();
                keys = new List<string>();
                break;
        }
    }

    public JSONObject(bool b)
    {
        type = Type.BOOL;
        this.b = b;
    }

    public JSONObject(float f)
    {
        type = Type.NUMBER;
        this.n = f;
    }

    public JSONObject()
    {
        type = Type.OBJECT;
        list = new List<JSONObject>();
        keys = new List<string>();
    }

    public JSONObject(string str)
    {
        //create a new JSON from a string (this will also create any children, and parse the whole string)
        //Debug.Log(str);

        if (str != null)
        {

            str = str.Replace("\\n", "");
            str = str.Replace("\\t", "");
            str = str.Replace("\\r", "");
            str = str.Replace("\t", "");
            str = str.Replace("\n", "");
            str = str.Replace("\\", "");
            str = str.Replace(" ", "");


            str = str.Trim();
            if (str.Length > 0)
            {
                switch (str)
                {
                    case "TRUE":
                    case "True":
                    case "true":
                        type = Type.BOOL;
                        b = true;
                        break;
                    case "FALSE":
                    case "False":
                    case "false":
                        type = Type.BOOL;
                        b = false;
                        break;
                    case "null":
                        type = Type.NULL;
                        break;
                    default:
                        switch (str[0])
                        {
                            case '\"':
                                type = Type.STRING;
                                this.str = str.Substring(1, str.Length - 2);
                                break;
                            case '[':
                            case '{':
                                {
                                    var tokenTmp = 0;
                                    /*
                                     * Checking for the following formatting (www.json.org)
                                     * object - {"field1":value,"field2":value}
                                     * array - [value,value,value]
                                     * value - string	- "string"
                                     *		 - number	- 0.0
                                     *		 - bool		- true -or- false
                                     *		 - null		- null
                                     */
                                    switch (str[0])
                                    {
                                        case '{':
                                            type = Type.OBJECT;
                                            keys = new List<string>();
                                            list = new List<JSONObject>();
                                            break;
                                        case '[':
                                            type = Type.ARRAY;
                                            list = new List<JSONObject>();
                                            break;
                                        default:
                                            type = Type.NULL;
                                            Debug.LogWarning("improper JSON formatting:" + str);
                                            return;
                                    }
                                    // Parse
                                    var depth = 0;
                                    var openquote = false;
                                    var inProp = false;

                                    for (var i = 1; i < str.Length; i++)
                                    {
                                        if (str[i] == '\\')
                                        {
                                            i++;
                                            continue;
                                        }
                                        switch (str[i])
                                        {
                                            case '\"':
                                                openquote = !openquote;
                                                break;
                                            case '{':
                                            case '[':
                                                depth++;
                                                break;
                                            default:
                                                if (depth == 0 && !openquote)
                                                {
                                                    if (str[i] != ':' || inProp)
                                                    {
                                                        switch (str[i])
                                                        {
                                                            case ',':
                                                                inProp = false;
                                                                list.Add(
                                                                    new JSONObject(str.Substring(tokenTmp + 1,i - tokenTmp - 1)));
                                                                tokenTmp = i;
                                                                break;
                                                            case '}':
                                                            case ']':
                                                                list.Add(
                                                                    new JSONObject(str.Substring(tokenTmp + 1,
                                                                                                 i - tokenTmp - 1)));
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        inProp = true;
                                                        try
                                                        {
                                                            //string propName = str.Substring(tokenTmp + 2,i - tokenTmp - 3);
                                                            string propName = str.Substring(tokenTmp + 1, i - tokenTmp - 1);
                                                            /*if (propName.StartsWith("\""))
                                                            {
                                                                propName = propName.Replace("\"", "");
                                                            }*/
                                                            propName = propName.Trim().Trim('"');
                                                            keys.Add(propName);
                                                        }
                                                        catch
                                                        {
                                                            Debug.Log(i + " - " + str.Length + " - " + str);
                                                        }
                                                        tokenTmp = i;
                                                    }
                                                }
                                                break;
                                        }
                                        if (str[i] == ']' || str[i] == '}')
                                            depth--;
                                    }
                                }
                                break;
                            default:
                                n = float.Parse(str); //System.Convert.ToDouble(str);
                                type = Type.NUMBER;
                                break;
                        }

                        break;
                }
            }
        }
        else
        {
            type = Type.NULL; //If the string is missing, this is a null
        }
    }

    public void AddField(bool val)
    {
        Add(new JSONObject(val));
    }

    public void AddField(float val)
    {
        Add(new JSONObject(val));
    }

    public void AddField(int val)
    {
        Add(new JSONObject(val));
    }

    public void Add(JSONObject jsonObject)
    {
        if (jsonObject)
        {
            //Don't do anything if the object is null
            if (type != Type.ARRAY)
            {
                type = Type.ARRAY; //Congratulations, son, you're an ARRAY now
                Debug.LogWarning(
                    "tried to add an object to a non-array JSON.  We'll do it for you, but you might be doing something wrong.");
            }
            list.Add(jsonObject);
        }
    }

    public void AddField(string name, bool val)
    {
        AddField(name, new JSONObject(val));
    }

    public void AddField(string name, float val)
    {
        AddField(name, new JSONObject(val));
    }

    public void AddField(string name, int val)
    {
        AddField(name, new JSONObject(val));
    }

    public void AddField(string name, string val)
    {
        AddField(name, new JSONObject {type = Type.STRING, str = val});
    }

    public void AddField(string name, JSONObject obj)
    {
        if (obj)
        {
            //Don't do anything if the object is null
            if (type != Type.OBJECT)
            {
                type = Type.OBJECT; //Congratulations, son, you're an OBJECT now
                Debug.LogWarning(
                    "tried to add a field to a non-object JSON.  We'll do it for you, but you might be doing something wrong.");
            }
            name = name.Trim().Trim('"');
            keys.Add(name);
            list.Add(obj);
        }
    }

    public void SetField(string name, JSONObject obj)
    {
        if (HasField(name))
        {
            list.Remove(this[name]);
            keys.Remove(name);
        }
        AddField(name, obj);
    }

    public JSONObject GetField(string name, JSONObject defaultVar = null)
    {
        if (type == Type.OBJECT)
        {
            for (var i = 0; i < keys.Count(); i++)
            {
                if (keys[i] == name)
                {
                    return list[i];
                }
            }
        }

        return defaultVar;
    }

    public string GetStringField(string name)
    {
        var value = GetField(name);
        return value.type == Type.STRING ? value.str : null;
    }

    public bool HasField(string name)
    {
        return type == Type.OBJECT && keys.Any(t => t == name);
    }

    public void Clear()
    {
        type = Type.NULL;
        list.Clear();
        keys.Clear();
        str = string.Empty;
        n = 0;
        b = false;
    }

    public JSONObject Copy()
    {
        return new JSONObject(Print().ToString());
    }

    public StringBuilder Print()
    {
        return Print(0);
    }

    public StringBuilder Print(int depth)
    {
        //Convert the JSON into a stiring
        if (depth++ > MaxDepth)
        {
            Debug.Log("reached max depth!");
            return new StringBuilder();
        }
        var stringBuilder = new StringBuilder();
        switch (type)
        {
            case Type.STRING:
                stringBuilder.AppendFormat("\"{0}\"", str);
                break;
            case Type.NUMBER:
                stringBuilder.Append(n);
                break;
            case Type.OBJECT:
                if (list.Count > 0)
                {
                    stringBuilder.AppendLine("{");

                    depth++;

                    for (var i = 0; i < list.Count; i++)
                    {
                        var jsonObject = list[i];
                        if (!jsonObject)
                        {
                            continue;
                        }
                        var key = keys[i];
                        for (var j = 0; j < depth; j++)
                            stringBuilder.Append('\t'); //for a bit more readability

                        stringBuilder.AppendFormat("\"{0}\":{1}", key, jsonObject.Print(depth));

                        //last element
                        if (i != list.Count - 1)
                        {
                            stringBuilder.AppendLine(",");
                        }
                        else
                        {
                            stringBuilder.AppendLine();
                        }
                    }
                    stringBuilder.Append('}');
                }
                else
                {
                    stringBuilder.Append("null");
                }
                break;
            case Type.ARRAY:
                if (list.Count > 0)
                {
                    stringBuilder.AppendLine("[");
                    depth++;

                    for (int i = 0; i < list.Count; i++)
                    {
                        JSONObject jsonObject = list[i];
                        if (!jsonObject)
                        {
                            continue;
                        }

                        for (int j = 0; j < depth; j++)
                        {
                            stringBuilder.Append("\t");
                        }

                        stringBuilder.Append(jsonObject.Print(depth));

                        //last element
                        if (i != list.Count - 1)
                        {
                            stringBuilder.AppendLine(",");
                        }
                        else
                        {
                            stringBuilder.AppendLine();
                        }
                    }
                    stringBuilder.Append(']');
                }
                break;
            case Type.BOOL:
                stringBuilder.Append(b.ToString());
                break;
            case Type.NULL:
                stringBuilder.Append("null");
                break;
        }
        return stringBuilder;
    }

    public JSONObject this[int index]
    {
        get { return list[index]; }
    }

    public JSONObject this[string index]
    {
        get { return GetField(index); }
    }

    public void PrintKeys()
    {
        foreach (var key in keys)
        {
            Debug.Log(key);
        }
    }

    public override string ToString()
    {
        return Print().ToString();
    }
}