﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Igor.BillScanner.Core {

	/// <summary>
	/// Class containing useful helper functionality for the application
	/// </summary>
	public static class WPFHelper {

		/// <summary>
		/// Returns an absolute path to this application's path inside the %appdata% (Roaming) folder
		/// </summary>
		public static string dataPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "BillScanner" + Path.DirectorySeparatorChar; } }

		/// <summary>
		/// Name of the database
		/// </summary>
		public static string databaseFileName => "ShoppingDB.db";

		/// <summary>
		/// Returns an absolute path to Resources inside the executable
		/// </summary>
		public static string resourcesPath { get { return @"pack://application:,,,/Igor.BillScanner.WPF.UI;component/Resources/"; } }

		/// <summary>
		/// Uses <see cref="resourcesPath"/> and adds part of the path to point into <see cref="MatchRating"/> images directory
		/// </summary>
		public static string imageRatingResourcesPath { get { return resourcesPath + "MatchRating/"; } }

		/// <summary>
		/// Helper function to merge a list of elements into one <see cref="string"/> separated by a <see cref="char"/> calls the <see cref="object.ToString()"/> function
		/// </summary>
		/// <typeparam name="T">Type of the data to merge</typeparam>
		/// <param name="list">List containing the data to merge</param>
		/// <param name="connector">A character to put between two elements of the list</param>
		public static string Merge<T>(this List<T> list, char connector) {
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < list.Count; i++) {
				builder.Append(list[i].ToString());
				builder.Append(connector + " ");
			}
			if (list.Count > 0) {
				builder.Remove(builder.Length - 2, 2);
			}
			return builder.ToString();
		}
	}
}
