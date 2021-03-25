using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using devTools.WiXComponents.Core.Commands;
using essentialMix.Extensions;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace devTools.WiXComponents.Core.ViewModels
{
	public abstract class ViewModelCancellableCommandBase : ViewModelCommandBase, IResettableView
	{
		private const int CTO_MIN = 0;
		private const int CTO_MAX = 3000;
		private const int CTO_MIN_WAIT = 20;

		private CancellationTokenSource _cancellationTokenSource;
		private int _cancellationTimeout;
		private volatile int _isBusy;

		/// <inheritdoc />
		protected ViewModelCancellableCommandBase(ILogger logger)
			: base(logger)
		{
			_cancellationTimeout = TimeSpanHelper.HALF_SCHEDULE;
			CancelCommand = new RelayCommand<ViewModelCancellableCommandBase>(vm => vm.Cancel(), vm => !vm.IsCancellationRequested);
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposing) Stop();
			base.Dispose(disposing);
		}

		public bool IsBusy
		{
			get => _isBusy != 0;
			protected set
			{
				Interlocked.CompareExchange(ref _isBusy, value
															? 1
															: 0, _isBusy);
				OnPropertyChanged();
			}
		}

		public int CancellationTimeout
		{
			get => _cancellationTimeout;
			set
			{
				if (_cancellationTimeout == value) return;
				_cancellationTimeout = value.Within(CTO_MIN, CTO_MAX);
				OnPropertyChanged();
			}
		}

		public CancellationTokenSource CancellationTokenSource
		{
			get => _cancellationTokenSource;
			set
			{
				_cancellationTokenSource = value;
				Token = _cancellationTokenSource?.Token ?? CancellationToken.None;
			}
		}

		public CancellationToken Token { get; private set; }

		public bool IsCancellationRequested => Token.CanBeCanceled && Token.IsCancellationRequested;

		[NotNull]
		public abstract ICommand StartCommand { get; }

		[NotNull]
		public ICommand CancelCommand { get; }

		public virtual void Cancel()
		{
			CancellationTokenSource.CancelIfNotDisposed();
		}

		public abstract void Reset();

		protected virtual void Prepare()
		{
			if (IsBusy)
			{
				Stop();
				if (IsBusy) throw new TimeoutException("Could not start the operation.");
			}

			IsBusy = true;
		}

		protected virtual void Stop()
		{
			if (!Token.CanBeCanceled) return;
			Cancel();

			int cto = CancellationTimeout;
			if (cto > CTO_MIN_WAIT && IsBusy) SpinWait.SpinUntil(() => !IsBusy, cto);
			Debug.Assert(!IsBusy, "Could not stop the current operation!");
		}
	}
}