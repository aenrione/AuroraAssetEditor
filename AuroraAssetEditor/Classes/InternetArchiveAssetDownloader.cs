//
// 	InternetArchiveDownloader.cs
// 	AuroraAssetEditor
//
// 	Created by aenrione
// 	Copyright (c) 2015 Swizzy. All rights reserved.

namespace AuroraAssetEditor.Classes
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;

	internal class InternetArchiveDownloader
	{
		private const string BaseUrl = "https://archive.org/download/xboxunity-covers-fulldump_202311/xboxunity-covers-fulldump/";
		public static EventHandler<StatusArgs> StatusChanged;

		internal static void SendStatusChanged(string msg)
		{
			var handler = StatusChanged;
			if (handler != null)
				handler.Invoke(null, new StatusArgs(msg));
		}

		// Get the list of subdirectories for a title ID
		public InternetArchiveAsset[] GetTitleInfo(uint titleId)
		{
			string titleFolder = string.Format("{0:X08}", titleId);
			List<InternetArchiveAsset> assets = new List<InternetArchiveAsset>();

			try
			{
				using (WebClient client = new WebClient())
				{
					string folderUrl = $"{BaseUrl}{titleFolder}/";
					string htmlContent = client.DownloadString(folderUrl);

					// Parse directory links from HTML
					foreach (string subDir in ParseDirectoriesFromHtml(htmlContent))
					{
						assets.Add(new InternetArchiveAsset
						{
							TitleId = titleId,
							MainFolder = titleFolder,
							SubFolder = subDir,
							AssetType = "Cover"
						});
					}
				}
			}
			catch (Exception ex)
			{
				SendStatusChanged($"Error fetching directory: {ex.Message}");
			}

			return assets.ToArray();
		}

		private IEnumerable<string> ParseDirectoriesFromHtml(string html)
		{
			List<string> directories = new List<string>();
			int currentIndex = 0;

			while (true)
			{
				int startAnchor = html.IndexOf("<a href=\"", currentIndex);
				if (startAnchor == -1) break;

				int startHref = startAnchor + 9; // length of '<a href="'
				int endHref = html.IndexOf("\"", startHref);
				if (endHref == -1) break;

				string href = html.Substring(startHref, endHref - startHref);

				// Skip parent directory link and process only subdirectories
				if (href != "../" && href.EndsWith("/"))
				{
					// Remove trailing slash
					directories.Add(href.TrimEnd('/'));
				}

				currentIndex = endHref + 1;
			}

			return directories;
		}

		// Method to download the actual image when selected
		public Image DownloadCover(InternetArchiveAsset asset)
		{
			try
			{
				string coverUrl = $"{BaseUrl}{asset.MainFolder}/{asset.SubFolder}/boxart.png";
				using (WebClient wc = new WebClient())
				{
					byte[] imageData = wc.DownloadData(coverUrl);

					// Validate that we actually got image data
					if (imageData == null || imageData.Length == 0)
					{
						SendStatusChanged("No image data received");
						return null;
					}

					// Create a new MemoryStream and keep it open
					MemoryStream ms = new MemoryStream(imageData);

					try
					{
						// Create image from stream and clone it to ensure we have a fresh copy
						using (Image originalImage = Image.FromStream(ms, true, true))
						{
							// Create a new bitmap from the original image
							return new Bitmap(originalImage);
						}
					}
					finally
					{
						ms.Dispose();
					}
				}
			}
			catch (WebException webEx)
			{
				SendStatusChanged($"Network error downloading cover: {webEx.Message}");
				return null;
			}
			catch (ArgumentException argEx)
			{
				SendStatusChanged($"Invalid image data received: {argEx.Message}");
				return null;
			}
			catch (Exception ex)
			{
				SendStatusChanged($"Error downloading cover: {ex.Message}");
				return null;
			}
		}

	}

	// Modified asset class to handle the nested structure
	public class InternetArchiveAsset
	{
		private Image _cover;
		public uint TitleId { get; set; }
		public string MainFolder { get; set; }  // The title ID folder
		public string SubFolder { get; set; }   // The subfolder containing the actual cover
		public string AssetType { get; set; }
		public bool HaveAsset { get { return _cover != null; } }

		public Image GetCover()
		{
			if (_cover == null)
			{
				var downloader = new InternetArchiveDownloader();
				var downloadedCover = downloader.DownloadCover(this);
				if (downloadedCover != null)
				{
					_cover = downloadedCover;
				}
				else
				{
					return null;
				}
			}
			return _cover;
		}

		public override string ToString()
		{
			return $"TitleID: {TitleId:X8} - Variant: {SubFolder}";
		}
	}

}
