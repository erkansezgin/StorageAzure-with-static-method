using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;

namespace WpfApplicationTestAzure
{
    public static class AzureInformation
    {
        #region Private Method
        private static string GetAccountName()
        {
            return Properties.Settings.Default.AccountName;
        }
        private static string GetContainerName()
        {
            return Properties.Settings.Default.ContainerName;
        }
        private static string GetKeyValue()
        {
            return Properties.Settings.Default.KeyValue;
        }
        #endregion


        #region Public Method
        public static StorageInformation Get()
        {
            var storageInformation =
               new StorageInformation
               {
                   AccountName = GetAccountName(),
                   ContainerName = GetContainerName(),
                   KeyValue = GetKeyValue()
               };
            return storageInformation;
        }
        #endregion
    }
}
