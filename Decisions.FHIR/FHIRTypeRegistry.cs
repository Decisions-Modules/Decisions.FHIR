using Decisions.FHIR.FlowSteps;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.DataStructure;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities;
using Hl7.Fhir.Model;
using System;
using System.IO;
using System.Linq;

namespace Decisions.FHIR
{
    public class FHIRTypeRegistry : IInitializable
    {
        void IInitializable.Initialize()
        {
            FHIRLog.LOG.Debug("Registering all FHIR Types");
            try
            {
                Type[] allFHIRTypes = typeof(DomainResource).Assembly.GetTypes();
                Type[] resources = allFHIRTypes.Where(
                    x => x.BaseType == typeof(DomainResource)
                        || typeof(Hl7.Fhir.Model.Element).IsAssignableFrom(x)
                        || typeof(Hl7.Fhir.Model.Resource).IsAssignableFrom(x)
                    ).ToArray();

                Type[] enumerations = allFHIRTypes.Where(
                        x => x.IsEnum == true
                    ).ToArray();

                ORM<NativeDataType> knownTypeORM = new ORM<NativeDataType>();
                NativeDataType[] allTypes = knownTypeORM.Fetch();
                foreach (NativeDataType eachTypeToRemove in allTypes.Where(x => x.DataTypeNameSpace.Equals(FHIRTypeUtil.FHIR_TYPE_PREFIX)))
                {
                    if (resources.FirstOrDefault(x => x.FullName.Equals(eachTypeToRemove.DataTypeFullName)) == null 
                        && enumerations.FirstOrDefault(x => x.FullName.Equals(eachTypeToRemove.DataTypeFullName)) == null)
                    {
                        FHIRLog.LOG.Debug("Removing data type no longer in use {0}", eachTypeToRemove.DataTypeFullName);
                        knownTypeORM.Delete(eachTypeToRemove);
                    }
                }
                
                foreach (Type t in resources)
                {
                    // Only register ones that are new!
                    if (allTypes.FirstOrDefault(x => x.DataTypeFullName == t.FullName) == null)
                    {
                        FHIRLog.LOG.Debug("Registering new datatype: {0}", t.FullName);
                        TypeUtilities.RegisterNativeType(t);
                    }
                }

                foreach (Type t in enumerations)
                {
                    // Only register ones that are new!
                    if (allTypes.FirstOrDefault(x => x.DataTypeFullName == t.FullName) == null)
                    {
                        FHIRLog.LOG.Debug("Registering new datatype: {0}", t.FullName);
                        TypeUtilities.RegisterNativeType(t);
                    }
                }
                
                // Customers reporting they were able to use SimpleQuantity in 6x but not 7x.
                // This was removed from the Firely FHIR library 5/2021 and Quantity should be used instead now.
                TypeUtilities.RenameType("Hl7.Fhir.Model.SimpleQuantity","Hl7.Fhir.Model.Quantity");
            }
            catch (Exception ex)
            {
                FHIRLog.LOG.Warn("Error registering FHIR types {0}", ex.ToString());
            }

            try
            {
                TryInstallImageIcons();
            }
            catch
            {
                // Could not load FHIR icon, not important.
            }
        }

        private void TryInstallImageIcons()
        {
            string basePath = Path.GetDirectoryName(Settings.SettingsFilePath);
            string iconFilePathToWrite = Path.Combine(basePath, "..", "images", "flow step images", "type_fhir.svg");

            if (File.Exists(iconFilePathToWrite) == false)
            {
                byte[] fhirIcon = LoadBytesFromResource("Decisions.FHIR.type_fhir.svg");
                byte[] fhirIconWhite = LoadBytesFromResource("Decisions.FHIR.type_fhir_white.svg");
                File.WriteAllBytes(iconFilePathToWrite, fhirIcon);
                iconFilePathToWrite = Path.Combine(basePath, "..", "images", "flow step images", "type_fhir_white.svg");
                File.WriteAllBytes(iconFilePathToWrite, fhirIconWhite);
            }
        }

        internal static byte[] LoadBytesFromResource(string resourceName)
        {
            var assembly = typeof(FHIRTypeRegistry).Assembly;
            byte[] buffer = new byte[16 * 1024];

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}
