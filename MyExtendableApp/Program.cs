using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CommonSnappableTypes;

namespace MyExtendableApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("***** Welcome to MyTypeViewer *****");

            do
            {
                Console.WriteLine("\nWould you like to load a snapin? [Y,N]");

                string answer = Console.ReadLine();
                if (!answer.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                try
                {
                    LoadSnapin();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Sorry, can't find snapin");
                }
            }
            while (true);
        }

        static void LoadSnapin()
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Filter = "assemblies (*.dll)|*.dll|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("User cancelled out of the open file dialog.");
                return;
            }
            if (dlg.FileName.Contains("CommonSnappableTypes"))
            {
                Console.WriteLine("CommonSnappableTypes has no snap-ins!");
            }
            else if (!LoadExternalModule(dlg.FileName))
            {
                Console.WriteLine("Nothing implements IAppFunctionality!");
            }
        }

        private static bool LoadExternalModule(string path)
        {
            bool foundSnapIn = false;
            Assembly theSnapInAsm = null;

            try
            {
                theSnapInAsm = Assembly.LoadFrom(path);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occured loading the snaping: {ex.Message}");
                return foundSnapIn;
            }

            var theClassTypes = from t in theSnapInAsm.GetTypes()
                                where t.IsClass && (t.GetInterface("IAppFunctionality") != null)
                                select t;

            foreach(Type t in theClassTypes)
            {
                foundSnapIn = true;
                IAppFunctionality itfApp = (IAppFunctionality)theSnapInAsm.CreateInstance(t.FullName, true);
                itfApp?.DoIt();
                DisplayCompanyData(t);
            }
            return foundSnapIn;
        }

        private static void DisplayCompanyData(Type t)
        {
            var companyInfo = from ci in t.GetCustomAttributes(false)
                              where (ci is CompanyInfoAttribute)
                              select ci;

            foreach (CompanyInfoAttribute c in companyInfo)
            {
                Console.WriteLine($"More info about {c.CompanyName} can be found at {c.CompanyUrl}");
            }
        }
    }
}
