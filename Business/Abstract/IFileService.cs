﻿using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IFileService
    {
        void addFile(FileKey newFile);
        int getFileCount(int userId);
        void shareFileFriend(SharedFile sharedFile);
        List<string> getSharedFiles(int userId);
        List<string> getFiles(int userId);
        FileKey getFileKey(string fileId);
    }
}
