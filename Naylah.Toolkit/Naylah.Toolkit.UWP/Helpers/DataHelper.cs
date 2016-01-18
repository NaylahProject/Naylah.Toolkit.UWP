//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Storage;
//using Windows.Storage.Streams;

//namespace Naylah.Toolkit.UWP.Helpers
//{
//    public class DataHelper<T>
//    {
//        public DataHelper(string _fileName)
//        {
//            this.fileName = _fileName;
//        }

//        public string fileName { get; set; }

//        public async Task SaveToStorage(object content)
//        {
//            var settings = new JsonSerializerSettings();
//            settings.PreserveReferencesHandling = PreserveReferencesHandling.All;
//            settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

//            string jsonContents = JsonConvert.SerializeObject(content);

//            StorageFolder localFolder = ApplicationData.Current.RoamingFolder;

//            StorageFile dataFile = await localFolder.CreateFileAsync(fileName,
//                                         CreationCollisionOption.ReplaceExisting);

//            using (IRandomAccessStream textStream = await dataFile.OpenAsync(FileAccessMode.ReadWrite))
//            {
//                using (DataWriter textWriter = new DataWriter(textStream))
//                {
//                    textWriter.WriteString(jsonContents);
//                    await textWriter.StoreAsync();
//                }
//            }

//        }

//        public async Task<T> LoadFromStorage()
//        {
//            StorageFolder localFolder = ApplicationData.Current.RoamingFolder;
//            try
//            {
//                // Getting JSON from file if it exists, or file not found exception if it does not  
//                StorageFile textFile = await localFolder.GetFileAsync(fileName);
//                using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
//                {
//                    // Read text stream     
//                    using (DataReader textReader = new DataReader(textStream))
//                    {
//                        //get size                       
//                        uint textLength = (uint)textStream.Size;
//                        await textReader.LoadAsync(textLength);
//                        // read it                    
//                        string jsonContents = textReader.ReadString(textLength);
//                        // deserialize back to our product!  
//                        return JsonConvert.DeserializeObject<T>(jsonContents);

//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return default(T);
//            }
//        }

//        public async Task DeleteStorage()
//        {
//            StorageFolder localFolder = ApplicationData.Current.RoamingFolder;
//            try
//            {
//                // Getting JSON from file if it exists, or file not found exception if it does not  
//                StorageFile textFile = await localFolder.GetFileAsync(fileName);

//                await textFile.DeleteAsync();

//            }
//            catch (Exception ex)
//            {

//            }
//        }
//    }
//}
