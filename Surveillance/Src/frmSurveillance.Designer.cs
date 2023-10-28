
namespace SurveillanceCSharp
{
  partial class FrmSurveillance
  {
    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Code généré par le Concepteur Windows Form

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSurveillance));
            this.cmdStart = new System.Windows.Forms.Button();
            this.gbBrowsers = new System.Windows.Forms.GroupBox();
            this.rbEdge = new System.Windows.Forms.RadioButton();
            this.rbFirefox = new System.Windows.Forms.RadioButton();
            this.rbChrome = new System.Windows.Forms.RadioButton();
            this.tbPW = new System.Windows.Forms.TextBox();
            this.lblPW = new System.Windows.Forms.Label();
            this.lblLogin = new System.Windows.Forms.Label();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.cmdPWVisible = new System.Windows.Forms.Button();
            this.cmdInstall = new System.Windows.Forms.Button();
            this.chkUserProfile = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdDoc = new System.Windows.Forms.Button();
            this.cmdChange = new System.Windows.Forms.Button();
            this.cmdConfig = new System.Windows.Forms.Button();
            this.cmdReport = new System.Windows.Forms.Button();
            this.gbBrowsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdStart
            // 
            this.cmdStart.Enabled = false;
            this.cmdStart.Location = new System.Drawing.Point(29, 22);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(75, 23);
            this.cmdStart.TabIndex = 0;
            this.cmdStart.Text = "Start";
            this.toolTip1.SetToolTip(this.cmdStart, "Start monitoring");
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.CmdStart_Click);
            // 
            // gbBrowsers
            // 
            this.gbBrowsers.Controls.Add(this.rbEdge);
            this.gbBrowsers.Controls.Add(this.rbFirefox);
            this.gbBrowsers.Controls.Add(this.rbChrome);
            this.gbBrowsers.Location = new System.Drawing.Point(29, 67);
            this.gbBrowsers.Name = "gbBrowsers";
            this.gbBrowsers.Size = new System.Drawing.Size(119, 122);
            this.gbBrowsers.TabIndex = 5;
            this.gbBrowsers.TabStop = false;
            this.gbBrowsers.Text = "Browser";
            // 
            // rbEdge
            // 
            this.rbEdge.AutoSize = true;
            this.rbEdge.Location = new System.Drawing.Point(14, 79);
            this.rbEdge.Name = "rbEdge";
            this.rbEdge.Size = new System.Drawing.Size(50, 17);
            this.rbEdge.TabIndex = 2;
            this.rbEdge.TabStop = true;
            this.rbEdge.Text = "Edge";
            this.toolTip1.SetToolTip(this.rbEdge, "Edge piloting only works with a temporary profile, by design at Microsoft");
            this.rbEdge.UseVisualStyleBackColor = true;
            this.rbEdge.Click += new System.EventHandler(this.RbEdge_Click);
            // 
            // rbFirefox
            // 
            this.rbFirefox.AutoSize = true;
            this.rbFirefox.Location = new System.Drawing.Point(14, 56);
            this.rbFirefox.Name = "rbFirefox";
            this.rbFirefox.Size = new System.Drawing.Size(56, 17);
            this.rbFirefox.TabIndex = 1;
            this.rbFirefox.TabStop = true;
            this.rbFirefox.Text = "Firefox";
            this.rbFirefox.UseVisualStyleBackColor = true;
            this.rbFirefox.Click += new System.EventHandler(this.RbFirefox_Click);
            // 
            // rbChrome
            // 
            this.rbChrome.AutoSize = true;
            this.rbChrome.Checked = true;
            this.rbChrome.Location = new System.Drawing.Point(14, 33);
            this.rbChrome.Name = "rbChrome";
            this.rbChrome.Size = new System.Drawing.Size(61, 17);
            this.rbChrome.TabIndex = 0;
            this.rbChrome.TabStop = true;
            this.rbChrome.Text = "Chrome";
            this.rbChrome.UseVisualStyleBackColor = true;
            // 
            // tbPW
            // 
            this.tbPW.Location = new System.Drawing.Point(250, 50);
            this.tbPW.Name = "tbPW";
            this.tbPW.Size = new System.Drawing.Size(100, 20);
            this.tbPW.TabIndex = 2;
            this.toolTip1.SetToolTip(this.tbPW, "Windows user password");
            this.tbPW.TextChanged += new System.EventHandler(this.TbPW_TextChanged);
            // 
            // lblPW
            // 
            this.lblPW.AutoSize = true;
            this.lblPW.Location = new System.Drawing.Point(185, 54);
            this.lblPW.Name = "lblPW";
            this.lblPW.Size = new System.Drawing.Size(59, 13);
            this.lblPW.TabIndex = 6;
            this.lblPW.Text = "Password :";
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(205, 27);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(39, 13);
            this.lblLogin.TabIndex = 8;
            this.lblLogin.Text = "Login :";
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(250, 24);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(100, 20);
            this.tbLogin.TabIndex = 1;
            this.toolTip1.SetToolTip(this.tbLogin, "Windows username");
            this.tbLogin.TextChanged += new System.EventHandler(this.TbLogin_TextChanged);
            // 
            // cmdPWVisible
            // 
            this.cmdPWVisible.Location = new System.Drawing.Point(356, 50);
            this.cmdPWVisible.Name = "cmdPWVisible";
            this.cmdPWVisible.Size = new System.Drawing.Size(26, 20);
            this.cmdPWVisible.TabIndex = 3;
            this.cmdPWVisible.Text = "*";
            this.toolTip1.SetToolTip(this.cmdPWVisible, "Show or hide the password");
            this.cmdPWVisible.UseVisualStyleBackColor = true;
            this.cmdPWVisible.Click += new System.EventHandler(this.CmdPWVisible_Click);
            // 
            // cmdInstall
            // 
            this.cmdInstall.Location = new System.Drawing.Point(250, 94);
            this.cmdInstall.Name = "cmdInstall";
            this.cmdInstall.Size = new System.Drawing.Size(75, 23);
            this.cmdInstall.TabIndex = 4;
            this.cmdInstall.Text = "Install";
            this.toolTip1.SetToolTip(this.cmdInstall, "Copy the folder to install");
            this.cmdInstall.UseVisualStyleBackColor = true;
            this.cmdInstall.Click += new System.EventHandler(this.CmdInstall_Click);
            // 
            // chkUserProfile
            // 
            this.chkUserProfile.AutoSize = true;
            this.chkUserProfile.Checked = true;
            this.chkUserProfile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUserProfile.Location = new System.Drawing.Point(29, 206);
            this.chkUserProfile.Name = "chkUserProfile";
            this.chkUserProfile.Size = new System.Drawing.Size(99, 17);
            this.chkUserProfile.TabIndex = 6;
            this.chkUserProfile.Text = "Use user profile";
            this.toolTip1.SetToolTip(this.chkUserProfile, "Use the Windows user profile for the selected browser, otherwise use a temporary " +
        "profile without cookies");
            this.chkUserProfile.UseVisualStyleBackColor = true;
            this.chkUserProfile.Click += new System.EventHandler(this.ChkUserProfile_Click);
            // 
            // cmdDoc
            // 
            this.cmdDoc.Location = new System.Drawing.Point(29, 240);
            this.cmdDoc.Name = "cmdDoc";
            this.cmdDoc.Size = new System.Drawing.Size(37, 36);
            this.cmdDoc.TabIndex = 7;
            this.cmdDoc.Text = "Doc";
            this.toolTip1.SetToolTip(this.cmdDoc, "Show the documentation (ReadMe.md)");
            this.cmdDoc.UseVisualStyleBackColor = true;
            this.cmdDoc.Click += new System.EventHandler(this.CmdDoc_Click);
            // 
            // cmdChange
            // 
            this.cmdChange.Location = new System.Drawing.Point(84, 240);
            this.cmdChange.Name = "cmdChange";
            this.cmdChange.Size = new System.Drawing.Size(37, 36);
            this.cmdChange.TabIndex = 8;
            this.cmdChange.Text = "Ch.";
            this.toolTip1.SetToolTip(this.cmdChange, "Show the change documentation (Changelog.md)");
            this.cmdChange.UseVisualStyleBackColor = true;
            this.cmdChange.Click += new System.EventHandler(this.CmdChange_Click);
            // 
            // cmdConfig
            // 
            this.cmdConfig.Location = new System.Drawing.Point(141, 240);
            this.cmdConfig.Name = "cmdConfig";
            this.cmdConfig.Size = new System.Drawing.Size(58, 36);
            this.cmdConfig.TabIndex = 9;
            this.cmdConfig.Text = "Config.";
            this.toolTip1.SetToolTip(this.cmdConfig, "Show the config. file (Surveillance.ini)");
            this.cmdConfig.UseVisualStyleBackColor = true;
            this.cmdConfig.Click += new System.EventHandler(this.CmdConfig_Click);
            // 
            // cmdReport
            // 
            this.cmdReport.Location = new System.Drawing.Point(219, 240);
            this.cmdReport.Name = "cmdReport";
            this.cmdReport.Size = new System.Drawing.Size(58, 36);
            this.cmdReport.TabIndex = 10;
            this.cmdReport.Text = "Report";
            this.toolTip1.SetToolTip(this.cmdReport, "Show the last Report");
            this.cmdReport.UseVisualStyleBackColor = true;
            this.cmdReport.Click += new System.EventHandler(this.CmdReport_Click);
            // 
            // FrmSurveillance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 288);
            this.Controls.Add(this.cmdReport);
            this.Controls.Add(this.cmdConfig);
            this.Controls.Add(this.cmdChange);
            this.Controls.Add(this.cmdDoc);
            this.Controls.Add(this.chkUserProfile);
            this.Controls.Add(this.cmdInstall);
            this.Controls.Add(this.cmdPWVisible);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.lblPW);
            this.Controls.Add(this.tbPW);
            this.Controls.Add(this.gbBrowsers);
            this.Controls.Add(this.cmdStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmSurveillance";
            this.Text = "Surveillance";
            this.Load += new System.EventHandler(this.FrmSurveillance_Load);
            this.Shown += new System.EventHandler(this.FrmSurveillance_Shown);
            this.gbBrowsers.ResumeLayout(false);
            this.gbBrowsers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.GroupBox gbBrowsers;
        private System.Windows.Forms.RadioButton rbEdge;
        private System.Windows.Forms.RadioButton rbFirefox;
        private System.Windows.Forms.RadioButton rbChrome;
        private System.Windows.Forms.TextBox tbPW;
        private System.Windows.Forms.Label lblPW;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.Button cmdPWVisible;
        private System.Windows.Forms.Button cmdInstall;
        private System.Windows.Forms.CheckBox chkUserProfile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdDoc;
        private System.Windows.Forms.Button cmdChange;
        private System.Windows.Forms.Button cmdConfig;
        private System.Windows.Forms.Button cmdReport;
    }
}

