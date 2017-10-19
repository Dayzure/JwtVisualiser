using Microsoft.VisualStudio.DebuggerVisualizers;
using System;

[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(JwtVisualiser.JwtTokenVisualiser),
typeof(VisualizerObjectSource),
Target = typeof(System.String),
Description = "JwtToken Visualiser")]
namespace JwtVisualiser
{
    public class JwtTokenVisualiser : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            // TODO: Get the object to display a visualizer for.
            //       Cast the result of objectProvider.GetObject() 
            //       to the type of the object being visualized.
            object data = (object)objectProvider.GetObject();
            
            
            // TODO: Display your view of the object.
            //       Replace displayForm with your own custom Form or Control.
            using (TokenForm displayForm = new TokenForm(data.ToString()))
            {
                displayForm.Text = "Token";
                windowService.ShowDialog(displayForm);
            }
        }

        // TODO: Add the following to your testing code to test the visualizer:
        // 
        //    JwtTokenVisualiser.TestShowVisualizer(new SomeType());
        // 
        /// <summary>
        /// Tests the visualizer by hosting it outside of the debugger.
        /// </summary>
        /// <param name="objectToVisualize">The object to display in the visualizer.</param>
        public static void TestShowVisualizer(object objectToVisualize)
        {
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(JwtTokenVisualiser));
            visualizerHost.ShowVisualizer();
        }
    }
}
