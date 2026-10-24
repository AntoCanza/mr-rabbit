using System;
using UnityEngine;

namespace MrRabbit
{
    // A semantic word-to-picture mapping, independent of which vowel is missing.
    public sealed class SchoolWordPicture : MonoBehaviour
    {
        [Serializable] public sealed class Entry
        {
            public string word;
            public Texture2D atlas;
            public Rect uv;
        }
        [SerializeField] private UnityEngine.UI.RawImage picture;
        [SerializeField] private Entry[] entries=Array.Empty<Entry>();
        public System.Collections.Generic.IReadOnlyList<Entry> Entries=>entries;
        public string ShownWord {get;private set;}
        public UnityEngine.UI.RawImage Picture=>picture;
        public void Configure(UnityEngine.UI.RawImage image,Entry[] vocabulary)
        {picture=image;entries=vocabulary;picture.raycastTarget=false;}
        public bool Show(string word)
        {
            ShownWord=null;
            if(!string.IsNullOrEmpty(word))
            {
                foreach(var entry in entries)
                {
                    if(entry.word!=word||entry.atlas==null)continue;
                    picture.texture=entry.atlas;picture.uvRect=entry.uv;
                    var fit=picture.GetComponent<UnityEngine.UI.AspectRatioFitter>();
                    if(fit!=null)fit.aspectRatio=entry.uv.width*entry.atlas.width/(entry.uv.height*entry.atlas.height);
                    ShownWord=word;gameObject.SetActive(true);return true;
                }
            }
            picture.texture=null;gameObject.SetActive(false);return false;
        }
    }
}
