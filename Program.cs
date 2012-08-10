///////////////////////////////////////////////////////////////////////////////
//
//	Copyright (C) 2003-2006 Bricksoft.com
//
//	Bricksoft.com Developer PowerCode
//
//	This source code contains information, which is the proprietary property of
//	Bricksoft.com.  This source code is received in confidence and its
//	contents cannot be disclosed or copied without the prior written consent of
//	Bricksoft.com.
//
//	Nothing in this source code constitutes a guaranty, warranty, or license,
//	express or implied. Bricksoft.com disclaims all liability for all
//	such guaranties, warranties, and licenses, including but not limited to: 
//	Fitness for a particular purpose; merchantability; not infringement of
//	intellectual property or other rights of any third party or of
//	Bricksoft.com; indemnity; and all others.  The reader is advised
//	that third parties can have intellectual property rights that can be
//	relevant to this source code and the technologies discussed herein, and is
//	advised to seek the advice of competent legal counsel, without obligation
//	of Bricksoft.com.
//
//	Bricksoft.com retains the right to make changes to this source
//	code at any time, without notice.  Bricksoft.com makes no warranty
//	for the use of this source code and assumes no responsibility for any
//	errors that can appear in the source code nor does it make a commitment to
//	update the information contained herein.
//
///////////////////////////////////////////////////////////////////////////////

using System;

namespace Bricksoft.Developer.DosToys.FastFileCopy {
	/// <summary>
	/// Default class.
	/// </summary>
	public class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args) {
			fcopy cmd = new fcopy(args);
			return cmd.Run();
		}
	}
}
