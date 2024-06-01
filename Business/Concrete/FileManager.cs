using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class FileManager : IFileService
    {
        private IFileDal _fileDal;
        public FileManager(IFileDal fileDal)
        {
            _fileDal = fileDal;
        }

        public void addFile(FileKey newFile)
        {
            _fileDal.addFile(newFile);
        }

        public int getFileCount(int userId)
        {
           return _fileDal.getFileCount(userId);
        }

        public FileKey getFileKey(string fileId)
        {
            return _fileDal.getFileKey(fileId);
        }

        public List<string> getFiles(int userId)
        {
            return _fileDal.getFiles(userId);
        }

        public List<string> getSharedFiles(int userId)
        {
            return _fileDal.getSharedFiles(userId);
        }

        public void shareFileFriend(SharedFile sharedFile)
        {
            _fileDal.shareFileFriend(sharedFile);
        }
    }
}
