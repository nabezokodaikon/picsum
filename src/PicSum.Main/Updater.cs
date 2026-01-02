using NLog;
using PicSum.Job.SyncJobs;
using SWF.Core.Base;
using System;

namespace PicSum.Main
{
    internal sealed class Updater
    {
        private static readonly Logger LOGGER = NLogManager.GetLogger();

        private readonly Version _12_0_0_0 = new(12, 0, 0, 0);
        private readonly Version _12_2_1_0 = new(12, 2, 1, 0);
        private readonly Version _12_2_2_0 = new(12, 2, 2, 0);
        private readonly Version _12_3_0_0 = new(12, 3, 0, 0);
        private readonly Version _17_1_0_0 = new(17, 1, 0, 0);

        public void VersionUpTo_12_0_0_0(Version version)
        {
            if (this._12_0_0_0 <= version)
            {
                return;
            }

            using (Measuring.Time(true, "VersionUpTo_12_0_0_0"))
            {
                LOGGER.Info($"バージョンが'{this._12_0_0_0}'未満のため、サムネイルを初期化します。");

                var thumbnailDBCleanupJob = new ThumbnailDBCleanupSyncJob();
                thumbnailDBCleanupJob.Execute();
            }
        }

        public void VersionUpTo_12_2_1_0(Version version)
        {
            if (this._12_2_1_0 <= version)
            {
                return;
            }

            using (Measuring.Time(true, "VersionUpTo_12_2_1_0"))
            {
                LOGGER.Info($"バージョンが'{this._12_2_1_0}'未満のため、評価値Tを更新します。");

                var job = new VersionUpTo_12_2_1_0_SyncJob();
                job.Execute();
            }
        }

        public void VersionUpTo_12_2_2_0(Version version)
        {
            if (this._12_2_2_0 <= version)
            {
                return;
            }

            using (Measuring.Time(true, "VersionUpTo_12_2_2_0"))
            {
                LOGGER.Info($"バージョンが'{this._12_2_2_0}'未満のため、ディレクトリ表示履歴Tを更新します。");

                var job = new VersionUpTo_12_2_2_0_SyncJob();
                job.Execute();
            }
        }

        public void VersionUpTo_12_3_0_0(Version version)
        {
            if (this._12_3_0_0 <= version)
            {
                return;
            }

            using (Measuring.Time(true, "VersionUpTo_12_3_0_0"))
            {
                LOGGER.Info($"バージョンが'{this._12_3_0_0}'未満のため、ディレクトリ表示履歴Tを更新します。");

                var job = new VersionUpTo_12_3_0_0_SyncJob();
                job.Execute();
            }
        }

        public void VersionUpTo_17_1_0_0(Version version)
        {
            if (this._17_1_0_0 <= version)
            {
                return;
            }

            using (Measuring.Time(true, "VersionUpTo_17_1_0_0"))
            {
                LOGGER.Info($"バージョンが'{this._17_1_0_0}'未満のため、サムネイルキャッシュを初期化します。");

                var job = new VersionUpTo_17_1_0_0_SyncJob();
                job.Execute();
            }
        }
    }
}
