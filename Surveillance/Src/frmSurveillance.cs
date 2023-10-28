
using Surveillance;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Windows.Forms;
//using static SurveillanceCSharp.WSASurveillance;

namespace SurveillanceCSharp
{
    public partial class FrmSurveillance : Form
    {

        #region "Installation"

        public string[] Args { get; set; }

        readonly string appTitle = Const.appTitle;
        readonly string profilePWName = "ProfilePW";

        public FrmSurveillance()
        {
            InitializeComponent();
        }
        
        private void FrmSurveillance_Load(object sender, EventArgs e)
        {
            if (WSASurveillance.ReadPassword(profilePWName, out string profilePW)) tbPW.Text = profilePW;
            tbPW.PasswordChar = '*';
            tbLogin.Text = Environment.UserName;
            this.Text = Const.appTitle + " " + Const.appVersion + " " + Const.appDate;
            if (Const.bDebug) this.Text += " (Debug)";

            rbEdge.Enabled = Const.MSEdgeEnabled;
            //rbEdge.Checked = true; // Debug
            if (Const.MSEdgeEnabled && rbEdge.Checked) chkUserProfile.Checked = false;
            //rbFirefox.Checked = true; // Debug
            //chkUserProfile.Checked = true; // Debug

            CheckInstall();
        }

        private void FrmSurveillance_Shown(object sender, EventArgs e)
        {
            if (Args?.Length > 0) ShortcutMode();
        }

        private void ShortcutMode()
        {
            string profilePW = tbPW.Text;
            string profile = tbLogin.Text;

            var sitesAuto = WSASurveillance.ReadConfigSites(profile, profilePW, out bool cancel);
            if (cancel) return;
            if (sitesAuto.Count == 0)
            {
                MessageBox.Show(
                    "No site is defined in the configuration file!",
                    appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (Site site in sitesAuto) site.Disabled = true;

            string siteNames = "";
            string siteToRun = "";
            foreach (Site site in sitesAuto)
                foreach (string arg in Args)
                {
                    siteToRun = arg;
                    siteNames += site.SiteName + "\n";
                    if (arg != site.SiteName) continue;
                    site.Disabled = false;
                }

            int nbSites = sitesAuto.Where(s => s.Disabled == false).ToList().Count;
            if (nbSites == 0)
            {
                MessageBox.Show(
                    "Site " + siteToRun + " was not found!\n" +
                    "Registered sites:\n" + siteNames,
                    appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ShortcutMode(sitesAuto, profilePW);
            Close();
        }

        private void TbLogin_TextChanged(object sender, EventArgs e)
        {
            CheckInstall();
        }

        private void TbPW_TextChanged(object sender, EventArgs e)
        {
            cmdStart.Enabled = true;
            CheckStartButton();
            if (tbPW.Text.Length > 0) WSASurveillance.SavePassword(profilePWName, tbPW.Text);
        }

        private void CheckStartButton()
        {
            string pathList = Application.StartupPath + @"\" + Const.webSitesListFile;
            if (tbPW.Text.Length == 0 && File.Exists(pathList)) cmdStart.Enabled = false;
        }

        private void CmdDoc_Click(object sender, EventArgs e)
        {
            string readMePath = Path.Combine(Application.StartupPath,
                "ReadMe" + Const.docMdSuffix + ".md");
            Util.StartProcess(readMePath);
        }

        private void CmdChange_Click(object sender, EventArgs e)
        {
            string changelogPath = Path.Combine(Application.StartupPath, 
                "Changelog" + Const.docMdSuffix + ".md");
            Util.StartProcess(changelogPath);
        }

        private void CmdConfig_Click(object sender, EventArgs e)
        {
            string configPath = Path.Combine(Application.StartupPath, Const.webSitesListFile);
            Util.StartProcess(configPath);
        }
        
        private void CmdReport_Click(object sender, EventArgs e)
        {
            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string reportPath = Path.Combine(userFolderPath, Const.report);
            Util.StartProcess(reportPath);
        }

        private void CheckInstall()
        {
            cmdStart.Enabled = false;
            cmdInstall.Enabled = false;
            if (Const.noInstallRequired)
                cmdStart.Enabled = true;
            else 
                if (!CheckInstallation()) cmdInstall.Enabled = true; else cmdStart.Enabled = true;
            //cmdInstall.Enabled = true; // Debug CheckInstallation
        }

        private void CmdInstall_Click(object sender, EventArgs e)
        {
            string startup = Application.StartupPath;
            string inst = "Installation";
            if (Const.autoInstall) 
            {
                string cache = ".cache";
                string selenium = "selenium";
                string instSrcDirChrome = Path.Combine(startup, inst, cache, selenium, WSASurveillance.chromeDriverPath);
                string instDestDirChrome = WSASurveillance.ChromeDriverFolderPath();
                string srcDriverChrome = Path.Combine(instSrcDirChrome, WSASurveillance.chromeDriverExe);
                string destDriverChrome = Path.Combine(instDestDirChrome, WSASurveillance.chromeDriverExe);
                string parentDirChrome = Path.GetDirectoryName(instDestDirChrome); // 114.0.5735.90 -> win32
                string parent2DirChrome = Path.GetDirectoryName(parentDirChrome); // win32 -> chromedriver
                string parent3DirChrome = Path.GetDirectoryName(parent2DirChrome); // chromedriver -> selenium
                string parent4Dir = Path.GetDirectoryName(parent3DirChrome); // selenium -> .cache

                string instSrcDirFirefox = Path.Combine(startup, inst, cache, selenium, WSASurveillance.firefoxDriverPath);
                string instDestDirFirefox = WSASurveillance.FirefoxDriverFolderPath();
                string srcDriverFirefox = Path.Combine(instSrcDirFirefox, WSASurveillance.firefoxDriverExe);
                string destDriverFirefox = Path.Combine(instDestDirFirefox, WSASurveillance.firefoxDriverExe);
                string parentDirFirefox = Path.GetDirectoryName(instDestDirFirefox); // 0.33.0 -> win64
                string parent2DirFirefox = Path.GetDirectoryName(parentDirFirefox); // win64 -> geckodriver
                string parent3DirFirefox = Path.GetDirectoryName(parent2DirFirefox); // geckodriver -> selenium

                string instSrcDirEdge = Path.Combine(startup, inst, cache, selenium, WSASurveillance.edgeDriverPath);
                string instDestDirEdge = WSASurveillance.EdgeDriverFolderPath();
                string srcDriverEdge = Path.Combine(instSrcDirEdge, WSASurveillance.edgeDriverExe);
                string destDriverEdge = Path.Combine(instDestDirEdge, WSASurveillance.edgeDriverExe);
                string parentDirEdge = Path.GetDirectoryName(instDestDirEdge); // 116.0.1938.69 -> win64
                string parent2DirEdge = Path.GetDirectoryName(parentDirEdge); // win64 -> msedgedriver
                string parent3DirEdge = Path.GetDirectoryName(parent2DirEdge); // msedgedriver -> selenium
                string srcDriver="", destDriver="";

                try
                {
                    if (!Directory.Exists(parent4Dir)) Directory.CreateDirectory(parent4Dir); // .cache

                    if (!Directory.Exists(parent3DirChrome)) Directory.CreateDirectory(parent3DirChrome);
                    if (!Directory.Exists(parent2DirChrome)) Directory.CreateDirectory(parent2DirChrome);
                    if (!Directory.Exists(parentDirChrome)) Directory.CreateDirectory(parentDirChrome);
                    if (!Directory.Exists(instDestDirChrome)) Directory.CreateDirectory(instDestDirChrome);
                    srcDriver = srcDriverChrome; destDriver = destDriverChrome;
                    if (!File.Exists(srcDriver))
                    {
                        string errorMsg = "Can't find file:\n" +
                        srcDriver + "\n" + 
                        "\n(message copied in the clipboard)";
                        Util.CopyClipBoard(errorMsg);
                        MessageBox.Show(errorMsg, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    if (!File.Exists(destDriverChrome))
                        File.Copy(srcDriverChrome, destDriverChrome);
                    
                    if (!Directory.Exists(parent3DirFirefox)) Directory.CreateDirectory(parent3DirFirefox);
                    if (!Directory.Exists(parent2DirFirefox)) Directory.CreateDirectory(parent2DirFirefox);
                    if (!Directory.Exists(parentDirFirefox)) Directory.CreateDirectory(parentDirFirefox);
                    if (!Directory.Exists(instDestDirFirefox)) Directory.CreateDirectory(instDestDirFirefox);
                    srcDriver = srcDriverFirefox; destDriver = destDriverFirefox;
                    if (!File.Exists(srcDriver))
                    {
                        string errorMsg = "Can't find file:\n" +
                        srcDriver + "\n" +
                        "\n(message copied in the clipboard)";
                        Util.CopyClipBoard(errorMsg);
                        MessageBox.Show(errorMsg, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    if (!File.Exists(destDriverFirefox))
                        File.Copy(srcDriverFirefox, destDriverFirefox);

                    if (Const.MSEdgeEnabled) {
                        if (!Directory.Exists(parent3DirEdge)) Directory.CreateDirectory(parent3DirEdge);
                        if (!Directory.Exists(parent2DirEdge)) Directory.CreateDirectory(parent2DirEdge);
                        if (!Directory.Exists(parentDirEdge)) Directory.CreateDirectory(parentDirEdge);
                        if (!Directory.Exists(instDestDirEdge)) Directory.CreateDirectory(instDestDirEdge);
                        srcDriver = srcDriverEdge; destDriver = destDriverEdge;
                        if (!File.Exists(srcDriver))
                        {
                            string errorMsg = "Can't find file:\n" +
                            srcDriver + "\n" +
                            "\n(message copied in the clipboard)";
                            Util.CopyClipBoard(errorMsg);
                            MessageBox.Show(errorMsg, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        if (!File.Exists(destDriverEdge))
                        File.Copy(srcDriverEdge, destDriverEdge);
                    }

                    cmdInstall.Enabled = false;
                    if (!CheckInstallation()) cmdInstall.Enabled = true;
                    return;
                }
                catch (Exception ex)
                {
                    string errorMsg = "Can't copy file:\n" +
                        srcDriver + "\nto\n" + destDriver + "\n" + ex.Message +
                        "\n(message copied in the clipboard)";
                    Util.CopyClipBoard(errorMsg);
                    MessageBox.Show(errorMsg, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }

            // Check files and directories
            string instSrcDir = startup + @"\" + inst +
                WSASurveillance.DriversFolderPath(); // selenium
            //string dfp = WSASurveillance.DriversFolderPath(); // selenium
            //string instSrcDir = Path.Combine(startup, inst, dfp); // Doesn't work?
            string instDestDir = WSASurveillance.DriversFolderFullPath(); // selenium

            string parentDirSrc = Path.GetDirectoryName(instSrcDir); // .cache
            string parentDirDest = Path.GetDirectoryName(instDestDir); // .cache
            string parentDir2Src = Path.GetDirectoryName(parentDirSrc); // user
            string parentDir2Dest = Path.GetDirectoryName(parentDirDest); // user
            string parentDirSrcMsg = instSrcDir;
            string parentDirDestMsg = instDestDir;

            // Open the source and destination folders, so that the user can do the copy/paste himself
            if (Directory.Exists(instDestDir)) // .cache\selenium
            {
                if (Directory.Exists(instSrcDir)) Util.ShowDirectory(instSrcDir);
                Util.ShowDirectory(instDestDir);
            }
            else if (Directory.Exists(parentDirDest)) // .cache
            {
                if (Directory.Exists(parentDirSrc)) Util.ShowDirectory(parentDirSrc);
                Util.ShowDirectory(parentDirDest);
                parentDirSrcMsg = parentDirSrc;
                parentDirDestMsg = parentDirDest;
            }
            else if (Directory.Exists(parentDir2Dest)) // user
            {
                if (Directory.Exists(parentDir2Src)) Util.ShowDirectory(parentDir2Src);
                Util.ShowDirectory(parentDir2Dest);
                parentDirSrcMsg = parentDir2Src;
                parentDirDestMsg = parentDir2Dest;
            }

            var msg =
                "Please copy all content from the source directory to the destination directory:\n" +
                "\nSource:\n" + parentDirSrcMsg + "\n" +
                "\nDestination:\n" + parentDirDestMsg +
                "\n\n(the paths are in the clipboard)";

            Util.CopyClipBoard(msg);
            MessageBox.Show(msg, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            cmdInstall.Enabled = false;
            if (!CheckInstallation()) cmdInstall.Enabled = true;
        }

        private void Activation(bool activ)
        {
            cmdStart.Enabled = activ;
            gbBrowsers.Enabled = activ;
            tbPW.Enabled = activ;
            tbLogin.Enabled = activ;
            cmdPWVisible.Enabled = activ;
            chkUserProfile.Enabled = activ;
            cmdDoc.Enabled = activ;
            cmdChange.Enabled = activ;
            cmdConfig.Enabled = activ;
            cmdReport.Enabled = activ;
            if (!activ) cmdInstall.Enabled = false;
            this.Invalidate();
        }

        private void CmdPWVisible_Click(object sender, EventArgs e)
        {
            if (tbPW.PasswordChar == '*') tbPW.PasswordChar = '\0'; else tbPW.PasswordChar = '*';
        }

        private bool CheckInstallation()
        {
            // Check Chrome driver
            string chromeFolder = WSASurveillance.ChromeDriverFolderPath();
            if (!Directory.Exists(chromeFolder)) return false;
            string chromeFile = WSASurveillance.ChromeDriverPath();
            if (!File.Exists(chromeFile)) return false;

            // Check Firefox driver
            string firefoxFolder = WSASurveillance.FirefoxDriverFolderPath();
            if (!Directory.Exists(firefoxFolder)) return false;
            string firefoxFile = WSASurveillance.FirefoxDriverPath();
            if (!File.Exists(firefoxFile)) return false;

            if (!Const.MSEdgeEnabled) return true;

            // Check Edge driver
            string edgeFolder = WSASurveillance.EdgeDriverFolderPath();
            if (!Directory.Exists(edgeFolder)) return false;
            string edgeFile = WSASurveillance.EdgeDriverPath();
            if (!File.Exists(edgeFile)) return false;

            if (Const.MSEdgeSeleniumManagerCheck) 
            {
                string edgeMngrFile = Application.StartupPath + WSASurveillance.edgeDriverManagerPath;
                    //@"\selenium-manager\windows\selenium-manager.exe";
                if (!File.Exists(edgeMngrFile)) 
                {
                    MessageBox.Show(
                        "Can't find MS-Edge manager file:\n"+ edgeMngrFile,
                        appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }
        
        private void RbEdge_Click(object sender, EventArgs e)
        {
            // Edge piloting only works with a temporary Profile, by design at Microsoft
            // https://github.com/MicrosoftEdge/EdgeWebDriver/issues/58
            if (rbEdge.Checked) chkUserProfile.Checked = false;
        }

        private void RbFirefox_Click(object sender, EventArgs e)
        {
            if (rbFirefox.Checked && !Const.FirefoxWithUserProfile) chkUserProfile.Checked = false;
        }

        private void ChkUserProfile_Click(object sender, EventArgs e)
        {
            // Edge piloting only works with a temporary Profile
            // With Firefox, it depends (and OK for Chrome)
            if (Const.FirefoxWithUserProfile)
                if (chkUserProfile.Checked && rbEdge.Checked) rbChrome.Checked = true;
            else
                if (chkUserProfile.Checked && !rbChrome.Checked) rbChrome.Checked = true;
        }

        #endregion

        #region "Surveillance"

        private void CmdStart_Click(object sender, EventArgs e)
        {
            var sitesTxt = WSASurveillance.ReadConfigSites(tbLogin.Text, tbPW.Text, out bool cancel);
            if (cancel) return;
            if (sitesTxt.Count == 0) {
                MessageBox.Show(
                    "No site is defined in the configuration file!",
                    appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                        
            Surveillance(sitesTxt, tbPW.Text);
        }

        bool CheckIfBrowserIsClosed()
        {
            var msg = false;
            if (rbChrome.Checked)
            {
                while (!WSASurveillance.CheckChromeBrowserIsClosed())
                {
                    if (!msg)
                    {
                        if (DialogResult.Cancel == MessageBox.Show(
                            "Please close the Chrome browser!", appTitle,
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                            return false;
                    }
                    msg = true;
                    System.Threading.Thread.Sleep(Const.checkRetryWaitTimeMSec);
                }
            }

            if (rbFirefox.Checked)
            {
                while (!WSASurveillance.CheckFirefoxBrowserIsClosed())
                {
                    if (!msg)
                    {
                        if (DialogResult.Cancel == MessageBox.Show(
                            "Please close the Firefox browser!", appTitle,
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                            return false; 
                    }
                    msg = true;
                    System.Threading.Thread.Sleep(Const.checkRetryWaitTimeMSec);
                }
            }

            if (rbEdge.Checked)
            {
                while (!WSASurveillance.CheckEdgeBrowserIsClosed())
                {
                    if (!msg)
                    {
                        var count = " (" + WSASurveillance.NbEdgeBrowsersOpen() + ")";
                        if (DialogResult.Cancel == MessageBox.Show(
                            "Please close the MS-Edge browser!" + count, appTitle,
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                            return false;
                    }
                    msg = true;
                    System.Threading.Thread.Sleep(Const.checkRetryWaitTimeMSec);
                }
            }
            return true;
        }
        
        void Surveillance(List<Site> sites, string profilePW)
        {
            Activation(activ: false);

            var profile = tbLogin.Text;
            var sitesConf = new SitesConfig
            {
                Sites = sites,
                UseProfile = chkUserProfile.Checked,
                Profile = profile,
                ProfilePW = profilePW
            };
            sitesConf.ComputeNbSites();
            
            string browser = "";
            if (Const.browserMustBeClosedToStart
                && sitesConf.NbWebSites > 0
                && !CheckIfBrowserIsClosed())
            { Activation(activ: true); return; }

            if (rbChrome.Checked) browser = "Chrome";
            if (rbFirefox.Checked) browser = "Firefox";
            if (rbEdge.Checked) browser = "MS-Edge";

            string opt = "\n(temporary profile)";
            if (chkUserProfile.Checked) opt = "";
            string showBrowser = "\nBrowser: " + browser + opt;
            if (sitesConf.NbWebSites == 0) showBrowser = "";
            string msgP = "Ready to start the monitoring?\n" +
                "Number of sites: " + sitesConf.NbSites + "\n" +
                "User login: " + profile + showBrowser;

            var dr = MessageBox.Show(msgP, appTitle,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel) { Activation(activ: true); return; }

            // Check again after also
            if (Const.browserMustBeClosedToStart
                && sitesConf.NbWebSites > 0
                && !CheckIfBrowserIsClosed())
            { Activation(activ: true); return; }

            var result = "";

            bool success = false;
            
            if (this.rbChrome.Checked)
                success = WSASurveillance.NavigateUsingChromeWithUserProfile(sitesConf, out result);
            else if (this.rbFirefox.Checked) 
                success = WSASurveillance.NavigateUsingFirefoxWithUserProfile(sitesConf, out result);
            else if (this.rbEdge.Checked) 
                success = WSASurveillance.NavigateUsingEdgeWithUserProfile(sitesConf, out result);

            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string reportPath = Path.Combine(userFolderPath, Const.report);
            bool success2 = Util.WriteFile(reportPath, result, out string errorMsg);
            string msg = "Session completed successfully!";
            if (!success) msg = "Browser monitoring failed!\n" + errorMsg;
            if (success2)
            {
                Util.StartProcess(reportPath);
                msg += "\nClick OK to stop controlling the browser.";
            }
            else
            {
                Util.CopyClipBoard(result);
                //MessageBox.Show(
                //    "Completed! (this information is in the clipboard):\n" + r +
                //    "You can check the displayed Sites and then close the browser.\n" +
                //    "Click OK to stop controlling the browser.", appTitle,
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                msg += "\n(this information is in the clipboard):\n" + result +
                    "You can check the displayed sites and then close the browser.\n" +
                    "Click OK to stop controlling the browser.";
            }

            Activation(activ: true);

            if (success)
                MessageBox.Show(msg, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(msg, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            if (sitesConf.NbWebSites > 0) 
            { 
                Activation(activ: false);
                WSASurveillance.Quit();
                Activation(activ: true);
            }
        }
        
        void ShortcutMode(List<Site> sites, string profilePW)
        {
            Activation(activ: false);

            var profile = tbLogin.Text;
            var sitesConf = new SitesConfig
            {
                Sites = sites,
                UseProfile = chkUserProfile.Checked,
                Profile = profile,
                ProfilePW = profilePW
            };
            sitesConf.ComputeNbSites();

            if (Const.browserMustBeClosedToStart
                && sitesConf.NbWebSites > 0
                && !CheckIfBrowserIsClosed())
            { Activation(activ: true); return; }

            var r = "";
            bool success = false;
            if (this.rbChrome.Checked) 
                success = WSASurveillance.NavigateUsingChromeWithUserProfile(sitesConf, out r);
            else if (this.rbFirefox.Checked) 
                success = WSASurveillance.NavigateUsingFirefoxWithUserProfile(sitesConf, out r);
            else if (this.rbEdge.Checked) 
                success = WSASurveillance.NavigateUsingEdgeWithUserProfile(sitesConf, out r);

            Util.CopyClipBoard(r);
            if (!success) MessageBox.Show(
                "Browser monitoring failed: (this information is in the clipboard):\n" + r,
                appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

            // ToDo: Do not quit, try to keep the browser open (no success so far)
            //WSASurveillance.Quit();

            Activation(activ: true);
        }

        #endregion
    }
}
