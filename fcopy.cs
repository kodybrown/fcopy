//
// Copyright (C) 2003-2006 Bricksoft.com.
// All Rights Reserved.
//
// Bricksoft.com PowerTools
//
// This program is unpublished proprietary source code of Bricksoft.com.
// Your use of this code is limited to those rights granted in the license between you and Bricksoft.com.
//
// Author: Kody Brown (kody@bricksoft.com)
//

using System;
using System.Diagnostics;
using System.IO;
using Bricksoft.PowerCode;

namespace Bricksoft.Developer.DosToys.FastFileCopy
{
	/// <summary>
	/// Summary description for fcopy.
	/// </summary>
	public class fcopy
	{
		private const int INVALIDARGUMENTS = 1;

		private CommandLineArguments args = null;
		//private string[] cmdLine = new string[] { };

		private static string source = "";
		private string destination = "";
		private string wildcard = "";

		private bool testOnly = false;
		private bool showReason = false;
		private bool verbose = false;
		private bool forceAllChangedFiles = false;
		private bool pause = false;
		private bool recurse = false;
		private bool runSilent = false;
		private bool showProgress = false;

		private bool bIgnoreDateTime = false;
		private bool bDoOnlyDateTime = false;
		private bool bIgnoreSize = false;
		private bool bDoOnlySize = false;
		private bool bIgnoreVersion = false;
		private bool bDoOnlyVersion = false;

		public fcopy( string[] arguments )
		{
			args = new CommandLineArguments(arguments);
			if (args.Contains("debug")) {
				System.Threading.Thread.Sleep(10000);
			}
			//cmdLine = arguments;
		}

		public int Run()
		{
			#region Get the command-line arguments into local variables

			if (args.Contains("?") || args.Contains("h") || args.Contains("help") || args.Count == 0) {
				printUsage();
				return 0;
			} else if (2 > args.Count) {
				Console.WriteLine("Error: invalid arguments\r\n");
				printUsage();
				return INVALIDARGUMENTS;
			}

			// exclude file date/time check
			if (args.Contains("xd") || args.Contains("xdatetime")) {
				bIgnoreDateTime = true;
			}
			// only perform file date/time check
			if (args.Contains("d") || args.Contains("datetime")) {
				bDoOnlyDateTime = true;
				bDoOnlySize = false;
				bDoOnlyVersion = false;
			}

			// exclude file size check
			if (args.Contains("xs") || args.Contains("xsize")) {
				bIgnoreSize = true;
			}
			// only perform file size check
			if (args.Contains("s") || args.Contains("size")) {
				bDoOnlyDateTime = false;
				bDoOnlySize = true;
				bDoOnlyVersion = false;
			}

			// exclude file version check
			if (args.Contains("xv") || args.Contains("xversion")) {
				bIgnoreVersion = true;
			}
			// only perform file version check
			if (args.Contains("v") || args.Contains("version")) {
				bDoOnlyDateTime = false;
				bDoOnlySize = false;
				bDoOnlyVersion = true;
			}

			if (args.Contains("verbose")) {
				verbose = true;
			}

			// force all different files
			if (args.Contains("f") || args.Contains("force")) {
				forceAllChangedFiles = true;
			}

			if (args.Contains("p") || args.Contains("pause")) {
				pause = true;
			}

			if (args.Contains("recurse")) {
				recurse = true;
			}

			// hidden options!!
			if (args.Contains("t") || args.Contains("test")) { // test run: do everything except actually copying the file
				testOnly = true;
			}

			if (args.Contains("r") || args.Contains("reason")) { // shows why a file was copied
				showReason = true;
				verbose = true;
			}

			if (bDoOnlyDateTime || bDoOnlySize || bDoOnlyVersion)
				forceAllChangedFiles = true;

			// shows progress (cannot be used in visual studio buildevents)
			if (args.Contains("progress")) {
				showProgress = true;
				verbose = true;
			}

			// silent. no output
			if (args.Contains("silent")) {
				runSilent = true;
				verbose = false;
			}

			#endregion

			#region Get the source folder

			wildcard = "*.*";

			if (args.Contains("source")) {
				source = args.GetString(string.Empty, "source");
			} else {
				Console.WriteLine("invalid or missing source folder\r\n");
				printUsage();
				return INVALIDARGUMENTS;
			}

			if (source.IndexOf("*") > -1 || source.IndexOf("?") > -1) {
				// has a wildcard
				wildcard = source.Substring(source.LastIndexOf("\\") + 1);
				source = source.Substring(0, source.LastIndexOf("\\"));
			} else {
				if (!Directory.Exists(source)) {
					if (File.Exists(source)) {
						wildcard = source.Substring(source.LastIndexOf("\\") + 1);
						source = source.Substring(0, source.LastIndexOf("\\"));
					} else {
						Console.WriteLine("invalid source folder|file|wildcard: {0}\r\n", source);
						printUsage();
						return INVALIDARGUMENTS;
					}
				}
			}

			if (source.EndsWith("\\")) {
				source = source.Substring(0, source.Length - 1);
			}

			#endregion

			#region Get the destination folder

			if (args.Contains("dest")) {
				destination = args.GetString(string.Empty, "dest");
			} else if (args.Contains("destination")) {
				destination = args.GetString(string.Empty, "destination");
			} else {
				Console.WriteLine("invalid or missing destination folder\r\nnote: if your destination folder ends with a \\\", you must use \\\\\"");
				printUsage();
				return INVALIDARGUMENTS;
			}

			if (destination.EndsWith("\\")) {
				destination = destination.Substring(0, destination.Length - 1);
			}

			if (!Directory.Exists(destination)) {
				Console.WriteLine("invalid destination folder: {0}\r\n", destination);
				printUsage();
				return INVALIDARGUMENTS;
			}

			#endregion

			#region Output what it will do

			if (!runSilent) {
				Console.WriteLine("fcopy.exe, beta version 0.93");
			}
			if (verbose && !runSilent) {
				Console.WriteLine("Bricksoft Developer DosToys, http://www.bricksoft.com/");
			}
			if (!runSilent) {
				Console.WriteLine("Copyright (c) 2006-2012 Bricksoft.com. All Rights Reserved.\r\n");
			}

			if (verbose && !runSilent) {
				if (testOnly) {
					Console.WriteLine("/t  : do everything but actually copying the file(s)");
				}
				if (showReason) {
					Console.WriteLine("/r  : shows why a file was copied (applies /v)");
				}

				if (forceAllChangedFiles) {
					Console.WriteLine("/f  : forcing all changed files");
				}

				if (!bIgnoreDateTime && !bDoOnlySize && !bDoOnlyVersion) {
					Console.WriteLine("    : comparing date/time");
				}
				if (!bIgnoreSize && !bDoOnlyDateTime && !bDoOnlyVersion) {
					Console.WriteLine("    : comparing size");
				}
				if (!bIgnoreVersion && !bDoOnlyDateTime && !bDoOnlySize) {
					Console.WriteLine("    : comparing version");
				}
			}

			if (!runSilent) {
				Console.WriteLine("");
			}

			#endregion

			// 
			// Compare the folders
			// 

			int fileCount = 0;
			int copyCount = 0;
			int startPos = source.Length;
			string[] sourcefiles = new string[] { };
			decimal percVar = 0;

			if (recurse) {
				sourcefiles = Directory.GetFiles(source, wildcard, SearchOption.AllDirectories);
			} else {
				sourcefiles = Directory.GetFiles(source, wildcard, SearchOption.TopDirectoryOnly);
			}

			if (0 < sourcefiles.Length) {
				percVar = Math.Round(100 / (decimal)sourcefiles.Length, 3);
			}

			if (verbose && !runSilent) {
				Console.WriteLine("Comparing {0} files", sourcefiles.Length);
			}

			foreach (string fullFileName in sourcefiles) {
				FileInfo f1 = new FileInfo(fullFileName);
				FileInfo f2 = new FileInfo(Path.Combine(destination, fullFileName.Substring(startPos + 1)));

				string details = "";
				bool copyIt = CheckFile(f1, f2, ref details);

				if (copyIt) {
					if (!runSilent && (showReason || verbose)) {
						if (showProgress) {
							Console.CursorLeft = 0; // overwrite progress line
						}
						if (showReason) {
							Console.WriteLine("copying: {0} --> ({1})", f1.Name, details);
						} else if (verbose) {
							Console.WriteLine("copying: {0}", f1.Name);
						}
					}

					if (!testOnly) {
						File.Copy(f1.FullName, f2.FullName, true);
					}

					copyCount++;
				}

				fileCount++;

				if (showProgress && !runSilent) {
					Console.CursorLeft = 0;
					Console.Write("{0}% complete           ", Math.Min(Math.Round(fileCount * percVar, 2), 100));
					System.Threading.Thread.Sleep(1);
				}
			}

			if (showProgress && !runSilent) {
				Console.CursorLeft = 0;
				Console.WriteLine("100.00% complete           ");
				Console.WriteLine("Copied {0} file{1}..", copyCount, copyCount == 1 ? "" : "s");
			}

			if (pause && !runSilent) {
				Console.ReadKey(true);
			}

			return 0;
		}

		private bool CheckFile( FileInfo f1, FileInfo f2, ref string details )
		{
			bool copyIt = false;

			if (forceAllChangedFiles) {
				details += "+f";
			}

			if (f2.Exists) {
				if (!bIgnoreDateTime && !bDoOnlySize && !bDoOnlyVersion) {
					if (forceAllChangedFiles && f1.LastWriteTime != f2.LastWriteTime) {
						copyIt = true;
						details += "+d";
					} else if (f1.LastWriteTime > f2.LastWriteTime) {
						copyIt = true;
						details += "+d";
					}
				}

				if (!bIgnoreSize && !bDoOnlyDateTime && !bDoOnlyVersion) {
					if (forceAllChangedFiles && f1.Length != f2.Length) {
						copyIt = true;
						details += "+s";
					}
				}

				if (!bIgnoreVersion && !bDoOnlyDateTime && !bDoOnlySize) {
					if (f1.Extension.Equals(".exe") || f1.Extension.Equals(".dll")) {
						FileVersionInfo fv1 = FileVersionInfo.GetVersionInfo(f1.FullName);
						FileVersionInfo fv2 = FileVersionInfo.GetVersionInfo(f2.FullName);

						if (fv1.FileMajorPart > fv2.FileMajorPart) {
							copyIt = true;
							details += "+v1";
						} else if (forceAllChangedFiles && fv1.FileMajorPart != fv2.FileMajorPart) {
							copyIt = true;
						} else {
							if (fv1.FileMinorPart > fv2.FileMinorPart) {
								copyIt = true;
								details += "+v2";
							} else if (forceAllChangedFiles && fv1.FileMinorPart != fv2.FileMinorPart) {
								copyIt = true;
							} else {
								if (fv1.FileBuildPart > fv2.FileBuildPart) {
									copyIt = true;
									details += "+v3";
								} else if (forceAllChangedFiles && fv1.FileBuildPart != fv2.FileBuildPart) {
									copyIt = true;
								} else {
									if (fv1.FilePrivatePart > fv2.FilePrivatePart) {
										copyIt = true;
										details += "+v4";
									} else if (forceAllChangedFiles && fv1.FilePrivatePart != fv2.FilePrivatePart) {
										copyIt = true;
									} else {
										// file version is the same or older
									}
								}
							}
						}
					}
				}
			} else {
				// destination file does not exist
				copyIt = true;
				details += "+x";
			}

			return copyIt;
		}

		private static void printUsage()
		{
			Console.WriteLine("fcopy.exe, beta version 0.91");
			Console.WriteLine(" ");
			Console.WriteLine("Bricksoft Developer DosToys, http://www.bricksoft.com/");
			Console.WriteLine("Copyright (c) 2006 Bricksoft.com. All Rights Reserved.");
			Console.WriteLine(" ");
			Console.WriteLine("Fast File Copy");
			Console.WriteLine("       This will copy newer and/or missing files from the source folder");
			Console.WriteLine("       to the destination folder.");
			Console.WriteLine(" ");
			Console.WriteLine("Usage: fcopy -source folder[\\file|\\wildcard] -dest folder [options]");
			Console.WriteLine("       (be sure to use quotes (\") to enclose paths with spaces in it)");
			Console.WriteLine(" ");
			Console.WriteLine("general options:");
			Console.WriteLine("  /verbose  show detailed output");
			Console.WriteLine("  /force    force any changed file to be copied");
			Console.WriteLine("  /pause    pause when finished");
			Console.WriteLine("  /test     do everything but actually copying the file");
			Console.WriteLine("  /reason   explains why the file was copied");
			Console.WriteLine("  /silent   does not produce any output, overrides all others");
			Console.WriteLine(" ");
			Console.WriteLine("filter options:");
			Console.WriteLine("  -xd  don't compare file date/time stamps");
			Console.WriteLine("  -d   compare only file date/time stamps (excludes other filters and applies /f)");
			Console.WriteLine("  -xs  don't compare file size");
			Console.WriteLine("  -s   compare only file size (excludes other filters and applies /f)");
			Console.WriteLine("  -xv  don't compare file version");
			Console.WriteLine("  -v   compare only file version (excludes other filters and applies /f)");
		}

	}

}

