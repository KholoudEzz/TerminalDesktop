using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Drawing;




namespace Terminal
{
     static class QRCodeReader
    {


        public static string QRCodeReaderFromImage(string QRCodeImagePath)
        {
            //return;
            // Load the image file (replace with your file path)
            //  string imagePath = @"C:\\XmlOutput\\tt0006_00000.jpg";
          //  string imagePath = @"C:\\XmlOutput\\tt0006_00000.ttf";

            // Load the image
            using (var bitmap = (Bitmap)Image.FromFile(QRCodeImagePath))
            {
                //    // Create a barcode reader instance
                var reader = new ZXing.Windows.Compatibility.BarcodeReader();




                //    // Decode the QR code from the image
                var result = reader.Decode(bitmap);

                          

                if (result != null)
                {
                    return result.Text;

                    Console.WriteLine("QR Code Text: " + result.Text);
                }
                else
                {
                    return "No QR code found.";
                }
            }

        }
    }
}




