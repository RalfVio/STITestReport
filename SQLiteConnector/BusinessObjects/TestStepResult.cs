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
                Step currentStep = null; string currentElement = null;

                using (TextReader xmlReader = new StringReader(TestCaseStepsXML))
                using (var reader = System.Xml.XmlReader.Create(xmlReader))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case System.Xml.XmlNodeType.Element:
                                currentElement = reader.Name;
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
                                break;


                            case System.Xml.XmlNodeType.Text:
                                if (currentStep != null && string.Compare(currentElement, "ParameterizedString", true) == 0)
                                {
                                    switch (currentStep.PSNumber)
                                    {
                                        case 0:
                                            currentStep.Instructions = reader.Value;
                                            currentStep.PSNumber++;
                                            break;
                                        case 1:
                                            currentStep.ExpectedResult = reader.Value;
                                            currentStep.PSNumber++;
                                            break;
                                        default:
                                            throw new NotImplementedException($"Step has {currentStep.PSNumber} paramaterized strings (more than 2)");
                                    }
                                }
                                break;

                            case System.Xml.XmlNodeType.EndElement:
                                currentElement = null;
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
                Step currentStep = null; string currentElement = null;

                using (TextReader xmlReader = new StringReader(SharedStepsXML))
                using (var reader = System.Xml.XmlReader.Create(xmlReader))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case System.Xml.XmlNodeType.Element:
                                currentElement = reader.Name;
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
                                break;


                            case System.Xml.XmlNodeType.Text:
                                if (currentStep != null && string.Compare(currentElement, "ParameterizedString", true) == 0)
                                {
                                    switch (currentStep.PSNumber)
                                    {
                                        case 0:
                                            currentStep.Instructions = reader.Value;
                                            currentStep.PSNumber++;
                                            break;
                                        case 1:
                                            currentStep.ExpectedResult = reader.Value;
                                            currentStep.PSNumber++;
                                            break;
                                        default:
                                            throw new NotImplementedException($"Step has {currentStep.PSNumber} paramaterized strings (more than 2)");
                                    }
                                }
                                break;

                            case System.Xml.XmlNodeType.EndElement:
                                currentElement = null;
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
        public string InstructionsText { get { return HtmlDecode(Instructions); } }
        public string ExpectedResult { get; set; }
        public string ExpectedResultText { get { return HtmlDecode(ExpectedResult); } }
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

        private static string HtmlDecode(string html)
        {
            if (html == null)
                return string.Empty;
            string attribute = null; System.Text.StringBuilder result = new System.Text.StringBuilder();
            for (int i = 0; i < html.Length; i++)
            {
                char c = html[i];
                switch (c)
                {
                    case '<': attribute = ""; break;
                    case '>':
                        if (attribute == null)
                            result.Append('>');
                        else
                        {
                            if (attribute.StartsWith("br", System.StringComparison.CurrentCultureIgnoreCase)) result.Append('\n');
                            if (attribute.StartsWith("p ", System.StringComparison.CurrentCultureIgnoreCase) && result.Length > 0) result.Append('\n');
                        }
                        attribute = null; break;
                    case '&':
                        if (attribute == null)
                        {
                            if (html.Length >= i + 4 && html.Substring(i, 4) == "&gt;")
                            { result.Append('>'); i += 4 - 1; }
                            else if (html.Length >= i + 4 && html.Substring(i, 4) == "&lt;")
                            { result.Append('<'); i += 4 - 1; }
                            else if (html.Length >= i + 6 && html.Substring(i, 6) == "&nbsp;")
                            { result.Append(' '); i += 6 - 1; }
                        }
                        else
                            attribute += c;
                        break;

                    default:
                        if (attribute == null)
                        {
                            result.Append(c);
                        }
                        else
                        {
                            attribute += c;
                        }
                        break;
                }
            }

            return result.ToString();
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