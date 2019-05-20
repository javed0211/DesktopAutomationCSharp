using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.IO;
//using SeleniumExtras.WaitHelpers;
//using SeleniumExtras.PageObjects;

namespace CommonMethods
{
    public static class ExtensionMethod
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool xtClick(this IWebElement element, bool value, IWebDriver driver = null)
        {
            bool result = false;
            if (value)
            {
                try
                {
                    element.Click();
                    log.Info("Element is clicked correctly");
                    result = true;
                }
                catch (Exception ex)
                {
                    if (ex is NoSuchElementException)
                        log.Error("Element is not clicked");
                }
            }
            else
            {
                try
                {
                    element.Click();
                    log.Error("Element" + element.Text + "Should not be displayed. Please log a defect");
                }
                catch (Exception ex)
                {
                    if (ex is NoSuchElementException)
                        result = true;
                }
            }
            return result;
        }

        public static IWebElement xtGetWebElement(this By by, IWebDriver driver, int timeoutInSeconds = 30)
        {
            by.xtWaitUntilVisible(driver, timeoutInSeconds);
            return driver.FindElement(by);
        }

        public static bool xtIsElementPresent(this IWebElement element, IWebDriver driver, int timeOutInSeconds = 5)
        {
            bool results = false;
            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeOutInSeconds);
                //results = element.Displayed;
                results = element.Displayed || element.Enabled;
                return results;
            }
            catch (NoSuchElementException ex)
            {
                return false;
            }
            finally
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            }
        }

        public static SelectElement xtGetSelectElement(this By by, IWebDriver driver)
        {
            return new SelectElement(by.xtGetWebElement(driver));
        }

        public static bool xtWaitUntilExists(this By by, IWebDriver driver, int timeoutInSeconds = 30)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                // wait.Until(ExpectedConditions.ElementExists(by));
                wait.Until(ExpectedConditions.ElementExists(by));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static void xtWaitForTextToBePresentInElementValue(this By by, IWebDriver driver, string text, int timeoutInSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(ExpectedConditions.TextToBePresentInElementValue(by, text));
        }

        public static void xtWaitUntilClickable(this IWebElement element, IWebDriver driver, int timeoutInSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(ExpectedConditions.ElementToBeClickable(element));
        }

        public static void xtWaitUntilVisible(this By by, IWebDriver driver, int timeoutInSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(ExpectedConditions.ElementIsVisible(by));
        }

        public static void xtWaitForSelectionStateToBe(this By by, IWebDriver driver, bool selected, int timeoutInSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(ExpectedConditions.ElementSelectionStateToBe(by, selected));
        }

        public static void xtWaitUntilInvisible(this By by, IWebDriver driver, int timeoutInSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(by));
        }

        public static void xtWaitForText(this By by, IWebDriver driver, string text, int timeoutInSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(ExpectedConditions.TextToBePresentInElementLocated(by, text));
        }

        public static void xtBringElementInView(this IWebElement element, IWebDriver driver = null)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        public static bool xtSelectByText(this IWebElement element, string value, IWebDriver driver)
        {
            bool result = false;
            try
            {
                if (element.xtIsElementPresent(driver))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(3000));
                    SelectElement selectddElement = new SelectElement(element);
                    selectddElement.SelectByText(value);
                    result = true;
                    log.Info("Drop Down value : " + value + " is selected correctly");
                }

            }
            catch (Exception ex)
            {
                return false;
                log.Error("Drop Down value " + element.Text + "  is not selected correctly");
            }
            return result;

        }

        public static bool xtSendKeys(this IWebElement element, string value, IWebDriver driver)
        {
            bool result = false;
            try
            {
                if (element.xtIsElementPresent(driver))
                {
                    element.Clear();
                    element.SendKeys(value);
                    result = true;
                    log.Info("Value is Entered in TextBox correctly as " + element.Text);
                }
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("Value is not Entered in TextBox correctly,due to error: " + ex.Message);
            }

            return result;
        }

        public static void MouseHover(IWebElement element, IWebDriver driver = null)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Actions action = new Actions(driver);
            action.MoveToElement(element).Perform();
        }
        public static bool xtSelected(this IWebElement element, bool value, IWebDriver driver = null)
        {
            bool result = false;
            if (value)
            {
                try
                {
                    if(element.Selected)
                    {
                        log.Info("Element is already selected");
                        
                    }
                    else
                    {
                        element.Click();
                        log.Info("Element selected");
                    }
                    result = true;

                }
                catch (Exception ex)
                {
                    if (ex is NoSuchElementException)
                        log.Error("Element is not clicked");
                }
            }
            else
            {
                try
                {
                    if (element.Selected)
                    {
                        element.Click();
                        log.Info("Element deselected");
                        result = true;
                    }
                    else
                    {
                        log.Info("Element is selected");
                    }
                }
                catch (Exception ex)
                {

                    result = false;
                    log.Error("Element is selected");
                }
            }
            return result;
        }

        public static bool FileExists(this string path)
        {
            if (File.Exists(path))
                return true;
            else
                return false;
        }

        public static string AppendTimeStamp(this string fileName)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(fileName),
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName)
                );
        }
    }
}
