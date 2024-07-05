
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Surveillance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SurveillanceCSharp
{
    public enum ParameterType // Enumeration of the method used to find a Web Element
    {
        ById,
        ByName,
        ByClassName,
        ByCssSelector
    }

    public class WSAParameter // Name of a Web Element to search for, and how to find it: which method to use
    {
        public string Value { get; set; }
        public ParameterType Type { get; set; }
        
        public WSAParameter() { }

        public WSAParameter(string value, ParameterType type)
        {
            Value = value;
            Type = type;
        }
    }

    public class WSASurveillance // WebSite and App Surveillance (monitoring)
    {
        #region "Installation"

        private static IWebDriver m_driver = null;

        public static string DriversFolderPath()
        {
            string seleniumDriverDir = @"\.cache\selenium";
            return seleniumDriverDir;
        }

        public static string DriversFolderFullPath()
        {
            string up = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string dfp = DriversFolderPath(); // "\.cache\selenium"
            //string seleniumDriverDir = Path.Combine(up, dfp); // Does not work?
            string seleniumDriverDir = up + dfp;
            return seleniumDriverDir;
        }

        public static string chromeDriverExe = "chromedriver.exe";
        //public static string chromeDriverPath = @"chromedriver\win32\" + Const.ChromeVersion;
        public static string chromeDriverPath = @"chromedriver\win64\" + Const.ChromeVersion;
        public static string ChromeDriverFolderPath()
        {
            string chromeDriverDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cache", "selenium", chromeDriverPath);
            return chromeDriverDir;
        }
        
        public static string ChromeDriverPath()
        {
            string path = Path.Combine(ChromeDriverFolderPath(), chromeDriverExe);
            return path;
        }

        public static string firefoxDriverExe = "geckodriver.exe";
        public static string firefoxDriverPath = @"geckodriver\win64\"+ Const.FirefoxVersion;
        public static string FirefoxDriverFolderPath()
        {
            string firefoxDriverDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cache", "selenium", firefoxDriverPath);
            return firefoxDriverDir;
        }
        
        public static string FirefoxDriverPath()
        {
            string path = Path.Combine(FirefoxDriverFolderPath(), firefoxDriverExe);
            return path;
        }

        public static string edgeDriverExe = "msedgedriver.exe";
        // \.cache\selenium\msedgedriver\win64\116.0.1938.69\msedgedriver.exe
        public static string edgeDriverPath = @"msedgedriver\win64\" + Const.MSEdgeVersion;
        public static string edgeDriverManagerPath = @"\selenium-manager\windows\selenium-manager.exe";

        public static string EdgeDriverFolderPath()
        {
            string edgeDriverDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cache", "selenium", edgeDriverPath);
            return edgeDriverDir;
        }
        
        public static string EdgeDriverPath()
        {
            string path = Path.Combine(EdgeDriverFolderPath(), edgeDriverExe);
            return path;
        }

        public static bool CheckBrowserIsClosed(string nomProcessNavigateur)
        {
            Process[] processes = Process.GetProcessesByName(nomProcessNavigateur);
            if (processes.Length > 0) return false; else return true;
        }

        public static bool CheckChromeBrowserIsClosed()
        {
            return CheckBrowserIsClosed("chrome");
        }
        
        public static bool CheckFirefoxBrowserIsClosed()
        {
            return CheckBrowserIsClosed("firefox");
        }
        public static bool CheckEdgeBrowserIsClosed()
        {
            //return CheckBrowserIsClosed("msedge");
            return !IsEdgeBrowserOpen();
        }

        static bool IsEdgeBrowserOpen()
        {
            Process[] processes = Process.GetProcessesByName("msedge");

            foreach (Process process in processes)
            {
                // Ignore untitled processes, there are several for Edge
                if (string.IsNullOrEmpty(process.MainWindowTitle)) continue;
                return true;
            }
            return false;
        }

        //public static int NbOpenBrowsers(string browserProcessName)
        //{
        //    Process[] processes = Process.GetProcessesByName(browserProcessName);
        //    return processes.Length; 
        //}

        public static int NbEdgeBrowsersOpen()
        {
            //return NbOpenBrowsers("msedge");
            Process[] processes = Process.GetProcessesByName("msedge");
            int nbOpenBrowsers = 0;
            foreach (Process process in processes)
            {
                // Ignore untitled processes, there are several for Edge
                if (string.IsNullOrEmpty(process.MainWindowTitle)) continue;
                nbOpenBrowsers++;
            }
            return nbOpenBrowsers;
        }

        #endregion

        #region "Chrome"

        public static void NavigateUsingChrome(string url)
        {
            m_driver = new ChromeDriver();
            m_driver.Navigate().GoToUrl(url);
        }
        
        public static bool NavigateUsingChromeWithUserProfile(SitesConfig sitesConf, out string result)
        {
            // Selenium don't use Status Code (404, ...)
            // https://www.selenium.dev/documentation/test_practices/discouraged/http_response_codes
            if (Const.navigate) WSAPing.PingWebSites(sitesConf.Sites);

            string title = DateTime.Now.ToString() + "\n";
            var sb = new StringBuilder(title);
            var sbFull = new StringBuilder();
            bool success = false;

            if (Const.navigate && sitesConf.NbWebSites > 0)
            {
                string chromeDriverDir = ChromeDriverFolderPath();

                ChromeOptions options = new ChromeOptions();

                if (sitesConf.UseProfile)
                {
                    string userDataDir =
                        @"user-data-dir=C:\Users\" +
                        sitesConf.Profile +
                        @"\AppData\Local\Google\Chrome\User Data";
                    options.AddArguments(userDataDir);
                }

                // Shortcut mode: try using the currently open browser: failed!
                // Either it opens another browser, but it crashes right after
                //  either it finds the open browser, but it remains blocked without error on the line:
                //  m_driver = new ChromeDriver(chromeDriverDir, options);
                //  and without opening the requested new page
                if (!Const.browserMustBeClosedToStart) 
                {
                    //options.AddArguments("--remote-allow-origins=*");
                    options.DebuggerAddress = "localhost:9222"; // 127.0.0.1:9222

                    // Hide the monitoring mode, try to navigate as much as possible like a user,
                    //  to avoid robot blocking
                    // https://github.com/SeleniumHQ/selenium/issues/12071
                    options.AddArguments(new string[] {
                        "--no-sandbox"
                        , "--disable-infobars"
                        , "--disable-dev-shm-usage"
                        , "--disable-blink-features=AutomationControlled"
                        }); 
                    //options.AddExcludedArgument("enable-automation"); // Not compatible
                    options.AddAdditionalOption("useAutomationExtension", false);
                    //options.BinaryLocation = ""; // Path to Chrome.exe

                    // Not compatible, and doesn't work
                    // There is already an option for the detach capability. Please use the LeaveBrowserRunning property instead.
                    //options.AddAdditionalOption("detach", true);
                    //options.LeaveBrowserRunning = true;

                    /*
                    // Doesn't work either:
                    //https://www.appsloveworld.com/csharp/100/1585/unable-to-get-chrome-performance-logs-in-selenium-c
                        //Based on your need you can change the following options
                        //options.AddUserProfilePreference("intl.accept_languages", "en-US");
                        options.AddUserProfilePreference("intl.accept_languages", "fr-FR");
                        options.AddUserProfilePreference("disable-popup-blocking", "true");
                        options.AddArgument("test-type");
                        options.AddArgument("--disable-gpu");
                        options.AddArgument("no-sandbox");
                    options.AddArgument("start-maximized");
                    options.LeaveBrowserRunning = true;
                    */

                    // https://csharp.hotexamples.com/examples/OpenQA.Selenium.Chrome/ChromeOptions/AddArguments/php-chromeoptions-addarguments-method-examples.html
                    //options.AddArguments("--test-type", "--start-maximized");
                    //options.AddArguments("--test-type", "--ignore-Certificate-errors");

                    // Error examples in case of incompatible options:

                    //Chrome monitoring failed: unknown error: Chrome failed to start: exited normally.
                    //  (unknown error: DevToolsActivePort file doesn't exist)
                    //  (The process started from chrome location C:\Program Files\Google\Chrome\Application\chrome.exe is no longer running, so ChromeDriver is assuming that Chrome has crashed.)

                    //Chrome monitoring failed: invalid argument: entry 0 of 'firstMatch' is invalid
                    //from invalid argument: cannot parse capability: goog:chromeOptions
                    //from invalid argument: unrecognized chrome option: excludeSwitches
                }
                else
                {
                    // Hide the monitoring mode, try to navigate as much as possible like a user,
                    //  to avoid robot blocking
                    // https://github.com/SeleniumHQ/selenium/issues/12071
                    options.AddArguments(new string[] {
                        "--no-sandbox"
                        , "--disable-infobars"
                        , "--disable-dev-shm-usage"
                        , "--disable-blink-features=AutomationControlled"
                        });
                    options.AddExcludedArgument("enable-automation");
                    options.AddAdditionalOption("useAutomationExtension", false);
                }

                try
                {
                    if (Const.hideSeleniumDriverDebugScreen)
                    {
                        var chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverDir);
                        chromeDriverService.HideCommandPromptWindow = true;
                        m_driver = new ChromeDriver(chromeDriverService, options);
                    }
                    else
                        m_driver = new ChromeDriver(chromeDriverDir, options);

                    success = true;
                }
                catch (Exception ex)
                {
                    sb.AppendLine("Chrome monitoring failed: " + ex.Message);
                }

                // Idea:
                /*
                int timeout = 0;
                while (m_driver.FindElements(By.ClassName("login")).Count == 0 && timeout < 500)
                {
                    Wait(1);
                    timeout++;
                } */

            }
            else success = true;

            var sbfinal = new StringBuilder();
            var sbfinalFull = new StringBuilder();

            if (!success)
            {
                sbfinal.AppendLine();
                sbfinal.Append(sb);
                result = sbfinal.ToString();
                return false;
            }

            var i = 0;
            foreach (Site site in sitesConf.Sites)
            {
                if (site.Disabled) continue;
                
                string siteName = site.DisplaySiteName(sitesConf.LongestSiteName);

                if (Const.navigate && !site.Executable)
                {
                    i++;
                    
                    // ImplicitWait for the first web site
                    if (i == 1)
                        ImplicitWait(Const.browserFirstWebSiteWaitTimeMSec);
                    else
                    {
                        try
                        {
                            // Opens a new tab and switches to new tab
                            m_driver.SwitchTo().NewWindow(WindowType.Tab);
                        }
                        // Example of error:
                        //  This context is already opened by another user.
                        //  This context will be retrieved as is if you open it.
                        //  Do you really want to open it?
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }

                    try
                    {
                        m_driver.Navigate().GoToUrl(site.SiteUrl);
                    }
                    catch (Exception ex)
                    {
                        // Example: OpenQA.Selenium.WebDriverException, HResult=0x80131500,
                        //  Message=unknown error: net::ERR_CONNECTION_TIMED_OUT
                        if (ex.Message.Contains("ERR_CONNECTION_TIMED_OUT"))
                            sb.AppendLine(siteName + Const.sep1 + "Ping: Error: CONNECTION_TIMED_OUT");
                        else
                            sb.AppendLine(siteName + Const.sep1 + "Ping: Error!");
                        sbFull.AppendLine(siteName + Const.sep1 + "Ping: Error: " + ex.Message);
                        continue;
                    }

                    if (site.SimplePing && !site.JustCheckElement && !site.CheckElement)
                    {
                        sb.AppendLine(siteName + Const.sep1 + "Ping: " + site.StatusCodeResult);
                        sbFull.AppendLine(siteName + Const.sep1 + "Ping: " + site.StatusCodeResult);
                        continue;
                    }
                }

                NavigateSite(site, sitesConf.LongestSiteName, sb, sbFull); 
            }

            sbfinal.Append(sb);
            sbfinal.AppendLine();
            sbfinal.AppendLine("Full report:");
            sbfinalFull.Append(sbFull);
            sbfinal.Append(sbfinalFull);
            result = sbfinal.ToString();

            return true;
        }

        #endregion

        #region "Firefox"

        public static void NavigateUsingFirefox(string url)
        {
            m_driver = new FirefoxDriver();
            m_driver.Navigate().GoToUrl(url);
        }
        
        public static bool NavigateUsingFirefoxWithUserProfile(SitesConfig sitesConf, out string result)
        {
            if (Const.navigate) WSAPing.PingWebSites(sitesConf.Sites);

            string title = DateTime.Now.ToString() + "\n";
            var sb = new StringBuilder(title);
            var sbFull = new StringBuilder();
            bool success = false;

            if (Const.navigate && sitesConf.NbWebSites > 0) 
            {
                string folder = FirefoxDriverFolderPath();
                //string file = folder + @"\geckodriver.exe";

                FirefoxOptions options = new FirefoxOptions();

                if (sitesConf.UseProfile)
                {
                    string profile_path = ReadFirefoxProfile();
                    options.AddArguments("-profile");
                    options.AddArguments(profile_path);
                }

                // https://github.com/SeleniumHQ/selenium/issues/12071
                //options.AddArguments(new string[] {
                //    "--no-sandbox"
                //    , "--disable-infobars"
                //    , "--disable-dev-shm-usage"
                //    , "--disable-blink-features=AutomationControlled"
                //    });
                ////options.AddExcludedArgument("enable-automation");
                //options.AddArguments("--disable-automation");
                //options.AddAdditionalOption("useAutomationExtension", false);

                try
                {
                    if (Const.hideSeleniumDriverDebugScreen)
                    {
                        var driverService = FirefoxDriverService.CreateDefaultService();
                        driverService.HideCommandPromptWindow = true;
                        m_driver = new FirefoxDriver(driverService, options);
                    }
                    else if (sitesConf.UseProfile)
                        m_driver = new FirefoxDriver(folder, options);
                    else
                        m_driver = new FirefoxDriver(folder);
                    success = true;
                }
                catch (Exception ex)
                {
                    // Exception sample:
                    // Firefox monitoring failed: Expected browser binary location,
                    //  but unable to find binary in default location,
                    //  no 'moz:firefoxOptions.binary' capability provided,
                    //  and no binary flag set on the command line (SessionNotCreated)
                    // Firefox may not be installed!

                    sb.AppendLine("Firefox monitoring failed: " + ex.Message);
                    sb.AppendLine("(possible cause: Firefox is not installed, or is updating)");
                }
            }
            else success = true;

            var sbfinal = new StringBuilder();
            var sbfinalFull = new StringBuilder();

            if (!success)
            {
                sbfinal.AppendLine();
                sbfinal.Append(sb);
                result = sbfinal.ToString();
                return false;
            }
            
            var i = 0;
            foreach (Site site in sitesConf.Sites)
            {
                if (site.Disabled) continue;
                string siteName = site.DisplaySiteName(sitesConf.LongestSiteName);
                if (Const.navigate && !site.Executable)
                {
                    i++;

                    // ImplicitWait for the first web site
                    if (i == 1)
                        ImplicitWait(Const.browserFirstWebSiteWaitTimeMSec);
                    else
                    { 
                        try
                        {
                            // Opens a new tab and switches to new tab
                            m_driver.SwitchTo().NewWindow(WindowType.Tab);
                        }
                        // Example of error:
                        //  This context is already opened by another user.
                        //  This context will be retrieved as is if you open it.
                        //  Do you really want to open it?
                        catch (Exception ex) 
                        { 
                            Debug.WriteLine(ex.Message); 
                        }
                    }

                    try
                    {
                        m_driver.Navigate().GoToUrl(site.SiteUrl);
                    }
                    catch (Exception ex)
                    {
                        // Example: OpenQA.Selenium.WebDriverException, HResult=0x80131500,
                        //  Message=unknown error: net::ERR_CONNECTION_TIMED_OUT
                        if (ex.Message.Contains("ERR_CONNECTION_TIMED_OUT"))
                            sb.AppendLine(siteName + Const.sep1 + "Ping: Error: CONNECTION_TIMED_OUT");
                        else
                            sb.AppendLine(siteName + Const.sep1 + "Ping: Error!");
                        sbFull.AppendLine(siteName + Const.sep1 + "Ping: Error: " + ex.Message);
                        continue;
                    }

                    if (site.SimplePing && !site.JustCheckElement && !site.CheckElement)
                    {
                        sb.AppendLine(siteName + Const.sep1 + "Ping: " + site.StatusCodeResult);
                        sbFull.AppendLine(siteName + Const.sep1 + "Ping: " + site.StatusCodeResult);
                        continue; 
                    }
                }

                NavigateSite(site, sitesConf.LongestSiteName, sb, sbFull); 
            }

            sbfinal.Append(sb);
            sbfinal.AppendLine();
            sbfinal.AppendLine("Full report:");
            sbfinalFull.Append(sbFull);
            sbfinal.Append(sbfinalFull);
            result = sbfinal.ToString();

            return true;
        }

        public static string ReadFirefoxProfile()
        {
            string apppath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mozilla = Path.Combine(apppath, "Mozilla");
            bool exist = Directory.Exists(mozilla);
            if (!exist) return "";

            string firefox = Path.Combine(mozilla, "firefox");
            if (!Directory.Exists(firefox)) return "";

            string prof_file = Path.Combine(firefox, "profiles.ini");
            bool file_exist = File.Exists(prof_file);
            if (!file_exist) return "";

            using (StreamReader rdr = new StreamReader(prof_file)) 
            { 
                string resp = rdr.ReadToEnd();
                string[] lines = resp.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                string location = lines
                    .First(x => x.Contains("Default=Profiles/"))
                    .Split(new string[] { "=" }, StringSplitOptions.None)[1];
                location = location.Replace("/", "\\");
                string prof_dir = Path.Combine(firefox, location);
                return prof_dir;
            }
        }

    #endregion

        #region "Edge"

        public static void NavigateUsingEdge(string url)
        {
            m_driver = new EdgeDriver();
            m_driver.Navigate().GoToUrl(url);
        }
        
        public static bool NavigateUsingEdgeWithUserProfile(SitesConfig sitesConf, out string result)
        {
            if (Const.navigate) WSAPing.PingWebSites(sitesConf.Sites);

            string title = DateTime.Now.ToString() + "\n";
            var sb = new StringBuilder(title);
            var sbFull = new StringBuilder();
            bool success = false;

            if (Const.navigate && sitesConf.NbWebSites > 0)
            {
                EdgeOptions options = new EdgeOptions();

                if (sitesConf.UseProfile) // Doesn't work
                {
                    // Not possible to indicate which Profile to use while using Edge via WebDriver
                    // https://github.com/MicrosoftEdge/EdgeWebDriver/issues/58

                    // https://learn.microsoft.com/en-us/microsoft-edge/webdriver-chromium/capabilities-edge-options
                    //string profilePath =
                    //    @"--user-data-dir=C:\Users\" +
                    //    sitesConf.Profile +
                    //    @"\AppData\Local\Microsoft\Edge\User Data";
                }

                // https://github.com/SeleniumHQ/selenium/issues/12071
                options.AddArguments(new string[] {
                    "--no-sandbox"
                    , "--disable-infobars"
                    , "--disable-dev-shm-usage"
                    , "--disable-blink-features=AutomationControlled"
                    });
                options.AddExcludedArgument("enable-automation");
                options.AddAdditionalOption("useAutomationExtension", false);

                try
                {
                    if (Const.hideSeleniumDriverDebugScreen)
                    {
                        var driverService = EdgeDriverService.CreateDefaultService();
                        driverService.HideCommandPromptWindow = true;
                        m_driver = new EdgeDriver(driverService, options);
                    }
                    else
                        m_driver = new EdgeDriver(options);

                    success = true;
                }
                catch (Exception ex)
                {
                    sb.AppendLine("Edge monitoring failed: " + ex.Message);
                }
            }
            else success = true;

            var sbfinal = new StringBuilder();
            var sbfinalFull = new StringBuilder();

            if (!success)
            {
                sbfinal.AppendLine();
                sbfinal.Append(sb);
                result = sbfinal.ToString();
                return false;
            }

            var i = 0;
            foreach (Site site in sitesConf.Sites)
            {
                if (site.Disabled) continue;
                string siteName = site.DisplaySiteName(sitesConf.LongestSiteName);
                if (Const.navigate && !site.Executable)
                {
                    i++;

                    // ImplicitWait for the first web site
                    if (i == 1)
                        ImplicitWait(Const.browserFirstWebSiteWaitTimeMSec);
                    else
                        // Opens a new tab and switches to new tab
                        m_driver.SwitchTo().NewWindow(WindowType.Tab);

                    try
                    {
                        m_driver.Navigate().GoToUrl(site.SiteUrl);
                    }
                    catch (Exception ex)
                    {
                        // Example: OpenQA.Selenium.WebDriverException, HResult=0x80131500,
                        //  Message=unknown error: net::ERR_CONNECTION_TIMED_OUT
                        if (ex.Message.Contains("ERR_CONNECTION_TIMED_OUT") // Chrome
                            || ex.Message.Contains("netTimeout")) // Firefox
                            sb.AppendLine(siteName + Const.sep1 + "Ping: Error: Time out!");
                        else
                            sb.AppendLine(siteName + Const.sep1 + "Ping: Error!");
                        sbFull.AppendLine(siteName + Const.sep1 + "Ping: Error: " + ex.Message);
                        continue;
                    }

                    if (site.SimplePing && !site.JustCheckElement && !site.CheckElement)
                    {
                        sb.AppendLine(siteName + Const.sep1 + "Ping:" + site.StatusCodeResult);
                        sbFull.AppendLine(siteName + Const.sep1 + "Ping: " + site.StatusCodeResult);
                        continue;
                    }
                }

                NavigateSite(site, sitesConf.LongestSiteName, sb, sbFull); 
            }

            sbfinal.Append(sb);
            sbfinal.AppendLine();
            sbfinal.AppendLine("Full report:");
            sbfinalFull.Append(sbFull);
            sbfinal.Append(sbfinalFull);
            result = sbfinal.ToString();

            return true;
        }

        #endregion

        #region "Common"

        public static void NavigateSite(
            Site site, int longestSiteName, StringBuilder sb, StringBuilder sbFull)
        {

            if (site.Executable)
            {
                PilotExecutable(site, longestSiteName, sb, sbFull);
                return;
            }

            string errorMsg = "";
            bool continueCheck = false;
            bool connectionSuccess = ConnectIntoTheSite(site, ref errorMsg, ref continueCheck);

            if (site.CheckElement && site.StatusCodeOk && (site.ConnectionOk || continueCheck)) 
            { 
                bool textFound = FindTextInWebPage(site, longestSiteName, sb, sbFull, ref errorMsg);
                return;
            }

            string siteName = site.DisplaySiteName(longestSiteName);
            if (connectionSuccess)
            {
                sb.AppendLine(siteName + 
                    site.DisplayPingStatus() +
                    site.DisplayConnectionStatus() + 
                    site.DisplayCheckTextStatus());
                sbFull.AppendLine(siteName + 
                    site.DisplayPingStatus() +
                    site.DisplayFullConnectionStatus() + 
                    site.DisplayFullCheckTextStatus());
            }
            else 
            { 
                // If the Login element is not present, the user may be already connected,
                //  ignore the error in this case
                bool bAlert = errorMsg.Contains("unexpected alert open");
                string msg = siteName + Const.sep1 + "Ping Ok?";
                if (bAlert) msg = siteName + Const.sep1 + "Ping Ok? (unexpected alert open)";
                if (site.AutoconnectionByProfile || bAlert)
                {
                    // As a result, we cannot be sure that the page is displayed correctly
                }
                else if (!site.StatusCodeOk)
                    msg = siteName + Const.sep1 + "Ping: " + site.StatusCodeResult;
                else
                    msg = siteName + Const.sep1 + "Error: " + errorMsg;
                
                sb.AppendLine(msg);
                sbFull.AppendLine(msg);
            }
        }

        public static void PilotExecutable(
            Site site, int longestSiteName, StringBuilder sb, StringBuilder sbFull)
        {
            string siteName = site.DisplaySiteName(longestSiteName);

            if (!System.IO.File.Exists(site.SiteUrl))
                Debug.WriteLine("Existe pas");
            else
            {
                Util.StartProcess(site.SiteUrl);
                Wait(Const.sendKeysFirstWaitTimeMSec);
                foreach (string keys in site.KeysExe)
                {
                    try
                    {
                        if (keys.StartsWith("{WAIT"))
                        {
                            string wait1 = keys.Substring("{WAIT".Length);
                            wait1 = wait1.Replace("}", "");
                            int.TryParse(wait1, out int delaySec); // No error is generated here
                            Wait(delaySec * 1000);
                        }
                        else
                        {
                            // List of keyboard key codes for SendKeys
                            // https://learn.microsoft.com/dotnet/api/system.windows.forms.sendkeys.send?view=windowsdesktop-7.0
                            // https://learn.microsoft.com/dotnet/api/system.windows.forms.sendkeys.sendwait?view=windowsdesktop-7.0
                            SendKeys.SendWait(keys);
                            //SendKeys.Send(keys);
                            Wait(Const.sendKeysWaitTimeMSec);
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(siteName + Const.sep1 + "keys: [" + keys + "]: Error!");
                        sbFull.AppendLine(siteName + Const.sep1 + "keys: [" + keys + "]: Error: " + ex.Message);
                    }
                }
                sb.AppendLine(siteName + Const.sep1 + "Ok?");
                sbFull.AppendLine(siteName + Const.sep1 + "Ok?");
            }
        }

        private static IWebElement FindElement(WSAParameter prm)
        {
            IWebElement selectElement = null;
            if (Const.navigate && prm != null && !String.IsNullOrEmpty(prm.Value))
            {
                switch (prm.Type)
                {
                    case ParameterType.ById:
                        selectElement = m_driver.FindElement(By.Id(prm.Value));
                        break;
                    case ParameterType.ByName:
                        selectElement = m_driver.FindElement(By.Name(prm.Value));
                        break;
                    case ParameterType.ByClassName:
                        selectElement = m_driver.FindElement(By.ClassName(prm.Value));
                        break;
                    case ParameterType.ByCssSelector:
                        selectElement = m_driver.FindElement(By.CssSelector(prm.Value));
                        break;
                }
            }
            return selectElement;
        }

        public static bool ConnectIntoTheSite(Site site, ref string errorMsg, ref bool continueCheck)
        {
            continueCheck = false;
            bool success = false;
            string tag = "";
            errorMsg = "";
            if (Const.navigate) ImplicitWait(Const.browserFirstWaitTimeMSec);
            try
            {
                Debug.WriteLine(site.SiteName);
                IWebElement usernameInput = null, passwordInput = null, loginButton = null;

                // Possibility to switch to one or more iframes
                if (site.InlineFrames?.Count > 0)
                foreach(WSAParameter prm in site.InlineFrames)
                {
                    IWebElement iframe = FindElement(prm);
                    // Switch to the frame
                    if (iframe != null) m_driver.SwitchTo().Frame(iframe); 
                }

                // For example: authentification type
                if (site.SelectFindElement != null && 
                    !String.IsNullOrEmpty(site.SelectFindElement.Value))
                {
                    IWebElement selectElement = FindElement(site.SelectFindElement);
                    if (selectElement != null)
                    {
                        SelectElement select = new SelectElement(selectElement);
                        // Selection of the option by index (0 for the first option, 1 for the second, etc.)
                        select.SelectByIndex(site.SelectIndex);
                    }
                }
                
                tag = "tag Login :" + site.LoginInputFindElement?.Value;
                usernameInput = FindElement(site.LoginInputFindElement);

                tag = "tag Password :" + site.PWInputFindElement?.Value;
                passwordInput = FindElement(site.PWInputFindElement);

                tag = "tag Connection button :" + site.ConnectionBtnFindElement?.Value;
                loginButton = FindElement(site.ConnectionBtnFindElement);

                if (site.JustCheckElement || !site.StatusCodeOk) 
                { 
                    success = site.StatusCodeOk;
                    site.ConnectionResult = "OK";
                    site.ConnectionOk = true;
                }
                else
                {
                    if (site.ConnectionByProfile)
                    {
                        usernameInput.Clear(); // Clear first, if the textbox is yet filled
                        usernameInput.SendKeys(site.UserProfileLogin);
                        ImplicitWait(Const.browserWaitTimeMSec);
                        passwordInput.Clear();
                        passwordInput.SendKeys(site.UserProfilePW);
                        ImplicitWait(Const.browserWaitTimeMSec);
                    }
                    else
                    {
                        usernameInput.Clear();
                        usernameInput.SendKeys(site.SiteLogin);
                        ImplicitWait(Const.browserWaitTimeMSec);
                        passwordInput.Clear();
                        passwordInput.SendKeys(site.SitePassWord);
                        ImplicitWait(Const.browserWaitTimeMSec);
                    }

                    loginButton.Click();
                    ImplicitWait(Const.browserAfterConnectionWaitTimeMSec);

                    site.ConnectionResult = "OK";
                    site.ConnectionOk = true;
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                // If the Login element is not present, user is already connected, ignore the error in this case
                if (site.AutoconnectionByProfile) 
                {
                    // As a result, we cannot be sure that the page is displayed correctly
                    // But if we find the expected CheckText in the web page, this is OK
                    site.ConnectionResult = "OK?";
                    continueCheck = true;
                    return true;
                }
                else
                {
                    errorMsg = ex.Message;
                    success = false;
                    site.ConnectionResult = "Error: " + errorMsg + " (" + tag + ")";
                    //site.connectionTag = tag;
                }
            }
            return success;
        }
        
        public static void Wait(int waitTimeMSec)
        {
            System.Threading.Thread.Sleep(waitTimeMSec);
        }

        public static void ImplicitWait(int waitTimeMSec) 
        {
            m_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(waitTimeMSec);
        }

        public static bool FindTextInWebPage(Site site, int longestSiteName, 
            StringBuilder sb, StringBuilder sbFull, ref string errorMsg)
        {
            string siteName = site.DisplaySiteName(longestSiteName);

            try
            {
                ImplicitWait(Const.browserCheckTextWaitTimeMSec);
                
                //if (site.SiteName == "") Debug.WriteLine("!");

                IWebElement td = FindElement(site.CheckTextFindElement);
                bool bSucces1 = false;

                if (String.IsNullOrEmpty(site.CheckText) && td != null)
                    bSucces1 = true; // Only check html element
                else if (td != null)
                {
                    ImplicitWait(Const.browserWaitTimeMSec);

                    IList<IWebElement> tables = td.FindElements(By.XPath(".//table"));
                    foreach (var item in tables)
                    {
                        Debug.WriteLine(item.Text);
                        if (item.Text == site.CheckText)
                        {
                            bSucces1 = true;
                            break;
                        }
                    }

                    if (!bSucces1)
                    {
                        IList<IWebElement> items = td.FindElements(By.XPath(".//*"));
                        foreach (var item in items)
                        {
                            Debug.WriteLine("Text found : [" + item.Text.Trim() + "]");
                            Debug.WriteLine("Text checked : [" + site.CheckText + "]");
                            //bool equality = (item.Text.Trim() == site.CheckText);
                            bool equality = (item.Text.Trim().Contains(site.CheckText));
                            Debug.WriteLine("Equality : " + equality);
                            if (equality) { bSucces1 = true; break; }
                        }
                    }

                    if (!bSucces1)
                    {
                        Debug.WriteLine("Text found : [" + td.Text + "]");
                        //if (td.Text == site.CheckText) bSucces1 = true;
                        if (td.Text.StartsWith(site.CheckText)) bSucces1 = true;
                    }
                }

                site.TextFound = bSucces1;
                if (String.IsNullOrEmpty(site.CheckText))
                {
                    if (bSucces1)
                        site.TextFoundResult = "tag found";
                    else
                        site.TextFoundResult = "tag not found";
                }
                else
                {
                    if (bSucces1)
                        site.TextFoundResult = "Text '" + site.CheckText + "': found";
                    else
                        site.TextFoundResult = "Text '" + site.CheckText + "': not found";
                }

                // If the Connection was not certain, but the CheckText is found, then the Connection was OK
                if (bSucces1 && site.Connection && !site.ConnectionOk && site.AutoconnectionByProfile)
                { 
                    site.ConnectionOk = true;
                    site.ConnectionResult = "OK";
                }
                
                sb.AppendLine(siteName + 
                    site.DisplayPingStatus() +
                    site.DisplayConnectionStatus() + 
                    site.DisplayCheckTextStatus());
                sbFull.AppendLine(siteName + 
                    site.DisplayPingStatus() +
                    site.DisplayFullConnectionStatus() + 
                    site.DisplayFullCheckTextStatus());

                return bSucces1;

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                Debug.WriteLine(ex.Message);

                if (site.IgnoreContextAllreadyOpenedAlert && ex.HResult == -2146233088)
                {
                    // Firefox specific: Failed to convert data to an object
                    sb.AppendLine(
                        siteName + 
                        site.DisplayPingStatus() +
                        site.DisplayConnectionStatus() +
                        Const.sep + "Check text: Text found?");
                    sbFull.AppendLine(
                        siteName + 
                        site.DisplayPingStatus() +
                        site.DisplayFullConnectionStatus() +
                        site.DisplayCheckTextLabel() + ": Text found?");
                    return true;
                }
                else
                {
                    sb.AppendLine(
                        siteName + 
                        site.DisplayPingStatus() +
                        site.DisplayConnectionStatus() +
                        Const.sep + "Check text: Error!");
                    sbFull.AppendLine(
                        siteName + 
                        site.DisplayPingStatus() +
                        site.DisplayFullConnectionStatus() +
                        site.DisplayCheckTextLabel() + ": Error: " + ex.Message);
                    return false;
                }
            }
        }

        public static void Quit() 
        {
            WaitForBrowserToClose();

            try { m_driver?.Close(); } catch (Exception) { }

            m_driver?.Quit();
            m_driver?.Dispose();
        }

        static void WaitForBrowserToClose()
        {
            while (m_driver != null)
            {
                try
                {
                    // Checks if the browser is still open
                    m_driver?.Url?.ToString();
                }
                catch (WebDriverException)
                {
                    // If a WebDriverException is thrown, it means the browser has been closed
                    break;
                }

                //if (m_driver.WindowHandles.Count == 0) break; // Other solution:

                // Wait a short time before checking again
                Wait(Const.checkRetryWaitTimeMSec);
            }
        }

        public static List<Site> ReadConfigSites(
            string profile, string profilePW, out bool cancel)
        {
            cancel = false;
            var sites = new List<Site>();
            var nullSites = new List<Site>();

            string pathList = System.Windows.Forms.Application.StartupPath + @"\" + Const.webSitesListFile;
            string[] content = Util.ReadFile(pathList, out string errorMsg,
                defaultEncoding: false, encode: Encoding.UTF8);
            if (content == null)
            {
                MessageBox.Show(
                    "Error reading file:\n" +
                    pathList + "\n" + errorMsg,
                    Const.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return sites;
            }

            bool siteInProgress = false;
            string value = "";
            string siteName = "";
            string siteURL = "";
            string login = "";

            string checkTextName="";
            WSAParameter loginPrm = null, PWPrm = null, connectionPrm = null,
                selectPrm = null, checkTextPrm = null;

            string checkText = "";

            int selectIndex = 0;
            var iFrames = new List<WSAParameter>();

            bool autoConnection = false;
            bool connectionByProfile = false;
            bool certif = false;
            bool ignoreContextAllreadyOpenedAlert = false;
            var lstSendkeys = new List<string>();
            string yes = "Yes";
            string no = "No";
            bool surveillance = true;
            foreach (string ligne in content)
            {
                if (String.IsNullOrWhiteSpace(ligne))
                {
                    if (!siteInProgress) continue;

                    if (connectionByProfile && loginPrm != null)
                    {
                        Debug.WriteLine("byClassName and connection via profile: " + siteName);
                        var site1 = Site.CreateInstanceSiteWithConnection(
                            siteName, surveillance, certif, siteURL,
                            profile, profilePW,
                            login, "", loginPrm, PWPrm, connectionPrm, 
                            connectionByProfile: true, autoconnection: autoConnection,
                            ignoreContextAllreadyOpenedAlert: ignoreContextAllreadyOpenedAlert,
                            checkText: checkText, checkTextPrm: checkTextPrm,
                            inlineFrames: iFrames, selectPrm: selectPrm, selectIndex: selectIndex);
                        sites.Add(site1);
                        siteInProgress = false;
                        continue;
                    }

                    if (loginPrm != null && PWPrm != null)
                    {
                        Debug.WriteLine("byClassName: " + siteName);
                        var site1 = Site.CreateInstanceSiteWithConnection(
                            siteName, surveillance, certif, siteURL,
                            profile, profilePW,
                            login, "", loginPrm, PWPrm, connectionPrm,
                            ignoreContextAllreadyOpenedAlert: ignoreContextAllreadyOpenedAlert,
                            checkText: checkText, checkTextPrm: checkTextPrm,
                            inlineFrames: iFrames, selectPrm: selectPrm, selectIndex: selectIndex);
                        sites.Add(site1);
                        siteInProgress = false;
                        continue;
                    }

                    if (connectionPrm !=null && loginPrm != null && PWPrm != null)
                    {
                        Debug.WriteLine("byName + byIdConnection: " + siteName);
                        var site1 = Site.CreateInstanceSiteWithConnection(
                            siteName, surveillance, certif, siteURL,
                            profile, profilePW,
                            login, "", loginPrm, PWPrm, connectionPrm,
                            ignoreContextAllreadyOpenedAlert: ignoreContextAllreadyOpenedAlert,
                            checkText: checkText, checkTextPrm: checkTextPrm,
                            inlineFrames: iFrames, selectPrm: selectPrm, selectIndex: selectIndex);
                        sites.Add(site1);
                        siteInProgress = false;
                        continue;
                    }

                    if (connectionPrm != null)
                    {
                        Debug.WriteLine("byIdConnection: " + siteName);
                        var site1 = Site.CreateInstanceSiteWithConnection(
                            siteName, surveillance, certif, siteURL,
                            profile, profilePW,
                            login, "", loginPrm, PWPrm, connectionPrm,
                            ignoreContextAllreadyOpenedAlert: ignoreContextAllreadyOpenedAlert,
                            checkText: checkText, checkTextPrm: checkTextPrm,
                            inlineFrames: iFrames, selectPrm: selectPrm, selectIndex: selectIndex);
                        sites.Add(site1);
                        siteInProgress = false;
                        continue;
                    }

                    if (lstSendkeys.Count > 0)
                    {
                        Debug.WriteLine("Sendkeys: " + siteName);
                        var site1 = Site.CreateInstanceExecutable(
                            siteName, surveillance, siteURL, 
                            lstSendkeys, autoConnection);
                        sites.Add(site1);
                        siteInProgress = false;
                        continue;
                    }

                    // Simple ping
                    if (siteName.Length > 0 && siteURL.Length > 0)
                    {
                        Debug.WriteLine("Simple ping: " + siteName);
                        var site1 = Site.CreateInstanceSiteSimplePing(
                                siteName, surveillance, certif, siteURL,
                                profile, profilePW,
                                checkText: checkText,
                                checkTextPrm: checkTextPrm, checkTextName: checkTextName);
                        sites.Add(site1);
                        siteInProgress = false;
                        continue;
                    }
                }

                value = ReadValue(ligne, "- Site : ");
                if (value.Length > 0)
                {
                    siteName = value;
                    siteURL = "";
                    login = "";
                    autoConnection = false; connectionByProfile = false;
                    checkText = "";

                    loginPrm = null; PWPrm = null; connectionPrm = null;
                    selectPrm = null; checkTextPrm = null; checkTextName = null;

                    selectIndex = 0;
                    iFrames = new List<WSAParameter>();

                    surveillance = true;
                    certif = false;
                    ignoreContextAllreadyOpenedAlert = false;
                    lstSendkeys = new List<string>();
                    siteInProgress = true;
                    continue;
                }
                if (!siteInProgress) continue;

                value = ReadValue(ligne, ". URL : ");
                if (value.Length > 0) { siteURL = value; continue; }

                value = ReadValue(ligne, ". Login : ");
                if (value.Length > 0) { login = value; continue; }

                value = ReadValue(ligne, ". ByIdLogin : ");
                if (value.Length > 0) { 
                    loginPrm = new WSAParameter(value, ParameterType.ById);
                    continue; 
                }
                value = ReadValue(ligne, ". ByNameLogin : ");
                if (value.Length > 0) { 
                    loginPrm = new WSAParameter(value, ParameterType.ByName);
                    continue; 
                }
                value = ReadValue(ligne, ". ByClassNameLogin : ");
                if (value.Length > 0)
                {
                    loginPrm = new WSAParameter(value, ParameterType.ByClassName);
                    continue;
                }
                value = ReadValue(ligne, ". ByCssSelectorLogin : ");
                if (value.Length > 0)
                {
                    loginPrm = new WSAParameter(value, ParameterType.ByCssSelector);
                    continue;
                }

                value = ReadValue(ligne, ". ByIdPW : ");
                if (value.Length > 0)
                {
                    PWPrm = new WSAParameter(value, ParameterType.ById);
                    continue;
                }
                value = ReadValue(ligne, ". ByNamePW : ");
                if (value.Length > 0) { 
                    PWPrm = new WSAParameter(value, ParameterType.ByName);
                    continue; 
                }
                value = ReadValue(ligne, ". ByClassNamePW : ");
                if (value.Length > 0)
                {
                    PWPrm = new WSAParameter(value, ParameterType.ByClassName);
                    continue;
                }
                value = ReadValue(ligne, ". ByCssSelectorPW : ");
                if (value.Length > 0)
                {
                    PWPrm = new WSAParameter(value, ParameterType.ByCssSelector);
                    continue;
                }

                value = ReadValue(ligne, ". ByIdConnection : ");
                if (value.Length > 0) { 
                    connectionPrm = new WSAParameter(value, ParameterType.ById);
                    continue; 
                }
                value = ReadValue(ligne, ". ByNameConnection : ");
                if (value.Length > 0)
                {
                    connectionPrm = new WSAParameter(value, ParameterType.ByName);
                    continue;
                }
                value = ReadValue(ligne, ". ByClassNameConnection : ");
                if (value.Length > 0)
                {
                    connectionPrm = new WSAParameter(value, ParameterType.ByClassName);
                    continue;
                }
                value = ReadValue(ligne, ". ByCssSelectorConnection : ");
                if (value.Length > 0)
                {
                    connectionPrm = new WSAParameter(value, ParameterType.ByCssSelector);
                    continue;
                }

                value = ReadValue(ligne, ". ByIdSelect : ");
                if (value.Length > 0) { 
                    selectPrm = new WSAParameter(value, ParameterType.ById);
                    continue; 
                }
                value = ReadValue(ligne, ". ByNameSelect : ");
                if (value.Length > 0)
                {
                    selectPrm = new WSAParameter(value, ParameterType.ByName);
                    continue;
                }
                value = ReadValue(ligne, ". ByClassNameSelect : ");
                if (value.Length > 0)
                {
                    selectPrm = new WSAParameter(value, ParameterType.ByClassName);
                    continue;
                }
                value = ReadValue(ligne, ". ByCssSelectorSelect : ");
                if (value.Length > 0)
                {
                    selectPrm = new WSAParameter(value, ParameterType.ByCssSelector);
                    continue;
                }

                value = ReadValue(ligne, ". ByIdInlineFrame : ");
                if (value.Length > 0) {
                    iFrames.Add(new WSAParameter(value, ParameterType.ById));
                    continue; 
                }
                value = ReadValue(ligne, ". ByNameInlineFrame : ");
                if (value.Length > 0)
                {
                    iFrames.Add(new WSAParameter(value, ParameterType.ByName));
                    continue;
                }
                value = ReadValue(ligne, ". ByClassNameInlineFrame : ");
                if (value.Length > 0)
                {
                    iFrames.Add(new WSAParameter(value, ParameterType.ByClassName));
                    continue;
                }
                value = ReadValue(ligne, ". ByCssSelectorInlineFrame : ");
                if (value.Length > 0)
                {
                    iFrames.Add(new WSAParameter(value, ParameterType.ByCssSelector));
                    continue;
                }

                value = ReadValue(ligne, ". SelectIndex : ");
                if (value.Length > 0) 
                { 
                    int.TryParse(value, out selectIndex ); 
                    continue; 
                }

                value = ReadValue(ligne, ". ConnectionByProfile : ");
                if (value.Length > 0)
                { if (value == yes) connectionByProfile = true; continue; }

                value = ReadValue(ligne, ". Autoconnection : "); 
                if (value.Length > 0)
                { if (value == yes) autoConnection = true; continue; }

                value = ReadValue(ligne, ". Sendkeys : ");
                if (value.Length > 0) { lstSendkeys.Add(value); continue; }

                value = ReadValue(ligne, ". Activation : ");
                if (value.Length > 0)
                { if (value == no) surveillance = false; continue; }

                value = ReadValue(ligne, ". Certificate : ");
                if (value.Length > 0)
                { if (value == yes) certif = true; continue; }

                value = ReadValue(ligne, ". CheckText : ");
                if (value.Length > 0) { checkText = value; continue; }

                value = ReadValue(ligne, ". ByIdCheckText : ");
                if (value.Length > 0)
                {
                    checkTextPrm = new WSAParameter(value, ParameterType.ById);
                    continue;
                }
                value = ReadValue(ligne, ". ByNameCheckText : "); 
                if (value.Length > 0) { 
                    checkTextPrm = new WSAParameter(value, ParameterType.ByName);
                    continue; 
                }
                value = ReadValue(ligne, ". ByClassNameCheckText : ");
                if (value.Length > 0)
                {
                    checkTextPrm = new WSAParameter(value, ParameterType.ByClassName);
                    continue;
                }
                value = ReadValue(ligne, ". ByCssSelectorCheckText : ");
                if (value.Length > 0)
                {
                    checkTextPrm = new WSAParameter(value, ParameterType.ByCssSelector);
                    continue;
                }

                value = ReadValue(ligne, ". CheckTextName : ");
                if (value.Length > 0) { checkTextName = value; continue; }

                value = ReadValue(ligne, ". IgnoreContextAllreadyOpenedAlert : ");
                if (value.Length > 0)
                { if (value == yes) ignoreContextAllreadyOpenedAlert = true; continue; }

            }

            foreach (Site site in sites)
            {
                //Debug.WriteLine(site.SiteName);
                if (site.SimplePing) continue;
                if (site.JustCheckElement) continue;
                if (site.ConnectionByProfile) continue;
                if (site.AutoconnectionByProfile) continue;
                if (String.IsNullOrEmpty(site.SitePassWord))
                {
                    ReadPassword(site.SiteName, out string input);
                    if (input.Length > 0)
                        site.SitePassWord = input;
                    else
                    {
                        if (DialogResult.Cancel == Util.ShowInputDialog(
                            "Please enter the site password:\n" + site.SiteName,
                            ref input))
                        { cancel = true; return nullSites; }
                        site.SitePassWord = input;
                        SavePassword(site.SiteName, site.SitePassWord);
                    }
                }
            }

            //{Password}{Login}{Server}
            foreach (Site site in sites)
            {
                if (!site.Executable) continue;
                for (int i = 0; i < site.KeysExe.Count; i++)
                {
                    if ((site.KeysExe[i] == "{Password}")) site.KeysExe[i] = site.SitePassWord;
                    if ((site.KeysExe[i] == "{Login}")) site.KeysExe[i] = site.SiteLogin;
                    //if ((site.KeysExe[i] == "{Server}")) site.KeysExe[i] = site.siteServer;
                }
            }

            return sites;

        }
        
        private static string UserPasswordsFilePath()
        {
            string path = Const.appTitle + Const.passwordFileSuffix + ".txt";
            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string passwordFilePath = Path.Combine(userFolderPath, path);
            return passwordFilePath;
        }

        public static bool ReadPassword(string variable, out string password)
        {
            string passwordFilePath = UserPasswordsFilePath();
            password = "";
            if (!System.IO.File.Exists(passwordFilePath)) return false;

            string[] content = Util.ReadFile(passwordFilePath, out string errorMsg);
            if (content == null)
            {
                MessageBox.Show(
                    "Error reading file:\n" +
                    passwordFilePath + "\n" + errorMsg,
                    Const.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            foreach (string line in content)
            {
                // We can't encrypt also the variable name, otherwise we can't easily update the password
                // (because the encryption of the same string is not unique)
                if (!line.StartsWith(variable)) continue;
                int pos = line.IndexOf(":");
                if (pos < 0) continue;

                string value = line.Substring(pos + 1);
                password = value;
                if (Const.encryptPassword)
                {
                    password = SymmetricEncryptionHelper.Decrypt(value);
                    Debug.WriteLine("Decryption: " + variable + " -> " + password);
                } else 
                    Debug.WriteLine(variable + " -> " + password);
                return true;
            }
            return false;

        }

        public static void SavePassword(string variable, string password)
        {
            string passwordFilePath = UserPasswordsFilePath();
            string text = variable + ":" + password;
            if (Const.encryptPassword) text = variable + ":" + SymmetricEncryptionHelper.Encrypt(password);
            // The encryption of the same string is not unique!
            //Debug.WriteLine("Encryption: " + variable + " -> " + text);
            bool success = false;
            string errorMsg = "";
            if (System.IO.File.Exists(passwordFilePath))
            {
                string[] content = Util.ReadFile(passwordFilePath, out errorMsg);
                if (content == null)
                {
                    MessageBox.Show(
                        "Error reading file:\n" +
                        passwordFilePath + "\n" + "",
                        Const.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var sb = new StringBuilder();
                bool found = false;
                foreach (string line in content)
                {
                    string lineCheck = line;
                    if (String.IsNullOrEmpty(lineCheck)) continue;
                    if (String.IsNullOrWhiteSpace(lineCheck)) continue;
                    if (line.StartsWith(variable)) { lineCheck = text; found = true; }
                    sb.AppendLine(lineCheck);
                }
                if (!found) sb.AppendLine(text);
                string contentFinal = sb.ToString();
                success = Util.WriteFile(passwordFilePath, contentFinal, out errorMsg);
            }
            else
            {
                // Autofilling passwords
                string autoFilling = Const.autoPW;
                if (!Const.encryptPassword) autoFilling = Const.autoPWDoNotUse;
                string total = text;
                if (autoFilling.Length > 0) total = text + "\n" + autoFilling;
                success = Util.WriteFile(passwordFilePath, total, out errorMsg);
            }
            if (!success) MessageBox.Show(
                "Error writing file:\n" +
                passwordFilePath + "\n" + errorMsg,
                Const.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static string ReadValue(string ligne, string chaine)
        {
            int pos = ligne.IndexOf(chaine);
            if (pos < 0) return "";
            string valeur = ligne.Substring(pos + chaine.Length);
            int posCom = valeur.IndexOf("// "); // Comment
            if (posCom < 0) return valeur;
            string valeurSansCom = valeur.Substring(0, posCom - 1);
            return valeurSansCom;
        }

        #endregion
    }
}
