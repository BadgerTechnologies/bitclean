using System;
using System.Collections.Generic;
using System.Xml.Linq;

/* /UI/Diagnostics.cs
 * Diagnostics interface. Allows the user to load object data from an xml file
 and displays the data along with statistics.
 */

namespace bitclean
{
    /// <summary>
    /// Diagnostics window.
    /// </summary>
    public partial class Diagnostics : Gtk.Window
    {
        // tag, decision, size, avghue, density, edgeratio, neighbors.count
        Gtk.ListStore store = new Gtk.ListStore
        (typeof(int), typeof(string), typeof(int),
        typeof(double), typeof(double), typeof(double),
        typeof(int), typeof(double));

        // attribute, max, min, avg
        Gtk.ListStore allstatsStore = new Gtk.ListStore
        (typeof(string), typeof(double), typeof(double), typeof(double));
        Gtk.ListStore structstatsStore = new Gtk.ListStore
        (typeof(string), typeof(double), typeof(double), typeof(double));
        Gtk.ListStore duststatsStore = new Gtk.ListStore
        (typeof(string), typeof(double), typeof(double), typeof(double));

        List<ChartObject> objects;

        // structs for object statistics
        AttributeStatistics totalSizeStats = new AttributeStatistics();
        AttributeStatistics totalDensityStats = new AttributeStatistics();
        AttributeStatistics totalEdgeratioStats = new AttributeStatistics();
        AttributeStatistics totalNeighborsStats = new AttributeStatistics();

        AttributeStatistics dustSizeStats = new AttributeStatistics();
        AttributeStatistics dustDensityStats = new AttributeStatistics();
        AttributeStatistics dustEdgeratioStats = new AttributeStatistics();
        AttributeStatistics dustNeighborsStats = new AttributeStatistics();

        AttributeStatistics structureSizeStats = new AttributeStatistics();
        AttributeStatistics structureDensityStats = new AttributeStatistics();
        AttributeStatistics structureEdgeratioStats = new AttributeStatistics();
        AttributeStatistics structureNeighborsStats = new AttributeStatistics();

        int dustcount, structurecount;

        ChartOptions configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Diagnostics"/> class.
        /// </summary>
        public Diagnostics() : base(Gtk.WindowType.Toplevel)
        {
            Build();

            objects = new List<ChartObject>();

            // full data tree
            datatree.AppendColumn("Tag", new Gtk.CellRendererText(), "text", 0);
            datatree.AppendColumn("Decision", new Gtk.CellRendererText(), "text", 1);
            datatree.AppendColumn("Size", new Gtk.CellRendererText(), "text", 2);
            datatree.AppendColumn("Avg Hue", new Gtk.CellRendererText(), "text", 3);
            datatree.AppendColumn("Density", new Gtk.CellRendererText(), "text", 4);
            datatree.AppendColumn("Edge Ratio", new Gtk.CellRendererText(), "text", 5);
            datatree.AppendColumn("Neighbors", new Gtk.CellRendererText(), "text", 6);
            datatree.AppendColumn("Neural Output", new Gtk.CellRendererText(), "text", 7);
            datatree.Model = store;
            datatree.ShowAll();

            // all stats tree
            alldatatree.AppendColumn("Attribute", new Gtk.CellRendererText(), "text", 0);
            alldatatree.AppendColumn("Max", new Gtk.CellRendererText(), "text", 1);
            alldatatree.AppendColumn("Min", new Gtk.CellRendererText(), "text", 2);
            alldatatree.AppendColumn("Avg", new Gtk.CellRendererText(), "text", 3);
            alldatatree.Model = allstatsStore;
            alldatatree.ShowAll();

            // structure stats tree
            structuredatatree.AppendColumn("Attribute", new Gtk.CellRendererText(), "text", 0);
            structuredatatree.AppendColumn("Max", new Gtk.CellRendererText(), "text", 1);
            structuredatatree.AppendColumn("Min", new Gtk.CellRendererText(), "text", 2);
            structuredatatree.AppendColumn("Avg", new Gtk.CellRendererText(), "text", 3);
            structuredatatree.Model = structstatsStore;
            structuredatatree.ShowAll();

            // dust stats tree
            dustdatatree.AppendColumn("Attribute", new Gtk.CellRendererText(), "text", 0);
            dustdatatree.AppendColumn("Max", new Gtk.CellRendererText(), "text", 1);
            dustdatatree.AppendColumn("Min", new Gtk.CellRendererText(), "text", 2);
            dustdatatree.AppendColumn("Avg", new Gtk.CellRendererText(), "text", 3);
            dustdatatree.Model = duststatsStore;
            dustdatatree.ShowAll();

            configuration = new ChartOptions
            {
                horizontalChoice = "Tag",
                verticalChoice = "Tag",
                function = new Linear(),
                squared = false,
                dust = true,
                structures = true
            };
        }

        /// <summary>
        /// Gets xml file name from user and calls functions to populate the tree view.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void LoadXMLData(object sender, EventArgs e)
        {
            // clear any data already in the tree store
            ClearTreeViews();

            // get file from user
            string result = null;
            Gtk.FileChooserDialog openDialog = new Gtk.FileChooserDialog("Open", null, Gtk.FileChooserAction.Open, "Cancel", Gtk.ResponseType.Cancel, "Open", Gtk.ResponseType.Accept);
            Gtk.FileFilter xmlFilter = new Gtk.FileFilter { Name = "xml" };
            xmlFilter.AddPattern("*.xml");
            openDialog.AddFilter(xmlFilter);

            if (openDialog.Run() == (int)Gtk.ResponseType.Accept)
                result = openDialog.Filename;

            openDialog.Destroy();

            // file dialog success, load xml data into memory and populate the treeview
            if (result != null)
            {
                try
                {
                    GetXMLData(result);
                }
                catch (Exception excp)
                {
                    Console.WriteLine(excp.Message);
                }

                if (objects.Count > 0)
                    PopulateTreeView();
            }

            datatree.ShowAll();
            alldatatree.ShowAll();
            structuredatatree.ShowAll();
            dustdatatree.ShowAll();
        }

        /// <summary>
        /// Loads the xml data into memory.
        /// </summary>
        /// <param name="path">Path.</param>
        private void GetXMLData(string path)
        {
            if (objects.Count != 0)
                objects.Clear();

            // load xml doc and set it's root
            XElement doc = XDocument.Load(path).Root;

            // iterate through each object in root
            foreach (var obj in doc.Descendants("object"))
            {
                // create chart object and populate it's data with xml attributes
                ChartObject chartObject = new ChartObject
                {
                    tag = (int)obj.Attribute("tag"),
                    decision = (string)obj.Attribute("decision"),
                    size = (int)obj.Element("size"),
                    avghue = (double)obj.Element("avg_hue"),
                    density = (double)obj.Element("density"),
                    edgeratio = (double)obj.Element("edge_ratio"),

                    neighbors = new List<int>()
                };
                foreach (var neighbor in obj.Descendants("tag"))
                    chartObject.neighbors.Add((int)neighbor);

                // add chart object to object list
                objects.Add(chartObject);
            }

        }

        /// <summary>
        /// Fills tree view with data from objects list
        /// </summary>
        private void PopulateTreeView()
        {
            // reset stats
            ResetStatisticsViews();

            // add chart objects to chart display and get statistics on each chart object
            foreach (ChartObject obj in objects)
            {
                try
                {
                    if (obj.decision == "object") // i screwed up my naming convention and don't feel like regenerating the data
                        obj.decision = "structure";

                    store.AppendValues(obj.tag, obj.decision, obj.size, obj.avghue, obj.density, obj.edgeratio, obj.neighbors.Count, 0);
                    StatsTreeViews(obj);
                }
                catch (Exception excp)
                {
                    Console.WriteLine(excp.Message);
                }
            }
            PopulateStatisticsTrees();
        }

        /// <summary>
        /// Calculates object statistics.
        /// </summary>
        /// <param name="obj">Object.</param>
        private void StatsTreeViews(ChartObject obj)
        {
            // if obj size|density|edgeratio|neighbors is min, set it so
            if (obj.size < totalSizeStats.min || Math.Abs(totalSizeStats.min - -1.0) < UIConstants.EPSILON)
                totalSizeStats.min = obj.size;
            if (obj.density < totalDensityStats.min || Math.Abs(totalDensityStats.min - -1.0) < UIConstants.EPSILON)
                totalDensityStats.min = obj.density;
            if (obj.edgeratio < totalEdgeratioStats.min || Math.Abs(totalEdgeratioStats.min - -1.0) < UIConstants.EPSILON)
                totalEdgeratioStats.min = obj.edgeratio;
            if (obj.neighbors.Count < totalNeighborsStats.min || Math.Abs(totalNeighborsStats.min - -1.0) < UIConstants.EPSILON)
                totalNeighborsStats.min = obj.neighbors.Count;

            // if obj size|density|edgeratio|neighbors is max, set it so
            if (obj.size > totalSizeStats.max || Math.Abs(totalSizeStats.max - -1.0) < UIConstants.EPSILON)
                totalSizeStats.max = obj.size;
            if (obj.density > totalDensityStats.max || Math.Abs(totalDensityStats.max - -1.0) < UIConstants.EPSILON)
                totalDensityStats.max = obj.density;
            if (obj.edgeratio > totalEdgeratioStats.max || Math.Abs(totalEdgeratioStats.max - -1.0) < UIConstants.EPSILON)
                totalEdgeratioStats.max = obj.edgeratio;
            if (obj.neighbors.Count > totalNeighborsStats.max || Math.Abs(totalNeighborsStats.max - -1.0) < UIConstants.EPSILON)
                totalNeighborsStats.max = obj.neighbors.Count;

            // add to average
            totalSizeStats.avg += obj.size;
            totalDensityStats.avg += obj.density;
            totalEdgeratioStats.avg += obj.edgeratio;
            totalNeighborsStats.avg += obj.neighbors.Count;

            // if dust, update min/max/avg stats
            if (obj.decision == "dust")
            {
                if (obj.size < dustSizeStats.min || Math.Abs(dustSizeStats.min - -1.0) < UIConstants.EPSILON)
                    dustSizeStats.min = obj.size;
                if (obj.density < dustDensityStats.min || Math.Abs(dustDensityStats.min - -1.0) < UIConstants.EPSILON)
                    dustDensityStats.min = obj.density;
                if (obj.edgeratio < dustEdgeratioStats.min || Math.Abs(dustEdgeratioStats.min - -1.0) < UIConstants.EPSILON)
                    dustEdgeratioStats.min = obj.edgeratio;
                if (obj.neighbors.Count < dustNeighborsStats.min || Math.Abs(dustNeighborsStats.min - -1.0) < UIConstants.EPSILON)
                    dustNeighborsStats.min = obj.neighbors.Count;

                if (obj.size > dustSizeStats.max || Math.Abs(dustSizeStats.max - -1.0) < UIConstants.EPSILON)
                    dustSizeStats.max = obj.size;
                if (obj.density > dustDensityStats.max || Math.Abs(dustDensityStats.max - -1.0) < UIConstants.EPSILON)
                    dustDensityStats.max = obj.density;
                if (obj.edgeratio > dustEdgeratioStats.max || Math.Abs(dustEdgeratioStats.max - -1.0) < UIConstants.EPSILON)
                    dustEdgeratioStats.max = obj.edgeratio;
                if (obj.neighbors.Count > dustNeighborsStats.max || Math.Abs(dustNeighborsStats.max - -1.0) < UIConstants.EPSILON)
                    dustNeighborsStats.max = obj.neighbors.Count;

                dustSizeStats.avg += obj.size;
                dustDensityStats.avg += obj.density;
                dustEdgeratioStats.avg += obj.edgeratio;
                dustNeighborsStats.avg += obj.neighbors.Count;

                dustcount++;
            }
            else
            { // update structure min/max/avg stats
                if (obj.size < structureSizeStats.min || Math.Abs(structureSizeStats.min - -1.0) < UIConstants.EPSILON)
                    structureSizeStats.min = obj.size;
                if (obj.density < structureDensityStats.min || Math.Abs(structureDensityStats.min - -1.0) < UIConstants.EPSILON)
                    structureDensityStats.min = obj.density;
                if (obj.edgeratio < structureEdgeratioStats.min || Math.Abs(structureEdgeratioStats.min - -1.0) < UIConstants.EPSILON)
                    structureEdgeratioStats.min = obj.edgeratio;
                if (obj.neighbors.Count < structureNeighborsStats.min || Math.Abs(structureNeighborsStats.min - -1.0) < UIConstants.EPSILON)
                    structureNeighborsStats.min = obj.neighbors.Count;

                if (obj.size > structureSizeStats.max || Math.Abs(structureSizeStats.max - -1.0) < UIConstants.EPSILON)
                    structureSizeStats.max = obj.size;
                if (obj.density > structureDensityStats.max || Math.Abs(structureDensityStats.max - -1.0) < UIConstants.EPSILON)
                    structureDensityStats.max = obj.density;
                if (obj.edgeratio > structureEdgeratioStats.max || Math.Abs(structureEdgeratioStats.max - -1.0) < UIConstants.EPSILON)
                    structureEdgeratioStats.max = obj.edgeratio;
                if (obj.neighbors.Count > structureNeighborsStats.max || Math.Abs(structureNeighborsStats.max - -1.0) < UIConstants.EPSILON)
                    structureNeighborsStats.max = obj.neighbors.Count;

                structureSizeStats.avg += obj.size;
                structureDensityStats.avg += obj.density;
                structureEdgeratioStats.avg += obj.edgeratio;
                structureNeighborsStats.avg += obj.neighbors.Count;

                structurecount++;
            }
        }

        /// <summary>
        /// Populates the statistics trees.
        /// </summary>
        private void PopulateStatisticsTrees()
        {
            // add all objects stats
            totalSizeStats.avg /= objects.Count;
            totalDensityStats.avg /= objects.Count;
            totalEdgeratioStats.avg /= objects.Count;
            totalNeighborsStats.avg /= objects.Count;

            // add dust stats
            dustSizeStats.avg /= dustcount;
            dustDensityStats.avg /= dustcount;
            dustEdgeratioStats.avg /= dustcount;
            dustNeighborsStats.avg /= dustcount;

            // add structure stats
            structureSizeStats.avg /= structurecount;
            structureDensityStats.avg /= structurecount;
            structureEdgeratioStats.avg /= structurecount;
            structureNeighborsStats.avg /= structurecount;

            // clear existing data
            allstatsStore.Data.Clear();
            allstatsStore.Clear();
            structstatsStore.Data.Clear();
            structstatsStore.Clear();
            duststatsStore.Data.Clear();
            duststatsStore.Clear();

            // create rows
            allstatsStore.AppendValues("size", totalSizeStats.max, totalSizeStats.min, totalSizeStats.avg);
            allstatsStore.AppendValues("density", totalDensityStats.max, totalDensityStats.min, totalDensityStats.avg);
            allstatsStore.AppendValues("edge ratio", totalEdgeratioStats.max, totalEdgeratioStats.min, totalEdgeratioStats.avg);
            allstatsStore.AppendValues("neighbors", totalNeighborsStats.max, totalNeighborsStats.min, totalNeighborsStats.avg);

            structstatsStore.AppendValues("size", structureSizeStats.max, structureSizeStats.min, structureSizeStats.avg);
            structstatsStore.AppendValues("density", structureDensityStats.max, structureDensityStats.min, structureDensityStats.avg);
            structstatsStore.AppendValues("edge ratio", structureEdgeratioStats.max, structureEdgeratioStats.min, structureEdgeratioStats.avg);
            structstatsStore.AppendValues("neighbors", structureNeighborsStats.max, structureNeighborsStats.min, structureNeighborsStats.avg);

            duststatsStore.AppendValues("size", dustSizeStats.max, dustSizeStats.min, dustSizeStats.avg);
            duststatsStore.AppendValues("density", dustDensityStats.max, dustDensityStats.min, dustDensityStats.avg);
            duststatsStore.AppendValues("edge ratio", dustEdgeratioStats.max, dustEdgeratioStats.min, dustEdgeratioStats.avg);
            duststatsStore.AppendValues("neighbors", dustNeighborsStats.max, dustNeighborsStats.min, dustNeighborsStats.avg);

        }

        /// <summary>
        /// Clears the tree views.
        /// </summary>
        private void ClearTreeViews()
        {
            store.Data.Clear();
            store.Clear();
            allstatsStore.Data.Clear();
            allstatsStore.Clear();
            structstatsStore.Data.Clear();
            structstatsStore.Clear();
            duststatsStore.Data.Clear();
            duststatsStore.Clear();

            datatree.ShowAll();
            alldatatree.ShowAll();
            structuredatatree.ShowAll();
            dustdatatree.ShowAll();
        }

        private void ResetStatisticsViews()
        {
            // clear existing data
            allstatsStore.Data.Clear();
            allstatsStore.Clear();
            structstatsStore.Data.Clear();
            structstatsStore.Clear();
            duststatsStore.Data.Clear();
            duststatsStore.Clear();

            // reset rows
            allstatsStore.AppendValues("size", -1, -1, -1);
            allstatsStore.AppendValues("density", -1, -1, -1);
            allstatsStore.AppendValues("edge ratio", -1, -1, -1);
            allstatsStore.AppendValues("neighbors", -1, -1, -1);

            structstatsStore.AppendValues("size", -1, -1, -1);
            structstatsStore.AppendValues("density", -1, -1, -1);
            structstatsStore.AppendValues("edge ratio", -1, -1, -1);
            structstatsStore.AppendValues("neighbors", -1, -1, -1);

            duststatsStore.AppendValues("size", -1, -1, -1);
            duststatsStore.AppendValues("density", -1, -1, -1);
            duststatsStore.AppendValues("edge ratio", -1, -1, -1);
            duststatsStore.AppendValues("neighbors", -1, -1, -1);

            alldatatree.ShowAll();
            structuredatatree.ShowAll();
            dustdatatree.ShowAll();
        }

        /// <summary>
        /// Opens the chart configuration window.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OpenChartConfiguration(object sender, EventArgs e)
        {
            UI.ChartConfiguration configwindow = new UI.ChartConfiguration(ref configuration);
            configwindow.Show();
        }

        /// <summary>
        /// Generates a chart in a new window.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void GenerateChart(object sender, EventArgs e)
        {
            List<object[]> objectdata = new List<object[]>();

            // create data for data points
            foreach (ChartObject chartObject in objects)
            {
                object[] data = {
                    chartObject.tag,
                    chartObject.decision,
                    chartObject.size,
                    chartObject.avghue,
                    chartObject.density,
                    chartObject.edgeratio,
                    chartObject.neighbors.Count
                };
                objectdata.Add(data);
            }

            UI.Chart chart = new UI.Chart(objectdata, configuration, datatree.Columns);
            chart.Show();

        }

        protected void RunSifter(object sender, EventArgs e)
        {
            if (dustcount == 0)
                return;

            AttributeStatistics[] stats =
            {
                dustSizeStats,
                dustEdgeratioStats,
                dustDensityStats
            };

            Sifter sifter = new Sifter(stats);
        }
    }
}
