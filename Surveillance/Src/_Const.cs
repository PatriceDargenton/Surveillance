
namespace Surveillance
{
    public static class Const
    {

#if DEBUG
        public static bool bDebug = true;
        public static bool bRelease = false;
#else
        public static bool bDebug = false;
        public static bool bRelease = true;
#endif

        public const string appTitle = "Surveillance";
        public const string appVersion = "1.07";
        public const string appDate = "07/10/2023";
        public const string webSitesListFile = "Surveillance.ini";
        public const string report = "Report.txt";

        // Delimiters to display the report
        public const string sep = "\t";    // General separator
        public const string sep1 = "\t: "; // First separator
        public const string sep2 = "\t\t"; // Separator after connection result
        /*
        public const string sep = ", ";   // General separator
        public const string sep1 = " : "; // First separator
        public const string sep2 = "";    // No separator after connection result
        */

        public const int checkRetryWaitTimeMSec = 500;
        public const int sendKeysFirstWaitTimeMSec = 3000; // 3000 milliseconds: 3 seconds
        public const int sendKeysWaitTimeMSec = 2000;
        public const int browserFirstWebSiteWaitTimeMSec = 5000;
        public const int browserFirstWaitTimeMSec = 3000;
        public const int browserWaitTimeMSec = 500;
        public const int browserAfterConnectionWaitTimeMSec = 4000;
        public const int browserCheckTextWaitTimeMSec = 3000;
        public const int allSitesWaitTimeSec = 10;

        public static bool showTagNameInFullReport = false;

        // If false: another Chrome is open and an error is raised:
        // unknown error: Chrome failed to start: exited normally.
        //  (unknown error: DevToolsActivePort file doesn't exist)
        //  (The process started from chrome location 
        //   C:\Program Files\Google\Chrome\Application\chrome.exe 
        //   is no longer running, so ChromeDriver is assuming that Chrome has crashed.)
        public static bool browserMustBeClosedToStart = true;

        public static bool hideSeleniumDriverDebugScreen = false;

        public static bool encryptPassword = true;
        
        // No encryption at all: Do not use!
        public const string autoPWDoNotUse = "";

        // If you need to force a password reset for all users on the network, change this suffix
        public const string passwordFileSuffix = "";


        // You may set your own suffix for your documentation, if required
        public const string docMdSuffix = "";

        public static bool disableSitesWithoutCertif = false;

        // MS-Edge can be disabled
        public static bool MSEdgeEnabled = true;
        public static bool MSEdgeSeleniumManagerCheck = true;
        public const string MSEdgeVersion = "117.0.2045.36";

        public const string ChromeVersion = "117.0.5938.88";
        public const string FirefoxVersion = "0.33.0";

        // If there are enough rights on the workstation to automatically download the drivers,
        //  this boolean can be left true
        public static bool noInstallRequired = true;

        // If not, try to copy from Installation directory,
        //  otherwise, let the user copy the driver himself

        // If possible, try to copy Installation\.cache to C:\User\[User]\.cache
        public static bool autoInstall = true;

        public static bool FirefoxWithUserProfile = true;

        // You may set your own key
        public const string appSymmetricEncryptionKey = "***" + appTitle + "***";

        // Autofilling passwords:
        public const string autoPW = ""; // Default: leave blank

    }
}
