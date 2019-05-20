using CommonMethods;
using InitWebDrivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SpecFlow.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SpecFlow
{
    public class Utility
    {
        #region LazyLoading
        private static readonly Lazy<Utility> lazy = new Lazy<Utility>(() => new Utility());
        public static Utility Instance { get { return lazy.Value; } }
        #endregion

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IWebDriver driver;
        
        public Utility()
        {
            driver = (IWebDriver)ScenarioContext.Current["driver"];
        }

        public void WhenISwitchToWindow(string windowName)
        {
            try
            {
                int flag = 0;
                string currentWindow = driver.CurrentWindowHandle;
                ReadOnlyCollection<string> lstWindowIds = driver.WindowHandles;
                foreach (string defwindow in lstWindowIds)
                {
                    driver.SwitchTo().Window(defwindow);
                    if (driver.Title.Equals(windowName))
                    {
                        //driver.SwitchTo().Window(defwindow);
                        log.Info("Switched to window" + windowName);
                        flag = 1;
                        break;
                    }
                }
                if (flag != 1)
                {
                    log.Error("Unable to wsitch to window = " + windowName);
                }
            }
            catch (Exception e)
            {
                log.Error("Error while switching between windows " + e.Message);
            }
        }
        public bool WhenISwitchToWindowUsingId(string windowId)
        {
            bool result = false;
            bool winFound = false;
            try
            {
                ReadOnlyCollection<string> lstWindowIds = driver.WindowHandles;
                ///string currentWindow = driver.CurrentWindowHandle;
                foreach (string defwindow in lstWindowIds)
                {
                    if (defwindow.Equals(windowId))
                    {
                        driver.SwitchTo().Window(windowId);
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        log.Info("Switched to window : " + windowId);
                        winFound = true;
                        result = true;
                    }
                }
                if (!winFound)
                {
                    log.Error("Unable to switch to window = " + windowId);
                }
            }
            catch (Exception e)
            {
                log.Error("Error while switching between windows " + e.Message);
            }
            return result;
        }
        public bool ValidateDropDownOptions(IWebElement element, List<String> expectedValues)
        {
            bool result = false;
            if (element.xtIsElementPresent(driver))
            {
                log.Info("Validate options in DropDown List");
                Task.Delay(TimeSpan.FromSeconds(5));
                element.xtWaitUntilClickable(driver);
                SelectElement selectddElement = new SelectElement(element);
                IList<IWebElement> actualOptions = selectddElement.Options;
                List<String> actualOptionsText = new List<String>();
                expectedValues.Sort();
                foreach (var item in actualOptions)
                {
                    string data = item.Text;
                    actualOptionsText.Add(data);
                }
                actualOptionsText.Sort();
                if (expectedValues.Count.Equals(actualOptionsText.Count))
                {
                    result = expectedValues.SequenceEqual(actualOptionsText);
                    log.Info("DropDown List Validated successfully");
                }
            }
            if (result == false)
            {
                log.Error("Validation of DropDown List failed");
            }
            return result;
        }
        public void switchWindow()
        {
            string currentWindow = driver.CurrentWindowHandle;
            ReadOnlyCollection<string> lstWindowIds = driver.WindowHandles;
            foreach (string winId in lstWindowIds)
            {
                if (winId != currentWindow)
                {
                    driver.SwitchTo().Window(winId);
                }
            }
        }
        public void switchWindow(out string curWin, out string newWin)
        {
            curWin = driver.CurrentWindowHandle;
            ReadOnlyCollection<string> lstWindowIds = driver.WindowHandles;
            foreach (string winId in lstWindowIds)
            {
                if (winId != curWin)
                {
                    driver.SwitchTo().Window(winId);
                }
            }
            newWin = driver.CurrentWindowHandle;
        }
        public bool scrollToSection(IWebElement element)
        {
            bool result = false;
            try
            {
                Actions scroll = new Actions(driver);
                log.Info("Scrolling to section :" + element.Text);
                scroll.MoveToElement(element);
                scroll.Perform();
                Task.Delay(TimeSpan.FromSeconds(5));
                result = true;
            }
            catch (Exception e)
            {
                log.Error("Unable to scroll to element due to error:" + e.Message);
            }
            return result;
        }
        public bool isElementPresentForPath(string elementPath, bool value, int timeOutInSeconds = 5)
        {
            bool result = false;
            if (value)
            {
                try
                {
                    IWebElement element = driver.FindElement(By.XPath(elementPath));
                    log.Info("Element found correctly for path: " + element.Text);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeOutInSeconds);
                    result = element.Displayed;
                    result = true;
                }
                catch (Exception ex)
                {
                    if (ex is NoSuchElementException)
                        log.Error("Element not found correctly for path: " + elementPath);
                }
            }
            else
            {
                try
                {
                    IWebElement element = driver.FindElement(By.XPath(elementPath));
                    result = element.Displayed;
                    if (result)
                    {
                        log.Error("Element Should not be displayed for path: " + elementPath + " ,Please log a defect");
                    }
                    else
                    {
                        result = true;
                    }
                    
                }
                catch (Exception ex)
                {
                    if (ex is NoSuchElementException)
                        result = true;
                }
            }
            return result;
        }
        public bool verifyUrl(string condition, string data)
        {
            bool result = false;
            log.Info("Verify the Url of Page");
            string actualUrl = driver.Url;
            result = actualUrl.Contains(data);
            if (condition.ToLower().Equals("contains"))
            {
                if (result)
                {
                    log.Info("Url:" + actualUrl + " contains expeceted value:" + data);
                }
                else
                {
                    log.Error("Url:" + actualUrl + " doesnot contain expeceted value:" + data);
                    result = false;
                }
            }
            else
            {
                if (result)
                {
                    log.Error("Url:" + actualUrl + "shouldnot contain expeceted value:" + data);
                    result = false;
                }
                else
                {
                    log.Info("Url:" + actualUrl + "doesnot contain expeceted value:" + data);
                }
            }
            return result;
        }
        public bool switchWindowAndVerifyUrl(string condition, string data)
        {
            bool result = false;
            string oldWindow = null;
            //log.Info("Switching Window");
            //string curWin = null;
            //string newWin = null;
            //switchWindow(out curWin, out newWin);
            //switchWindow();

            ReadOnlyCollection<string> lstWindowIds = driver.WindowHandles;
            oldWindow = driver.CurrentWindowHandle;

            if (switchWindow(oldWindow, lstWindowIds))
            {
                log.Info("Verify Url");
                result = verifyUrl(condition, data);
            }
            else
            {
                log.Error("Windows not switched correctly");
            }

            //oldWindow = driver.CurrentWindowHandle;           
            //driver.Close();

            //switchWindow(oldWindow, lstWindowIds);



            return result;
        }
        public bool verifyElementValue(IWebElement element, string expectedText)
        {
            bool result = false;
            string actualText = null;
            log.Info("Verify Element Value");
            if (element.xtIsElementPresent(driver))
            {
                try
                {
                    log.Info("Get Element Value");
                    actualText = element.GetAttribute("value");
                }
                catch (Exception)
                {
                    log.Error("Unable to Get Value of Elemenet");
                }

            }

            if (String.IsNullOrEmpty(expectedText) || String.IsNullOrWhiteSpace(expectedText))
            {
                expectedText = "";
            }

            if (actualText.Equals(expectedText))
            {
                log.Info("Actual value is same as Expected value");
                result = true;
            }
            else if(actualText.Contains(expectedText))
            {
                log.Info("Actual value contains Expected value");
                result = true;
            }
            else
            {
                log.Error("Actual value does not match Expected value");
            }
            return result;
        }
        public bool verifyPageSourceContainsData(string expectedData)
        {
            bool result = false;
            try
            {
                log.Info("Get the Page Source of open Url :" + driver.Url);
                Task.Delay(TimeSpan.FromSeconds(10));
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
                string pageSourcedata = driver.PageSource;
                if (pageSourcedata.Contains(expectedData))
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                log.Error("Unable to get the Page Source of Url");
            }
            return result;
        }
        public bool closeWindowUsingWindowId(string windowId)
        {
            bool result = false;
            try
            {
                log.Info("Close Window Using Window Id");
                WhenISwitchToWindowUsingId(windowId);
                driver.Close();
                result = true;
            }
            catch (Exception)
            {
                log.Error("Unable to close window");
            }
            return result;
        }
        public bool closeOtherWindows()
        {
            bool result = false;
            try
            {
                log.Info("Close Windows");
                ReadOnlyCollection<string> lstWindowIds = driver.WindowHandles;
                string oldWindow = driver.CurrentWindowHandle;
                driver.Close();
                Thread.Sleep(TimeSpan.FromSeconds(10));
                switchWindow(oldWindow, lstWindowIds);
                result = true;
            }
            catch (Exception)
            {
                log.Error("Unable to close window");
            }
            return result;
        }
        public bool VerifyClassVisible(string tag, string className, bool value)
        {
            bool result = false;
            try
            {
                string pagesource = driver.PageSource;
                var pattern = @"<" + tag + " class=\"" + className + ".*>(.*)</" + tag + ">";
                MatchCollection match = Regex.Matches(pagesource, pattern);
                if (match.Count > 0)
                {
                    foreach (Match mt in match)
                    {
                        if (value)
                        {
                            if (mt.Success)
                            {
                                log.Info("h4 id tag is replaced by p class");
                                result = true;
                            }
                        }
                        else
                        if (!mt.Success)
                        {
                            log.Info("p class tag is not there");
                            result = true;
                        }
                    }
                }
                else
                {
                    if (!value)
                    {
                        log.Info("p class tag is not there");
                        result = true;
                    }
                }
            }
                
            catch (Exception)
            {
                log.Error("Unable to find p tag");
            }
            return result;
        }
        public bool switchWindow(string oldWindowId, ReadOnlyCollection<string> lstWindowIds)
        {
            bool result = false;
            try
            {
                foreach (string winId in lstWindowIds)
                {
                    if (winId != oldWindowId)
                    {
                        driver.SwitchTo().Window(winId);
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        log.Info("Window switched to  : " + winId);
                        result = true;
                        //((IJavaScriptExecutor)driver).ExecuteScript("window.resizeTo(1366, 768);");
                    }
                }
            }
            catch (Exception)
            {
                log.Error("Unable to switch window");
            }
            return result;
        }
        public bool fetchDataFromDataFolder(string TemplatesText)
        {
            bool result = false;
            try
            {
                log.Info("fetch the  data folder");
                var refPath = Directory.GetParent(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()))).FullName;
                string finalRef = Path.Combine(refPath, @"SpecFlow\Data", TemplatesText + ".txt");
                string content = File.ReadAllText(finalRef);
                result = true;
            }
            catch (Exception)
            {
                log.Error("Unable to fetch the  data");
            }
            return result;
        }

        public object ValidatePage(string page)
        {
            page = page.Replace(" ", string.Empty);
            Object obj = null;
            switch (page.ToLower())
            {
                case "healthinformationandourexpertise":
                case "healthcareprofessionals":
                    obj = new HealthcareProfessionalsPage(driver).Load();
                    break;
                case "billingandpayments":
                    obj = new BillingAndPaymentPage(driver).Load();
                    break;
                case "aboutus":
                    obj = new AboutUsPage(driver).Load();
                    break;
                case "business":
                    obj = new BusinessPage(driver).Load();
                    break;
                case "bupahome":
                    obj = new HomePage(driver).Load();
                    break;
                case "dentalclaims":
                    Thread.Sleep(5000);
                    obj = new DentalClaimsPage(driver).Load();
                    break;
                case "careservices":
                case "typeofcare":
                    Thread.Sleep(5000);
                    obj = new CareServicesPage(driver).Load();
                    break;
                case "cmshomepage":
                    obj = new CMSHomePage(driver).Load();
                    break;
                case "carehome":
                case "lifeinacarehome":
                    Thread.Sleep(5000);
                    obj = new CareHomePage(driver).Load();
                    break;
                case "healthinfoconjunctivitis":
                    obj = new HealthInfoConjunctivitisPage(driver).Load();
                    break;
                case "business_onlinequote":
                    Thread.Sleep(5000);
                    obj = new BusinessHealthInsuranceOnlineQuotePage(driver).Load();
                    break;
                case "individuals":
                    Thread.Sleep(5000);
                    obj = new IndividualPage(driver).Load();
                    break;
                case "careservices_requestaguide":
                    obj = new CareServicesRequestAGuidePage(driver).Load();
                    break;
                case "careservices_careers":
                    obj = new CareServicesCareersPage(driver).Load();
                    break;
                case "dental":
                    obj = new DentalPage(driver).Load();
                    break;
                case "dental_finddentalcentre":
                    obj = new DentalFindDentalCentrePage(driver).Load();
                    break;
                case "dental_londoncentraldentalcentre":
                    obj = new DentalLondonCentralDentalCentrePage(driver).Load();
                    break;
                case "healthassessment":
                    obj = new HealthAssessmentPage(driver).Load();
                    break;
                case "cbt":
                    obj = new CBTPage(driver).Load();
                    break;
                case "hcp":
                    Thread.Sleep(5000);
                    obj = new HCPPage(driver).Load();
                    break;
                case "hcp_aboutus":
                    obj = new HCPAboutUsPage(driver).Load();
                    break;
                case "informationforyourrole":
                case "hcp_informationforyourrole":
                    obj = new HCPInformationForYourRolePage(driver).Load();
                    break;
                case "hcp_dentists":
                    obj = new HCPDentistsPage(driver).Load();
                    break;
                case "hcp_finder":
                    obj = new HCPFinderPage(driver).Load();
                    break;
                case "health":
                    obj = new HealthPage(driver).Load();
                    break;
                case "travel":
                    obj = new TravelPage(driver).Load();
                    break;
                case "careers":
                    obj = new CareersPage(driver).Load();
                    break;
                case "healthinformation":
                    obj = new HealthInformationPage(driver).Load();
                    break;
                case "healthinformation_alcohol":
                    obj = new HealthInformationAlcoholPage(driver).Load();
                    break;
                case "healthblog":
                    obj = new HealthBlog(driver).Load();
                    break;
                case "healthblog_tipsmarathontraining":
                    obj = new HealthBlogTipsMarathonTrainingPage(driver).Load();
                    break;
                case "newsroom":
                    obj = new NewsroomPage(driver).Load();
                    break;
                case "health_dermatology":
                    obj = new HealthDermatologyPage(driver).Load();
                    break;
                case "health_ondemandkneereplacement":
                    obj = new HealthOnDemandKneeReplacementPage(driver).Load();
                    break;
                case "health_healthinsurance":
                    obj = new HealthInsurancePage(driver).Load();
                    break;
                case "health_quoteyourdetails":
                    obj = new HealthQuoteYourDetailsPage(driver).Load();
                    break;
                case "corporate":
                    obj = new CorporatePage(driver).Load();
                    break;
                case "intermediaries":
                    obj = new IntermediariesPage(driver).Load();
                    break;
                case "intermediaries_intbusproducts":
                    obj = new IntermediariesIntBusProductsPage(driver).Load();
                    break;
                case "b2b_gshealth":
                    obj = new B2BGSHealthPage(driver).Load();
                    break;
                case "b2b_portalbombardier":
                    obj = new B2BPortalBombardierPage(driver).Load();
                    break;
                case "ha_ourcentres":
                    obj = new HAOurCentersPage(driver).Load();
                    break;
                case "ha_aberdeen":
                case "ha_brighton":
                case "ha_brentwood":
                case "ha_eastmidlands":
                case "ha_southampton":
                    obj = new HAAberdeenPage(driver).Load();
                    break;
                case "intermediariesnewandinformation":
                    obj = new IntermediariesNewAndInformationPage(driver).Load();
                    break;
                case "affinity":
                    obj = new HomePage(driver).Load();
                    break;
                case "careservices_abbotsleighmewskent":
                case "careservices_arbrookhousesurrey":
                case "careservices_allingtoncourthertfordshire":
                case "careservices_ardenleacourtsolihull":
                case "careservices_amberleycourtbirmingham":
                case "careservices_alveston":
                case "careservices_oakcrofthousesurrey":
                case "careservices_suttonlodgesurrey":
                    obj = new CareServicesAbbotsleighMewsKentPage(driver).Load();
                    break;
                case "business_businesshealthinsurance":
                    obj = new BusinessHealthInsurancePage(driver).Load();
                    break;
                case "scl_tablecontainer":
                    obj = new TablecontainerPage(driver).Load();
                    break;
                case "healthinformation_adhesions":
                case "healthinformation_degeneration":
                case "healthinformation_angiogram":
                case "healthinformation_copd":
                    obj = new HealthInformationAtoZPage(driver).Load();
                    break;
                case "quote-your-details":
                    obj = new BBYHomePage(driver).Load();
                    break;
                case "providers_online":
                    obj = new POLPage(driver).Load();
                    break;
                case "mybupa":
                    obj = new MyBupaPage(driver).Load();
                    break;
                case "cashclaimhome":
                    obj = new CashClaimPage(driver).Load();
                    break;
                case "elderlycaresupportline":
                    Thread.Sleep(5000);
                    obj = new ElderlyCareSupportLinePage(driver).Load();
                    break;
                default:
                    break;
            }
            return obj;
        }

    }
}
