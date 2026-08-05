using System.Collections.Generic;
using System.Threading.Tasks;

public class JobManagerBase<TBatch, T> where TBatch : Batch<T>, new()
{
	protected readonly List<TBatch> _batches_active = new List<TBatch>();

	private readonly Stack<TBatch> _batches_free = new Stack<TBatch>();

	public string id;

	public string benchmark_id;

	private Dictionary<string, double> _dict_benchmark_time = new Dictionary<string, double>();

	private Dictionary<string, int> _dict_benchmark_counter = new Dictionary<string, int>();

	public JobManagerBase(string pID)
	{
		id = pID;
	}

	public void updateBase(float pElapsed)
	{
		clearJobBenchmarks();
		updateBaseJobsPre(pElapsed);
		updateBaseJobsParallel(pElapsed);
		updateBaseJobsPost(pElapsed);
		saveJobBenchmarks();
	}

	private void clearJobBenchmarks()
	{
		if (!Bench.bench_enabled)
		{
			return;
		}
		for (int i = 0; i < _batches_active.Count; i++)
		{
			TBatch tBatch = _batches_active[i];
			for (int j = 0; j < tBatch.jobs_pre.Count; j++)
			{
				Job<T> job = tBatch.jobs_pre[j];
				job.time_benchmark = 0.0;
				job.counter = 0;
			}
			for (int k = 0; k < tBatch.jobs_post.Count; k++)
			{
				Job<T> job2 = tBatch.jobs_post[k];
				job2.time_benchmark = 0.0;
				job2.counter = 0;
			}
		}
	}

	private void saveJobBenchmarks()
	{
		if (!Bench.bench_enabled)
		{
			return;
		}
		_dict_benchmark_time.Clear();
		_dict_benchmark_counter.Clear();
		for (int i = 0; i < _batches_active.Count; i++)
		{
			TBatch tBatch = _batches_active[i];
			checkListForBenchmark(tBatch.jobs_pre);
			checkListForBenchmark(tBatch.jobs_post);
		}
		foreach (KeyValuePair<string, double> item in _dict_benchmark_time)
		{
			item.Deconstruct(out var key, out var value);
			string tID = key;
			double tTime = value;
			int tCounter = _dict_benchmark_counter[tID];
			Bench.benchSave(tID, tTime, tCounter, benchmark_id);
			Bench.saveAverageCounter(tID, benchmark_id);
		}
	}

	private void checkListForBenchmark(List<Job<T>> pList)
	{
		for (int j = 0; j < pList.Count; j++)
		{
			Job<T> tJob = pList[j];
			if (!_dict_benchmark_time.ContainsKey(tJob.id))
			{
				_dict_benchmark_time.Add(tJob.id, 0.0);
				_dict_benchmark_counter.Add(tJob.id, 0);
			}
			_dict_benchmark_time[tJob.id] += tJob.time_benchmark;
			_dict_benchmark_counter[tJob.id] += tJob.counter;
		}
	}

	internal void updateBaseJobsPre(float pElapsed)
	{
		for (int i = 0; i < _batches_active.Count; i++)
		{
			_batches_active[i].updateJobsPre(pElapsed);
		}
	}

	internal void updateBaseJobsPost(float pElapsed)
	{
		for (int i = 0; i < _batches_active.Count; i++)
		{
			_batches_active[i].updateJobsPost(pElapsed);
		}
	}

	internal void updateBaseJobsParallel(float pElapsed)
	{
		clearParallelResults();
		Bench.bench("update_jobs_parallel", benchmark_id);
		if (Config.parallel_jobs_updater)
		{
			Parallel.ForEach(_batches_active, World.world.parallel_options, delegate(TBatch pBatch)
			{
				pBatch.updateJobsParallel(pElapsed);
			});
		}
		else
		{
			List<TBatch> tBatches = _batches_active;
			int tCount = tBatches.Count;
			for (int i = 0; i < tCount; i++)
			{
				tBatches[i].updateJobsParallel(pElapsed);
			}
		}
		Bench.benchEnd("update_jobs_parallel", benchmark_id, pSaveCounter: false, 0L);
		applyParallelResults();
	}

	internal void clearParallelResults()
	{
		Bench.bench("clear_parallel_results", benchmark_id);
		for (int i = 0; i < _batches_active.Count; i++)
		{
			_batches_active[i].clearParallelResults?.Invoke();
		}
		Bench.benchEnd("clear_parallel_results", benchmark_id, pSaveCounter: false, 0L);
	}

	internal void applyParallelResults()
	{
		Bench.bench("apply_parallel_results", benchmark_id);
		for (int i = 0; i < _batches_active.Count; i++)
		{
			_batches_active[i].applyParallelResults?.Invoke();
		}
		Bench.benchEnd("apply_parallel_results", benchmark_id, pSaveCounter: false, 0L);
	}

	internal void removeObject(T pObject, TBatch pBatch)
	{
		pBatch.remove(pObject);
		checkFree(pBatch);
	}

	protected TBatch newBatch()
	{
		TBatch tBatch = new TBatch();
		_batches_active.Add(tBatch);
		return tBatch;
	}

	internal virtual void addNewObject(T pObject)
	{
		TBatch tBatch = getBatch();
		tBatch.add(pObject);
		tBatch.main.checkAddRemove();
		if (tBatch.main.Count >= JobConst.MAX_ELEMENTS)
		{
			tBatch.free_slots = false;
			_batches_free.Pop();
		}
	}

	internal TBatch getBatch()
	{
		if (_batches_free.Count == 0)
		{
			TBatch tNewBatch = newBatch();
			tNewBatch.batch_id = _batches_active.Count;
			makeFree(tNewBatch);
			return tNewBatch;
		}
		TBatch tBatch = _batches_free.Peek();
		if (tBatch.main.Count == 0)
		{
			_batches_active.Add(tBatch);
		}
		return tBatch;
	}

	protected void checkFree(TBatch pBatch)
	{
		pBatch.main.checkAddRemove();
		if (pBatch.main.Count < JobConst.MAX_ELEMENTS)
		{
			makeFree(pBatch);
		}
		if (pBatch.main.Count == 0)
		{
			_batches_active.Remove(pBatch);
		}
	}

	protected virtual void makeFree(TBatch pBatch)
	{
		if (!pBatch.free_slots)
		{
			pBatch.free_slots = true;
			_batches_free.Push(pBatch);
		}
	}

	internal void clear()
	{
		_batches_free.Clear();
		for (int i = 0; i < _batches_active.Count; i++)
		{
			TBatch tBatch = _batches_active[i];
			tBatch.clear();
			tBatch.free_slots = false;
			makeFree(tBatch);
		}
	}

	internal void clearHelperLists()
	{
		for (int i = 0; i < _batches_active.Count; i++)
		{
			_batches_active[i].clearHelperLists();
		}
	}

	public void debug(DebugTool pTool)
	{
		int tObjects = 0;
		for (int i = 0; i < _batches_active.Count; i++)
		{
			TBatch tBatch = _batches_active[i];
			tObjects += tBatch.main.Count;
		}
		pTool.setText("batches all", _batches_active.Count, 0f, pShowBar: false, 0L);
		pTool.setText("objects", tObjects, 0f, pShowBar: false, 0L);
		pTool.setSeparator();
		pTool.setText("parallel_jobs_updater_on", Config.parallel_jobs_updater, 0f, pShowBar: false, 0L);
	}

	public string debugBatchCount()
	{
		return _batches_active.Count + " / " + _batches_free.Count;
	}

	public string debugJobCount()
	{
		int tCount = 0;
		foreach (TBatch tBatch in _batches_active)
		{
			tCount += tBatch.jobs_post.Count;
			tCount += tBatch.jobs_pre.Count;
			tCount += tBatch.jobs_parallel.Count;
		}
		return tCount.ToString();
	}
}
