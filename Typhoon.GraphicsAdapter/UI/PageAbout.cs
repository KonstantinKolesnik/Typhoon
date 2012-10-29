using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace Typhoon.GraphicsAdapter.UI
{
    public class PageAbout : BasePage
    {
        public PageAbout()
        {
            TextFlow textFlow = new TextFlow();
            //textFlow.ScrollingStyle = ScrollingStyle.LineByLine;
            textFlow.HorizontalAlignment = HorizontalAlignment.Center;
            textFlow.VerticalAlignment = VerticalAlignment.Center;
            textFlow.TextAlignment = TextAlignment.Center;
            textFlow.SetMargin(10);
            Child = textFlow;
            
            textFlow.TextRuns.Add("Typhoon", Program.FontCourierNew12, Colors.Orange);
            textFlow.TextRuns.Add(TextRun.EndOfLine);
            textFlow.TextRuns.Add("Central Station", Program.FontCourierNew10, Colors.Orange);
            textFlow.TextRuns.Add(TextRun.EndOfLine);
            textFlow.TextRuns.Add(TextRun.EndOfLine);
            textFlow.TextRuns.Add("Version: " + Program.Info.Version, Program.FontCourierNew9, Colors.White);
            textFlow.TextRuns.Add(TextRun.EndOfLine);
            textFlow.TextRuns.Add("© Konstantin Kolesnik", Program.FontCourierNew9, Colors.White);

            Buttons.Focus(textFlow);
        }
    }
}
