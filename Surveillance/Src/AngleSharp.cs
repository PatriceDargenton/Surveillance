
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using Surveillance; // Const

public class WSAPing // WebSite and App simple ping, and file check
{
    static readonly string statusCodeOK = "OK";
    static readonly string sep1 = " : ";

    public static string PingWebSites(List<Site> sites)
    {
        Task<string> mainTask = Task.Run(async () => await PingWebSitesAsync(sites));
        mainTask.Wait();
        var r = mainTask.Result;
        return r;
    }

    private static async Task<String> PingWebSitesAsync(List<Site> sites)
    {
        // ImplicitWait 10 sec. for all tasks
        var ts = new TimeSpan(0, 0, Const.allSitesWaitTimeSec);
        int timeoutMilliseconds = (int)(ts.TotalMilliseconds);

        var tasks = new List<Task<string>>();

        foreach (Site site in sites)
        {
            tasks.Add(GetStatusCodeWithTimeOutAsync(site.SiteUrl, timeoutMilliseconds));
        }

        await Task.WhenAll(tasks);

        var sb = new StringBuilder();
        for (int i = 0; i < sites.Count; i++)
        {
            string statusCode = tasks[i].Result;
            if (statusCode == statusCodeOK)
            { sites[i].StatusCodeOk = true; sites[i].StatusCodeResult = statusCodeOK; }
            else
                sites[i].StatusCodeResult = statusCode;
            string site = sites[i].SiteName;
            sb.AppendLine(site + sep1 + statusCode);
        }

        return sb.ToString();
    }

    public static async Task<string> GetStatusCodeWithTimeOutAsync(string url, int timeoutMilliseconds)
    {
        // if (url.ToLower().EndsWith(".exe"))
        string urlMin = url.ToLower();
        if (!urlMin.Contains("http://") && !urlMin.Contains("https://")) 
        {
            if (File.Exists(url))
                return statusCodeOK; // "OK"; 
            else 
                return "File not found!";
        }

        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var documentTask = context.OpenAsync(url);

        if (await Task.WhenAny(documentTask, Task.Delay(timeoutMilliseconds)) != documentTask)
            return "Too long (timeout)";

        var document = await documentTask;
        var sc = document.StatusCode;
        if (sc == HttpStatusCode.OK) return sc.ToString();
        int statuscode = (int)sc;
        return "Error " + statuscode + ": " + sc.ToString();
    }
}