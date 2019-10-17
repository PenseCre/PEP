using System.IO;
using System.Collections;
using UnityEngine;
using System;

namespace PenseCre
{
    public class FileUtils
    {
        public static string UniqueNameFullPath(string path, string name, string extension)
        {
            string proposedFullName = path + name + extension;
            int i = 0;
            while (File.Exists(proposedFullName))
            {
                if(name.Contains("(") && name.Contains(")") && name.IndexOf(")") == name.Length - 1)
                {
                    int indexLastOpenParenthesis = name.LastIndexOf("(")+1;
                    string contentBetweenParenthesis = name.Substring(indexLastOpenParenthesis, name.Length - 1 - indexLastOpenParenthesis);
                    int existingNumber = -1;
                    bool isContentANumber = int.TryParse(contentBetweenParenthesis, out existingNumber);
                    if (isContentANumber)
                    {
                        name = name.Substring(0, indexLastOpenParenthesis) + (existingNumber + 1).ToString() + ")";
                        proposedFullName = path + name + extension;
                        continue;
                    }
                }
                name += " (" + i.ToString() + ")";
                proposedFullName = path + name + extension;
                i++;
            }
            return proposedFullName;
        }

        public static string GetFormattedDate()
        {
            return GetFormattedDate(DateTime.Now);
        }

        public static string GetFormattedDate(DateTime dateTime)
        {
            string ret = dateTime.Year.ToString();
            ret += dateTime.Month.ToString();
            ret += dateTime.Day.ToString();
            ret += "_";
            ret += dateTime.Hour.ToString();
            ret += dateTime.Minute.ToString();
            ret += "-";
            ret += dateTime.Second.ToString();
            ret += dateTime.Millisecond.ToString();
            return ret;
        }

    }
}