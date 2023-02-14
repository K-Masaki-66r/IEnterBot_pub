using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEnterBot
{
	internal class MutexWrapper : IDisposable
	{
		private Mutex _mutex;
		private CountdownEvent _waitEndEvent = new(1);
		private CountdownEvent _releaseEvent = new(1);
		private bool _waitResult = false;
		private bool _disposedValue = false;

		internal MutexWrapper(bool initiallyOwned, string name)
		{
			_mutex = new(initiallyOwned, name);
		}

		internal virtual bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			return _mutex.WaitOne(millisecondsTimeout, exitContext);
		}

		internal virtual Task<bool> WaitOneAsync(int millisecondsTimeout, bool exitContext)
		{
			Task.Factory.StartNew(() => {
				MutexControlTask(millisecondsTimeout, exitContext);
			}, TaskCreationOptions.LongRunning);

			return Task.Factory.StartNew(() => {
				// 取得完了まで待受
				_waitEndEvent.Wait();
				_waitEndEvent.Reset();
				return _waitResult;
			}, TaskCreationOptions.LongRunning);
		}

		private void MutexControlTask(int millisecondsTimeout, bool exitContext)
		{
			// Mutex取得開始
			_waitResult = _mutex.WaitOne(millisecondsTimeout, exitContext);
			_waitEndEvent.Signal();

			// Mutexの開放まで待機
			_releaseEvent.Wait();
			_releaseEvent.Dispose();

			// Mutex開放
			try {
				_mutex.ReleaseMutex();
			} catch(Exception e) {
				MessageBox.Show(e.Message);
			} finally {
				_mutex.Dispose();
				_mutex = null;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if(!_disposedValue) {
				_releaseEvent.Signal();
				_waitEndEvent.Dispose();
				_disposedValue = true;
			}
		}

		~MutexWrapper()
		{
			Dispose(false);
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
