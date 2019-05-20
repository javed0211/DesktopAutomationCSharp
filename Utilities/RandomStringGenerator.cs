using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Xml;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.IO;
using System.Text.RegularExpressions;

namespace SALTPages.Utility
{
    public class RandomStringGenerator
    {
        private static readonly Lazy<RandomStringGenerator> lazy = new Lazy<RandomStringGenerator>(() => new RandomStringGenerator());
        public static RandomStringGenerator Instance { get { return lazy.Value; } }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string RandomString(int size)
        {
            string input = "abcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            var chars = Enumerable.Range(0, size)
                                  .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string RandomPassword(int size)
        {
            string pwd = null;
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "0123456789";
            string specialChar = "@";

            Random random = new Random();
            var upperChars = Enumerable.Range(0, 1)
                                 .Select(x => upper[random.Next(0, upper.Length)]);
            pwd = new string(upperChars.ToArray());

            var lowerChars = Enumerable.Range(0, size - 3)
                                  .Select(x => lower[random.Next(0, lower.Length)]);
            pwd += new string(lowerChars.ToArray());

            var specialChars = Enumerable.Range(0, 1)
                                  .Select(x => specialChar[random.Next(0, specialChar.Length)]);
            pwd += new string(specialChar.ToArray());

            var numberChars = Enumerable.Range(0, 1)
                                  .Select(x => numbers[random.Next(0, numbers.Length)]);
            pwd += new string(numberChars.ToArray());

            return pwd;
            //Console.WriteLine(pwd);
        }

        /// <summary>
        /// gets the exact file path
        /// </summary>
        /// <param name="fileName"></param>
        public string GetFilePath(string fileName)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            baseDirectory = baseDirectory.Contains("SpecFlowProject") ? Regex.Split(baseDirectory, "SpecFlowProject")[0] : Regex.Split(baseDirectory, "TestResults")[0];
            DirectoryInfo BaseDirectoryPath = new DirectoryInfo(baseDirectory);
            FileInfo[] filesInDir = BaseDirectoryPath.GetFiles(fileName + ".*", SearchOption.AllDirectories);
            int FoundFilesCount = filesInDir.Count();
            if (FoundFilesCount.Equals(0))
            {
                log.Error("No Files Found with the name : " + fileName + " In directory :" + baseDirectory);
                return null;
            }
            return filesInDir[0].FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetEmail(int size = 8)
        {
            string input = "abcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            var chars = Enumerable.Range(0, size)
                                  .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray()) + "@email.com";
        }

        
        /// <summary>
        /// 
        /// </summary>
        //Function to get random number
        private static readonly Random getrandom = new Random();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int GetRandomNumber(int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(max);
            }
        }
    }
}
