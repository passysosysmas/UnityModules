using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Leap.Unity.Interaction2 {

	public static class WorkerUtil {

		public delegate void WorkerJobFunc<TArg, TResult>(TArg jobArg,
																											ref TResult resultArg);

		public static void Do<TArg, TResult>(WorkerJobFunc<TArg, TResult> jobFunc, TArg jobArg,
																				 ref TResult resultArg, Action onComplete) {
			var worker = getWorker();

			var workerJob = new WorkerJob<TArg, TResult>() {
				worker     = worker,
				jobFunc    = jobFunc,
				jobArg     = jobArg,
				onComplete = onComplete,
				resultArg  = resultArg
			};

			worker.DoWork += doWorkerJob<TArg, TResult>;
			worker.RunWorkerCompleted += onWorkerComplete<TResult>;
			worker.RunWorkerAsync(workerJob);
		}

		#region Worker Jobs

		private struct WorkerJob<TArg, TResult> {
			public BackgroundWorker    				  worker;
			public WorkerJobFunc<TArg, TResult> jobFunc;
			public TArg                   			jobArg;
			public Action           						onComplete;
			public TResult 							     		resultArg;
		}

		private struct WorkerResult<TResult> {
			public TResult         jobResult;
			public Action 				 onComplete;
		}

		private static void doWorkerJob<TArg, TResult>(object sender, DoWorkEventArgs e) {
			var job = (WorkerJob<TArg, TResult>)(e.Argument);

			job.jobFunc(job.jobArg, ref job.resultArg);

			var workerResult = new WorkerResult<TResult>() {
				jobResult = job.resultArg,
				onComplete = job.onComplete
			};
			
			e.Result = workerResult;
		}
		
		private static void onWorkerComplete<TResult>(object sender, RunWorkerCompletedEventArgs e) {
			var onComplete = ((WorkerResult<TResult>)e.Result).onComplete;
			onComplete();
		}

		#endregion

		#region Worker Management
		
		private static Queue<BackgroundWorker> _freeWorkers = new Queue<BackgroundWorker>(32);
		private static Queue<BackgroundWorker> _busyWorkers = new Queue<BackgroundWorker>(32);

		private static BackgroundWorker getWorker() {
			if (_freeWorkers.Count > 0) {
				return _freeWorkers.Dequeue();
			}
			else {
				return new BackgroundWorker();
			}
		}

		private static void returnWorker(BackgroundWorker worker) {
			_freeWorkers.Enqueue(worker);
		}

		#endregion

	}

}
