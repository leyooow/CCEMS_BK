using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class Utilities
    {
        
        public static string GetQueryString(string queryName)
        {
            string queryValue = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(string.Format(@"{0}\Queries\{1}", Infrastructure.Helpers.Settings.Default.BASEDIR, queryName)))
                {
                    queryValue = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return queryValue;
        }

        public static void WriteLog(string message)
        {
            //string logName = string.Format(@"BIRLogs_{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
            //string logPath = string.Format(@"{0}\Logs\{1}", Default.BASEDIR, logName);

            //if (!Directory.Exists(string.Format(@"{0}\Logs", Default.BASEDIR)))
            //{
            //    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            //    File.WriteAllText(logPath, "");
            //}

            //File.AppendAllText(logPath, string.Format(@"{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //    message.TrimStart() + Environment.NewLine));
        }

        public string PaddingFieldValue(string value, bool isLeft, int size, string pad)
        {
            if (value.Length > size)
            {
                if (isLeft)
                    value = value.Substring(value.Length - size, size);
                else
                    value = value.Substring(0, size);
            }
            else
            {
                string addedValue = string.Empty;
                for (int i = value.Length; i < size; i++)
                {
                    addedValue += pad;
                }

                if (isLeft)
                    value = value + addedValue;
                else
                    value = addedValue + value;
            }
            return value;
        }
    }
}
