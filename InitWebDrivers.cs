using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Winium;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace InitWebDrivers
{
    /// <summary>
    /// A static factory object for creating WebDriver instances
    /// </summary>
    public class WebDriverFactory
    {
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static WiniumDriver driver;

        public void TestFixtureTearnDown()
        {
            driver.Quit();
        }

        /// <summary>
        /// Types of browser available for proxy examples.
        /// </summary>
        public enum BrowserType
        {
            IE,
            Chrome,
            Firefox
        }


        /// <summary>
        /// Creates Chrome Driver instance.
        /// </summary>
        /// <returns>A new instance of Chrome Driver</returns>
        public static WiniumDriver SetWiniumDriver(WiniumDriverService service, string path)
        {
            var options = new DesktopOptions();
            options.ApplicationPath = path;
            driver = new WiniumDriver(service, options);
            return driver;
        }

    }
}