using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class IERust {

	#region DLL Imports
	
	[DllImport("libierust")]
	public static extern void process();

	[DllImport("libierust")]
	private static extern void send_some_bytes(IntPtr bytes, int size);

	[DllImport("libierust")]
	private static extern void make_float32s_negative(IntPtr float32s, int size);
	
	[DllImport("libierust")]
	private static extern void set_debug_callback(IntPtr callbackPtr);

	#endregion

	#region Debugging Support

  /// <summary>
	/// This delegate defines the type signature of ierustDebugLog, a
	/// pointer to which is passed to IERust so that the unmanaged code
	/// can pass UTF-8 byte array debugging messages into Unity at
	/// any time.
	/// </summary>
	private delegate void IERustDebugLog(IntPtr byteChars, int size);

	/// <summary>
	/// References the debug logging function delegate that is passed
	/// as an IntPtr to IERust.
	/// 
	/// Maintaining this managed reference prevents the GC
  /// from collecting the delegate.
	/// </summary>
	private static IERustDebugLog ierustDebugLogFunc;

	/// <summary>
	/// Interprets the provided pointer and size as a series of 8-bit
	/// UTF-8 characters and prints the result via UnityEngine.Debug.Log.
	/// 
	/// This method allocates garbage. It's for debugging only.
	/// </summary>
	private static void ierustDebugLog(IntPtr byteChars, int size) {
		byte[] bytes = new byte[size];
		Marshal.Copy(byteChars, bytes, 0, size);
		Debug.Log(System.Text.Encoding.UTF8.GetString(bytes));
	}

	#endregion

	static IERust() {
		// Debugging support.
		ierustDebugLogFunc = ierustDebugLog;
		IntPtr logFuncPtr = Marshal.GetFunctionPointerForDelegate(ierustDebugLogFunc);
		set_debug_callback(logFuncPtr);
	}

	/// <summary>
	/// Fills a byte array with the corresponding array indices. This
	/// method merely demonstrates that it's possible to receive data
	/// from native Rust.
	/// </summary>
	public static void FillBytes(ref byte[] bytes) {
	  unsafe {
			fixed (byte* p = bytes) {
				IntPtr ptr = (IntPtr)p;
				send_some_bytes(ptr, bytes.Length);	
			}
		}
	}

  /// <summary>
	/// Flips the sign of each float in the argument array. This method
	/// demonstrates that it's possible to read values from C# to Rust,
	/// then modify and return data based on those values.
	/// </summary>
	public static void MakeFloatsNegative(ref float[] floats) {
		unsafe {
			fixed (float* p = floats) {
				IntPtr ptr = (IntPtr)p;
				make_float32s_negative(ptr, floats.Length);
			}
		}
	}

}
