using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Azure
{
    public static class FileStorage
    {
        #region Private Method
        private static CloudBlobContainer GetContainer(CloudBlobClient blobClient, string containerName)
        {
            return blobClient.GetContainerReference(containerName);
        }

        private static CloudBlobClient InitializeAzureConnection(StorageInformation storageInformation)
        {
            var storageCredential = new StorageCredentials(storageInformation.AccountName, storageInformation.KeyValue);
            var storageAccount = new CloudStorageAccount(storageCredential, true);
            return storageAccount.CreateCloudBlobClient();
        }
        private static CloudBlockBlob GetCloudBlockBlob(CloudBlobContainer cloudBlobContainer)
        {
            var guid = Guid.NewGuid();
            return cloudBlobContainer.GetBlockBlobReference(guid.ToString());
        }
        private static CloudBlockBlob InitializeForUpload(StorageInformation storageInformation)
        {
            var blobClient = InitializeAzureConnection(storageInformation);
            var container = GetContainer(blobClient, storageInformation.ContainerName);
            return GetCloudBlockBlob(container);
        }
        #endregion    

        #region Public Method
        /// <summary>
        /// Upload file from disk to Azure
        /// </summary>
        /// <param name="path"></param>
        /// <param name="storageInformation"></param>
        /// <returns></returns>       
        public static string UploadOnAzure(string path, StorageInformation storageInformation)
        {
            using (var fileStream = File.OpenRead(path))
            {
                var blob = InitializeForUpload(storageInformation);
                blob.UploadFromStream(fileStream);
                return blob.Name;
            }
        }
        /// <summary>
        /// Upload file from stream to Azure
        /// </summary>
        /// <param name="file"></param>
        /// <param name="storageInformation"></param>
        /// <returns>return the real name of your file on Azure (guid)</returns>
        public static string UploadOnAzure(Stream file, StorageInformation storageInformation)
        {
            var binary = new BinaryReader(file);
            var binData = binary.ReadBytes((int)file.Length);
            var blob = InitializeForUpload(storageInformation);
            blob.UploadFromByteArray(binData, 0, binData.Length);
            return blob.Name;
        }
        /// <summary>
        /// Get all file name from your Azure storage 
        /// </summary>
        /// <param name="storageInformation"></param>
        /// <returns>retrun list of file name</returns>
        public static List<string> GetFilesName(StorageInformation storageInformation)
        {
            var blobClient = InitializeAzureConnection(storageInformation);
            var container = GetContainer(blobClient, storageInformation.ContainerName);
            return container.ListBlobs().Select(blobItem => blobItem.Uri.ToString().Substring(blobItem.Uri.ToString().LastIndexOf('/') + 1)).ToList();
        }
        public static void DeleteFile(string nameFile, StorageInformation storageInformation)
        {
            var blobClient = InitializeAzureConnection(storageInformation);
            var container = GetContainer(blobClient, storageInformation.ContainerName);
            var file = container.GetBlockBlobReference(nameFile);
            file.DeleteIfExists();
        }

        public static void DeleteAllFile(StorageInformation storageInformation)
        {
            var fileList = FileStorage.GetFilesName(storageInformation);
            foreach (string fileName in fileList)
            {
                FileStorage.DeleteFile(fileName, storageInformation);
            }
        }

        public static FileStreamResult DownloadFile(string nameFile, string realName, string contentType, StorageInformation storageInformation)
        {
            var blobClient = InitializeAzureConnection(storageInformation);
            var container = GetContainer(blobClient, storageInformation.ContainerName);
            var file = container.GetBlockBlobReference(nameFile);

            var stream = new MemoryStream { Position = 0 };
            file.DownloadToStream(stream);
            var result = new FileStreamResult(stream, contentType);
            stream.Position = 0;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(realName, Encoding.UTF8));
            return result;

        }
        #endregion      

    }
}
