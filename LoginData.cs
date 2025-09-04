using System;

namespace ApiCMS.Login
{
    [System.Serializable]
    public class Data
    {
        public string token;
        public User user;
    }

    [System.Serializable]
    public class Result
    {
        public bool success;
        public Data data;
        public string message;
    }

    [System.Serializable]
    public class User
    {
        public int id;
        public string name;
        public string department;
        public string phone_number;
        public string email;
        public int permission;
        public object access_token;
        public object deleted_at;
        public string created_at;
        public string updated_at;
    }
}