using Npoi.Mapper.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Further.Abp.Npoi.Mapper
{
    public interface IVirtualFileNpoiReader : Volo.Abp.DependencyInjection.ITransientDependency
    {
        List<T> Read<T>(string filePath = null, string sheetName = null, Action<global::Npoi.Mapper.Mapper> action=null)
            where T : class, new();

        List<T> ReadToTreeNode<T>(string filePath = null, string sheetName = null)
            where T : ITreeNode<T>, new();

        List<string> GetSheetNames(string filePath);

    }
}
