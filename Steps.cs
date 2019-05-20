using OpenQA.Selenium.Winium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using InitWebDrivers;
using System.IO;
using System.Threading;

namespace HSF.Automation
{
    [Binding]
   public class Steps
    {
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static WiniumDriver driver;

        [Given(@"I want to save a note in noptepad")]
        public void GivenIWantToSaveANoteInNoptepad()
        {
            string notepadPath = @"C:\Windows\System32\notepad.exe";
            string winiumDriverPath = @"C:\Users\Javed\Downloads\Winium.Desktop.Driver";

            var service =  WiniumDriverService.CreateDesktopService(winiumDriverPath);

            WiniumDriver driver = WebDriverFactory.SetWiniumDriver(service,notepadPath);
            ScenarioContext.Current["driver"] = driver;
        }

        [When(@"I open a note pad")]
        public void WhenIOpenANotePad()
        {
            log.Info("NotePad Started");
        }

        [When(@"I create a new note")]
        public void WhenICreateANewNote()
        {
            driver = (WiniumDriver) ScenarioContext.Current["driver"];
            driver.FindElementByName("File").Click();
            Thread.Sleep(1000);
            driver.FindElementByName("Save").Click();
            Thread.Sleep(1000);
            driver.FindElementByName("File name:").SendKeys("NewFile");
            Thread.Sleep(1000);
            driver.FindElementByName("Save").Click();
            Thread.Sleep(1000);
        }

        [When(@"I save it on local path")]
        public void WhenISaveItOnLocalPath()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"I should see a note created")]
        public void ThenIShouldSeeANoteCreated()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I create a new note with '(.*)' and '(.*)'")]
        public void WhenICreateANewNoteWithAnd(string p0, string p1)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I save it on local '(.*)'")]
        public void WhenISaveItOnLocal(string p0)
        {
            ScenarioContext.Current.Pending();
        }

    }
}
