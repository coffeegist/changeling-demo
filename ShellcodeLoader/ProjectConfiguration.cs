using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace ShellcodeLoader
{
    [DataContract]
    class ProjectConfiguration
    {
        [DataMember]
        public string TARGET_PROCESS { get; set; }

        public static byte[] GetEmbeddedResource(string resourceName)
        {
            byte[] result; 

            var assembly = Assembly.GetExecutingAssembly();
            resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(resourceName));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    result = reader.ReadBytes((int)stream.Length);
                }
            }

            return result;
        }

        public static ProjectConfiguration GetEmbeddedSettings()
        {
            byte[] embeddedBytes = GetEmbeddedResource("projectConfiguration.json");
            embeddedBytes = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(embeddedBytes, 0, embeddedBytes.Length));

            ProjectConfiguration deserializedSettings = new ProjectConfiguration();
            var ms = new MemoryStream(embeddedBytes);
            var ser = new DataContractJsonSerializer(deserializedSettings.GetType());
            deserializedSettings = ser.ReadObject(ms) as ProjectConfiguration;
            ms.Close();

            return deserializedSettings;
        }
    }
}
