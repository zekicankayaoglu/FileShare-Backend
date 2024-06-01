using DataAccess.Abstract;
using Entities.Concrete;
using Entities.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class FileDal : IFileDal
    {
        private DataContext _context;
        public FileDal(DataContext context)
        {
            _context = context;
        }

        public void addFile(FileKey newFile)
        {
            try
            {
                _context.FileKeys.Add(newFile);
                _context.SaveChanges();
                Console.WriteLine("Veritabanına başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanına ekleme hatası: {ex.Message}");
                _context.SaveChangesAsync();
            }
        }

        public int getFileCount(int userId)
        {
            var count = _context.FileKeys.Where(f => f.UserId == userId).Count();
            return count;
        }

        public List<string> getFiles(int userId)
        {
            var fileIds = _context.FileKeys
                          .Where(f => f.UserId == userId)
                          .Select(f => f.FileId)
                          .ToList();
            return fileIds;

        }

        public List<string> getSharedFiles(int userId)
        {
            var sharedFiles = _context.SharedFiles.Where(f => f.SharingUserId == userId).ToList();
            var sharedFileIds = sharedFiles.Select(f => f.FileId).ToList();
            
            return sharedFileIds;
        }

        public void shareFileFriend(SharedFile sharedFile)
        {
            _context.SharedFiles.Add(sharedFile);
            _context.SaveChanges();
        }

        public FileKey getFileKey(string fileId)
        {
            var key = _context.FileKeys.FirstOrDefault(f => f.FileId == fileId);
            return key;
        }
    }
}
