using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class Settings
    {
        public static IConfiguration Configuration { get; set; }

        static Settings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public struct Default
        {
            public static readonly string BASEDIR = AppDomain.CurrentDomain.BaseDirectory;
        }

        public struct Config
        {
            public readonly static string DB2CONN = Configuration.GetSection("DB2Conn").Value;
            public readonly static string LDAP_PATH = Configuration.GetSection("LDAPPath").Value;
            public readonly static string LDAP_DOMAIN = Configuration.GetSection("LDAPDomain").Value;
            public readonly static string SVC_USER = Configuration.GetSection("SVCUser").Value;
            public readonly static string SVC_PASS = Configuration.GetSection("SVCPass").Value;
            public readonly static string SMTP_SETTINGS = Configuration.GetSection("SMTPSettings").Value;
            public readonly static string SENDER_NAME = Configuration.GetSection("NotifSenderName").Value;
            public readonly static string SENDER_EMAIL = Configuration.GetSection("NotifSenderEmail").Value;
            public readonly static string HOST = Configuration.GetSection("Host").Value;
            public readonly static string EMAILCONTENT_PATH = Path.Combine(Directory.GetCurrentDirectory(), "EmailContents");//Configuration.GetSection("EmailContentPath").Value;
            public readonly static string REPORTS_DIRECTORY = Configuration.GetSection("ReportsDirectory").Value;
            public readonly static string MONETARY_FILES = Configuration.GetSection("MonetaryFiles").Value;
            public readonly static string NONMONETARY_FILES = Configuration.GetSection("NonMonetaryFiles").Value;
            public readonly static string PR_PARAMS = Configuration.GetSection("PervasiveAverageParams").Value;
            public readonly static string DB2CONN_TEMP = Configuration.GetSection("DB2ConnTemp").Value;
            public readonly static string EnableAllViewingOfClosedException = Configuration.GetSection("EnableAllViewingOfClosedException").Value;
            public readonly static string IncludeOnlyEscalationReportAgingDays = Configuration.GetSection("IncludeOnlyEscalationReportAgingDays").Value;
            public readonly static string AOOGenerateReportWithoutApproval = Configuration.GetSection("AOOGenerateReportWithoutApproval").Value;
            public readonly static string PulloutRequestSourceFolder = Path.Combine(Directory.GetCurrentDirectory(), "ExcelTemplate");//Configuration.GetSection("PulloutRequestSourceFolder").Value;
            public readonly static string PulloutRequestSourceFile = Path.Combine(Directory.GetCurrentDirectory(), "ExcelTemplate", "Pulloutrequest.xlsx");//Configuration.GetSection("PulloutRequestSourceFile").Value;
            public readonly static string PulloutRequestTempFile = Path.Combine(Directory.GetCurrentDirectory(), "ExcelTemplateTemp");//Configuration.GetSection("PulloutRequestTempFile").Value;
            public readonly static string PulloutRequestRecipient = Configuration.GetSection("PulloutRequestRecipient").Value;
            public readonly static string PulloutRequestRoleID = Configuration.GetSection("PulloutRequestRoleID").Value;




        }
    }
}

