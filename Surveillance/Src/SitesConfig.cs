
using System.Collections.Generic;
using System.Linq;

namespace Surveillance
{
    public class SitesConfig
    {
        public List<Site> Sites { get; set; }

        public string Profile { get; set; }
        public string ProfilePW { get; set; }
        public int NbSites { get; set; }
        public int NbWebSites { get; set; }
        public int LongestSiteName { get; set; }
        public bool UseProfile { get; set; }

        public void ComputeNbSites()
        {
            // Websites that do not require a certificate
            var webSitesWithoutCertif = this.Sites
                .Where(s =>
                    s.Disabled == false &&
                    s.Executable == false &&
                    s.Certificate == false)
                .ToList();
            if (Const.disableSitesWithoutCertif) this.Sites = webSitesWithoutCertif;

            //var webSitesConnOrExe = this.Sites
            //    .Where(s =>
            //        s.Disabled == false &&
            //        (s.Connection == true || s.JustCheckElement || s.Executable == true))
            //    .ToList();
            //this.Sites = webSitesConnOrExe;

            //var webSitesConn = this.Sites
            //    .Where(s =>
            //        s.Disabled == false &&
            //        s.Executable == false &&
            //        s.Connection == true)
            //    .ToList();
            //this.Sites = webSitesConn;

            this.NbSites = this.Sites
                .Where(s => s.Disabled == false)
                .ToList().Count;

            // Web Sites, without executables
            this.NbWebSites = this.Sites
                .Where(s => s.Disabled == false && s.Executable == false)
                .ToList().Count;

            this.LongestSiteName = this.Sites
                .Where(s => s.Disabled == false)
                .Max(s => s.SiteName.Length);
        }
    }
}
