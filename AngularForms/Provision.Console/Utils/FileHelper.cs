using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
    public class FileHelper
    {
		public static void UploadDocument(ClientContext clientContext, string sourceFilePath, string serverRelativeDestinationPath)
		{
			using (var fs = new FileStream(sourceFilePath, FileMode.Open))
			{
					var fi = new FileInfo(sourceFilePath);
				Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, serverRelativeDestinationPath , fs, true);
			}
		}

		public static void UploadFolder(ClientContext clientContext, System.IO.DirectoryInfo folderInfo, Folder folder)
		{
			System.IO.FileInfo[] files = null;
			System.IO.DirectoryInfo[] subDirs = null;

			try
			{
					files = folderInfo.GetFiles("*.*");
			}
			catch (UnauthorizedAccessException e)
			{
					System.Console.WriteLine(e.Message);
			}

			catch (System.IO.DirectoryNotFoundException e)
			{
					System.Console.WriteLine(e.Message);
			}

			if (files != null)
			{
					foreach (System.IO.FileInfo fi in files)
					{
						System.Console.WriteLine(fi.FullName);
						clientContext.Load(folder);
						clientContext.ExecuteQuery();
						UploadDocument(clientContext, fi.FullName, folder.ServerRelativeUrl + "/" + fi.Name);
					}

					subDirs = folderInfo.GetDirectories();

					foreach (System.IO.DirectoryInfo dirInfo in subDirs)
					{
						Folder subFolder = folder.Folders.Add(dirInfo.Name);
						clientContext.ExecuteQuery();
						UploadFolder(clientContext, dirInfo, subFolder);
					}
			}
		}

		public static void UploadFoldersRecursively(ClientContext clientContext, string sourceFolder, string destinationLigraryTitle)
		{
			Web web = clientContext.Web;
			var query = clientContext.LoadQuery(web.Lists.Where(p => p.Title == destinationLigraryTitle));
			clientContext.ExecuteQuery();
			List documentsLibrary = query.FirstOrDefault();
			var folder = documentsLibrary.RootFolder;
			System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sourceFolder);

			clientContext.Load(documentsLibrary.RootFolder);
			clientContext.ExecuteQuery();

			folder = documentsLibrary.RootFolder.Folders.Add(di.Name);
			clientContext.ExecuteQuery();

			FileHelper.UploadFolder(clientContext, di, folder);
		}

		public static void DownloadLibrary(ClientContext clientContext, string LibraryTitle, string destinationFolder)
		{
			var web = clientContext.Web;
			clientContext.Load(web);
			clientContext.ExecuteQuery();
			List list = web.Lists.GetByTitle(LibraryTitle);
			clientContext.Load(list);
			clientContext.ExecuteQuery();
			clientContext.Load(list.RootFolder);
			clientContext.ExecuteQuery();
			clientContext.Load(list.RootFolder.Folders);
			clientContext.ExecuteQuery();
			DownloadFilesFromFolder(clientContext, list.RootFolder, destinationFolder);
		}

		public static void DownloadFilesFromFolder(ClientContext clientContext, Folder folder, string destinationFolder)
		{
			string siteUrl = (new Uri(clientContext.Web.Url)).PathAndQuery;
			string localDestinationFolder = destinationFolder + "/" + folder.ServerRelativeUrl.Replace(siteUrl, string.Empty);
			localDestinationFolder = (new Uri(localDestinationFolder)).LocalPath;
			if (!Directory.Exists(localDestinationFolder))
			{
				Directory.CreateDirectory(localDestinationFolder);
				System.Console.WriteLine("Created Folder: {0}", localDestinationFolder);
			}

			clientContext.Load(folder.Files);
			clientContext.ExecuteQuery();
			foreach (Microsoft.SharePoint.Client.File file in folder.Files)
			{
            Stream fs = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, file.ServerRelativeUrl).Stream;
				byte[] binary = getByteArray(fs);
				string destinationFileName = localDestinationFolder + "/" + file.Name;
            FileStream stream = new FileStream(destinationFileName, FileMode.Create);
				BinaryWriter writer = new BinaryWriter(stream);
				writer.Write(binary);
				writer.Close();

				System.Console.WriteLine("Downloaded file: {0}", destinationFileName);
			}
			clientContext.Load(folder.Folders);
			clientContext.ExecuteQuery();
			foreach (Folder subFolder in folder.Folders)
			{
				DownloadFilesFromFolder(clientContext, subFolder, destinationFolder);
			}
		}

		public static byte[] getByteArray(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					 "Source directory does not exist or could not be found: "
					 + sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}
}
