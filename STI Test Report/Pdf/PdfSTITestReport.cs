using System;
using System.Collections.Generic;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Linq;
using SQLite.BusinessObjects;

namespace STI_Test_Report.OutPdf
{
    class PdfSTITestReport
    {
        #region Commons
        PdfDocument _doc = null;
        XGraphics _page = null;

        const double xPageLeft = 20; const double xTab2 = xPageLeft + 10; const double xTab2_1 = xTab2 + 10; const double xTab3 = xPageLeft + 20;  const double xPageRight = 8.5 * 25.4 - 10; //8.5
        const double deltaY = 5; const double deltaY2 = 7; const double deltaY3 = 9; const double deltaY4 = 11;
        double _yPos = 0;
        const double yPageTop = 28; const double yPageBottom = 11 * 25.4 - 25;

        const double xTabStartTestSuite = xPageLeft;
        const double xTabStartTestPoint = xPageLeft + 3;
        const double xTabStartTestResult = xPageLeft + 6;
        const double xTabStartTestAction = xTabStartTestResult;
        const double xTabStartAssociatedWorkItems = xTabStartTestResult;
        const double xTab1Label = xPageLeft + 15;
        const double xTab1Data = xTab1Label + 30;
        const double xTab2Label = xPageLeft + 100;
        const double xTab2Data = xTab2Label + 20;

        const double xTabOutcome = xPageRight - 15;
        //const double xTabOutcomeLabel = xTabOutcome - 25;

        XFont font1 = null;
        XFont boldFont = null;
        XFont headerFont = null;
        XFont sectionHeaderFont = null;
        TestPlan _testPlan = null;
        #endregion

        public PdfSTITestReport()
        {
            // Create a new PDF document 
            _doc = new PdfDocument();

            boldFont = new XFont("Arial", 4, XFontStyle.Bold);
            font1 = new XFont("Arial", 4, XFontStyle.Regular);
            //XFont underlineFont = new XFont("Arial", 4, XFontStyle.Underline);//| XFontStyle.Bold
            headerFont = new XFont("Arial", 8, XFontStyle.Bold);
            sectionHeaderFont = new XFont("Arial", 4, XFontStyle.Bold);

        }
        enum eLastPrinted {first,testSuite, testPoint, testResult ,testAction, associatedWorkItem, testActionAttachment }
        eLastPrinted lastPrinted = eLastPrinted.first;

        bool _showFullReport = true;
        public void PrintStart(TestPlan testPlan, int suiteMaxLevelInContentList, bool showFullReport)
        {
            _testPlan = testPlan; _suiteMaxLevelInContentList=suiteMaxLevelInContentList;
            _showFullReport = showFullReport;
            if (_showFullReport)
            {
                CreatePage();
                XStringFormat xstringFormat = new XStringFormat() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
                _page.DrawString("Test Case Report", headerFont, XBrushes.Black, .5 * (xPageLeft + xPageRight), .5 * (yPageTop + yPageBottom) - 2 * headerFont.Height, xstringFormat);
                _page.DrawString("for", headerFont, XBrushes.Black, .5 * (xPageLeft + xPageRight), .5 * (yPageTop + yPageBottom), xstringFormat);
                
                // Break name in multiple lines if length exceeds page width
                List<string> lines = SplitToMultiLine(string.Format("Test Plan '{0} ({1})'", _testPlan.Title, _testPlan.Id), xPageRight - xPageLeft, ref headerFont);
                for (int i = 0; i < lines.Count; i++)
                    _page.DrawString(lines[i].Trim(), headerFont, XBrushes.Black, .5 * (xPageLeft + xPageRight), .5 * (yPageTop + yPageBottom) + (2+ 1.2 * i) * headerFont.Height, xstringFormat);
            }
            CreatePage();
            lastPrinted = eLastPrinted.first;
        }
        string _suiteHierarchy = null;
        int _suiteMaxLevelInContentList = 0;
        bool IsNewSuiteNode(TestSuite testSuite)
        {
            int pos = 0;int level = 0;
            for (int i = 0; i < testSuite.SuitePath.Length; i++)
                if ( level< _suiteMaxLevelInContentList)
                {  if(testSuite.SuitePath[i] == '/') level++; else pos = i; }

            if (string.IsNullOrEmpty(_suiteHierarchy) || _suiteHierarchy != testSuite.SuitePath.Substring(0, pos+1))
            {

                _suiteHierarchy = testSuite.SuitePath.Length> pos?testSuite.SuitePath.Substring(0, pos+1) : "";
                return true;
            }

            return false;
        }
        public void Print(TestSuite testSuite)
        {
            if (_suiteMaxLevelInContentList > 0 && IsNewSuiteNode(testSuite))
                if (_showFullReport)
                    _doc.Outlines.Add(string.Format("{0}", _suiteHierarchy), _actualPage, false, PdfOutlineStyle.Regular);

            switch (lastPrinted)
            {
                case eLastPrinted.testResult:
                    NewLine(deltaY4 - deltaY2, 0); break;
                case eLastPrinted.testAction:
                    CloseActionTable();
                    NewLine(10, 0); break;
                case eLastPrinted.associatedWorkItem:
                    NewLine(10, 0); break;
            }
            _page.DrawString("Test Suite", sectionHeaderFont, XBrushes.Black, xTabStartTestSuite, _yPos);
            NewLine(sectionHeaderFont);

            List<string> lines = SplitToMultiLine(testSuite.SuitePath, xPageRight - xTab1Data, xPageRight - xTab1Data, ref font1,'/');
            for (int i = 0; i < lines.Count; i++)
            {
                if (i == 0)
                    _page.DrawString("Path:", font1, XBrushes.Black, xTab1Label, _yPos);
                else
                    NewLine();
                 _page.DrawString(lines[i], font1, XBrushes.Black, xTab1Data, _yPos);
            }

            NewLine(deltaY2, 0);
            lastPrinted = eLastPrinted.testSuite;
        }
        public void Print(TestPoint testPoint)
        {
            const double xTab1Data_2 = xTab1Label + 45;
            const double xTab1Data_3 = xTab1Data + 5;
            switch (lastPrinted)
            {
                case eLastPrinted.testResult:
                    NewLine(deltaY3 - deltaY, 0); break;
                case eLastPrinted.testAction:
                    CloseActionTable();
                    NewLine(10, 0); break;
                case eLastPrinted.associatedWorkItem:
                    NewLine(10, 0); break;
            }

            _page.DrawString("Test Point", sectionHeaderFont, XBrushes.Black, xTabStartTestPoint, _yPos);
            NewLine(sectionHeaderFont);
            List<string> testCaseTitleLines = SplitToMultiLine(testPoint.TestCaseTitle, xPageRight- xTab1Data_3 - 2, xPageRight - xTab1Data_2 - 2, ref font1,' ');
            _page.DrawString("Test Case:",font1,  XBrushes.Black, xTab1Label, _yPos); _page.DrawString(testPoint.TestCaseId.ToString(), font1, XBrushes.Black, xTab1Data, _yPos); _page.DrawString(testCaseTitleLines[0], font1, XBrushes.Black, xTab1Data_2, _yPos);
            for (int i = 1; i < testCaseTitleLines.Count; i++)
            { NewLine(); _page.DrawString(testCaseTitleLines[i], font1, XBrushes.Black, xTab1Data_3, _yPos); }
            NewLine();
            _page.DrawString("Configuration:", font1, XBrushes.Black, xTab1Label, _yPos); _page.DrawString(testPoint.ConfigurationTitle, font1, XBrushes.Black, xTab1Data, _yPos);
            //_page.DrawString("Outcome:", font1, XBrushes.Black, xTabOutcome-1 , _yPos,new XStringFormat() {Alignment=XStringAlignment.Far,LineAlignment=XLineAlignment.BaseLine }); _page.DrawString(testPoint.Result, font1, XBrushes.Black, xTabOutcome, _yPos);

            NewLine(deltaY2, 0);
            lastPrinted = eLastPrinted.testPoint;
        }

   
        public void Print(TestStepResult testResult, WorkItem testCase)
        {
            switch (lastPrinted)
            {
                case eLastPrinted.testAction:
                    CloseActionTable(); NewLine(10, 0); break;
                case eLastPrinted.associatedWorkItem:
                    NewLine(10, 0); break;
            }
            _page.DrawString("Test Result", sectionHeaderFont, XBrushes.Black, xTabStartTestResult, _yPos);
            NewLine(sectionHeaderFont);

            bool isError = false; // testResult.IsError();
            if (isError)
                _page.DrawRectangle(XBrushes.LightPink, xTabStartTestResult - 1, _yPos - deltaY + 1, xPageRight - xTabStartTestResult + 2, deltaY + 1);
            _page.DrawString("Execution Date:", font1, XBrushes.Black, xTab1Label, _yPos); _page.DrawString(string.Format("{0:dd-MMM-yyyy}", testResult.TestRun.CompletedDate), font1, XBrushes.Black, xTab1Data, _yPos);
            var tester = CutString(testResult.TestRun.OwnerDisplayName, xTabOutcome - xTab2Data-_page.MeasureString("Tester:",font1).Width-2, ref font1,null);
            _page.DrawString("Tester:", font1, XBrushes.Black, xTab2Label, _yPos); _page.DrawString(tester??"", font1, XBrushes.Black, xTab2Data, _yPos);
            _page.DrawString("Run Id:", font1, XBrushes.Black, xTabOutcome - 1, _yPos, new XStringFormat() { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.BaseLine }); _page.DrawString(testResult.TestRun.Id.ToString(), font1, XBrushes.Black, xTabOutcome, _yPos);
            NewLine();
            if (isError)
                _page.DrawRectangle(XBrushes.LightPink, xTabStartTestResult - 1, _yPos - deltaY + 1, xPageRight - xTabStartTestResult + 2, deltaY + 1);
            _page.DrawString("Build:", font1, XBrushes.Black, xTab1Label, _yPos); if(!string.IsNullOrEmpty(testResult.BuildName))_page.DrawString(CutString(testResult.BuildName, xTab2Label- xTab1Data-.5, ref font1,null), font1, XBrushes.Black, xTab1Data, _yPos);
            _page.DrawString("TC State:", font1, XBrushes.Black, xTab2Label, _yPos); if(!string.IsNullOrEmpty(testResult.TestCaseState)) _page.DrawString(testResult.TestCaseState, font1, XBrushes.Black, xTab2Data, _yPos);

            _page.DrawString("Outcome:", font1, XBrushes.Black, xTabOutcome - 1, _yPos, new XStringFormat() { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.BaseLine });_page.DrawString(testResult.OutcomeExternal(), font1, XBrushes.Black, xTabOutcome, _yPos);
            var equipmentRecords = testResult.EquipmentRecordsGet(", ");
            if (equipmentRecords != null)
            {
                foreach (var record in equipmentRecords)
                {
                    double xTab1EquipmentRecord = xTab1Label + _page.MeasureString(record.Title + ":", font1).Width + 2;
                    double xTab1EquipmentRecord2 = xTab1Label + 10;
                    var lines = SplitToMultiLine(record.Description, xPageRight - xTab1EquipmentRecord2 - 2, xPageRight - xTab1EquipmentRecord - 2, 25, ref font1, ',');
                    NewLine();
                    _page.DrawString($"{record.Title}:", font1, XBrushes.Black, xTab1Label, _yPos);
                    _page.DrawString(lines[0], font1, XBrushes.Black, xTab1EquipmentRecord, _yPos);
                    for (int i = 1; i < lines.Count; i++)
                    { NewLine(); _page.DrawString(lines[i].TrimStart(), font1, XBrushes.Black, xTab1EquipmentRecord2, _yPos); }
                }
            }

                //TFSApi.APIRevision approval = null;
                //if (!string.IsNullOrEmpty(testResult.TestCaseState) && testResult.TestCaseState=="Approved" && testCase !=null)
                //{
                //    foreach (TFSApi.APIRevision revision in testCase.Revisions.Where(r=>r.ChangedDate<testResult.DateStarted))
                //    {
                //        TFSApi.APIRevisionField revisionField=revision.Fields.Where(f => (f.Value as string) == "Approved" && (f.OriginalValue as string) != "Approved").FirstOrDefault();
                //        if (revisionField == null)
                //            continue;

                //        if (approval == null)
                //            approval = revision;
                //        else if (revision.ChangedDate>approval.ChangedDate)
                //            approval = revision;
                //    }
                //}
                //if(approval!=null)
                //{
                //    NewLine();
                //    _page.DrawString("Approved:", font1, XBrushes.Black, xTab2Label, _yPos); _page.DrawString(string.Format("{0} on {1:dd-MMM-yyyy}",approval.ChangedBy ,approval.ChangedDate), font1, XBrushes.Black, xTab2Data, _yPos);

                //}


                NewLine(deltaY2, 0);
            lastPrinted = eLastPrinted.testResult;
        }
        /// <summary>
        /// Prints a test action/ test step
        /// </summary>
        /// <param name="testAction"></param>
        public void Print(Step step)
        {
            // Column positions           
            const double xTabTestActionStep = xTabStartTestAction + 10;
            const double xTabTestAction2 = xTabTestActionStep + 2;
            double xTabTestAction2Ex = xTabTestAction2;
            const double xTabTestActionOutcome = xTabTestAction2 + 100; // xTabOutcome - 5;
            const double xTabTestActionComment = xTabTestActionOutcome + 18;
            
            // Right and left padding within a grid cell
            const double xPadding = .7;

            // Row height for header 
            double deltaYHeader = 1.3*deltaY;

            // Brushes for cell background
            XBrush headerBrush = XBrushes.CornflowerBlue;
            XBrush stepBrush = XBrushes.Aqua ;

            List<string> lines = null;
            List<string> commentLines = null;

            // Pens for grid lines
            XPen xLinePen = new XPen(XColor.FromArgb(System.Drawing.Color.Black.ToArgb())) { Width = .3 };
            XPen xVLinePen = new XPen(XColor.FromArgb(System.Drawing.Color.Black.ToArgb())) { Width = .15 };

            double yPos = _yPos - deltaY + 1 - xLinePen.Width;
            XStringFormat xstringFormatStepNumber = new XStringFormat() { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.BaseLine };

            if (lastPrinted != eLastPrinted.testAction)
            {
                // Print header rows
                NewLine(deltaYHeader, 2* deltaYHeader);
                DrawCellBorder(xTabStartTestAction, xTabTestActionOutcome, deltaYHeader, ref xLinePen, ref xVLinePen, ref headerBrush,new bool[] {false,true,true,true,true});
                DrawCellText(xTabStartTestAction, xTabTestActionOutcome, deltaYHeader,xPadding, "Protocol", XStringAlignment.Center,ref boldFont);
                DrawCellBorder(xTabTestActionOutcome, xPageRight, deltaYHeader, ref xLinePen, ref xVLinePen, ref headerBrush, new bool[] { false, true, true, true, true });
                DrawCellText(xTabTestActionOutcome, xPageRight, deltaYHeader, xPadding, "Result", XStringAlignment.Center, ref boldFont);
                NewLine(deltaYHeader, 0);

                DrawCellBorder(xTabStartTestAction, xTabTestAction2, deltaYHeader, ref xLinePen, ref xVLinePen, ref headerBrush, new bool[] { false, true, true, true, true });
                DrawCellText(xTabStartTestAction, xTabTestAction2, deltaYHeader, xPadding, "Step", XStringAlignment.Far, ref boldFont);
                DrawCellBorder(xTabTestAction2, xTabTestActionOutcome, deltaYHeader, ref xLinePen, ref xVLinePen, ref headerBrush, new bool[] { false, true, true, true, true });
                DrawCellText(xTabTestAction2, xTabTestActionOutcome, deltaYHeader, xPadding, "Action", XStringAlignment.Near, ref boldFont);
                DrawCellBorder(xTabTestActionOutcome, xTabTestActionComment, deltaYHeader, ref xLinePen, ref xVLinePen, ref headerBrush, new bool[] { false, true, true, true, true });
                DrawCellText(xTabTestActionOutcome, xTabTestActionComment, deltaYHeader, xPadding, "Outcome", XStringAlignment.Center, ref boldFont);
                DrawCellBorder(xTabTestActionComment, xPageRight, deltaYHeader, ref xLinePen, ref xVLinePen, ref headerBrush, new bool[] { false, true, true, true, true });
                DrawCellText(xTabTestActionComment, xPageRight, deltaYHeader, xPadding, "Comment", XStringAlignment.Near, ref boldFont);
                NewLine(deltaY, 0);
            }
            string instructions = step.InstructionsText.Trim();
            if (step.Parameters != null)
                foreach (var stepParameter in step.Parameters)
                    instructions=instructions.Replace("@"+stepParameter.ParameterName, $"@{stepParameter.ParameterName}:{stepParameter.Value}");
            lines =  SplitToMultiLine(instructions, xTabTestActionOutcome - xTabTestAction2-2, ref font1);


            if (!string.IsNullOrEmpty(step.Comment))
            {
                string fieldContent = step.Comment.Trim();
                commentLines = SplitToMultiLine(fieldContent, xPageRight - xTabTestActionComment - 2, xPageRight - xTabTestActionComment - 2, ref font1, ' ');
            }
            
            int lineNumber = 0;
            
            // Print action title
            for (int i = 0; i < lines.Count; i++)
            {
                DrawCellBorder(xTabStartTestAction, xTabTestAction2, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, (i==0), true, false, true });
                if (i == 0) DrawCellText(xTabStartTestAction, xTabTestAction2, deltaY, xPadding, step.GetStepNumber(), XStringAlignment.Far, ref font1);
                DrawCellBorder(xTabTestAction2, xTabTestActionOutcome, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, (i == 0), true, false, true });
                DrawCellText(xTabTestAction2, xTabTestActionOutcome, deltaY, xPadding, lines[i], XStringAlignment.Near, ref font1);
                DrawCellBorder(xTabTestActionOutcome, xTabTestActionComment, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, (i == 0), true, false, true });
                if (i == 0 && step.TypeOfStep!=Step.TypesOfStep.SharedStepsRef)
                    DrawCellText(xTabTestActionOutcome , xTabTestActionComment, deltaY, xPadding, step.Outcome, XStringAlignment.Near, ref font1);
                DrawCellBorder(xTabTestActionComment , xPageRight, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, (i == 0), true, false, true });
                if (commentLines != null && lineNumber < commentLines.Count) DrawCellText(xTabTestActionComment, xPageRight, deltaY, xPadding, commentLines[lineNumber], XStringAlignment.Near, ref font1);

                lineNumber++;
                NewLine();
            }

            // Print expected result
            if (!string.IsNullOrEmpty(step.ExpectedResult))
            {
                xTabTestAction2Ex = xTabTestAction2 + _page.MeasureString("Expected result:", boldFont).Width + 1;
                string expectedResult = (step.ExpectedResultText??"").Trim();
                if (step.Parameters != null)
                    foreach (var stepParameter in step.Parameters)
                        expectedResult = expectedResult.Replace("@"+stepParameter.ParameterName, $"@{stepParameter.ParameterName}:{stepParameter.Value}");
                lines = SplitToMultiLine(expectedResult, xTabTestActionOutcome - xTabTestAction2 - 2, xTabTestActionOutcome - xTabTestAction2Ex - 2, ref font1,' ');
                for (int i=0;i<lines.Count;i++)
                {
                    DrawCellBorder(xTabStartTestAction, xTabTestAction2, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, false, true, false, true });
                    DrawCellBorder(xTabTestAction2, xTabTestActionOutcome, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, false , true, false, true });
                    if(i==0)
                        DrawCellText(xTabTestAction2, xTabTestAction2Ex, deltaY, xPadding, "Expected result:", XStringAlignment.Near,ref boldFont);
                    DrawCellText(i == 0 ? xTabTestAction2Ex : xTabTestAction2, xTabTestActionOutcome, deltaY, xPadding, lines[i], XStringAlignment.Near, ref font1);
                    DrawCellBorder(xTabTestActionOutcome, xTabTestActionComment, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, false, true, false, true });
                    DrawCellBorder(xTabTestActionComment, xPageRight, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, false, true, false, true });
                    if (commentLines != null && lineNumber < commentLines.Count) DrawCellText(xTabTestActionComment, xPageRight, deltaY, xPadding, commentLines[lineNumber], XStringAlignment.Near, ref font1);

                    lineNumber++;
                    NewLine();
                }
            }

            if (commentLines != null)
                for (int i = lineNumber; i < commentLines.Count; i++)
                {
                    DrawCellBorder(xTabStartTestAction, xTabTestAction2, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, false, true, false, true });
                    DrawCellBorder(xTabTestAction2, xTabTestActionOutcome, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, false, true, false , true });
                    DrawCellBorder(xTabTestActionOutcome, xTabTestActionComment, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, false, true, false , true });
                    DrawCellBorder(xTabTestActionComment, xPageRight, deltaY, ref xLinePen, ref xVLinePen, ref stepBrush, new bool[] { false, false, true, false, true });
                    if (commentLines != null && lineNumber < commentLines.Count) DrawCellText(xTabTestActionComment, xPageRight, deltaY, xPadding, commentLines[i], XStringAlignment.Near, ref font1);

                    NewLine();
                }

            lastPrinted = eLastPrinted.testAction;
        }

        /// <summary>Draws the bottom line for the last action
        /// </summary>
        void CloseActionTable()
        {
            XPen xLinePen = new XPen(XColor.FromArgb(System.Drawing.Color.Black.ToArgb())) { Width = .15 };
            _page.DrawLine(xLinePen, xTabStartTestAction, _yPos-deltaY, xPageRight, _yPos - deltaY);
        }

        void DrawCellText(double xPos, double xcellRightPos, double cellHeight,double xcellPadding, string cellText,XStringAlignment alignment, ref XFont font)
        {
           if (string.IsNullOrEmpty(cellText))
                return;
            XStringFormat stringFormat = new XStringFormat() { Alignment = alignment, LineAlignment = XLineAlignment.Center };
            double xCellPos = 0;
            switch(alignment)
            {
                case XStringAlignment.Near: xCellPos = xPos + xcellPadding; break;
                case XStringAlignment.Center: xCellPos = (xPos + xcellRightPos)/2; break;
                case XStringAlignment.Far: xCellPos = xcellRightPos - xcellPadding; break;
            }
            _page.DrawString(cellText, font, XBrushes.Black, xCellPos, _yPos - .5* cellHeight, stringFormat);
        }
        void DrawCellBorder(double xPos, double xcellRightPos, double cellHeight, ref XPen xLineHPen, ref XPen yLineVPen, ref XBrush backgroundBrush, bool[] flags)
        {
            if (flags[0])
                _page.DrawRectangle(backgroundBrush, xPos, _yPos - cellHeight, xcellRightPos-xPos, cellHeight);

            if (flags[1])  //top
                _page.DrawLine(xLineHPen, xPos, _yPos-cellHeight, xcellRightPos, _yPos - cellHeight);
            if (flags[2])  //right
                _page.DrawLine(yLineVPen, xcellRightPos, _yPos-cellHeight, xcellRightPos, _yPos);
            if (flags[3])  //bottom
                _page.DrawLine(xLineHPen, xPos, _yPos , xcellRightPos, _yPos );
            if (flags[4])  //left
                _page.DrawLine(yLineVPen, xPos , _yPos - cellHeight, xPos , _yPos);
        }

        public void Print(WorkItem associatedWorkItem)
        {
            const double xTabWiId = xTabStartAssociatedWorkItems + 15;
            const double xTabWiState = xTabWiId + 2;
            const double xTabWiTitle = xTabWiState + 35;
            const double yCellTopBorderOffset = - deltaY*.75;
            const double yCellBottomBorderOffset = yCellTopBorderOffset + deltaY;

            XPen xLinePen = new XPen(XColor.FromArgb(System.Drawing.Color.Black.ToArgb())) { Width = .3 };
            XPen xVLinePen = new XPen(XColor.FromArgb(System.Drawing.Color.Black.ToArgb())) { Width = .15 };

            switch (lastPrinted)
            {
                case eLastPrinted.testAction:
                    CloseActionTable(); break;
            }
            if (lastPrinted != eLastPrinted.associatedWorkItem)
            {
                NewLine(.7 * deltaY, 15);
                _page.DrawString("Defect(s):", boldFont, XBrushes.Black, xTabStartAssociatedWorkItems, _yPos);
                NewLine(.3 * deltaY, 0);
            }

            XStringFormat xstringFormat = new XStringFormat() { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.BaseLine };
            List<string> titleLines = SplitToMultiLine(associatedWorkItem.Title, xPageRight - xTabWiTitle - .2, ref font1);
            List<string> stateReasonLines = SplitToMultiLine(associatedWorkItem.State, xTabWiTitle - xTabWiState - 1, ref font1);
            stateReasonLines.AddRange(SplitToMultiLine(associatedWorkItem.Reason, xTabWiTitle - xTabWiState - 1, ref font1));
            int lines = Math.Max(titleLines.Count, stateReasonLines.Count);

            for (int i = 0; i < lines; i++)
            {
                NewLine();
                if (i == 0)
                    _page.DrawLine(xLinePen, xTabStartAssociatedWorkItems, _yPos + yCellTopBorderOffset, xPageRight, _yPos + yCellTopBorderOffset);

                if (i == 0)
                    _page.DrawString(associatedWorkItem.Id.ToString(), font1, XBrushes.Black, xTabWiId, _yPos, xstringFormat);
                if (stateReasonLines.Count > i)
                    _page.DrawString(stateReasonLines[i], font1, XBrushes.Black, xTabWiState, _yPos);
                if (titleLines.Count > i)
                    _page.DrawString(titleLines[i], font1, XBrushes.Black, xTabWiTitle, _yPos);

                _page.DrawLine(xLinePen, xTabStartAssociatedWorkItems, _yPos + yCellTopBorderOffset, xTabStartAssociatedWorkItems, _yPos + yCellBottomBorderOffset);
                _page.DrawLine(xVLinePen, xTabWiState - 1, _yPos + yCellTopBorderOffset, xTabWiState - 1, _yPos + yCellBottomBorderOffset);
                _page.DrawLine(xVLinePen, xTabWiTitle - 1, _yPos + yCellTopBorderOffset, xTabWiTitle - 1, _yPos + yCellBottomBorderOffset);
                _page.DrawLine(xLinePen, xPageRight, _yPos + yCellTopBorderOffset, xPageRight, _yPos + yCellBottomBorderOffset);
            }
            _page.DrawLine(xLinePen, xTabStartAssociatedWorkItems, _yPos + yCellBottomBorderOffset, xPageRight, _yPos + yCellBottomBorderOffset);


            lastPrinted = eLastPrinted.associatedWorkItem;
        }

        public void PrintEnd(string filePath )
        {
            PrintEnd2();

            if (_doc.PageCount == 0)
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            else
                _doc.Save(filePath);

            _doc.Close();_doc.Dispose();
        }
        public System.IO.MemoryStream PrintEnd()
        {
            PrintEnd2();
            if (_doc.PageCount == 0)
                return null;

            System.IO.MemoryStream result = new System.IO.MemoryStream();
            _doc.Save(result, false);
            return result;
        }
        class FieldItem { public int Number { get; set; } public string Description { get; set; } }
        private void PrintEnd2()
        {
            if(lastPrinted==eLastPrinted.testAction)
                CloseActionTable();

            XFont pageNoFont = new XFont("Arial", 3.5, XFontStyle.Regular);

            if (_page != null)
                _page.Dispose();

            DateTime printTime = DateTime.Now;
            for (int pageNo = 0; pageNo < _doc.Pages.Count; pageNo++)
            {
                _page = XGraphics.FromPdfPage(_doc.Pages[pageNo], XGraphicsUnit.Millimeter);

                XStringFormat stringFormat = new XStringFormat()
                {
                    Alignment = XStringAlignment.Center,
                    LineAlignment = XLineAlignment.Far,
                };
                _yPos = _page.PageSize.Height - 10;
                if(pageNo>=1||!_showFullReport)
                _page.DrawString(string.Format("- {0} of {1} -", pageNo + 1, _doc.Pages.Count), pageNoFont, XBrushes.Black, _page.PageSize.Width / 2, _yPos, stringFormat);

                stringFormat.Alignment = XStringAlignment.Near;
                _page.DrawString("Restricted", pageNoFont, XBrushes.Black, xPageLeft, _yPos, stringFormat);
                stringFormat.Alignment = XStringAlignment.Far;
                _page.DrawString(string.Format("{0:dd-MMM-yyyy HH:mm}",printTime), pageNoFont, XBrushes.Black, xPageRight, _yPos, stringFormat);

                _page.Dispose();
            }
        }

        void NewLine() { NewLine(deltaY, 0); }
        void NewLine(XFont font) { NewLine(font, 1.2); }
        void NewLine(XFont font, double delta) { NewLine(delta * font.GetHeight(), 0); }
        void NewLine(double yLine, double lineBreakPos)
        {
            _yPos += yLine;
            if (_yPos + lineBreakPos > yPageBottom) CreatePage();
        }
        string CutString(string text, double maxLength, ref XFont font, char? splitChar)
        {
            List<string> lines = SplitToMultiLine(text, maxLength, ref font,splitChar);
            if (lines != null && lines.Count>0)
                return lines[0];

            return text;
        }
        List<string> SplitToMultiLine(string text, double maxLen, ref XFont font) { return SplitToMultiLine(text, maxLen, ref font,' '); }
        List<string> SplitToMultiLine(string text, double maxLen, ref XFont font, char? splitChar) { return SplitToMultiLine(text, maxLen, maxLen, ref font, splitChar); }
        List<string> SplitToMultiLine(string text, double maxLen, double maxLenFirstLine, ref XFont font, char? splitChar) { return SplitToMultiLine(text, maxLen, maxLenFirstLine, 10, ref font, splitChar); }
        List<string> SplitToMultiLine(string text, double maxLen, double maxLenFirstLine, int splitCharSearch,ref XFont font, char? splitChar)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(text))
                return result;

            int pos = 0;
            while (pos < text.Length)
            {
                int posLast = text.Length - 1;

                // check for line breaks in remaining text
                int lineBreak = text.Substring(pos, posLast - pos + 1).IndexOf("\n");
                // if linebreak set pos last
                if (lineBreak >= 0) posLast = pos + lineBreak;

                // limit string characters to maxlen
                bool hardLb = true;
                while (_page.MeasureString(text.Substring(pos, posLast - pos + 1), font).Width > (result.Count == 0 ? maxLenFirstLine : maxLen))
                { posLast--; hardLb = false; }

                // try to find a better break position
                if(!hardLb && splitChar.HasValue)
                {
                    int splitPos=text.Substring(pos, posLast - pos + 1).LastIndexOf(splitChar.Value);
                    if (splitPos > 1 && splitPos >= posLast - pos - splitCharSearch)
                        posLast = pos + splitPos;
                }
                //if (!hardLb && text.Substring(pos, posLast - pos + 1).LastIndexOf(splitChar) >= posLast - pos - 10)
                //    posLast = pos + text.Substring(pos, posLast - pos + 1).LastIndexOf(' ');
                result.Add(text.Substring(pos, posLast - pos + 1).Replace("\n",""));
                pos = posLast + 1;
            }

            return result;
        }

        PdfPage _actualPage = null;
        void CreatePage()
        {
            _actualPage = _doc.AddPage();
            _actualPage.Width = new XUnit(8.5, XGraphicsUnit.Inch);_actualPage.Height = new XUnit(11, XGraphicsUnit.Inch);

            if (_page != null)
                _page.Dispose();
            // Get an XGraphics object for drawing 
            _page = XGraphics.FromPdfPage(_actualPage, XGraphicsUnit.Millimeter);
            if (_doc.Pages.Count > 1)
            {
                _yPos = 18;
                _page.DrawString(string.Format("Test Plan: {0} ({1})", _testPlan.Title, _testPlan.Id), boldFont, XBrushes.Black, xPageLeft, _yPos); //_page.DrawString(_testPlan.Id.ToString(), boldFont, XBrushes.Black, xTab3, _yPos);
                _page.DrawLine(new XPen(XColors.Black, 0.2), xPageLeft, _yPos + 1.2, xPageRight, _yPos + 1.2);
            }
            _yPos = yPageTop;


        }

    }
}
