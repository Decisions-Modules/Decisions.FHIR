﻿using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer.Services.ContextData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Decisions.FHIR.FlowSteps
{
    [Writable]
    [AutoRegisterStep("Cast FHIR Resource to FHIR Type", "Integration/FHIR")]
    [ShapeImageAndColorProvider(null, "flow step images|type_fhir.svg")]
    public class CastFHIRResourceToType : ISyncStep, IDataConsumer, IDataProducer, INotifyPropertyChanged
    {
        const string IN_RESOURCE = "FHIR Resource";
        const string OUT_RES_ITEM = "FHIR Resource Result";
        const string PATH_DONE = "Done";
        const string PATH_INCORRECT = "Incorrect Type";

        private FhirVersion fhirVersion = FhirVersion.STU3;

        [WritableValue]
        public FhirVersion FhirVersion
        {
            get => fhirVersion;
            set
            {
                fhirVersion = value;
                OnPropertyChanged(nameof(FHIRTypeName));
            }
        }

        private string fhirTypeName;

        [WritableValue]
        [SelectStringEditor("TypesToShow")]
        public string FHIRTypeName
        {
            get => fhirTypeName;
            set
            {
                fhirTypeName = value;
                OnPropertyChanged(nameof(OutcomeScenarios));
            }
        }

        [PropertyHidden]
        public Type FHIRTypeToCast => FHIRTypeUtil.GetTypeFromSimpleName(FHIRTypeName, fhirVersion);

        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                List<OutcomeScenarioData> outcomes = new List<OutcomeScenarioData>();
                if (string.IsNullOrEmpty(FHIRTypeName) == false)
                {
                    outcomes.Add(
                        new OutcomeScenarioData(PATH_DONE, new DataDescription[]
                        {
                            new DataDescription(FHIRTypeToCast, OUT_RES_ITEM)
                        }));
                }
                outcomes.Add(new OutcomeScenarioData(PATH_INCORRECT));
                return outcomes.ToArray();
            }
        }

        public DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] {
                    new DataDescription(typeof(object), IN_RESOURCE)
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            object resultElement = data[IN_RESOURCE];

            if (FHIRTypeToCast.IsAssignableFrom(resultElement.GetType())) 
            {
                return new ResultData(PATH_DONE, new DataPair[] 
                {
                    new DataPair(OUT_RES_ITEM, resultElement)
                });
            }

            return new ResultData(PATH_INCORRECT);
        }

        [PropertyHidden]
        public string[] TypesToShow => FHIRTypeUtil.GetSimpleTypeNames(fhirVersion);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
