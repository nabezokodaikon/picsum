using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessorTest
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class FileInfoDBTest
        : IDisposable
    {
        public FileInfoDBTest()
        {
            Directory.CreateDirectory(AppConstants.DATABASE_DIRECTORY);
            if (!Directory.Exists(AppConstants.DATABASE_DIRECTORY))
            {
                Directory.CreateDirectory(AppConstants.DATABASE_DIRECTORY);
            }

            Instance<IFileInfoDB>.Initialize(
                new FileInfoDB(AppConstants.FILE_INFO_DATABASE_FILE));
        }

        public void Dispose()
        {
            Instance<IFileInfoDB>.Value.Dispose();

            if (Directory.Exists(AppConstants.DATABASE_DIRECTORY))
            {
                Directory.Delete(AppConstants.DATABASE_DIRECTORY, true);
            }
        }

        [Fact]
        public void FileCreationSqlTest()
        {
            using (var tran = Instance<IFileInfoDB>.Value.BeginTransaction())
            {
                var creationSql = new FileCreationSql("test");
                Instance<IFileInfoDB>.Value.Update(creationSql);

                var readSql = new AllFilesReadSql();
                var dtoList = Instance<IFileInfoDB>.Value.ReadList(readSql);

                Assert.Single(dtoList);

                Assert.Equal(1, dtoList[0].FileID);
                Assert.Equal("test", dtoList[0].FilePath);

                tran.Commit();
            }
        }
    }
}
