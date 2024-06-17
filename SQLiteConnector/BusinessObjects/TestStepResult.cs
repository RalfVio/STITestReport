using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SQLite.BusinessObjects
{
    public class TestStepResult
    {
        public int DBId { get; set; }
        public int RunId { get; set; }
        public int ResultId { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public string Outcome { get; set; }
        public string OutcomeExternal() { return Outcome; }
        public int Revision { get; set; }
        public string State { get; set; }
        public int TestCaseId { get; set; }
        public int TestCaseRevision { get; set; }
        public string TestCaseTitle { get; set; }
        public string RunBy { get; set; }
        public List<int> AssociatedWIIds { get; set; }
        public int TestPointId { get; set; }
        public string TestCaseStepsXML { get; set; }
        public string TestCaseState { get; set; }
        public string TestCaseDescription { get; set; }
        public string TestCaseDescriptionText { get { return SQLiteConnector.Helpers.HtmlDecode(TestCaseDescription); } }
        public bool HasSharedSteps { get; set; }
        public string Comment { get; set; }

        public class EquipmentData 
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }
        public List<EquipmentData> EquipmentRecordsGet(string sep)
        {
            if (EquipmentRecords == null || EquipmentRecords.Count == 0)
                return null;

            var result = new List<EquipmentData>();
            foreach (var equipmentRecord in EquipmentRecords)
            {
                var equipmentData = new EquipmentData();
                var sb = new StringBuilder();
                bool first = true;
                foreach (var line in equipmentRecord.Data.Replace("\r", "").Split('\n'))
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (first)
                    {
                        first = false;
                        equipmentData.Title = line.Trim();
                        if (equipmentData.Title.EndsWith(':'))
                            equipmentData.Title = equipmentData.Title.Substring(0, equipmentData.Title.Length - 1);
                    }
                    else
                        sb.Append((sb.Length == 0 ? "" : sep) + line);
                }
                equipmentData.Description = sb.ToString();
                result.Add(equipmentData);
            }
            return result;
        }
        public int BuildId { get; set; }
        public string BuildName { get; set; }
        public List<TestResultIteration> Iterations { get; set; }
        public List<SharedSteps> SharedSteps { get; set; }
        public List<EquipmentRecord> EquipmentRecords { get; set; }
        public TestRun TestRun { get; set; }

        public List<Step> GetTestCaseSteps()
        {
            try
            {
                var result = new List<Step>();
                Step currentStep = null;
                var nodeHierarchy = new List<string>();

                using (TextReader xmlReader = new StringReader(TestCaseStepsXML))
                using (var reader = System.Xml.XmlReader.Create(xmlReader))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case System.Xml.XmlNodeType.Element:
                                
                                while (nodeHierarchy.Count>0 && nodeHierarchy.Count > reader.Depth)
                                    nodeHierarchy.RemoveAt(nodeHierarchy.Count - 1);
                                nodeHierarchy.Add(reader.Name);

                                if (string.Compare(reader.Name, "Step", true) == 0)
                                {
                                    currentStep = new Step(); result.Add(currentStep);
                                    if (reader.HasAttributes)
                                    {
                                        if (int.TryParse(reader.GetAttribute("id"), out int id))
                                            currentStep.Id = id;
                                        switch (reader.GetAttribute("type"))
                                        {
                                            default:
                                                currentStep.TypeOfStep = Step.TypesOfStep.TestStep; break;
                                        }
                                    }
                                }
                                else if (string.Compare(reader.Name, "compref", true) == 0)
                                {
                                    currentStep = new Step(); result.Add(currentStep);
                                    if (reader.HasAttributes)
                                    {
                                        if (int.TryParse(reader.GetAttribute("id"), out int id))
                                            currentStep.Id = id;
                                        if (int.TryParse(reader.GetAttribute("ref"), out int sharedStepsId))
                                            currentStep.SharedStepsId = sharedStepsId;
                                        currentStep.TypeOfStep = Step.TypesOfStep.SharedStepsRef;
                                    }
                                }
                                else if (string.Compare(reader.Name, "ParameterizedString", true)==0 && nodeHierarchy.Count() == 3 && currentStep != null)
                                {
                                    currentStep.PSNumber++;

                                    switch (currentStep.PSNumber)
                                    {
                                        case 1:
                                            if (!string.IsNullOrEmpty(reader.Value))
                                                currentStep.Instructions = reader.Value;
                                            break;
                                        case 2:
                                            if (!string.IsNullOrEmpty(reader.Value))
                                                currentStep.ExpectedResult = reader.Value;
                                            break;
                                    }
                                }
                                break;


                            case System.Xml.XmlNodeType.Text:
                                if ( nodeHierarchy.Count > 2 && string.Compare(nodeHierarchy[2], "ParameterizedString", true) == 0 && currentStep != null)
                                {
                                    switch (currentStep.PSNumber)
                                    {
                                        case 1:
                                            if (!string.IsNullOrEmpty(reader.Value))
                                                currentStep.Instructions = reader.Value;
                                            break;

                                        case 2:
                                            if (!string.IsNullOrEmpty(reader.Value))
                                                currentStep.ExpectedResult = reader.Value;
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }

                for (int i = 0; i < result.Count; i++)
                    result[i].StepNumber = i + 1;

                return result;
            }

            catch { return null; }
        }

    }

    public class SharedSteps
    {
        public int Id { get; set; }
        public int Revision { get; set; }
        public string Title { get; set; }
        public string SharedStepsXML { get; set; }
        public int StepId { get; set; }
        public List<Step> GetSteps(Step sharedStepRef)
        {
            try
            {
                var result = new List<Step>();
                Step currentStep = null;
                var nodeHierarchy = new List<string>();

                using (TextReader xmlReader = new StringReader(SharedStepsXML))
                using (var reader = System.Xml.XmlReader.Create(xmlReader))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case System.Xml.XmlNodeType.Element:
                                while (nodeHierarchy.Count > 0 && nodeHierarchy.Count > reader.Depth)
                                    nodeHierarchy.RemoveAt(nodeHierarchy.Count - 1);
                                nodeHierarchy.Add(reader.Name);

                                if (string.Compare(reader.Name, "Step", true) == 0)
                                {
                                    currentStep = new Step() { TypeOfStep = Step.TypesOfStep.TestStepInSharedSteps, StepNumber = sharedStepRef.StepNumber, Id = sharedStepRef.Id }; result.Add(currentStep);
                                    if (reader.HasAttributes)
                                    {
                                        if (int.TryParse(reader.GetAttribute("id"), out int id))
                                            currentStep.SharedStepId = id;
                                        if (int.TryParse(reader.GetAttribute("ref"), out int sharedStepsId))
                                            currentStep.SharedStepsId = sharedStepsId;
                                    }
                                }
                                else if (string.Compare(reader.Name, "ParameterizedString", true) == 0 && nodeHierarchy.Count() == 3 && currentStep != null)
                                {
                                    currentStep.PSNumber++;

                                    switch (currentStep.PSNumber)
                                    {
                                        case 1:
                                            if (!string.IsNullOrEmpty(reader.Value))
                                                currentStep.Instructions = reader.Value;
                                            break;
                                        case 2:
                                            if (!string.IsNullOrEmpty(reader.Value))
                                                currentStep.ExpectedResult = reader.Value;
                                            break;
                                    }
                                }
                                break;


                            case System.Xml.XmlNodeType.Text:
                                if (nodeHierarchy.Count > 2 && string.Compare(nodeHierarchy[2], "ParameterizedString", true) == 0 && currentStep != null)
                                {
                                    switch (currentStep.PSNumber)
                                    {
                                        case 1:
                                            if (!string.IsNullOrEmpty(reader.Value))
                                                currentStep.Instructions = reader.Value;
                                            break;

                                        case 2:
                                            if (!string.IsNullOrEmpty(reader.Value))
                                                currentStep.ExpectedResult = reader.Value;
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }

                for (int i = 0; i < result.Count; i++)
                    result[i].SharedStepNumber = i + 1;

                return result;
            }

            catch { return null; }
        }
    }
    public class Step
    {
        public enum TypesOfStep { TestStep, SharedStepsRef, TestStepInSharedSteps };
        public int Id { get; set; }
        public TypesOfStep TypeOfStep { get; set; }
        public int SharedStepsId { get; set; }
        public string Instructions { get; set; }
        public string InstructionsText { get { return SQLiteConnector.Helpers.HtmlDecode(Instructions); } }
        public string ExpectedResult { get; set; }
        public string ExpectedResultText { get { return SQLiteConnector.Helpers.HtmlDecode(ExpectedResult); } }
        public string Outcome { get; set; }
        public string Comment { get; set; }
        public List<TestResultParameter> Parameters { get; set; }
        public int PSNumber { get; set; }

        public int StepNumber { get; set; }
        public int SharedStepNumber { get; set; }
        public int SharedStepId { get; set; }

        public string GetStepNumber()
        {
            switch (TypeOfStep)
            {
                case Step.TypesOfStep.TestStepInSharedSteps:
                    return $"{StepNumber}.{SharedStepNumber}";
                default: return $"{StepNumber}";
            }
        }
        public string GetStepIdentifier()
        {
            switch (TypeOfStep)
            {
                case Step.TypesOfStep.TestStepInSharedSteps:
                    return $"{Id};{SharedStepId}";
                default: return $"{Id}";
            }
        }

        
        public bool IsPassedOrFailed()
        {
            switch (Outcome)
            {
                case "Passed":
                case "Failed":
                    return true;
                default:
                    return false;
            }
        }
    }

    public class TestResultIteration
    {
        public int Id { get; set; }
        public string Outcome { get; set; }
        public string Comment { get; set; }

        public string ActionResultsJson { get; set; }
        public List<TestResultActionResult> GetActionResults()
        {
            if (string.IsNullOrEmpty(ActionResultsJson))
                return null;
            dynamic actionResultsJObject = JObject.Parse("{\"actionResults\":" + ActionResultsJson + "}");

            var result = new List<TestResultActionResult>();
            int actionResultsCount = actionResultsJObject.actionResults.Count;
            for (int i = 0; i < actionResultsCount; i++)
            {
                dynamic actionResultJObject = actionResultsJObject.actionResults[i];
                if (actionResultJObject.sharedStepModel != null)
                    // Shared Steps reference
                    result.Add(new TestResultActionResult()
                    {
                        ActionPath = actionResultJObject.actionPath ?? "",
                        StepIdentifier = actionResultJObject.stepIdentifier ?? "",
                        Outcome = actionResultJObject.outcome ?? "",
                        ErrorMessage = actionResultJObject.errorMessage ?? "",
                        IterationId = actionResultJObject.iterationId ?? 0,
                        SharedStepsId = actionResultJObject.sharedStepModel.id,
                        SharedStepsRevision = actionResultJObject.sharedStepModel.revision,
                    });
                else
                    result.Add(new TestResultActionResult()
                    {
                        ActionPath = actionResultJObject.actionPath ?? "",
                        StepIdentifier = actionResultJObject.stepIdentifier ?? "",
                        Outcome = actionResultJObject.outcome ?? "",
                        ErrorMessage = actionResultJObject.errorMessage ?? "",
                        IterationId = actionResultJObject.iterationId ?? 0,
                    });

            }
            return result;
        }

        public string ParametersJson { get; set; }
        public List<TestResultParameter> GetParameterResults()
        {
            if (string.IsNullOrEmpty(ParametersJson))
                return null;
            dynamic parametersJObject = JObject.Parse("{\"parameters\":" + ParametersJson + "}");

            var result = new List<TestResultParameter>();
            int parametersCount = parametersJObject.parameters.Count;
            for (int i = 0; i < parametersCount; i++)
            {
                dynamic parameterJObject = parametersJObject.parameters[i];
                result.Add(new TestResultParameter()
                {
                    ParameterName = parameterJObject.parameterName ?? "",
                    Value = parameterJObject.value ?? "",
                    StepIdentifier = parameterJObject.stepIdentifier ?? "",
                    IterationId = parameterJObject.iterationId ?? 0,
                });
            }
            return result;
        }
    }
    public class TestResultActionResult
    {
        public string ActionPath { get; set; }
        public string StepIdentifier { get; set; }
        public string Outcome { get; set; }
        public string ErrorMessage { get; set; }
        public int IterationId { get; set; }
        public int SharedStepsId { get; set; }
        public int SharedStepsRevision { get; set; }
    }
    public class TestResultParameter
    {
        public string ParameterName { get; set; }
        public string Value { get; set; }
        public string StepIdentifier { get; set; }
        public int IterationId { get; set; }
    }

    public class EquipmentRecord
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Data { get; set; }

    }

}