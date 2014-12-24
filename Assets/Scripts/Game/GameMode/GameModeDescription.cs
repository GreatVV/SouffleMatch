using System;
using Game.Utility;
using UnityEngine;

namespace Game.GameMode
{
    [Serializable]
    public class GameModeDescription
    {
        public int Amount;
        public GameModes Mode;

        public int TargetScore;
        public int Turns;


        public static GameModeDescription CreateFromJson(JSONObject jsonObject)
        {
            if (jsonObject == null)
            {
                Debug.LogWarning("There is no gameMode");
                return new GameModeDescription
                       {
                           Mode = GameModes.TargetScore,
                           TargetScore = 3000,
                           Turns = 40
                       };
                /*return new GameModeDescription()
            {
                Mode = "TargetChuzzle",
                Turns = 30,
                Amount = 20
            };*/
                /*return new GameModeDescription()
            {
                Mode = "TargetPlace",
                Turns = 30
            };*/
            }


            var desc = new GameModeDescription
                       {
                           Mode = (GameModes) Enum.Parse(typeof (GameModes), jsonObject.GetField("Mode").str),
                           Turns = (int) jsonObject.GetField("Turns").n,
                           TargetScore = jsonObject.HasField("TargetScore") ? (int) jsonObject.GetField("TargetScore").n : 0,
                           Amount = jsonObject.HasField("Amount") ? (int) jsonObject.GetField("Amount").n : 0
                       };
            return desc;
        }

        public JSONObject Serialize()
        {
            var json = new JSONObject();
            json.AddField("Mode", Mode.ToString());
            json.AddField("Turns",Turns);
            json.AddField("TargetScore",TargetScore);
            json.AddField("Amount",Amount);
            return json;
        }
    }
}