﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GitScc
{
    internal class ChangesetFileStatus
    {
        private int _secondsUntilStale;

        public DateTime StatusTime { get; private set; }
        public GitFileStatus Status { get; private set; }

        public bool IsStale => ((DateTime.UtcNow - StatusTime).Seconds > _secondsUntilStale);

        public ChangesetFileStatus(GitFileStatus status, int secondsUntilStale = 5)
        {
            StatusTime = DateTime.UtcNow;
        }
    }

    public class GitChangesetManager
    {
        private ConcurrentDictionary<string, ChangesetFileStatus> _fileStatus;


        public GitChangesetManager()
        {
            _fileStatus = new ConcurrentDictionary<string, ChangesetFileStatus>();
        }

        #region Public Methods


        /// <summary>
        /// 
        /// </summary>
        /// <param name="newChangeSet"></param>
        /// <returns>true if files have changed</returns>
        public Dictionary<string, GitFileStatus> LoadChangeSet(List<GitFile> newChangeSet)
        {
            return CreateRepositoryUpdateChangeSet(newChangeSet);
        }

        /// <summary>
        /// Send filename and status, and returns true if file status is different than last known status
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool StatusChanged(string filename, GitFileStatus status)
        {

            var file = filename.ToLower();
            ChangesetFileStatus fileStatus;


            if (_fileStatus.TryGetValue(file, out fileStatus))
            {
                if (!fileStatus.IsStale  && fileStatus.Status == status)
                {
                    return false;
                }
                _fileStatus[file] = new ChangesetFileStatus(status);
                return true;
            }

            _fileStatus.TryAdd(file, new ChangesetFileStatus(status));
            return true;
        }

        #endregion


        public void SetStatus(string filename, GitFileStatus status)
        {
            if (!String.IsNullOrWhiteSpace(filename))
            {
                var fileKey = filename.ToLower();
                var changeStatus = GitFile.IsChangedStatus(status) ? status : GitFileStatus.Unaltered;
                if (_fileStatus.ContainsKey(fileKey))
                {
                    if (changeStatus == GitFileStatus.Unaltered)
                    {
                        ChangesetFileStatus dStatus;
                        _fileStatus.TryRemove(fileKey,out dStatus);
                    }
                    else
                    {
                        _fileStatus.AddOrUpdate(fileKey, new ChangesetFileStatus(changeStatus), (key, val) => new ChangesetFileStatus(changeStatus));
                    }
                }
            }
        }

        /// <summary>
        /// Takes the new changeset and returns a list of files that have changed status
        /// </summary>
        /// <param name="newChangeSet"></param>
        /// <returns></returns>
        private Dictionary<string, GitFileStatus> CreateRepositoryUpdateChangeSet(List<GitFile> newChangeSet)
        {
            var updatedFiles = new Dictionary<string, GitFileStatus>();
            var newChangeKeys = newChangeSet.Select(x => x.FilePath).ToList(); //_fileStatus.Where(x => x.Value.Status != GitFileStatus.Unaltered).Select(x => x.Key).ToList();
            var newUnchangedFiles = _fileStatus.Keys.Except(newChangeKeys).ToList();
            foreach (var file in newUnchangedFiles)
            {
                updatedFiles.Add(file, GitFileStatus.Unaltered);
                ChangesetFileStatus dStatus;
                _fileStatus.TryRemove(file, out dStatus);

            }

            foreach (var gitFile in newChangeSet)
            {
                ChangesetFileStatus fileStatus;
                if (_fileStatus.TryGetValue(gitFile.FilePath, out fileStatus))
                {
                    if (fileStatus.IsStale || fileStatus.Status != gitFile.Status)
                    {
                        updatedFiles.Add(gitFile.FilePath, gitFile.Status);
                        _fileStatus.AddOrUpdate(gitFile.FilePath, new ChangesetFileStatus(gitFile.Status), (key, value) => new ChangesetFileStatus(gitFile.Status));
                    }
                }
                else
                {
                    updatedFiles.Add(gitFile.FilePath, gitFile.Status);
                    _fileStatus.AddOrUpdate(gitFile.FilePath, new ChangesetFileStatus(gitFile.Status), (key, value) => new ChangesetFileStatus(gitFile.Status));
                }
            }
            return updatedFiles;
        }



        private List<string> GetFullPathForGitFiles(List<GitFile> files)
        {
            return files.Select(gitFile => gitFile.FilePath).ToList();
        }
    }
}
