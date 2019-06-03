using System;

/* /UI/ChartConfiguration.cs
 * Chart configuration window interface. Allows user to select the options for
 displaying a plot of object data.
 */

namespace bitclean.UI
{
    /// <summary>
    /// Allows user to configure the chart.
    /// </summary>
    public partial class ChartConfiguration : Gtk.Window
    {
        private ChartOptions configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.UI.ChartConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public ChartConfiguration(ref ChartOptions configuration) : base(Gtk.WindowType.Toplevel)
        {
            Build();

            this.configuration = configuration;
            SetUpOptions();
        }

        /// <summary>
        /// Initializes the configuration options.
        /// </summary>
        private void SetUpOptions()
        {
            // set the horizontal radio button
            foreach (Gtk.RadioButton radio in radioHTag.Group) {
                if (configuration.horizontalChoice == radio.Label)
                    radio.Active = true;
            }

            // set the vertical radio button
            foreach (Gtk.RadioButton radio in radioVTag.Group) {
                if (configuration.verticalChoice == radio.Label)
                    radio.Active = true;
            }

            // set the function choice combo box
            if (configuration.function.ToString() == "bitclean.Linear")
                functionsCombo.Active = 0;
            else if (configuration.function.ToString() == "bitclean.Logistic")
                functionsCombo.Active = 1;
            //TODO: create a way to adjust the linear function through an entry box
            //TODO: create logistic parameters entry box and update those parameters

            // set the preprocessing and filter check boxes
            squaredCheck.Active = configuration.squared;
            showDustChecked.Active = configuration.dust;
            showStructuresChecked.Active = configuration.structures;
        }

        /// <summary>
        /// Called when window is closed.
        /// </summary>
        /// <param name="o">O.</param>
        /// <param name="args">Arguments.</param>
        protected void OnDeleteConfiguration(object o, Gtk.DeleteEventArgs args)
        {
            // determine the horizontal radio button selected
            foreach (Gtk.RadioButton radio in radioHTag.Group) {
                if (radio.Active) 
                    configuration.horizontalChoice = radio.Label;
            }

            // determine the vertical radio button selected
            foreach (Gtk.RadioButton radio in radioVTag.Group) {
                if (radio.Active) 
                    configuration.verticalChoice = radio.Label;
            }

            // determine the function choice
            if (functionsCombo.ActiveText == "Logistic")
                configuration.function = new Logistic();
            else if (functionsCombo.ActiveText == "None")
                configuration.function = new Linear();

            // determine preprocess and filter options
            if (squaredCheck.Active)
                configuration.squared = true;
            else
                configuration.squared = false;

            if (showDustChecked.Active)
                configuration.dust = true;
            else
                configuration.dust = false;

            if (showStructuresChecked.Active)
                configuration.structures = true;
            else
                configuration.structures = false;

            /*
            Console.WriteLine("**************************************");
            Console.WriteLine("hAxis:{0}", configuration.horizontalChoice);
            Console.WriteLine("vAxis:{0}", configuration.verticalChoice);
            Console.WriteLine("function:{0}", configuration.function);
            Console.WriteLine("squared:{0}", configuration.squared);
            Console.WriteLine("dust:{0}", configuration.dust);
            Console.WriteLine("structures:{0}", configuration.structures);
            */
        }
    }
}
