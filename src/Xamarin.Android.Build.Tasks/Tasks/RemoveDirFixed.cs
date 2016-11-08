﻿//
// RemoveDir.cs: Removes a directory.
//
// Author:
//   Marek Sieradzki (marek.sieradzki@gmail.com)
// 
// (C) 2005 Marek Sieradzki
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

// The RemoveDir task that ships with Mono 2.10 doesn't work for
// recursive directories, so we ship this one until Mono 2.12 is
// the norm.
namespace Xamarin.Android.Tasks {
	public class RemoveDirFixed : Task {
	
		ITaskItem[]	directories;
		ITaskItem[]	removedDirectories;

		public RemoveDirFixed ()
		{
		}

		public override bool Execute ()
		{
			if (directories.Length == 0)
				return true;

			List <ITaskItem> temporaryRemovedDirectories = new List <ITaskItem> ();
			
			foreach (ITaskItem directory in directories) {
				try {
					try {
						// try to do a normal "fast" delete of the directory.
						Directory.Delete (directory.GetMetadata ("FullPath"), true);
						temporaryRemovedDirectories.Add (directory);
					} catch {
						// if that fails we probably have readonly files (or locked files)
						// so try to make them writable and try again.
						MonoAndroidHelper.SetDirectoryWriteable (directory.GetMetadata ("FullPath"));
						Directory.Delete (directory.GetMetadata ("FullPath"), true);
						temporaryRemovedDirectories.Add (directory);
					}
				}
				catch (DirectoryNotFoundException ex) {
					Log.LogErrorFromException (ex);
				}
				catch (PathTooLongException ex) {
					Log.LogErrorFromException (ex);
				}
				catch (ArgumentNullException ex) {
					Log.LogErrorFromException (ex);
				}
				catch (ArgumentException ex) {
					Log.LogErrorFromException (ex);
				}
				catch (IOException ex) {
					Log.LogErrorFromException (ex);
				}
				catch (SecurityException ex) {
					Log.LogErrorFromException (ex);
				}
				catch (Exception ex) {
					Log.LogErrorFromException (ex);
				}
			}
			
			removedDirectories = temporaryRemovedDirectories.ToArray ();

			Log.LogDebugTaskItems ("  RemovedDirectories: ", RemovedDirectories);
			
			return true;
		}
		
		[Required]
		public ITaskItem[] Directories {
			get {
				return directories;
			}
			set {
				directories = value;
			}
		}

		[Output]
		public ITaskItem[] RemovedDirectories {
			get {
				return removedDirectories;
			}
			set {
				removedDirectories = value;
			}
		}
	}
}
