
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best9Instagram
{
    public interface IFileService
    {
        string Save(byte[] data, string name);
    }
}