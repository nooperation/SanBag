using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler.Metadata;

namespace CommonUI.ViewModels.ResourceViewModels
{
    class MyAssemblyResolver : IAssemblyResolver
    {
        private readonly PEStreamOptions _streamOptions;
        private readonly MetadataReaderOptions _metadataOptions;
        private readonly IAssemblyResolver _baseAssemblyResolver;

        public List<string> AdditionalPathsToSearch { get; set; } = new List<string>();

        public MyAssemblyResolver(
            string mainAssemblyFileName,
            bool throwOnError,
            string targetFramework,
            PEStreamOptions streamOptions = PEStreamOptions.Default,
            MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default)
        {
            _streamOptions = streamOptions;
            _metadataOptions = metadataOptions;

            _baseAssemblyResolver = new UniversalAssemblyResolver(
                mainAssemblyFileName,
                throwOnError,
                targetFramework,
                streamOptions,
                metadataOptions
            );
        }

        public PEFile Resolve(IAssemblyReference reference)
        {
            var resolvedModule = _baseAssemblyResolver.Resolve(reference);
            if (resolvedModule != null)
            {
                return resolvedModule;
            }

            foreach(var path in AdditionalPathsToSearch)
            {
                var pathToCheck = $@"{path}\{reference.Name}.dll";
                if (File.Exists(pathToCheck)) {
                    resolvedModule = new PEFile(
                        pathToCheck,
                        new FileStream(pathToCheck, FileMode.Open, FileAccess.Read),
                        _streamOptions,
                        _metadataOptions
                    );
                }
            }

            return resolvedModule;
        }

        public PEFile ResolveModule(PEFile mainModule, string moduleName)
        {
            return _baseAssemblyResolver.ResolveModule(mainModule, moduleName);
        }
    }
}
