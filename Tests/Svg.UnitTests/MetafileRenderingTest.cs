﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Svg.UnitTests
{

    /// <summary>
    /// Test Class of rendering SVGs as meterfile.
    /// Based on Issue 210.
    /// </summary>
    /// <remarks>
    /// Test use the following embedded resources:
    ///   - Issue210_Metafile\3DSceneSnapshotBIG.svg
    /// </remarks>
    [TestClass]
    public class MetafileRenderingTest : SvgTestHelper
    {
        protected override string TestResource { get { return GetFullResourceString("Issue210_Metafile.3DSceneSnapshotBIG.svg"); } }
        protected override int ExpectedSize { get { return 12500; } } //12896


        [TestMethod]
        [TestProperty(name: "speed", value: "slow")]
        public void TestMetafileRendering()
        {
            LoadSvg(GetXMLDocFromResource());
        }


        protected override Image DrawSvg(SvgDocument svgDoc)
        {
            // GDI+
            Metafile metafile;
            using (var stream = new MemoryStream())
            using (var img = new Bitmap((int)svgDoc.Width.Value, (int)svgDoc.Height.Value)) // Not necessary if you use Control.CreateGraphics().
            using (Graphics ctrlGraphics = Graphics.FromImage(img)) // Control.CreateGraphics()
            {
                IntPtr handle = ctrlGraphics.GetHdc();

                var rect = new RectangleF(0, 0, svgDoc.Width, svgDoc.Height);
                metafile = new Metafile(stream,
                    handle,
                    rect,
                    MetafileFrameUnit.Pixel,
                    EmfType.EmfPlusOnly);

                using (Graphics ig = Graphics.FromImage(metafile))
                {
                    svgDoc.Draw(ig);
                }

                ctrlGraphics.ReleaseHdc(handle);
            }

            return metafile;
        }
    }
}
