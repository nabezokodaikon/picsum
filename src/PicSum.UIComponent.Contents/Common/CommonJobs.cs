using PicSum.Job.Jobs;
using SWF.Core.Job;
using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.Common
{
    [SupportedOSPlatform("windows")]
    public static class CommonJobs
    {
        private static OneWayJob<ImageFileCacheJob, ListParameter<string>> imageFileCacheJob = null;

        public static void DisposeStaticResources()
        {
            imageFileCacheJob?.Dispose();
        }

        public static void StartImageFileCacheJob(Control sender, ListParameter<string> parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (imageFileCacheJob != null)
            {
                imageFileCacheJob.Dispose();
            }

            imageFileCacheJob = new();
            imageFileCacheJob.StartJob(sender, parameter);
        }
    }
}
