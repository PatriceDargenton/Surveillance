
using SurveillanceCSharp;
using System;
using System.Collections.Generic;

namespace Surveillance
{
    public class Site
    {
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public string UserProfileLogin { get; set; }
        public string UserProfilePW { get; set; }

        /// <summary>
        /// Website Disabled: only for shortcut mode (run the only site that is not disabled)
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Does the site require a connection?
        /// </summary>
        public bool Connection { get; set; }

        /// <summary>
        /// Ignore this alert?
        ///  This context is already opened by another user.
        ///  This context will be retrieved as is if you open it.
        ///  Do you really want to open it?
        /// </summary>
        public bool IgnoreContextAllreadyOpenedAlert { get; set; }

        /// <summary>
        /// Result of the simple ping: is the site online and responding well?
        /// </summary>
        public bool SimplePing { get; set; }

        /// <summary>
        /// Is the site an executable application, and not a website?
        /// </summary>
        public bool Executable { get; set; }

        /// <summary>
        /// Just check an element on the web page? (no connection is required)
        /// </summary>
        public bool JustCheckElement { get; set; }
        /// <summary>
        /// Check an element on the web page?
        /// </summary>
        public bool CheckElement { get; set; }

        /// <summary>
        /// If the site is in autoconnection mode, then we may be already connected,
        ///  and we will not find the connection button, ignore the error in this case, and display "OK?"
        ///  (if we can find an expected text in the page, then that was a good guess)
        /// </summary>
        public bool AutoconnectionByProfile { get; set; }

        /// <summary>
        /// Use the Windows user Profile to connect
        /// </summary>
        public bool ConnectionByProfile { get; set; }

        /// <summary>
        /// Does the site require a Certificate?
        /// </summary>
        public bool Certificate { get; set; }

        public string SiteLogin { get; set; }
        public string SitePassWord { get; set; }

        /// <summary>
        /// CheckText to find in the page
        /// </summary>
        public string CheckText { get; set; }
        /// <summary>
        /// Name of the html element that should contains the CheckText to find
        /// </summary>
        public string CheckTextName { get; set; }

        /// <summary>
        /// List of inline frames to switch to, before searching any web element
        /// </summary>
        public List<WSAParameter> InlineFrames { get; set; }

        /// <summary>
        /// Index of the select web element to choose
        /// </summary>
        public int SelectIndex { get; set; }

        // Methods to find an html element
        public WSAParameter LoginInputFindElement { get; set; }
        public WSAParameter PWInputFindElement { get; set; }
        public WSAParameter ConnectionBtnFindElement { get; set; }
        public WSAParameter CheckTextFindElement { get; set; }
        public WSAParameter SelectFindElement { get; set; }

        // Boolean results
        public bool StatusCodeOk { get; set; }
        public bool ConnectionOk { get; set; }
        public bool TextFound { get; set; }

        // String results
        public string StatusCodeResult { get; set; }
        public string ConnectionResult { get; set; }
        public string TextFoundResult { get; set; }

        /// <summary>
        /// List of keys to send to executable application
        /// </summary>
        public List<string> KeysExe { get; set; }

        public static Site CreateInstanceExecutable(string name, bool surveillance, string url,
            List<string> keys, bool autoconnection = false)
        {
            var instance = new Site(name, surveillance, url, keys, autoconnection);
            return instance;
        }

        private Site(string name, bool surveillance, string url,
            List<string> keys, bool autoconnection = false)
        {
            SiteName = name;
            Disabled = !surveillance;
            SiteUrl = url;
            Executable = true;
            KeysExe = keys;
            AutoconnectionByProfile = autoconnection;
            Certificate = false;
            Connection = false;
        }

        public static Site CreateInstanceSiteSimplePing(
            string name, bool surveillance, bool certif, string url,
            string profile, string profilePW,
            string checkText = null, 
            WSAParameter checkTextPrm = null,
            string checkTextName = null)
        {
            var instance = new Site(name, surveillance, certif, url, profile, profilePW,
                checkText: checkText,
                checkTextPrm: checkTextPrm,
                checkTextName: checkTextName);
            return instance;
        }
        private Site(string name, bool surveillance, bool certif, string url,
            string profile, string profilePW,
            string checkText = null, WSAParameter checkTextPrm = null, string checkTextName = null)
        {
            SiteName = name;
            Disabled = !surveillance;
            Certificate = certif;
            SiteUrl = url;
            SimplePing = false;
            UserProfileLogin = profile;
            UserProfilePW = profilePW;
            SimplePing = true;
            Connection = false;

            JustCheckElement = false;
            CheckElement = false;
            if (checkTextPrm != null)
            {
                CheckElement = true;
                JustCheckElement = SimplePing;
                this.CheckText = checkText;
                this.CheckTextFindElement = checkTextPrm;
                this.CheckTextName = checkTextName;
            }
        }

        public static Site CreateInstanceSiteWithConnection(
            string name, bool surveillance, bool certif, string url,
            string profile, string profilePW, string login, string pw,
            WSAParameter loginPrm = null, 
            WSAParameter PWPrm = null, 
            WSAParameter connectionPrm = null,
            bool connectionByProfile = false, bool autoconnection = false,
            bool ignoreContextAllreadyOpenedAlert = false,
            string checkText = null, WSAParameter checkTextPrm = null,
            List<WSAParameter> inlineFrames = null, 
            WSAParameter selectPrm = null, int selectIndex = 0)
        {
            var instance = new Site(name, surveillance, certif, url, profile, profilePW, login, pw,
                loginPrm: loginPrm,
                PWPrm: PWPrm,
                connectionPrm: connectionPrm,
                connectionByProfile: connectionByProfile,
                autoconnection: autoconnection,
                ignoreContextAllreadyOpenedAlert: ignoreContextAllreadyOpenedAlert,
                checkText: checkText, checkTextPrm: checkTextPrm,
                inlineFrames: inlineFrames, selectPrm: selectPrm, selectIndex: selectIndex);
            return instance;
        }
        private Site(string name, bool surveillance, bool certif, string url,
            string profile, string profilePW, string login, string pw,
            WSAParameter loginPrm, WSAParameter PWPrm, WSAParameter connectionPrm = null,
            bool connectionByProfile = false, bool autoconnection = false,
            bool ignoreContextAllreadyOpenedAlert = false,
            string checkText = null, WSAParameter checkTextPrm = null,
            List<WSAParameter> inlineFrames = null, WSAParameter selectPrm = null, int selectIndex = 0)
        {
            SiteName = name;
            Disabled = !surveillance;
            Certificate = certif;
            SiteUrl = url;
            SimplePing = false;
            Connection = true;
            UserProfileLogin = profile;
            UserProfilePW = profilePW;
            SiteLogin = login;
            SitePassWord = pw;
            LoginInputFindElement = loginPrm;
            PWInputFindElement = PWPrm;
            ConnectionBtnFindElement = connectionPrm;
            this.ConnectionByProfile = connectionByProfile;
            AutoconnectionByProfile = autoconnection;
            this.IgnoreContextAllreadyOpenedAlert = ignoreContextAllreadyOpenedAlert;

            this.InlineFrames = inlineFrames;
            this.SelectFindElement = selectPrm;
            this.SelectIndex = selectIndex;

            CheckElement = false;
            if (checkTextPrm != null)
            {
                CheckElement = true;
                this.CheckText = checkText;
                this.CheckTextFindElement = checkTextPrm;
            }
        }


        public string DisplaySiteName(int longestSiteName)
        {
            string format = "{0,-" + longestSiteName + "}";
            return String.Format(format, this.SiteName);
        }

        public string DisplayPingStatus()
        {
            return Const.sep1 + "Ping: " + this.StatusCodeResult;
        }

        public string DisplayConnectionStatus()
        {
            if (this.JustCheckElement) return Const.sep2;
            if (!this.Connection) return Const.sep2;

            if (!this.ConnectionOk && this.AutoconnectionByProfile)
                // If the Login element is not present, user may be already connected,
                //  ignore the error in this case
                // As a result, we cannot be sure that the page is displayed correctly
                // But if we find the expected CheckText in the web page, then that was a good guess
                return Const.sep + "Connection: OK?";
            else
                return Const.sep + "Connection: " + (this.ConnectionOk ? "OK" : "Not OK");
        }

        public string DisplayFullConnectionStatus()
        {
            if (this.JustCheckElement) return Const.sep2;
            if (!this.Connection) return Const.sep2;
            return Const.sep + "Connection: " + this.ConnectionResult;
        }

        public string DisplayCheckTextLabel()
        {
            if (!this.CheckElement) return "";
            if (String.IsNullOrEmpty(CheckText))
                return "";
            else
                return Const.sep + "Text '" + this.CheckText + "'";
        }

        public string DisplayCheckTextStatus()
        {
            if (!this.CheckElement) return "";
            return Const.sep + (this.TextFound ? "tag found" : "tag not found");
        }

        public string DisplayFullCheckTextStatus()
        {
            if (!this.CheckElement) return "";

            string tagName = "";
            if (Const.showTagNameInFullReport)
                tagName = " (tag:'" + this.CheckTextFindElement.Value + "')";

            if (String.IsNullOrEmpty(CheckText))
                return Const.sep + (this.TextFound ? "tag found" : "tag not found") +
                    (!String.IsNullOrEmpty(this.CheckTextName) ? ": '" + this.CheckTextName + "'" : "") + tagName;
            else
                return Const.sep + "Text '" + this.CheckText + (this.TextFound ? "': found" : "': not found") + tagName;
        }
    }
}
