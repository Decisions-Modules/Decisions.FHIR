extern alias stu3;
extern alias r4;
extern alias r4b;
extern alias r5;

using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.StepImplementations;
using Hl7.Fhir.Model;
using Newtonsoft.Json;
using System.IO;

using Stu3 = stu3::Hl7.Fhir.Serialization;
using R4 = r4::Hl7.Fhir.Serialization;
using R4B = r4b::Hl7.Fhir.Serialization;
using R5 = r5::Hl7.Fhir.Serialization;

namespace Decisions.FHIR.FlowSteps
{
    [AutoRegisterMethodsOnClass(true, "Integration/FHIR")]
    [ShapeImageAndColorProvider(null, "flow step images|type_fhir.svg")]
    public class FHIRDeserializeSteps
    {
        public Bundle ReadBundleFromJSONData(string jsonFHIRData, FhirVersion version = FhirVersion.STU3)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(jsonFHIRData));
            Bundle b = null;

            switch (version)
            {
                case FhirVersion.STU3:
                    b = new Stu3.FhirJsonParser().Parse<Bundle>(reader);
                    break;
                case FhirVersion.R4:
                    b = new R4.FhirJsonParser().Parse<Bundle>(reader);
                    break;
                case FhirVersion.R4B:
                    b = new R4B.FhirJsonParser().Parse<Bundle>(reader);
                    break;
                case FhirVersion.R5:
                    b = new R5.FhirJsonParser().Parse<Bundle>(reader);
                    break;
            }

            return b;
        }

        public string GetJsonForFHIRResource(Resource FHIRResource, FhirVersion version = FhirVersion.STU3) 
        {
            switch (version)
            {
                case FhirVersion.STU3:
                    return new Stu3.FhirJsonSerializer().SerializeToString(FHIRResource);
                case FhirVersion.R4:
                    return new R4.FhirJsonSerializer().SerializeToString(FHIRResource);
                case FhirVersion.R4B:
                    return new R4B.FhirJsonSerializer().SerializeToString(FHIRResource);
                case FhirVersion.R5:
                    return new R5.FhirJsonSerializer().SerializeToString(FHIRResource);
            }

            return string.Empty;
        }

        public string GetJsonForFHIRBundle(Bundle FHIRBundle, FhirVersion version = FhirVersion.STU3) 
        {
            switch (version)
            {
                case FhirVersion.STU3:
                    return new Stu3.FhirJsonSerializer().SerializeToString(FHIRBundle);
                case FhirVersion.R4:
                    return new R4.FhirJsonSerializer().SerializeToString(FHIRBundle);
                case FhirVersion.R4B:
                    return new R4B.FhirJsonSerializer().SerializeToString(FHIRBundle);
                case FhirVersion.R5:
                    return new R5.FhirJsonSerializer().SerializeToString(FHIRBundle);
            }

            return string.Empty;
        }

        public Resource ReadResourceFromJson(string jsonFHIRData, FhirVersion version = FhirVersion.STU3) 
        {
            switch (version)
            {
                case FhirVersion.STU3:
                    return new Stu3.FhirJsonParser().Parse<Resource>(jsonFHIRData);
                case FhirVersion.R4:
                    return new R4.FhirJsonParser().Parse<Resource>(jsonFHIRData);
                case FhirVersion.R4B:
                    return new R4B.FhirJsonParser().Parse<Resource>(jsonFHIRData);
                case FhirVersion.R5:
                    return new R5.FhirJsonParser().Parse<Resource>(jsonFHIRData);
            }

            return null;
        }

        public string GetJsonForFHIRObject(Base FHIRBaseObject, FhirVersion version = FhirVersion.STU3) 
        {
            switch (version)
            {
                case FhirVersion.STU3:
                    return new Stu3.FhirJsonSerializer().SerializeToString(FHIRBaseObject);
                case FhirVersion.R4:
                    return new R4.FhirJsonSerializer().SerializeToString(FHIRBaseObject);
                case FhirVersion.R4B:
                    return new R4B.FhirJsonSerializer().SerializeToString(FHIRBaseObject);
                case FhirVersion.R5:
                    return new R5.FhirJsonSerializer().SerializeToString(FHIRBaseObject);
            }

            return string.Empty;
        }
    }
}
