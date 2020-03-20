using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BiztalkConfigLoader
{
    public static class Utilities // 
    {
        public static T DeserializeXMLFileToObject<T>(string XmlFilename)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(XmlFilename)) return default(T);

            try
            {
                StreamReader xmlStream = new StreamReader(XmlFilename);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                returnObject = (T)serializer.Deserialize(xmlStream);
            }
            catch (Exception exception)
            {
                throw new ApplicationException(string.Concat("Failed to Deserialize the file \"", XmlFilename,"\"."), exception);
            }
            return returnObject;
        }


        public static string GetUsername(string arg)
        {
            string username = null;
            if (arg.ToLower().Contains("username"))
            {
                username =  arg.Split(new char[] { ':' })[1];
            }
                     
            return username;
        }

        public static string GetPassword(string arg)
        {
            string userPassword = null;
            if (arg.ToLower().Contains("password"))
            {
                userPassword = arg.Split(new char[] { ':' })[1];
            }
            return userPassword;
        }
        public static string GetConfig(string arg)
        {
            string filepath = null;
            if (arg.ToLower().Contains("path"))
            {
                filepath = arg.Split(new char[] { ':' })[2];
            }
            return filepath;
        }
    }
}