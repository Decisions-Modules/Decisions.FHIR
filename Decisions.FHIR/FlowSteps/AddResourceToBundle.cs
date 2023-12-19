using DecisionsFramework;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Decisions.FHIR.FlowSteps
{
    [Writable]
    [AutoRegisterStep("Add Resource to Bundle", "Integration/FHIR")]
    [ShapeImageAndColorProvider(null, "flow step images|type_fhir.svg")]
    public class AddResourceToBundle : ISyncStep, IDataConsumer
    {
        const string IN_BUNDLE = "FHIR Bundle";
        const string IN_RESOURCE = "Resource to Add";
        const string IN_RESOURCE_URL = "Resource URL";

        const string OUT_BUNDLE = "FHIR Bundle Result";
        const string PATH_DONE = "Done";

        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                List<OutcomeScenarioData> outcomes = new List<OutcomeScenarioData>();

                outcomes.Add(new OutcomeScenarioData(PATH_DONE,
                    new DataDescription[] {
                        new DataDescription(typeof(Bundle), OUT_BUNDLE)
                    }));
                return outcomes.ToArray();
            }
        }

        public DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] {
                    new DataDescription(typeof(Bundle), IN_BUNDLE),
                    new DataDescription(typeof(Resource), IN_RESOURCE),
                    new DataDescription(typeof(Resource), IN_RESOURCE_URL),
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            Bundle fhirBundle = data[IN_BUNDLE] as Bundle;
            if (fhirBundle == null)
            {
                throw new BusinessRuleException($"{IN_BUNDLE} is not a valid FHIR Bundle object. Please use Read Bundle from JSON Step");
            }

            Resource newResource = data[IN_RESOURCE] as Resource;

            if (newResource == null) {
                throw new BusinessRuleException($"{IN_RESOURCE} is not a valid FHIR Resource object. Please add a valid resource to bundle");
            }

            string url = data[IN_RESOURCE_URL] as string;
            if (string.IsNullOrEmpty(url)) {
                url = $"http://hl7.org/fhir/{newResource.TypeName}/";
            }

            fhirBundle.AddResourceEntry(newResource, url);

            return new ResultData(PATH_DONE, new DataPair[] {
                new DataPair(OUT_BUNDLE, fhirBundle)
            });
        }
    }
}
