using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor.CategoryTool
{
    public static class OnlineSpellChecker
    {
        /*public static void CheckSpelling(string word, Action<string> onResult)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(SendRequest(word, onResult));
        }

        private static IEnumerator SendRequest(string word, Action<string> onResult)
        {
            WWWForm form = new WWWForm();
            form.AddField("text", word);
            form.AddField("language", "en-US");

            using (UnityWebRequest www = UnityWebRequest.Post("https://api.languagetool.org/v2/check", form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    var json = www.downloadHandler.text;
                    var data = JsonUtility.FromJson<LTResponseWrapper>(json);

                    if (data.matches.Length > 0 && data.matches[0].replacements.Length > 0)
                    {
                        string corrected = data.matches[0].replacements[0].value;
                        onResult?.Invoke(corrected);
                        Debug.Log($"✅ اصلاح تایپ: '{word}' → '{corrected}'");
                    }
                    else
                    {
                        onResult?.Invoke(word); // No correction
                    }
                }
                else
                {
                    Debug.LogError("❌ SpellCheck Failed: " + www.error);
                    onResult?.Invoke(word); // fallback
                }
            }
        }

        [Serializable]
        public class LTResponseWrapper
        {
            public Match[] matches;
        }

        [Serializable]
        public class Match
        {
            public Replacement[] replacements;
        }

        [Serializable]
        public class Replacement
        {
            public string value;
        }*/
    }
}