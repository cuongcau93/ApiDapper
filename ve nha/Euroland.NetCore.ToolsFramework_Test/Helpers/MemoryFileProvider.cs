using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;
using System.IO;

namespace Euroland.NetCore.ToolsFramework.Test.Helpers
{

    public class MemoryFileProvider : IFileProvider
    {
        private readonly string _content;
        private readonly string _fileName;
        public MemoryFileProvider(string content, string fileName)
        {
            _content = content;
            _fileName = fileName;
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return new MemoryFileInfo(_content, _fileName);   
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }

    public class MemoryFileInfo : IFileInfo
    {
        private readonly string _content;
        private readonly string _fileName;
        public MemoryFileInfo(string content, string fileName)
        {
            _content = content;
            _fileName = fileName;
        }
        public bool Exists => true;

        public long Length => throw new NotImplementedException();

        public string PhysicalPath => _fileName;

        public string Name => _fileName;

        public DateTimeOffset LastModified => throw new NotImplementedException();

        public bool IsDirectory => throw new NotImplementedException();

        public Stream CreateReadStream()
        {
            return StreamHelper.StringToStream(_content);
        }
    }
}
