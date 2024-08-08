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
using System.Runtime.InteropServices;
using Cgen; 
namespace uptime
{
    public static class StringExtensions
    { //https://docs.google.com/document/d/1DoUuXCl_dMMp3OM-cSrSLi2zR763YKuYPYu3b-yQmC0/edit
        public static string Repeat(this char chatToRepeat, int repeat)
        {
            return new string(chatToRepeat, repeat);
        }
        public static string Repeat(this string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }
    }
    partial class uptime
    {
        // https://www.codeproject.com/Tips/5255355/How-to-Put-Color-on-Windows-Console
        const string GREEN = "\x1B[92m";
        const string CYAN = "\x1B[96m";
        const string YELLOW = "\x1B[93m";
        const string RED = "\x1B[91m";
        const string ORANGE = "\x1B[38;2;255;165;0m";
        const string MAGENTA = "\x1B[95m";
        const string TIFFANY_BLUE = "\u001b[38;5;123m";
        const string GRAY = "\x1B[90m";
        const string REVERSE = "\x1B[7m";
        const string UNDERLINE = "\x1B[4m";
        const string RESET = "\x1B[0m\x1B[037m";
        
        ///////////////////////////////////////////////////////////////////////////////////////////////
        static void ListAsm()
        {
            Cgen.Utilities.ListAsm(Globals.verbose);
            System.Environment.Exit(-1);
           
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        static void OpenBrowser(string url)
        {
            if (url == "")
                System.Diagnostics.Process.Start("www.google.com");
            else
                System.Diagnostics.Process.Start(url);
            System.Environment.Exit(-1);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        static void OpenURL()
        {
            string browser = string.Empty;
            RegistryKey key = null;

            // setup in Regedit
            // http/shell/Open/Command/Default , value "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"

            try
            {
                key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command");
                if (key != null)
                {
                    // Get default Browser
                    browser = key.GetValue(null).ToString().ToLower().Trim(new[] { '"' });
                }
                if (!browser.EndsWith("exe"))
                {
                    //Remove all after the ".exe"
                    browser = browser.Substring(0, browser.LastIndexOf(".exe", StringComparison.InvariantCultureIgnoreCase) + 4);
                }
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            } 
            // Open the browser.
            Process proc = Process.Start(browser,
                 "https://docs.google.com/document/d/1H8CeoDzojcC-e778EQj3gXfkNnmLa1vWs6yzjDhynNs/edit");
            System.Environment.Exit(-1);

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        static void Help()
        {
            string lohelp = "";
            string tab = "    "; 
            string vfmt01 = "{0,-35} {1,-60}";
            string vprog = System.Reflection.Assembly.GetExecutingAssembly().ToString();
            string vexec = vprog.Split(',')[0];
            
            var asm = Assembly.GetExecutingAssembly();
            var b = asm.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(TargetFrameworkAttribute));
            var strFramework = b.NamedArguments[0].TypedValue.Value;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("help ℹ️");
            Console.ResetColor();
            Console.WriteLine("....");
            lohelp = lohelp + "Display Machine Uptime. Send mail with option -m, using 10.3.0.76  Version : " + YELLOW + "2024/07/09" + RESET
                + "\n" + String.Format(vfmt01, vexec + " -m 2", "# send mail if last boot time within n days")
                + "\n" + String.Format(vfmt01, vexec + " -v", "# Verbose")
                + "\n" + String.Format(vfmt01, vexec + " -t", "# test mode (send mail and Telegram")
                + "\n" + String.Format(vfmt01, vexec + " -asm", "# Get list of assemblies")
                + "\n" + String.Format(vfmt01, vexec + " -url", "# Open URL ")
                + "\n" + String.Format(vfmt01, vexec + " -h", "# help ")
                + "\n" + GREEN + "Source:" + RESET
                + "\n" + "F:\\prog-c\\Utilities\\uptime\\uptime\\bin\\Debug" + "\tFramework Version : " +  CYAN +  strFramework + RESET
                + "\n" + tab + "• Support redirect (WIP), using " + CYAN + vexec + " > f:\\temp\\uptime.log" + RESET
                + "\n" + GREEN + "Setting:" + RESET
                + "\n" + "Base on Properties Settings file " + vexec + ".exe.config" 
                ;
            Console.WriteLine(lohelp);
            System.Environment.Exit(-1);
        }
    }
}
