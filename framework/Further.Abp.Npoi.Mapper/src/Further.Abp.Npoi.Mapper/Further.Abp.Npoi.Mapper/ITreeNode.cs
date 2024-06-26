﻿using System.Collections.Generic;

namespace Further.Abp.Npoi.Mapper
{
    public interface ITreeNode<T>
        where T: ITreeNode<T>
    {
        List<T> Children { get; set; }
        string Code { get; set; }
    }
}