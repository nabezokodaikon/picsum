using System.Reflection;

namespace SWF.Core.Base
{
    public static class AssemblyPreloader
    {
        private static readonly HashSet<string> LoadedAssemblies = new HashSet<string>();

        public static void PreloadAssemblies(string directoryPath)
        {
            // 指定されたディレクトリ内のすべてのDLLファイルを取得
            var assemblyFiles = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);

            // 並列でアセンブリを読み込み
            Parallel.ForEach(assemblyFiles, assemblyPath =>
            {
                try
                {
                    if (!LoadedAssemblies.Contains(assemblyPath))
                    {
                        Assembly.LoadFile(assemblyPath);
                        LoadedAssemblies.Add(assemblyPath);
                    }
                }
                catch (BadImageFormatException)
                {
                    // マネージアセンブリでない場合は無視
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"アセンブリの読み込みに失敗しました: {assemblyPath}. Error: {ex.Message}");
                }
            });
        }

        public static void PreloadSpecificAssemblies(params string[] assemblyNames)
        {
            foreach (var assemblyName in assemblyNames)
            {
                try
                {
                    if (!IsAssemblyLoaded(assemblyName))
                    {
                        Assembly.Load(assemblyName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"アセンブリの読み込みに失敗しました: {assemblyName}. Error: {ex.Message}");
                }
            }
        }

        private static bool IsAssemblyLoaded(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
