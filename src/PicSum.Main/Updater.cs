using NLog;
using PicSum.Job.SyncJobs;
using SWF.Core.Base;
using System;

namespace PicSum.Main
{
    internal class Updater
    {
        private static readonly Logger LOGGER = Log.GetLogger();

        private readonly Version _12_0_0_0 = new(12, 0, 0, 0);
        private readonly Version _12_2_1_0 = new(12, 2, 1, 0);
        private readonly Version _12_2_2_0 = new(12, 2, 2, 0);

        public void VersionUpTo_12_0_0_0(Version version)
        {
            using (TimeMeasuring.Run(true, "VersionUpTo_12_0_0_0"))
            {
                if (this._12_0_0_0 <= version)
                {
                    return;
                }

                LOGGER.Info($"バージョンが'{this._12_0_0_0}'未満であるため、サムネイルを初期化します。");

                var thumbnailDBCleanupJob = new ThumbnailDBCleanupSyncJob();
                thumbnailDBCleanupJob.Execute();
            }
        }

        public void VersionUpTo_12_2_1_0(Version version)
        {
            using (TimeMeasuring.Run(true, "VersionUpTo_12_2_1_0"))
            {
                if (this._12_2_1_0 <= version)
                {
                    return;
                }

                LOGGER.Info($"バージョンが'{this._12_2_1_0}'であるため、評価値Tを更新します。");

                var job = new VersionUpTo_12_2_1_0_Job();
                job.Execute();
            }
        }

        public void VersionUpTo_12_2_2_0(Version version)
        {
            using (TimeMeasuring.Run(true, "VersionUpTo_12_2_2_0"))
            {
                if (this._12_2_2_0 <= version)
                {
                    return;
                }

                LOGGER.Info($"バージョンが'{this._12_2_2_0}'であるため、ディレクトリ表示履歴Tを更新します。");
            }
        }
    }
}
