using Microsoft.SharePoint.Client;
using Provision.Console.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Console.Downloader
{
	class Program
	{
		static void Main(string[] args)
		{
			ClientContext clientContext = ContextHelper.GetClientContext("http://contoso", @"user@domaub", "password");
			string destination = @"C:\Temp\App";
			FileHelper.DownloadLibrary(clientContext, "Assets", destination);
			Directory.Delete(destination + @"\App", true);
			FileHelper.DirectoryCopy(@"C:\Temp\App\Assets\App", destination + @"\App", true);
		}
	}
}
