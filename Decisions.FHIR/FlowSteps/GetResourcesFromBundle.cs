using DecisionsFramework;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;
using DecisionsFramework.Design.Properties;

namespace Decisions.FHIR.FlowSteps
{
    [Writable]
    [AutoRegisterStep("Get Resources from Bundle", "Integration/FHIR")]
    [ShapeImageAndColorProvider(null, "flow step images|type_fhir.svg")]
    public class GetResourcesFromBundle : ISyncStep, IDataConsumer
    {
        const string IN_BUNDLE = "FHIR Bundle";
        const string OUT_RES_LIST = "Resource List";
        const string PATH_DONE = "Done";

        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                List<OutcomeScenarioData> outcomes = new List<OutcomeScenarioData>();

                outcomes.Add(new OutcomeScenarioData(PATH_DONE,
                    new DataDescription[] {
                        new DataDescription(typeof(Resource), OUT_RES_LIST, true)
                    }));
                return outcomes.ToArray();
            }
        }

        public DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] {
                    new DataDescription(typeof(Bundle), IN_BUNDLE)
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

            Resource[] allResources = fhirBundle.GetResources().ToArray();

            return new ResultData(PATH_DONE, new DataPair[] {
                new DataPair(OUT_RES_LIST, allResources)
            });
        }


    }
}
