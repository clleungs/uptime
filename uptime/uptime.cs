using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;  // Process 
using System.IO;
using System.Net;
using System.Management;  // Need Install
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Versioning;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Data;

/* 
 2024/07/09 Review Change smtp
 2024/07/08 review
 2023/06/12 Apply Redirect
 2023/06/10 review, Send email have issue, stablished connection failed because connected host has failed
 2021/01/22 Reveiw
 2019/09/11 Apply Cipher
 2019/07/09
 System Update time
 2019/08/19 Add -m n detect last boot time

 Run on office PC
 ----------------
 getAccoutKey.dll 

 Object reference
 ----------------
 System.NullReferenceException: Object reference not set to an instance of an object.
   at getAccountKey.AccountName..ctor(String vAccount) in F:\prog-c\ClassLibrary\Framework\getAccountKey\getAccountKey\clsAccount.cs:line 34
  user account : 
  kef\eric.leung
*/

namespace uptime
{
    public static class Globals
    {
        public static int vOffset = 0;
        public static decimal vdays = 0;
        public static string myIP = "";
        public static string hostName = "";
        public static Boolean vConfig = false;
        public static DateTime dt = DateTime.Now;
        public static Boolean isRedirect = false;
        public static Boolean verbose = false;
        public static Boolean testMode = false; 
        public static String url = "https://docs.google.com/document/d/1H8CeoDzojcC-e778EQj3gXfkNnmLa1vWs6yzjDhynNs/edit"; 
        public static CultureInfo culture = new CultureInfo("en-UK");
        const string GREEN = "\x1B[92m";
        const string CYAN = "\x1B[96m";
        const string YELLOW = "\x1B[93m";
        const string GRAY = "\x1B[90m";
        const string RESET = "\x1B[0m\x1B[037m";
        static Globals()
        {

        }
        public static void DisplayConfig()
        {
            if (vConfig == false) return;
            Console.WriteLine(GREEN + "DisplayConfig()" + RESET);
        }
        public static void Log(String logStr)
        {
            if (verbose == true)
            {
                Console.WriteLine("{2}{0} {1}{3}", DateTime.Now.ToString("HH:mm:ss.ff", Globals.culture), logStr, GRAY, RESET);
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    partial class uptime
    {
       static getAccountKey.AccountName accA = new getAccountKey.AccountName(Properties.Settings.Default.siteUser);

        static void Sendemail()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(0);
            if (Globals.verbose)
            {
                Console.WriteLine("{1}Procedure Name:{0}{2}", sf?.GetMethod(), GRAY, RESET);
            }

            // send mail and offset need 
            string mailto = Properties.Settings.Default.mail_to;
            Console.WriteLine("Sendmail .. " + mailto);
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(mailto));
            msg.From = new MailAddress(Properties.Settings.Default.siteUser);
            msg.Subject = "Machine " + Globals.hostName + " Last Boot on " + Globals.dt.ToLocalTime().ToString();
            msg.IsBodyHtml = true;
            msg.Body = "Machine      : " + Globals.hostName + "<br>" +
                       "Last Boot    : " + Globals.dt.ToLocalTime().ToString() + "<br>" +
                       "Total Uptime : " + Globals.vdays.ToString() + " days <br>" +
                       "Offset value : " + Globals.vOffset.ToString() + " days <br>" +
                       "Send mail    : 10.3.0.76<br>" +
                       "Program ID   : uptime.exe";

            SmtpClient client = new SmtpClient();
            {
             //   string username1 = Properties.Settings.Default.siteUser;
             //   string password1 = accA.getPassword();
             //   client.UseDefaultCredentials = false;
             //   client.Credentials = new
             //   System.Net.NetworkCredential(username1, password1);
            }
            client.Port = 25;
            client.Host = "10.3.0.76";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            // client.EnableSsl = true;
            Console.WriteLine("Sending...by {0}", Properties.Settings.Default.siteUser);
            try
            {
                client.Send(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Done");

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        static void SendTelegram()
        {
            // using curl send telegram bot message 
            // https://medium.com/@xabaras/sending-a-message-to-a-telegram-channel-the-easy-way-eb0a0b32968

            var st = new StackTrace();
            var sf = st.GetFrame(0);
            if (Globals.verbose)
            {
                Console.WriteLine("{1}Procedure Name:{0}{2}", sf?.GetMethod(),GRAY,RESET); 
            }

            string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";

            string apiToken = Cipher.Cipher.Decrypt(Properties.Settings.Default.TelegramToken, "GP2013KEF");
            string chatId = "91321874";   // @moonhkt
            string text = "Machine      : " + Globals.hostName + "\n" +
                         "Last Boot    : " + Globals.dt.ToString() + "\n" +
                         "Total Uptime : " + Globals.vdays.ToString() + " days \n" +
                         "Offset value : " + Globals.vOffset.ToString() + " days \n" +
                         "Program ID   : uptime.exe\n" +
                         "Machine Reboot" + "\n" +

            Properties.Settings.Default.emji;

            urlString = String.Format(urlString, apiToken, chatId, text);
            
            Globals.Log(String.Format("Procedure Name:{0}", sf?.GetMethod()));

            WebRequest request = WebRequest.Create(urlString);
            Stream rs = request.GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(rs);
            string line = "";

            StringBuilder sb = new StringBuilder();
            while (line != null)
            {
                line = reader.ReadLine();
                if (line != null)
                    sb.Append(line);
            }
            Console.WriteLine("Telegram Sent");

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        static void Listing()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(0);
            if (Globals.verbose)
            {
                Globals.Log(String.Format("Procedure Name:{0}", sf?.GetMethod()));
            }

            Console.WriteLine("{1}{0}{2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", Globals.culture), YELLOW, RESET);
            // http://technico.qnownow.com/how-to-get-operating-system-properties-using-wmi-in-c/
            SelectQuery query = new SelectQuery(@"Select * from Win32_OperatingSystem");

            //initialize the searcher with the query it is supposed to execute

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                //execute the query
                foreach (ManagementObject process in searcher.Get())
                {
                    Console.WriteLine("{0,-20} {1}", "Caption:", process["Caption"]);
                    String UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    Globals.hostName = System.Net.Dns.GetHostName();
                    //myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                    // myIP = Dns.GetHostEntry(hostName).AddressList[0].ToString();
                    //IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress[] ipaddress = Dns.GetHostAddresses(Globals.hostName);
                    // https://www.c-sharpcorner.com/UploadFile/1e050f/getting-ip-address-and-host-name-using-dns-class/
                    foreach (IPAddress ip in ipaddress)
                    {
                        if (ip.ToString().StartsWith("192.168.1."))
                        {
                            // Console.WriteLine(ip.ToString());
                            Globals.myIP = ip.ToString();
                        }
                        if (ip.ToString().StartsWith("10."))
                        {
                            Globals.myIP = ip.ToString();
                        }
                    }
                    Console.WriteLine("{0,-20} {1}", "IP Address:", Globals.myIP);
                    Console.WriteLine("{0,-20} {1}", "Hostname:", Globals.hostName);
                    Console.WriteLine("{0,-20} {1}", "User Name:", UserName);
                    string stringdate = process["LastBootUpTime"].ToString();
                    //dt = ManagementDateTimeConverter.ToDateTime(stringdate);
                    // dt = ManagementDateTimeConverter.ToDateTime(stringdate).ToLocalTime();
                    // Console.WriteLine("{0,-20} {1}", "LastBootUpTime:", dt);
                    // https://www.c-sharpcorner.com/article/get-current-time-zone-in-C-Sharp/
                    const string dataFmt = "{0,-20} {1}";
                    const string timeFmt = "{0,-20} {1:MM-dd-yyyy HH:mm}";
                    TimeZone curTimeZone = TimeZone.CurrentTimeZone;
                    // What is TimeZone name?  
                    Console.WriteLine(dataFmt, "TimeZone Name:", curTimeZone.StandardName);

                    // What is GMT/UTC offset ?  
                    TimeSpan currentOffset = curTimeZone.GetUtcOffset(DateTime.Now);
                    Console.WriteLine(dataFmt, "UTC offset:", currentOffset);

                    Globals.dt = ManagementDateTimeConverter.ToDateTime(stringdate);
                    // Console.WriteLine("{0,-20} {1}", "LastBootUpTime:", Globals.dt.ToString());
                    Console.WriteLine("{0,-20} {1}", "LastBootUpTime:", Globals.dt.ToString("yyyy/MM/dd HH:MM"));
                    Console.WriteLine("{0,-20} {1}", "UpTime:", DateTime.Now - Globals.dt);
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        static void Main(string[] args)
        {
            //  vcolor = args.Length > i + 1 ? args[i + 1] : "";
            if (IsOutputRedirected() == true)  // Check Redirect to File
            {
                Globals.isRedirect = true;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-h")
                    Help();
                else if (args[i] == "-url")
                {
                    // OpenURL();
                    OpenBrowser(Globals.url);
                    System.Environment.Exit(-1);
                }
                else if (args[i] == "-v")
                {
                    Globals.verbose = true;
                }
                else if (args[i] == "-t")
                {
                    Globals.testMode = true;
                }
                else if (args[i] == "-m")     //  Send mail if delta found 
                {
                    if (args.Length > i + 1)
                    {
                        Globals.vOffset = Convert.ToInt32(args[i + 1]);
                    }
                }
                else if (args[i] == "-config")
                {
                    Globals.vConfig = true;
                    Globals.DisplayConfig();
                    System.Environment.Exit(-1);
                }
                else if (args[i] == "-asm")
                {
                    ListAsm();
                }
            }

          //  Globals.Log(Properties.Settings.Default.siteUser);
            Listing();
            if (Globals.vOffset > 0 & Globals.vOffset >= Globals.vdays)
            {
                if (Globals.verbose)
                {
                    Console.WriteLine("{1}Globals.vOffset = {0} Globals.vdays = {1}{2}", Globals.vOffset, Globals.vdays, GRAY, RESET); 
                }
                Sendemail();
                SendTelegram();
            }
            else if (Globals.testMode)
            {
                Sendemail();
                SendTelegram();
            }
        }
    }
}
