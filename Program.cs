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

namespace Bricksoft.Developer.DosToys.FastFileCopy
{
	/// <summary>
	/// Default class.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main( string[] args )
		{
			fcopy cmd = new fcopy(args);
			return cmd.Run();
		}
	}
}
