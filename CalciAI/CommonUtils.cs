using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CalciAI
{
    public static class CommonUtils
    {
        public static string Banner(string bannerText)
        {
            return Figgle.FiggleFonts.Standard.Render(bannerText.Split(".").Last());
        }

        public static string EnvironmentName
        {
            get
            {
                var dotnetEnvironment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                var aspnetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                var environmentName = string.IsNullOrEmpty(dotnetEnvironment) ? aspnetEnvironment : dotnetEnvironment;

                if (environmentName == null)
                {
                    return EnvironmentNames.Production;
                }

                return environmentName;
            }
        }

        public static readonly ImmutableHashSet<string> ASSEMBLIES = ImmutableHashSet.Create("CalciAI");

        public static bool IsValidUsername(string username)
        {
            // only a-z or 0-9
            var pattern = @"^[a-z0-9]{3,20}$";

            Regex regex = new(pattern);
            return regex.IsMatch(username);
        }

        public static bool ValidatePasswordPolicy(string password)
        {
            // only a-z or 0-9
            var pattern = @"(?=[A-Za-z0-9@#$%^&+!=]+$)^(?=.*[a-z])(?=.*[0-9])(?=.{8,}).*$";
            Regex regex = new(pattern);
            return regex.IsMatch(password);
        }

        public static string FilePathSlash => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";

        public static string RootFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + FilePathSlash;

        public static string ConfigFolder => RootFolder + "configs" + FilePathSlash;

        public static string ReportFolder => RootFolder + "reports" + FilePathSlash;

        public static string QueryFolder => RootFolder + "queries" + FilePathSlash;

    }
}
