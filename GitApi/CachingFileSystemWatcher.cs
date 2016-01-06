using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GitScc
{
    public delegate void CachingFileSystemUpdateEventHandler(object sender, CachingFileSystemEventArgs e);

    public class CachingFileSystemEventArgs : EventArgs
    {
        public List<string> FileCollection { get; set; }

        public CachingFileSystemEventArgs(List<string> fileCollection)
        {
            FileCollection = fileCollection;
        }
    }

    public class CachingFileSystemWatcher : IDisposable
    {
        private FileSystemWatcher _watcher;
        //private MemoryCache _fileCache;
        private readonly int _interFileEventDelayMilliseconds;
        private readonly int _absoluteEventDelayMilliseconds;
        private ConcurrentDictionary<string, WatcherChangeTypes> _fileCollection;
        //private CacheItemPolicy _fileCacheItemPolicy;
        private CacheEntryChangeMonitor monitor;
        private const string TOTAL_EVENT_TOKEN = "TOTAL_EVENT_TOKEN";
        private const string LAST_FILE_EVENT_TOKEN = "LAST_FILE_EVENT_TOKEN";
        private readonly object _cacheLock = new object();
        private DateTime lastFileEvent = DateTime.MinValue;
        private DateTime startFileEvent = DateTime.MinValue;
        private bool _watchingEvent = false;

        private event CachingFileSystemUpdateEventHandler _onFilesUpdateEventHandler;

        public CachingFileSystemWatcher(string path)
        {
            _interFileEventDelayMilliseconds = 50;
            _absoluteEventDelayMilliseconds = 2000;
            SetupFileWatcher(path);
        }

        public CachingFileSystemWatcher(string path, int interFileEventDelayMilliseconds, int absoluteEventDelayMilliseconds)
        {
            _interFileEventDelayMilliseconds = interFileEventDelayMilliseconds;
            _absoluteEventDelayMilliseconds = absoluteEventDelayMilliseconds;
            SetupFileWatcher(path);
        }

        public event CachingFileSystemUpdateEventHandler FilesChanged
        {
            add
            {
                _onFilesUpdateEventHandler += value;
            }
            remove
            {
                _onFilesUpdateEventHandler -= value;
            }
        }

        public bool EnableRaisingEvents
        {
            set
            {
                if (_watcher != null)
                {
                    _watcher.EnableRaisingEvents = value;
                }
            }
            get { return _watcher?.EnableRaisingEvents ?? false; }
        }

        private void SetupFileWatcher(string path)
        {
            //_fileCache = new MemoryCache("file",new NameValueCollection{ "pollingInterval" , "00:02:00" });
            //_fileCache.PollingInterval = TimeSpan.FromMilliseconds(_interFileEventDelayMilliseconds/2);
            _fileCollection = new ConcurrentDictionary<string, WatcherChangeTypes>();
            _watcher = new FileSystemWatcher(path);
            _watcher.NotifyFilter =
                            NotifyFilters.FileName
                            | NotifyFilters.Attributes
                            | NotifyFilters.LastWrite
                            | NotifyFilters.Size
                            | NotifyFilters.CreationTime
                            | NotifyFilters.DirectoryName;

            _watcher.IncludeSubdirectories = true;
            _watcher.Changed += HandleFileSystemChanged;
            _watcher.Created += HandleFileSystemChanged;
            _watcher.Deleted += HandleFileSystemChanged;
            _watcher.Renamed += HandleFileSystemChanged;
        }

        private async void HandleFileSystemChanged(object sender, FileSystemEventArgs e)
        {
            await AddChangedFile(e.FullPath.ToLower(),e.ChangeType);
        }

        private async Task AddChangedFile(string filename, WatcherChangeTypes changeType)
        {

                if (!_watchingEvent)
                {
                    StartWatcher();
                }
                lastFileEvent = DateTime.UtcNow;
                _fileCollection.AddOrUpdate(filename, changeType, (key, oldValue) => changeType);
                //_fileCache.Set(LAST_FILE_EVENT_TOKEN, files, GetPolicy(), null);
        }


        private async Task StartWatcher()
        {
            lastFileEvent = DateTime.UtcNow;
            startFileEvent = DateTime.UtcNow;
            _watchingEvent = true;
            RunFileWatcher();
        }

        public async Task RunFileWatcher()
        {
            await Task.Delay(_interFileEventDelayMilliseconds/2);
            if(((int)(DateTime.UtcNow - lastFileEvent).TotalMilliseconds >= _interFileEventDelayMilliseconds) 
                || ((int)(DateTime.UtcNow - startFileEvent).TotalMilliseconds >= _absoluteEventDelayMilliseconds))
            {
                lastFileEvent = DateTime.MinValue;
                startFileEvent = DateTime.MinValue;
                _watchingEvent = false;
                var files = _fileCollection.Keys.ToList();
                _fileCollection.Clear();
                FireFilesChangedEvent(files);
            }
            else
            {
                await RunFileWatcher();
            }
        }

        //private bool WatchingEvent => _fileCache.Contains(TOTAL_EVENT_TOKEN) && _fileCache.Contains(LAST_FILE_EVENT_TOKEN);

        //private void StartFileEvent()
        //{

        //    CacheItemPolicy itemPolicyAbs = new CacheItemPolicy();
        //    itemPolicyAbs.AbsoluteExpiration = DateTimeOffset.UtcNow + TimeSpan.FromMilliseconds(_absoluteEventDelayMilliseconds);
        //    _fileCache.Set(TOTAL_EVENT_TOKEN, new object(), itemPolicyAbs, null);

        //    monitor = _fileCache.CreateCacheEntryChangeMonitor(new string[] { TOTAL_EVENT_TOKEN });

        //    //add sliding exp
        //    //var _fileCacheItemPolicy = new CacheItemPolicy();
        //    //_fileCacheItemPolicy.ChangeMonitors.Add(monitor);
        //    //_fileCacheItemPolicy.SlidingExpiration = TimeSpan.FromMilliseconds(_interFileEventDelayMilliseconds);
        //    //_fileCacheItemPolicy.RemovedCallback += RemovedCallback;
        //    _fileCache.Add(LAST_FILE_EVENT_TOKEN, new ConcurrentDictionary<string, WatcherChangeTypes>(), GetPolicy(), null);
        //}

        //private CacheItemPolicy GetPolicy()
        //{
        //    var fileCacheItemPolicy = new CacheItemPolicy();
        //    fileCacheItemPolicy.ChangeMonitors.Add(monitor);
        //    fileCacheItemPolicy.SlidingExpiration = TimeSpan.FromMilliseconds(_interFileEventDelayMilliseconds);
        //    fileCacheItemPolicy.RemovedCallback += RemovedCallback;
        //    return fileCacheItemPolicy;
        //}

        //private void RemovedCallback(CacheEntryRemovedArguments arguments)
        //{
        //    lock (_cacheLock)
        //    {
        //        var key = arguments.CacheItem?.Key;
        //        var val = arguments.CacheItem?.Value as ConcurrentDictionary<string, WatcherChangeTypes>;
        //        if (string.Equals(key, LAST_FILE_EVENT_TOKEN) && val != null)
        //        {
        //            FireFilesChangedEvent(val.Keys.ToList());
        //        }
        //    }
        //}

        private void FireFilesChangedEvent(List<string> fileCollection)
        {
            _onFilesUpdateEventHandler?.Invoke(this, new CachingFileSystemEventArgs(fileCollection));
        }


        public void DisableRepositoryWatcher()
        {
            if (_watcher != null)
            {
                _watcher.Changed += HandleFileSystemChanged;
                _watcher.Created += HandleFileSystemChanged;
                _watcher.Deleted += HandleFileSystemChanged;
                _watcher.Renamed += HandleFileSystemChanged;
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
            }
          
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            DisableRepositoryWatcher();
        }

        #endregion
    }
}
