using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction2 {

	public interface IIndexable<T> {
		T this[int idx] { get; set; }
	}

	public struct ArrayStruct<T> : IIndexable<T> {
		public T[] arr;
		public T this[int idx] { get { return arr[idx]; } set { arr[idx] = value; } }
	}

	public interface IForLoopJob<T, U, V, W> where T : IIndexable<V>
																					 where U : IIndexable<W> {
		T array0 { get; }
		U array1 { get; }

		void PerformJob(T array0, U array1, int startIdx, int endIdx);		
	}

	public struct CopyFloatsJobStruct : IForLoopJob<ArrayStruct<float>,
																									ArrayStruct<float>,
																									float, float> {

		public ArrayStruct<float> inFloats;
		public ArrayStruct<float> outFloats;

		public ArrayStruct<float> array0 { get { return inFloats; } }
		public ArrayStruct<float> array1 { get { return outFloats; } }

		public void PerformJob(ArrayStruct<float> array0,
														ArrayStruct<float> array1,
														int startIdx, int endIdx) {
			unsafe {
				for (int i = startIdx; i < endIdx; i++) {
					array1[i] = array0[i];
				}
			}
		}
	}
  
}