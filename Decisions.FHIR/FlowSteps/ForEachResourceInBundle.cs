using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.CoreSteps;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Properties;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace Decisions.FHIR.FlowSteps
{
    [Writable]
    [AutoRegisterStep("For Each Resource in FHIR Bundle", "Integration/FHIR")]
    [ShapeImageAndColorProvider(null, "flow step images|type_fhir.svg")]
    public class ForEachResourceInFHIRBundle : BaseFlowAwareStep, ISyncStep, IDataConsumer
    {
        const string IN_BUNDLE = "FHIR Bundle";
        const string OUT_NEXT_RESOURCE = "Next Resource";
        const string PATH_NEXT_RESOURCE = "Next";
        const string PATH_DONE = "Done";

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                List<OutcomeScenarioData> outcomes = new List<OutcomeScenarioData>();
                outcomes.Add(new OutcomeScenarioData(PATH_NEXT_RESOURCE, 
                    new DataDescription(typeof(Resource), OUT_NEXT_RESOURCE)));
                outcomes.Add(new OutcomeScenarioData(PATH_DONE));
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
            PrimaryFlowTrackingData trackingData = FlowEngine.GetTopFlowTrackingData(data.FlowTrackingID);
            int index;

            string key = this.FlowStep.Id + data.FlowTrackingID;
            object iterator = trackingData.GetExecutionData(key);
            if (iterator == null)
            {
                index = 0;
                trackingData.SetExecutionData(key, index);
            }
            else
            {
                index = ((int)iterator);
                index++;
                trackingData.SetExecutionData(key, index);
            }

            // Get Envelope from Flow
            Bundle myFHIRBundle = data.Data[IN_BUNDLE] as Bundle;

            if (myFHIRBundle != null)
            {
                Resource[] allResources = myFHIRBundle.GetResources().ToArray();
                if (allResources != null && allResources.Length > index) // there are any more pieces
                {
                    Dictionary<string, object> outputData = new Dictionary<string, object>();
                    Resource r = allResources[index];

                    outputData[OUT_NEXT_RESOURCE] = r;
                    return new ResultData(PATH_NEXT_RESOURCE, outputData);

                }
            }

            return new ResultData(PATH_DONE);
        }
    }
}