extern alias stu3;
extern alias r4;
extern alias r4b;
extern alias r5;

using Hl7.Fhir.Model;
using System;
using System.Linq;

using Stu3 = stu3::Hl7.Fhir.Model;
using R4 = r4::Hl7.Fhir.Model;
using R4B = r4b::Hl7.Fhir.Model;
using R5 = r5::Hl7.Fhir.Model;

namespace Decisions.FHIR.FlowSteps
{
    internal class FHIRTypeUtil
    {
        public const string FHIR_TYPE_PREFIX = "Hl7.Fhir.Model";

        internal static string[] GetSimpleTypeNames(FhirVersion version)
        {
            Type[] allVersionSpecificFhirTypes = GetTypesForVersion(version);
            Type[] resources = allVersionSpecificFhirTypes.Where(x => x.BaseType == typeof(DomainResource)).ToArray();
            return resources.Select(x => x.Name).ToArray();
        }

        internal static Type GetTypeFromSimpleName(string fhirTypeName, FhirVersion version)
        {
            if (string.IsNullOrEmpty(fhirTypeName) == false) 
            {
                Type[] allVersionSpecificFhirTypes = GetTypesForVersion(version);
                Type t = allVersionSpecificFhirTypes.FirstOrDefault(type => type.Name == fhirTypeName);
                return t;
            }
            
            return null;
        }

        internal static Type[] GetTypesForVersion(FhirVersion version)
        {
            Type[] allVersionSpecificFhirTypes = Array.Empty<Type>();
            
            switch (version)
            {
                case FhirVersion.STU3:
                    allVersionSpecificFhirTypes = typeof(DomainResource).Assembly.GetTypes().Concat(typeof(Stu3.AdverseEvent).Assembly.GetTypes()).ToArray();
                    break;
                case FhirVersion.R4:
                    allVersionSpecificFhirTypes = typeof(DomainResource).Assembly.GetTypes().Concat(typeof(R4.AdverseEvent).Assembly.GetTypes()).ToArray();
                    break;
                case FhirVersion.R4B:
                    allVersionSpecificFhirTypes = typeof(DomainResource).Assembly.GetTypes().Concat(typeof(R4B.AdverseEvent).Assembly.GetTypes()).ToArray();
                    break;
                case FhirVersion.R5:
                    allVersionSpecificFhirTypes = typeof(DomainResource).Assembly.GetTypes().Concat(typeof(R5.AdverseEvent).Assembly.GetTypes()).ToArray();
                    break;
            }

            return allVersionSpecificFhirTypes;
        }
    }
}