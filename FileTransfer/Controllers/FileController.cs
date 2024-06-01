using Business.Abstract;
using Business.Concrete;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System; // Eklenen satýr
using System.IO;
using System.Linq;
namespace FileTransfer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
       
        private IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int userId, [FromForm] int[] friends)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Dosya seçilmedi.");

                Console.WriteLine($"Uploaded File Name: {file.FileName}, Size: {file.Length} bytes");

                const string connectionUri = "mongodb+srv://zekicankayaoglu:1zMnWTZqJzJSSZ3v@filetransfer.ja7eqhv.mongodb.net/?retryWrites=true&w=majority&appName=FileTransfer";
                var settings = MongoClientSettings.FromConnectionString(connectionUri);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                var client = new MongoClient(settings);
                var db = client.GetDatabase("FileTransfer");
                var bucket = new GridFSBucket(db);

                //using (var stream = file.OpenReadStream())
                //{
                //    var fileId = await bucket.UploadFromStreamAsync(file.FileName, stream);
                //    Console.WriteLine($"Dosya yüklendi. Dosya ID: {fileId}");
                //}
                byte[] encryptedFile;
                byte[] encryptionKey;
                byte[] iv;

                using (var stream = file.OpenReadStream())
                {
                    (encryptedFile, encryptionKey, iv) = EncryptionHelper.EncryptFile(stream);
                }

                ObjectId fileId;
                using (var encryptedStream = new MemoryStream(encryptedFile))
                {
                    fileId = await bucket.UploadFromStreamAsync(file.FileName, encryptedStream);
                    Console.WriteLine($"Dosya yüklendi. Dosya ID: {fileId}");
                }
                var FileKey = new FileKey()
                {
                    FileId = fileId.ToString(),
                    EncryptionKey = Convert.ToBase64String(encryptionKey),
                    IV = Convert.ToBase64String(iv),
                    UserId = userId
                };
                Console.Write(userId);
                _fileService.addFile(FileKey);
                if(friends.Length > 0)
                {
                    foreach(var friend in friends)
                    {
                        var sharedFile = new SharedFile()
                        {
                            SharedUserId = userId,
                            SharingUserId = friend,
                            FileId = fileId.ToString()
                        };
                        _fileService.shareFileFriend(sharedFile);
                    }
                }
                
                return Ok("Dosya baþarýyla yüklendi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ýstekte bir hata oluþtu: {ex.Message}");
            }
        }
    

        [HttpGet("ConnectCheck")]
        public async Task<IActionResult> ConnectCheck()
        {

            const string connectionUri = "mongodb+srv://zekicankayaoglu:1zMnWTZqJzJSSZ3v@filetransfer.ja7eqhv.mongodb.net/?retryWrites=true&w=majority&appName=FileTransfer";

            var settings = MongoClientSettings.FromConnectionString(connectionUri);

            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(settings);
            var db = client.GetDatabase("FileTransfer");
            var bucket = new GridFSBucket(db);
            using (var stream = new FileStream("log.txt", FileMode.Open))
            {
                var fileId = await bucket.UploadFromStreamAsync("log.txt", stream);
                Console.WriteLine($"Dosya yüklendi. Dosya ID: {fileId}");
            }

            var downloadStream = new MemoryStream();
            var fileName = "log.txt";
            await bucket.DownloadToStreamByNameAsync(fileName, downloadStream);
            Console.WriteLine($"Dosya indirildi. Ýçerik: {System.Text.Encoding.UTF8.GetString(downloadStream.ToArray())}");
            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
                return Ok("Dosya yükleme ve indirme iþlemi baþarýyla tamamlandý.");
        }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Ok();
        }

        [HttpGet("userfilecount/{userId}")]
        public async Task<IActionResult> GetUserFileCount(int userId)
        {
            return Ok(_fileService.getFileCount(userId));   
        }

        [HttpGet("myfiles/{userId}")]
        public async Task<IActionResult> GetMyFiles(int userId)
        {

            const string connectionUri = "mongodb+srv://zekicankayaoglu:1zMnWTZqJzJSSZ3v@filetransfer.ja7eqhv.mongodb.net/?retryWrites=true&w=majority&appName=FileTransfer";

            var settings = MongoClientSettings.FromConnectionString(connectionUri);

            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(settings);
            var database = client.GetDatabase("FileTransfer");
            var bucket = new GridFSBucket(database);

            var files = new List<string>();
            var myFiles = _fileService.getFiles(userId);
            foreach (var fileId in myFiles)
            {
                var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(fileId));
                var fileInfo = await bucket.Find(filter).FirstOrDefaultAsync();
                if (fileInfo != null)
                {
                    files.Add(fileInfo.Filename);
                }
            }
            

            var sharedFileIds = _fileService.getSharedFiles(userId);
            var sharedFiles = new List<string>();
            foreach (var fileId in sharedFileIds)
            {
                var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(fileId));
                var fileInfo = await bucket.Find(filter).FirstOrDefaultAsync();
                if (fileInfo != null)
                {
                    sharedFiles.Add(fileInfo.Filename);
                }
            }
            //var sharedFileDetails = _files.Find(f => sharedFileIds.Contains(f.FileId)).ToListAsync();


            return Ok(new
            {
                MyFiles = files,
                SharedFiles = sharedFiles
            });
        }


        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            try
            {
                
                // Dosya anahtarlarýný al
               
                const string connectionUri = "mongodb+srv://zekicankayaoglu:1zMnWTZqJzJSSZ3v@filetransfer.ja7eqhv.mongodb.net/?retryWrites=true&w=majority&appName=FileTransfer";

                var settings = MongoClientSettings.FromConnectionString(connectionUri);

                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                var client = new MongoClient(settings);
                var database = client.GetDatabase("FileTransfer");
                var bucket = new GridFSBucket(database);
                var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileId);
                var fileInfo = await bucket.FindAsync(filter).Result.FirstOrDefaultAsync();
                var fileKey = _fileService.getFileKey(fileInfo.Id.ToString());
                
                if (fileKey == null)
                {
                    return NotFound("Dosya anahtarý bulunamadý.");
                }
                var filter2 = Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, new ObjectId(fileInfo.Id.ToString()));
                var fileInfo2 = await bucket.FindAsync(filter).Result.FirstOrDefaultAsync();

                if (fileInfo2 == null)
                {
                    return NotFound("Dosya bulunamadý.");
                }

                // Dosyayý indir
                var stream = new MemoryStream();
                await bucket.DownloadToStreamAsync(fileInfo2.Id, stream);

                var decryptedFile = EncryptionHelper.DecryptFile(stream.ToArray(), fileKey.EncryptionKey, fileKey.IV);

                // Dosyayý kullanýcýya geri gönder
                return File(decryptedFile, "application/octet-stream", fileInfo2.Filename);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ýndirme sýrasýnda bir hata oluþtu: {ex.Message}");
            }
        }
    }
}
