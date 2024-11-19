using System.Reflection;
using System.Runtime.CompilerServices;

namespace SWF.Core.Base
{
    public static class SingleFilePreloader
    {
        private static readonly HashSet<string> PreloadedAssemblies = new(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> ProcessingAssemblies = new(StringComparer.OrdinalIgnoreCase);

        public static void PreloadAssembly<T>()
        {
            var assemblyName = typeof(T).Assembly.GetName().Name;
            if (!string.IsNullOrEmpty(assemblyName))
            {
                PreloadAssemblyByName(assemblyName);
            }
        }

        public static void PreloadAssemblies(params Type[] types)
        {
            foreach (var type in types)
            {
                var assemblyName = type.Assembly.GetName().Name;
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    PreloadAssemblyByName(assemblyName);
                }
            }
        }

        public static void PreloadAssemblyByName(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName) ||
                PreloadedAssemblies.Contains(assemblyName) ||
                ProcessingAssemblies.Contains(assemblyName))
            {
                return;
            }

            try
            {
                ProcessingAssemblies.Add(assemblyName);

                var assembly = Assembly.Load(new AssemblyName(assemblyName));

                // 型の初期化
                try
                {
                    RuntimeHelpers.RunModuleConstructor(assembly.GetType().Module.ModuleHandle);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"モジュール初期化中にエラーが発生しました: {assemblyName}. Error: {ex.Message}");
                }

                // 安全に依存アセンブリを読み込む
                LoadReferencedAssemblies(assembly);

                PreloadedAssemblies.Add(assemblyName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"アセンブリの事前読み込みに失敗しました: {assemblyName}. Error: {ex.Message}");
            }
            finally
            {
                ProcessingAssemblies.Remove(assemblyName);
            }
        }

        private static void LoadReferencedAssemblies(Assembly assembly)
        {
            try
            {
                var referencedAssemblies = assembly.GetReferencedAssemblies()
                    .Where(a => !PreloadedAssemblies.Contains(a.Name) &&
                               !ProcessingAssemblies.Contains(a.Name))
                    .ToList();

                foreach (var refAssembly in referencedAssemblies)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(refAssembly.Name))
                        {
                            Assembly.Load(refAssembly);
                            PreloadedAssemblies.Add(refAssembly.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"依存アセンブリの読み込みに失敗しました: {refAssembly.Name}. Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"依存アセンブリの取得中にエラーが発生しました. Error: {ex.Message}");
            }
        }

        public static void PreloadAssembliesInNamespace(string namespacePrefix)
        {
            if (string.IsNullOrEmpty(namespacePrefix))
            {
                return;
            }

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location));

            foreach (var assembly in loadedAssemblies)
            {
                try
                {
                    var hasMatchingTypes = assembly.GetTypes()
                        .Any(t => t.Namespace != null &&
                                 t.Namespace.StartsWith(namespacePrefix, StringComparison.OrdinalIgnoreCase));

                    if (hasMatchingTypes)
                    {
                        var assemblyName = assembly.GetName().Name;
                        if (!string.IsNullOrEmpty(assemblyName))
                        {
                            PreloadAssemblyByName(assemblyName);
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // タイプロードエラーは無視
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"名前空間のチェック中にエラーが発生しました: {assembly.FullName}. Error: {ex.Message}");
                }
            }
        }

        // 読み込み済みアセンブリの一覧を取得
        public static IEnumerable<string> GetPreloadedAssemblies()
        {
            return [.. PreloadedAssemblies];
        }
    }
}
