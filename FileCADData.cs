using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ApiCMS.FileCAD
{
    [Serializable]
    public class Datum
    {
        public int id;
        public string file_name;
        public string file;
        public string size;
        public string memo;
        public string reference_point;
        public string thumbnail;
        public string path;
        public int created_by;
        public object deleted_at;
        public string created_at;
        public string updated_at;
        public void GetThumnail(MonoBehaviour behaviour,string token ,UnityAction<Sprite> callback)
        {
            Thumbnail thumbnail = new Thumbnail(id, token);
            thumbnail.Get(behaviour, callback);
        }
        public void GetModel(MonoBehaviour behaviour, string token, UnityAction<UnityEngine.Object> callback)
        {
            Model model = new Model(id, token);
            model.Get(behaviour, callback);
        }
        public Datum() { }
    }
    [Serializable]
    public class Link
    {
        public string url;
        public string label;
        public bool active;
    }
    [Serializable]
    public class Result //ListFile
    {
        public int current_page;
        public List<Datum> data;
        public string first_page_url;
        public int from;
        public int last_page;
        public string last_page_url;
        public List<Link> links;
        public object next_page_url;
        public string path;
        public int per_page;
        public object prev_page_url;
        public int to;
        public int total;
    }

}