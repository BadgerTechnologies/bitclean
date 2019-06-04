using System;
using Gtk;
using Gdk;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Xml.Linq;

/* /UI/MainWindow.cs
 * The starting window. Lets the user load/save an image, run bitclean, export
 the object data to an xml, and open the diagnostics window.
 */

namespace bitclean.UI
{
    /// <summary>
    /// Main window.
    /// </summary>
    public partial class MainWindow : Gtk.Window
    {
        Bitmap bitmap;
        List<ObjectData> objects;
        MessageDialog errdialog;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:bitclean.UI.MainWindow"/> class.
        /// </summary>
        public MainWindow() : base(Gtk.WindowType.Toplevel)
        {
            Build();
            errdialog = new MessageDialog(this, DialogFlags.DestroyWithParent, 
                MessageType.Error, ButtonsType.Close, null);
        }

        /// <summary>
        /// When the window is closed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="a">The alpha component.</param>
        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }

        /// <summary>
        /// Loads and displays an image.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void LoadImage(object sender, EventArgs e)
        {
            // get file path from user
            string result = null;
            FileChooserDialog openDialog = new FileChooserDialog("Open", null, 
                FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", 
                ResponseType.Accept);
            FileFilter imageFilter = new FileFilter { 
                    Name = "jpeg, png, bmp, ico" 
                };

            imageFilter.AddPattern("*.jpeg");
            imageFilter.AddPattern("*.png");
            imageFilter.AddPattern("*.bmp");
            imageFilter.AddPattern("*.ico");
            openDialog.AddFilter(imageFilter);

            if (openDialog.Run() == (int)ResponseType.Accept)
            {
                result = openDialog.Filename;

                // load and display image
                try
                {
                    Pixbuf buffer = new Pixbuf(result);
                    displayedImage.Pixbuf = buffer;

                    // open image as a bitmap and parse it to change the floor 
                    // color
                    bitmap = new Bitmap(result);
                    ImageOperations.ParseImage(ref bitmap);
                }
                catch (Exception excp)
                {
                    // display error to user
                    errdialog.Text = "Error Loading: " + result + "\n\n" + 
                        excp.Message;
                    errdialog.Run();
                    errdialog.Hide();
                        
                    Console.WriteLine(excp.Message);
                }

            }

            openDialog.Destroy();
        }

        /// <summary>
        /// Saves the image to a valid image file type of jpeg, png, bmp, or 
        /// ico.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void SaveImage(object sender, EventArgs e)
        {
            if (displayedImage.Pixbuf == null)
            {
                // display error to user
                errdialog.Text = "Error Saving file, no image loaded.";
                errdialog.Run();
                errdialog.Hide();

                Console.WriteLine("No image loaded.");
                return;
            }

            // get file path from user
            string result = null;
            FileChooserDialog saveDialog = new FileChooserDialog("Save as", 
                null, FileChooserAction.Save, "Cancel", ResponseType.Cancel, 
                "Save", ResponseType.Accept);

            FileFilter imageFilter = new FileFilter { 
                    Name = "jpeg, png, bmp, ico" 
                };

            imageFilter.AddPattern("*.jpeg");
            imageFilter.AddPattern("*.png");
            imageFilter.AddPattern("*.bmp");
            imageFilter.AddPattern("*.ico");
            saveDialog.AddFilter(imageFilter);

            if (saveDialog.Run() == (int)ResponseType.Accept)
            {
                // get extension
                result = saveDialog.Filename;
                string extension = System.IO.Path.GetExtension(result);

                try
                { // try to save file
                    if (!displayedImage.Pixbuf
                            .Save(result, extension.Substring(1)))
                    {
                        // display error to user
                        errdialog.Text = "Error Saving: " + result + 
                            "\n\n cannot save file extension " + extension;
                        errdialog.Run();
                        errdialog.Hide();

                        Console.WriteLine("cannot save file extension {0}", 
                            extension);
                    }

                }
                catch (Exception excp)
                {
                    // display error to user
                    errdialog.Text = "Error Saving: " + result + "\n\n" +
                        excp.Message;
                    errdialog.Run();
                    errdialog.Hide();

                    Console.WriteLine(excp.Message);
                }
            }

            saveDialog.Destroy();
        }

        /// <summary>
        /// Runs bitclean.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void RunBitclean(object sender, EventArgs e)
        {
            if (displayedImage.Pixbuf == null)
            {
                // display error to user
                errdialog.Text = "Error Running Bitclean, no image loaded.";
                errdialog.Run();
                errdialog.Hide();

                Console.WriteLine("No image loaded.");
                return;
            }

            // set up data structures for toolbox
            Data imgdata;
            imgdata.height = displayedImage.Pixbuf.Height;
            imgdata.width = displayedImage.Pixbuf.Width;
            imgdata.totalpixels = imgdata.height * imgdata.width;

            Pixel[] pixels;
            pixels = new Pixel[imgdata.totalpixels];
            ImageOperations.PopulatePixelArray(bitmap, ref pixels);

            // create toolbox
            Toolbox toolbox = new Toolbox(pixels, imgdata);

            // run toolbox
            toolbox.Run();

            // get object data from toolbox
            objects = toolbox.GetObjectData();

            // update displayed image
            ImageOperations.PushPixelsToImage(bitmap, pixels);

            MemoryStream newpixelstream = new MemoryStream();
            bitmap.Save(newpixelstream, System.Drawing.Imaging.ImageFormat.Png);

            newpixelstream.Seek(0, SeekOrigin.Begin);
            displayedImage.Pixbuf = new Pixbuf(newpixelstream);
        }

        /// <summary>
        /// Exports data returned from bitclean to xml.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void ExportToXML(object sender, EventArgs e)
        {
            if (objects == null)
            {
                // display error to user
                errdialog.Text = "Error exporting to XML, no data loaded.";
                errdialog.Run();
                errdialog.Hide();

                Console.WriteLine("No objects found");
                return;
            }

            // get file name from user
            string result = null;
            FileChooserDialog saveDialog = new FileChooserDialog("Save as", 
                null, FileChooserAction.Save, "Cancel", ResponseType.Cancel, 
                "Save", ResponseType.Accept);

            FileFilter xmlFilter = new FileFilter { Name = "xml" };
            xmlFilter.AddPattern("*.xml");
            saveDialog.AddFilter(xmlFilter);

            if (saveDialog.Run() == (int)ResponseType.Accept)
            {
                // get extension
                result = saveDialog.Filename;
                string extension = System.IO.Path.GetExtension(result);
            }
            else
            {
                saveDialog.Destroy();
                return;
            }

            saveDialog.Destroy();

            // create xml document, add objects root
            var xml = new XDocument();
            var root = new XElement("objects");
            xml.Add(root);

            // for each object found
            for (int i = 0; i < objects.Count; i++)
            {
                ObjectData obj = objects[i];

                // create object entry and it's attributes
                var objectElement = new XElement("object");
                objectElement.SetAttributeValue("tag", obj.tag);
                objectElement.SetAttributeValue("decision", obj.objconf.decision);

                // create object's attributes
                var sizeElement = new XElement("size", obj.size);
                var avgHueElement = new XElement("avg_hue", obj.avghue);
                var densityElement = new XElement("density", obj.density);
                var edgeRatioElement = new XElement("edge_ratio", obj.edgeratio);

                var boundsElement = new XElement("bounds");
                var topElement = new XElement("top", obj.bounds.top);
                var leftElement = new XElement("left", obj.bounds.left);
                var bottomElement = new XElement("bottom", obj.bounds.bottom);
                var rightElement = new XElement("right", obj.bounds.right);
                boundsElement.Add(topElement);
                boundsElement.Add(leftElement);
                boundsElement.Add(bottomElement);
                boundsElement.Add(rightElement);

                var coordinatesElement = new XElement("coordinates");
                var xElement = new XElement("x", obj.position.x);
                var yElement = new XElement("y", obj.position.y);
                coordinatesElement.Add(xElement);
                coordinatesElement.Add(yElement);

                var neighborsElement = new XElement("neighbors");
                neighborsElement.SetAttributeValue("total", obj.neighbors.Count);

                for (int j = 0; j < obj.neighbors.Count; j++)
                {
                    var neighborTagElement = new XElement("tag", obj.neighbors[j]);
                    neighborsElement.Add(neighborTagElement);
                }

                // add attributes to the object
                objectElement.Add(sizeElement);
                objectElement.Add(avgHueElement);
                objectElement.Add(densityElement);
                objectElement.Add(edgeRatioElement);
                objectElement.Add(boundsElement);
                objectElement.Add(coordinatesElement);
                objectElement.Add(neighborsElement);

                // add object to objects root node
                root.Add(objectElement);
            }

            // save xml file
            xml.Save(result);
        }

        /// <summary>
        /// Opens the diagnostics window.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OpenDiagnostics(object sender, EventArgs e)
        {
            Diagnostics diagnostics = new Diagnostics();
            diagnostics.Show();
        }
    }
}
