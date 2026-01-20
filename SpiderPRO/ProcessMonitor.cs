using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderPRO;

public class ProcessMonitor
{
	private readonly List<string> _processesToKill;

	private readonly List<string> _processPatterns;

	private CancellationTokenSource _cancellationTokenSource;

	private Task _monitoringTask;

	private bool _isMonitoring;

	public event EventHandler<ProcessKilledEventArgs> ProcessKilled;

	public ProcessMonitor()
	{
		_processesToKill = new List<string>
		{
			"lol"
		};
		_processPatterns = new List<string> { "*sigmo*" };
		_cancellationTokenSource = new CancellationTokenSource();
	}

	public void StartMonitoring()
	{
		if (!_isMonitoring)
		{
			_isMonitoring = true;
			_cancellationTokenSource = new CancellationTokenSource();
			_monitoringTask = Task.Run(async delegate
			{
				await MonitorProcessesAsync(_cancellationTokenSource.Token);
			});
			OnLogMessageReceived("\ud83d\udd12 Bypass Process is Ready");
		}
	}

	public void StopMonitoring()
	{
		if (_isMonitoring)
		{
			_isMonitoring = false;
			_cancellationTokenSource?.Cancel();
			try
			{
				_monitoringTask?.Wait(3000);
			}
			catch (AggregateException)
			{
			}
			OnLogMessageReceived("\ud83d\udd13 .. \ud83d\udd13");
		}
	}

	private async Task MonitorProcessesAsync(CancellationToken cancellationToken)
	{
		OnLogMessageReceived("\ud83d\udee1\ufe0f Background process protection activated");
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				CheckAndKillProcesses();
				await Task.Delay(2000, cancellationToken);
			}
			catch (TaskCanceledException)
			{
				break;
			}
			catch (Exception ex2)
			{
				OnLogMessageReceived("⚠\ufe0f Process monitor error: " + ex2.Message);
				await Task.Delay(5000, cancellationToken);
			}
		}
	}

	private void CheckAndKillProcesses()
	{
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			try
			{
				if (string.IsNullOrEmpty(process.ProcessName))
				{
					continue;
				}
				string processName = process.ProcessName.ToLower();
				bool shouldKill = _processesToKill.Any((string p) => processName.Equals(p, StringComparison.OrdinalIgnoreCase));
				if (!shouldKill)
				{
					shouldKill = _processPatterns.Any(delegate(string pattern)
					{
						string value = pattern.Replace("*", "").ToLower();
						return processName.Contains(value);
					});
				}
				if (!shouldKill)
				{
					shouldKill = IsSuspiciousProcess(process);
				}
				if (shouldKill)
				{
					KillProcess(process);
				}
			}
			catch (Exception)
			{
			}
		}
	}

	private bool IsSuspiciousProcess(Process process)
	{
		try
		{
			string processName = process.ProcessName.ToLower();
			if (new string[6] { "ollydbg", "x64dbg", "x32dbg", "windbg", "ida", "immunity" }.Any((string d) => processName.Contains(d)))
			{
				return true;
			}
			if (new string[5] { "proxyman", "zap", "mitm", "packet", "traffic" }.Any((string p) => processName.Contains(p)))
			{
				return true;
			}
			if (processName.Contains("tcp") && processName.Contains("view"))
			{
				return true;
			}
			if (processName.Contains("hook"))
			{
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private void KillProcess(Process process)
	{
		try
		{
			string processName = process.ProcessName;
			int processId = process.Id;
			if (!process.HasExited)
			{
				process.Kill();
				if (process.WaitForExit(3000))
				{
					OnProcessKilled(processName, processId, "Terminated");
					OnLogMessageReceived($"❌ Blocked: {processName} (PID: {processId})");
				}
				else
				{
					OnLogMessageReceived($"⚠\ufe0f Could not terminate: {processName} (PID: {processId})");
				}
			}
		}
		catch (Exception ex)
		{
			OnLogMessageReceived("⚠\ufe0f Failed to kill process " + process.ProcessName + ": " + ex.Message);
		}
	}

	public bool KillProcessByName(string processName)
	{
		try
		{
			Process[] processes = Process.GetProcessesByName(processName);
			if (processes.Length == 0)
			{
				return false;
			}
			bool killedAny = false;
			Process[] array = processes;
			foreach (Process process in array)
			{
				try
				{
					if (!process.HasExited)
					{
						process.Kill();
						process.WaitForExit(2000);
						killedAny = true;
						OnProcessKilled(process.ProcessName, process.Id, "Manually terminated");
					}
				}
				catch
				{
				}
			}
			return killedAny;
		}
		catch (Exception ex)
		{
			OnLogMessageReceived("⚠\ufe0f Manual kill failed for " + processName + ": " + ex.Message);
			return false;
		}
	}

	public void AddProcessToKillList(string processName)
	{
		if (!_processesToKill.Contains(processName, StringComparer.OrdinalIgnoreCase))
		{
			_processesToKill.Add(processName);
			OnLogMessageReceived("\ud83d\udccb Added to kill list: " + processName);
		}
	}

	public bool IsProcessRunning(string processName)
	{
		return Process.GetProcessesByName(processName).Length != 0;
	}

	public List<string> GetRunningSuspiciousProcesses()
	{
		List<string> suspicious = new List<string>();
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			try
			{
				string processName = process.ProcessName.ToLower();
				if (_processesToKill.Any((string p) => processName.Equals(p, StringComparison.OrdinalIgnoreCase)) || _processPatterns.Any((string pattern) => processName.Contains(pattern.Replace("*", "").ToLower())) || IsSuspiciousProcess(process))
				{
					suspicious.Add($"{process.ProcessName} (PID: {process.Id})");
				}
			}
			catch
			{
			}
		}
		return suspicious;
	}

	private void OnProcessKilled(string processName, int processId, string reason)
	{
		this.ProcessKilled?.Invoke(this, new ProcessKilledEventArgs(processName, processId, reason));
	}

	private void OnLogMessageReceived(string message)
	{
	}

	public void Dispose()
	{
		StopMonitoring();
		_cancellationTokenSource?.Dispose();
	}
}
