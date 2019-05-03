using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace CachedFileSystemWatcher
{
    public sealed class CachedFileSystemWatcher : IDisposable
    {
        private readonly string _watchFolder;
        private readonly string _watchFilter;
        private readonly Action<List<string>> _process;
        private readonly ConcurrentQueue<CachedFile> _cache;
        private readonly Timer _timer;

        private FileSystemWatcher _fileSystemWatcher;

        private const int TimerIntervalSeconds = 10;
        private const int MaxRetries = 10;

        public CachedFileSystemWatcher(string watchFolder, string watchFilter, Action<List<string>> process)
        {
            _watchFolder = watchFolder;
            _watchFilter = watchFilter;
            _process = process;
            _cache = new ConcurrentQueue<CachedFile>();
            _timer = new Timer {Interval = TimerIntervalSeconds * 1000};
            _timer.Elapsed += ProcessCache;
        }

        public void Start()
        {
            _timer.Enabled = true;

            _fileSystemWatcher = new FileSystemWatcher
            {
                Path = _watchFolder,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true
            };

            if (!string.IsNullOrEmpty(_watchFilter))
            {
                _fileSystemWatcher.Filter = _watchFilter;
            }

            _fileSystemWatcher.Created += OnCreated;

            Console.WriteLine( $"Started watching {_watchFilter} files created under {_watchFolder}");
        }

        private void ProcessCache(object sender, ElapsedEventArgs e)
        {
            var items = new List<string>();
            while (_cache.TryDequeue(out var cachedItem))
            {
                var path = cachedItem.Path;

                if (cachedItem.RetryCount > MaxRetries)
                {
                    Console.Error.WriteLine($"Maximum retry number {MaxRetries} exceeded, skipping {path}");
                }

                if (new FileInfo(path).IsLocked())
                {
                    Console.WriteLine($"{path} is locked, will try again later");
                    cachedItem.RetryCount++;
                    _cache.Enqueue(cachedItem);
                    continue;
                }

                items.Add(path);
            }

            if (items.Count > 0)
            {
                _process(items);
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            var cachedFile = new CachedFile
            {
                Path = e.FullPath,
                RetryCount = 0,
            };

            _cache.Enqueue(cachedFile);
        }

        public void Dispose()
        {
            _fileSystemWatcher?.Dispose();
            _timer?.Dispose();

            Console.WriteLine($"Stopped watching {_watchFolder} for {_watchFilter} files");
        }
    }
}
